using FluentValidation;

public static class PriorityRules
{
    public static FluentValidation.IRuleBuilderOptions<T, int> ApplyPriorityRules<T>(
        this FluentValidation.IRuleBuilder<T, int> ruleBuilder
    )
    {
        return ruleBuilder
            .InclusiveBetween(1, 5)
            .WithMessage($"The priority must be between 1 and 5");
    }
}
