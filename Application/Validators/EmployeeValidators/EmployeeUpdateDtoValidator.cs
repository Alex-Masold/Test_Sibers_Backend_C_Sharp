using Application.Contracts.EmployeeContracts;
using Application.Validators.Rules;
using Domain.Constants;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators.EmployeeValidators;

public class EmployeeUpdateDtoValidator : AbstractValidator<EmployeeUpdateDto>
{
    private const int FirstNameMaxLength = FieldLimits.Employee.FirstNameMaxLength;
    private const int MiddleNameMaxLength = FieldLimits.Employee.MiddleNameMaxLength;
    private const int LastNameMaxLength = FieldLimits.Employee.LastNameMaxLength;
    private const int EmailMaxLength = FieldLimits.Employee.EmailMaxLength;
    private const string ExistingEmployee = "ExistingEmployee";

    public EmployeeUpdateDtoValidator(IEmployeeStore employeeStore)
    {
        RuleFor(dto => dto.FirstName!)
            .ApplyFirstNameRules("First name cannot be empty")
            .When(dto => !string.IsNullOrEmpty(dto.FirstName));

        RuleFor(dto => dto.MiddleName.Value)
            .MaximumLength(MiddleNameMaxLength)
            .WithMessage($"Middle name must not exceed {MiddleNameMaxLength} characters")
            .When(dto => dto.MiddleName.HasValue && !string.IsNullOrEmpty(dto.MiddleName.Value));

        RuleFor(dto => dto.LastName!)
            .ApplyLastNameRules("Last name cannot be empty")
            .When(dto => !string.IsNullOrEmpty(dto.LastName));

        RuleFor(x => x.Email!)
            .Cascade(CascadeMode.Stop)
            .ApplyUpdatedEmailRules(employeeStore)
            .When(dto => !string.IsNullOrEmpty(dto.Email));

        RuleFor(dto => dto.Role)
            .IsInEnum()
            .WithMessage("Invalid role")
            .When(dto => dto.Role.HasValue);
    }
}
