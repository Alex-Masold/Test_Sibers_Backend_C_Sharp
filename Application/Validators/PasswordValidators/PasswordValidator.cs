using Domain.Constants;
using FluentValidation;

namespace Application.Validators.PasswordValidators;

public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(password => password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(FieldLimits.Password.MinLength)
            .WithMessage($"Password must be at least {FieldLimits.Password.MinLength} characters")
            .MaximumLength(FieldLimits.Password.MaxLength)
            .WithMessage($"Password must not exceed {FieldLimits.Password.MaxLength} characters")
            .Matches(FieldLimits.Password.UppercasePattern)
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches(FieldLimits.Password.LowercasePattern)
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches(FieldLimits.Password.DigitPattern)
            .WithMessage("Password must contain at least one number")
            .Matches(FieldLimits.Password.SpecialCharacterPattern)
            .WithMessage("Password must contain at least one special character");
    }
}
