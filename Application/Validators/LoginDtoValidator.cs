using Application.Contracts.AuthContracts;
using Application.Validators.Rules;
using FluentValidation;

namespace Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email).ApplyEmailRules();

        RuleFor(dto => dto.Password).NotEmpty().WithMessage("Password is required");
    }
}
