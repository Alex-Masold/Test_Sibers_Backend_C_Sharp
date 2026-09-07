using Domain.Constants;
using Domain.Models;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators.Rules;

public static class EmailRules
{
    private static int EmailMaxLength = FieldLimits.Employee.EmailMaxLength;

    public static IRuleBuilderOptions<T, string> ApplyEmailRules<T>(
        this IRuleBuilder<T, string> ruleBuilder
    )
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(EmailMaxLength)
            .WithMessage($"Email must not exceed {EmailMaxLength} characters");
    }

    public static IRuleBuilderOptions<T, string> ApplyCreatedEmailRules<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IEmployeeStore employeeStore
    )
    {
        return ruleBuilder
            .ApplyEmailRules()
            .MustAsync(async (email, ct) => !await employeeStore.EmailExistsAsync(email, ct))
            .WithMessage("Email already exist");
    }

    public static IRuleBuilderOptions<T, string> ApplyUpdatedEmailRules<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IEmployeeStore employeeStore
    )
    {
        return ruleBuilder
            .ApplyEmailRules()
            .MustAsync(
                async (dto, email, context, ct) =>
                {
                    if (
                        context.RootContextData.TryGetValue("ExistingEmployee", out var obj)
                        && obj is Employee existingEmployee
                    )
                    {
                        if (
                            string.Equals(
                                existingEmployee.Email,
                                email,
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                            return true;
                    }

                    return !await employeeStore.EmailExistsAsync(email, ct);
                }
            )
            .WithMessage("Email already exists");
    }
}
