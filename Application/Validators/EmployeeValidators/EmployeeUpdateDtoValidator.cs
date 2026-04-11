using Application.Contracts.EmployeeContracts;
using Domain.Constants;
using Domain.Models;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators.EmployeeValidators;

public class EmployeeUpdateDtoValidator : AbstractValidator<EmployeeUpdateDto>
{
    private const int FirstNameMaxLength = FieldLimits.Employee.FirstNameMaxLength;
    private const int MiddleNameMaxLength = FieldLimits.Employee.MiddleNameMaxLength;
    private const int LastNameMaxLength = FieldLimits.Employee.LastNameMaxLength;
    private const int EmailMaxLength = FieldLimits.Employee.EmailMaxLength;

    public EmployeeUpdateDtoValidator(IEmployeeStore employeeStore)
    {
        RuleFor(dto => dto.FirstName)
            .NotEmpty()
            .WithMessage("First name cannot be empty")
            .MaximumLength(FirstNameMaxLength)
            .WithMessage($"First name must not exceed {FirstNameMaxLength} characters")
            .When(dto => dto.FirstName is not null);

        RuleFor(dto => dto.MiddleName.Value)
            .MaximumLength(MiddleNameMaxLength)
            .WithMessage($"Middle name must not exceed {MiddleNameMaxLength} characters")
            .When(dto => dto.MiddleName.HasValue && !string.IsNullOrEmpty(dto.MiddleName.Value));

        RuleFor(dto => dto.LastName)
            .NotEmpty()
            .WithMessage("Last name cannot be empty")
            .MaximumLength(LastNameMaxLength)
            .WithMessage($"Last name must not exceed {LastNameMaxLength} characters")
            .When(dto => dto.LastName is not null);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(EmailMaxLength)
            .WithMessage($"Email must not exceed {EmailMaxLength} characters")
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

                    bool isExist = await employeeStore.EmailExistsAsync(email, ct);

                    return !isExist;
                }
            )
            .WithMessage("Email already exists")
            .When(dto => dto.Email is not null);

        RuleFor(dto => dto.Role)
            .IsInEnum()
            .WithMessage("Invalid role")
            .When(dto => dto.Role.HasValue);
    }
}
