using Domain.Constants;
using FluentValidation;

namespace Application.Validators.Rules;

public static class NameRules
{
    private static int FirstNameMaxLength = FieldLimits.Employee.FirstNameMaxLength;
    private static int MiddleNameMaxLength = FieldLimits.Employee.MiddleNameMaxLength;
    private static int LastNameMaxLength = FieldLimits.Employee.LastNameMaxLength;

    private static int ProjectNameMaxLength = FieldLimits.Project.NameMaxLength;

    private const int CompanyNameMaxLength = FieldLimits.Project.CompanyNameMaxLength;

    public static IRuleBuilderOptions<T, string> ApplyFirstNameRules<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string message = "First name is required"
    )
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage(message)
            .MaximumLength(FirstNameMaxLength)
            .WithMessage($"First name must not exceed {FirstNameMaxLength} characters");
    }

    public static IRuleBuilderOptions<T, string?> ApplyMiddleNameRules<T>(
        this IRuleBuilder<T, string?> ruleBuilder
    )
    {
        return ruleBuilder
            .MaximumLength(MiddleNameMaxLength)
            .WithMessage($"Middle name must not exceed {MiddleNameMaxLength} characters");
    }

    public static IRuleBuilderOptions<T, string> ApplyLastNameRules<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string message = "Last name is required"
    )
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage(message)
            .MaximumLength(LastNameMaxLength)
            .WithMessage($"Last name must not exceed {LastNameMaxLength} characters");
    }

    public static IRuleBuilderOptions<T, string> ApplyProjectNameRules<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string message = "Project name is required"
    )
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage(message)
            .MaximumLength(ProjectNameMaxLength)
            .WithMessage($"Name must not exceed {ProjectNameMaxLength} characters");
    }

    public static IRuleBuilderOptions<T, string> ApplyCompanyNameRules<T>(
        this IRuleBuilder<T, string> ruleBuilder
    )
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Company ordering is required")
            .MaximumLength(FieldLimits.Project.CompanyNameMaxLength)
            .WithMessage(
                $"Company Name must not exceed {FieldLimits.Project.CompanyNameMaxLength} characters"
            );
    }

    public static IRuleBuilderOptions<T, string?> ApplyOptionalCompanyNameRules<T>(
        this IRuleBuilder<T, string?> ruleBuilder
    )
    {
        return ruleBuilder
            .MaximumLength(FieldLimits.Project.CompanyNameMaxLength)
            .WithMessage(
                $"Company Name must not exceed {FieldLimits.Project.CompanyNameMaxLength} characters"
            );
    }
}
