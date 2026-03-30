using System.Linq.Expressions;
using Domain.Filters;
using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;

namespace Domain.Stores;

public interface IEmployeeStore
{
    Task<bool> EmailExistAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmployeeExistAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<int>> GetExistingIdsAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    );
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Employee>> GetRangeByIdsAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    );
    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellation = default);
    Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetPagedAsync<T>(
        int pageNumber,
        int pageSize,
        Expression<Func<Employee, T>> projection,
        EmployeeFilter? filter = null,
        SortOptions<EmployeeSortField>? options = null,
        CancellationToken cancellationToken = default
    );

    Employee Create(Employee employee);
    Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    );
}
