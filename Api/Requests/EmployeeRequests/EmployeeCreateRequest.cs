using Application.Contracts.EmployeeContracts;
using Domain.Models;

namespace Api.Requests.EmployeeRequests;

public record EmployeeCreateRequest
{
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }

    public required string Email { get; init; }

    public required Role Role { get; init; }

    public EmployeeCreateDto ToDto() =>
        new()
        {
            FirstName = FirstName,
            MiddleName = MiddleName,
            LastName = LastName,
            Email = Email,
            Role = Role,
        };
}
