using System.Linq.Expressions;
using Domain.Common;
using FluentValidation;

namespace Application.Validators.Rules;

public static class DateRules
{
    public static IRuleBuilderOptions<T, DateOnly> ApplyDeadlineRules<T>(
        this IRuleBuilder<T, DateOnly> ruleBuilder,
        Expression<Func<T, DateOnly>> startDateSelector
    )
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(startDateSelector)
            .WithMessage("The deadline cannot be earlier than the start date");
    }

    public static IRuleBuilderOptions<T, DateOnly?> ApplyMinStartDateRules<T> (
        this IRuleBuilder<T, DateOnly?> ruleBuilder
        )
    {
      return ruleBuilder
        .GreaterThan(new DateOnly(2000, 1,1))
        .WithMessage("Start date must be after 2000-01-01");
    }

    public static IRuleBuilderOptions<T, T> ApplyUpdateDeadlineRules<T, TEntity>(
        this IRuleBuilder<T, T> ruleBuilder,
        string contextKey,
        Func<T, DateOnly?> getDtoStart,
        Func<T, Optional<DateOnly?>> getDtoEnd,
        Func<TEntity, DateOnly> getEntityStart,
        Func<TEntity, DateOnly?> getEntityEnd)
      where TEntity: class{
        return ruleBuilder.Custom((dto, context) =>
        {
            if (context.RootContextData.TryGetValue(contextKey, out var obj) && obj is TEntity existingEntity)
            {
                var newStart = getDtoStart(dto) ?? getEntityStart(existingEntity);
                
                var dtoEnd = getDtoEnd(dto);
                var newEnd = dtoEnd.HasValue ? dtoEnd.Value : getEntityEnd(existingEntity);

                if (newEnd.HasValue && newEnd < newStart)
                {
                    context.AddFailure("EndDate", "The deadline cannot be earlier than the start date");
                }
            } 
      }
        );
};
