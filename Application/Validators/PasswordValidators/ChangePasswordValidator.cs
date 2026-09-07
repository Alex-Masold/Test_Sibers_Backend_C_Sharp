using Application.Contracts.EmployeeContracts;
using FluentValidation;

namespace Application.Validators.PasswordValidators;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordValidator()
    {
        RuleFor(dto => dto.NewPassword)
            .SetValidator(new PasswordValidator())
            .When(dto => string.IsNullOrEmpty(dto.CurrentPassword));

        RuleFor(dto => dto.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Passwords do not match");

        RuleFor(dto => dto.NewPassword)
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New password must differ from current password")
            .When(dto => !string.IsNullOrEmpty(dto.CurrentPassword));
    }
}
