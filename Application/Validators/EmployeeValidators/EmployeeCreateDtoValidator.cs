using Application.Contracts.EmployeeContracts;
using Application.Validators.PasswordValidators;
using Application.Validators.Rules;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators.EmployeeValidators;

public class EmployeeCreateDtoValidator : AbstractValidator<EmployeeCreateDto>
{
    public EmployeeCreateDtoValidator(IEmployeeStore employeeStore)
    {
        RuleFor(dto => dto.FirstName).ApplyFirstNameRules();

        RuleFor(dto => dto.MiddleName)
            .ApplyMiddleNameRules()
            .When(dto => dto.MiddleName is not null);

        RuleFor(dto => dto.LastName).ApplyLastNameRules();

        RuleFor(dto => dto.Email).ApplyCreatedEmailRules(employeeStore);

        RuleFor(dto => dto.Role).IsInEnum().WithMessage("Invalid role");

        RuleFor(dto => dto.Password).SetValidator(new PasswordValidator());
    }
}
