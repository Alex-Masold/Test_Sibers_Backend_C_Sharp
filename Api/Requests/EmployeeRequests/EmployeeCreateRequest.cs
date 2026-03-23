using System.ComponentModel.DataAnnotations;
using Api.ValidateAttributes;
using Application.Contracts.EmployeeContracts;
using Domain.Models;

namespace Api.Requests.EmployeeRequests;

public record EmployeeCreateRequest
{
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }

    [EmailAddress(ErrorMessage = "Incorrect Email")]
    public required string Email { get; init; }

    [InEnum(typeof(Role), ErrorMessage = "The value is not included in the enumeration")]
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
