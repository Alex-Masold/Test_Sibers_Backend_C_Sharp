using Application.Contracts.EmployeeContracts;
using Application.Validators.PasswordValidators;
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
        Include(new EmployeeFieldsValidator(employeeStore));

        RuleFor(dto => dto.Password).SetValidator(new PasswordValidator());
    }
}
