using Application.Contracts.Base;
using Domain.Models;
using Shared.Helpers;

namespace Application.Contracts.AuthContracts;

public record RegisterDto : ICreateDto<Employee, string>, IEmployeeFields
{
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }

    public Role Role { get; init; } = Role.Worker;

    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string PasswordConfirm { get; init; }

    public Employee ToEntity(string passwordHash) =>
        new()
        {
            FirstName = FirstName.Trim(),
            MiddleName = StringHelpers.NormalizeOrNull(MiddleName),
            LastName = LastName.Trim(),

            Email = Email.Trim(),
            Role = Role,
            PasswordHash = passwordHash,
        };
}
