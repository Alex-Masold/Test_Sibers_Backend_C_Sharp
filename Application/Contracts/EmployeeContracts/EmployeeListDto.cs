using System.Linq.Expressions;
using Application.Contracts.Base;
using Domain.Models;

namespace Application.Contracts.EmployeeContracts;

public record EmployeeListDto : IReadDto<Employee, EmployeeListDto>
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required int TaskTotalCount { get; init; }

    public Role Role { get; init; }

    public static Expression<Func<Employee, EmployeeListDto>> Projection =>
        employee => new EmployeeListDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            MiddleName = employee.MiddleName,
            LastName = employee.LastName,
            Email = employee.Email,
            TaskTotalCount = employee.AssignedTasks.Count,
            Role = employee.Role,
        };

    public static EmployeeListDto From(Employee employee) =>
        new()
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            MiddleName = employee.MiddleName,
            LastName = employee.LastName,
            Email = employee.Email,
            TaskTotalCount = employee.AssignedTasks.Count,
            Role = employee.Role,
        };
}
