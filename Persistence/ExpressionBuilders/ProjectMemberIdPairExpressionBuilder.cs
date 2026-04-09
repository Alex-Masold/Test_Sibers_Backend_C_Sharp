using System.Linq.Expressions;
using Domain.Models;

namespace Persistence.ExpressionBuilders;

internal static class ProjectMemberExpressionBuilder
{
    internal static Expression<Func<ProjectMember, bool>> BuildKeyPair(
        IReadOnlyCollection<(int ProjectId, int EmployeeId)> pairs
    )
    {
        if (pairs.Count == 0)
            throw new ArgumentException("Pairs collection cannot be empty", nameof(pairs));

        var parameter = Expression.Parameter(typeof(ProjectMember), "pm");

        var projectIdProperty = Expression.Property(parameter, nameof(ProjectMember.ProjectId));
        var employeeIdProperty = Expression.Property(parameter, nameof(ProjectMember.EmployeeId));

        Expression? combined = null;

        foreach (var (projectId, employeeId) in pairs.Distinct())
        {
            var projectEquals = Expression.Equal(projectIdProperty, Expression.Constant(projectId));
            var employeeEquals = Expression.Equal(
                employeeIdProperty,
                Expression.Constant(employeeId)
            );

            var pairCondition = Expression.AndAlso(projectEquals, employeeEquals);

            combined =
                combined == null ? pairCondition : Expression.OrElse(combined, pairCondition);
        }

        return Expression.Lambda<Func<ProjectMember, bool>>(combined!, parameter);
    }
}
