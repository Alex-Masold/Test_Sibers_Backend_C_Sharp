using System.Linq.Expressions;
using Domain.Filters;
using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;
using Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Persistence.DataContext;
using Persistence.Extensions.Filters;
using Persistence.Extensions.Sort;

namespace Persistence.Repositories;

public class EmployeeRepository(ApplicationContext context) : IEmployeeStore
{
    public async Task<bool> EmailExistAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        return await context.Employees.AnyAsync(
            e => EF.Functions.Collate(e.Email, "NOCASE") == email,
            cancellationToken
        );
    }

    public async Task<bool> EmployeeExistAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return await context.Employees.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<int>> GetExistingIdsAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Employees.Where(e => idList.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await context.Employees.FindAsync([id], cancellationToken);
        return employee;
    }

    public async Task<IReadOnlyCollection<Employee>> GetRangeByIdsAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    )
    {
        if (idList == null || idList.Count == 0)
            return new List<Employee>();

        var employees = await context
            .Employees.Where(e => idList.Contains(e.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return employees;
    }

    public async Task<Employee?> GetByEmailAsync(
        string email,
        CancellationToken cancellation = default
    )
    {
        var employee = await context
            .Employees.AsNoTracking()
            .SingleOrDefaultAsync(e => e.Email == email, cancellation);
        return employee;
    }

    public async Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetPagedAsync<T>(
        int pageNumber,
        int pageSize,
        Expression<Func<Employee, T>> projection,
        EmployeeFilter? filter = null,
        SortOptions<EmployeeSortField>? options = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Employees.AsNoTracking().ApplyFilter(filter);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .ApplyOrdering(options)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(projection)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Employee Create(Employee employee)
    {
        var createdEmployee = context.Employees.Add(employee);
        return createdEmployee.Entity;
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Employees.Where(e => e.Id == id).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int> DeleteRangeAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Employees.Where(e => idList.Contains(e.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
