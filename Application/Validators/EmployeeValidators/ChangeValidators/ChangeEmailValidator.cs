using Application.Contracts.EmployeeContracts;
using Domain.Constants;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators.EmployeeValidators.ChangeValidators;

public class ChangeEmailValidator : AbstractValidator<ChangeEmailDto>
{
    public ChangeEmailValidator(IEmployeeStore employeeStore)
    {
        RuleFor(dto => dto.NewEmail)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("New email is required")
            .EmailAddress()
            .WithMessage("Ivalid email format")
            .MaximumLength(FieldLimits.Employee.EmailMaxLength)
            .WithMessage($"Email must not exceed {FieldLimits.Employee.EmailMaxLength} characters")
            .MustAsync(
                async (email, ct) =>
                {
                    var exists = await employeeStore.EmailExistsAsync(email, ct);

                    return !exists;
                }
            )
            .WithMessage("This email is already in use by another account");
    }
}
