using Application.Contracts.Base;
using Domain.Models;
using Shared.Helpers;

namespace Application.Contracts.EmployeeContracts;

public record EmployeeCreateDto : ICreateDto<Employee>
{
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }

    public required Role Role { get; init; }

    public Employee ToEntity() =>
        new Employee()
        {
            FirstName = FirstName.Trim(),
            MiddleName = StringHelpers.NormalizeOrNull(MiddleName),
            LastName = LastName.Trim(),

            Email = Email.Trim(),

            Role = Role,
        };
}
