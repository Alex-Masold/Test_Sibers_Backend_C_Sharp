using System.Linq.Expressions;
using Application.Contracts.Base;
using Domain.Models;

namespace Application.Contracts.EmployeeContracts;

public record EmployeeReadDto : IReadDto<Employee, EmployeeReadDto>
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }

    public Role Role { get; init; }

    public static Expression<Func<Employee, EmployeeReadDto>> Projection =>
        employee =>
            new()
            {
                Id = employee.Id,

                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName,
                LastName = employee.LastName,

                Email = employee.Email,

                Role = employee.Role,
            };

    public static EmployeeReadDto From(Employee employee) =>
        new()
        {
            Id = employee.Id,

            FirstName = employee.FirstName,
            MiddleName = employee.MiddleName,
            LastName = employee.LastName,

            Email = employee.Email,

            Role = employee.Role,
        };
}
