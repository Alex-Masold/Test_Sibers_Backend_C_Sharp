using Domain.Models;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators.Rules;

public static class ManagerRules
{
    public static IRuleBuilderOptions<T, int?> ApplyRoleManagerRules<T>(
        this IRuleBuilder<T, int?> ruleBuilder,
        IEmployeeStore store
    )
    {
        return ruleBuilder
            .MustAsync(
                async (managerId, ct) =>
                {
                    if (!managerId.HasValue)
                        return true;
                    var employee = await store.GetByIdAsync(managerId.Value, ct);
                    return employee is not null && employee.Role == Role.Manager;
                }
            )
            .WithMessage("Manager must exist and have Manager role");
    }
}
