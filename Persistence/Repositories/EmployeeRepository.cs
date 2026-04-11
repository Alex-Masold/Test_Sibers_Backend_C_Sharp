using System.Linq.Expressions;
using Domain.Filters;
using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;
using Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Persistence.DataContext;
using Persistence.Extensions.Filters;
using Persistence.Extensions.Helpers;
using Persistence.Extensions.Sort;

namespace Persistence.Repositories;

public class EmployeeRepository(ApplicationContext context) : IEmployeeStore
{
    public async Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        return await context.Employees.AnyAsync(e => e.Email == email, cancellationToken);
    }

    public async Task<bool> EmployeeExistsAsync(
        int employeeId,
        CancellationToken cancellationToken = default
    )
    {
        return await context.Employees.AnyAsync(e => e.Id == employeeId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<int>> GetExistingIdsAsync(
        IReadOnlyCollection<int> employeeIdList,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Employees.Where(e => employeeIdList.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// return the tracked object for updating via UnitOfWork.
    /// </summary>
    public async Task<Employee?> GetByIdAsync(
        int employeeId,
        CancellationToken cancellationToken = default
    )
    {
        var employee = await context.Employees.FirstOrDefaultAsync(
            e => e.Id == employeeId,
            cancellationToken
        );

        return employee;
    }

    public async Task<IReadOnlyCollection<Employee>> GetRangeByIdsAsync(
        IReadOnlyCollection<int> employeeIdList,
        CancellationToken cancellationToken = default
    )
    {
        if (employeeIdList == null || employeeIdList.Count == 0)
            return new List<Employee>();

        var employees = await context
            .Employees.Where(e => employeeIdList.Contains(e.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return employees;
    }

    public async Task<Employee?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        var employee = await context
            .Employees.AsNoTracking()
            .SingleOrDefaultAsync(e => e.Email == email, cancellationToken);

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

        var result = await query
            .ApplyOrdering(options)
            .ToPagedListAsync(pageNumber, pageSize, projection, cancellationToken);

        return result;
    }

    public Employee Create(Employee employee)
    {
        var createdEmployee = context.Employees.Add(employee);
        return createdEmployee.Entity;
    }

    public async Task<int> DeleteAsync(
        int employeeId,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Employees.Where(e => e.Id == employeeId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int> DeleteAsync(
        IReadOnlyCollection<int> employeeIdList,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Employees.Where(e => employeeIdList.Contains(e.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
