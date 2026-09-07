using Application.Contracts.AuthContracts;
using Application.Validators.PasswordValidators;
using Application.Validators.Rules;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator(IEmployeeStore employeeStore)
    {
        RuleFor(dto => dto.FirstName).ApplyFirstNameRules();

        RuleFor(dto => dto.MiddleName)
            .ApplyMiddleNameRules()
            .When(dto => dto.MiddleName is not null);

        RuleFor(dto => dto.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .ApplyLastNameRules();

        RuleFor(dto => dto.Email).ApplyCreatedEmailRules(employeeStore);

        RuleFor(dto => dto.Role).IsInEnum().WithMessage("Invalid role");

        RuleFor(dto => dto.Password).SetValidator(new PasswordValidator()!);

        RuleFor(dto => dto.PasswordConfirm)
            .Equal(dto => dto.Password)
            .WithMessage("Passwords do not match");
    }
}
