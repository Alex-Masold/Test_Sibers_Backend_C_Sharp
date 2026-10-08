using Application.Contracts.AuthContracts;
using Application.Validators.PasswordValidators;
using Application.Validators.Rules;
using FluentValidation;

namespace Application.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(dto => dto.FirstName).NotEmpty().ApplyFirstNameRules();
        RuleFor(dto => dto.MiddleName)
            .ApplyMiddleNameRules()
            .When(dto => dto.MiddleName is not null);
        RuleFor(dto => dto.LastName).NotEmpty().ApplyLastNameRules();

        RuleFor(dto => dto.Password).SetValidator(new PasswordValidator()!);

        RuleFor(dto => dto.PasswordConfirm)
            .Equal(dto => dto.Password)
            .WithMessage("Passwords do not match");
    }
}
