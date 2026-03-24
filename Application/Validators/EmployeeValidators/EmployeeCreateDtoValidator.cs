using Application.Contracts.EmployeeContracts;
using Domain.Constants;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators.EmployeeValidators;

public class EmployeeCreateDtoValidator : AbstractValidator<EmployeeCreateDto>
{
    private const int FirstNameMaxLength = FieldLimits.Employee.FirstNameMaxLength;
    private const int MiddleNameMaxLength = FieldLimits.Employee.MiddleNameMaxLength;
    private const int LastNameMaxLength = FieldLimits.Employee.LastNameMaxLength;
    private const int EmailMaxLength = FieldLimits.Employee.EmailMaxLength;

    public EmployeeCreateDtoValidator(IEmployeeStore employeeStore)
    {
        RuleFor(dto => dto.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(FirstNameMaxLength)
            .WithMessage($"First name must not exceed {FirstNameMaxLength} characters");

        RuleFor(dto => dto.MiddleName)
            .MaximumLength(MiddleNameMaxLength)
            .WithMessage($"Middle name must not exceed {MiddleNameMaxLength} characters")
            .When(dto => dto.MiddleName is not null);

        RuleFor(dto => dto.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(LastNameMaxLength)
            .WithMessage($"Last name must not exceed {LastNameMaxLength} characters");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(EmailMaxLength)
            .WithMessage($"Email must not exceed {EmailMaxLength} characters")
            .MustAsync(
                async (email, ct) =>
                {
                    var isExist = await employeeStore.EmailExistAsync(email, ct);
                    return !isExist;
                }
            )
            .WithMessage("Email already exists");

        RuleFor(dto => dto.Role).IsInEnum().WithMessage("Invalid role");
    }
}
