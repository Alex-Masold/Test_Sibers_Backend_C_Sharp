using Application.Contracts.EmployeeContracts;
using Domain.Common;
using Domain.Models;

namespace Api.Requests.EmployeeRequests;

public record EmployeeUpdateRequest
{
    public string? FirstName { get; init; }
    public Optional<string?> MiddleName { get; init; }
    public string? LastName { get; init; }

    public string? Email { get; init; }

    public Role? Role { get; init; }

    public EmployeeUpdateDto ToDto() =>
        new()
        {
            FirstName = FirstName,
            MiddleName = MiddleName,
            LastName = LastName,
            Email = Email,
            Role = Role,
        };
}
