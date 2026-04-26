using Application.Contracts.AuthContracts;
using Application.Validators.EmployeeValidators;
using Application.Validators.PasswordValidators;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator(IEmployeeStore employeeStore)
    {
        Include(new EmployeeFieldsValidator(employeeStore));

        RuleFor(dto => dto.Password).SetValidator(new PasswordValidator()!);

        RuleFor(dto => dto.PasswordConfirm)
            .Equal(dto => dto.Password)
            .WithMessage("Passwords do not match");
    }
}
