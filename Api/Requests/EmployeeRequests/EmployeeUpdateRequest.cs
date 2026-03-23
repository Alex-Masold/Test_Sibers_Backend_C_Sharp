using System.ComponentModel.DataAnnotations;
using Api.ValidateAttributes;
using Application.Contracts.EmployeeContracts;
using Domain.Common;
using Domain.Models;

namespace Api.Requests.EmployeeRequests;

public record EmployeeUpdateRequest
{
    public string? FirstName { get; init; }
    public Optional<string?> MiddleName { get; init; }
    public string? LastName { get; init; }

    [EmailAddress(ErrorMessage = "Incorrect Email")]
    public string? Email { get; init; }

    [InEnum(typeof(Role), ErrorMessage = "The value is not included in the enumeration")]
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
