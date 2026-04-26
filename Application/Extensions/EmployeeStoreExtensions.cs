using Domain.Exceptions;
using Domain.Models;
using Domain.Stores;

namespace Application.Extensions;

public static class EmployeeStoreExtensions
{
    public static async Task<Employee> GetOrThrowAsync(
        this IEmployeeStore employeeStore,
        int employeeId,
        CancellationToken ct = default
    )
    {
        var employee = await employeeStore.GetByIdAsync(employeeId, ct);
        if (employee is null)
            throw new NotFoundException(nameof(Employee), employeeId);

        return employee;
    }

    public static async Task<Employee> GetOrThrowAsync(
        this IEmployeeStore employeeStore,
        string email,
        CancellationToken ct = default
    )
    {
        var employee = await employeeStore.GetByEmailAsync(email, ct);
        if (employee is null)
            throw new NotFoundException(nameof(Employee), email);

        return employee;
    }

    public static async Task EnsureExists(
        this IEmployeeStore employeeStore,
        int employeeId,
        CancellationToken ct = default
    )
    {
        var exist = await employeeStore.EmployeeExistsAsync(employeeId, ct);
        if (!exist)
            throw new NotFoundException(nameof(Employee), employeeId);
    }

    public static async Task<IReadOnlyCollection<int>> EnsureAllExist(
        this IEmployeeStore employeeStore,
        IReadOnlyCollection<int> employeeIdList,
        CancellationToken ct = default
    )
    {
        var distinctIdList = employeeIdList.Distinct().ToList();
        var existingEmployeeId = await employeeStore.GetExistingIdsAsync(distinctIdList, ct);

        if (existingEmployeeId.Count != distinctIdList.Count)
        {
            var nonExistingIds = distinctIdList
                .Where(id => !existingEmployeeId.Contains(id))
                .ToList();
            throw new NotFoundException(nameof(Employee), nonExistingIds);
        }

        return existingEmployeeId;
    }
}
