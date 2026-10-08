using Application.Contracts.Base;
using Domain.Models;
using Shared.Helpers;

namespace Application.Contracts.AuthContracts;

public record RegisterDto : ICreateDto<Employee, (string email, Role role, string passwordHash)>
{
    public required string Token { get; init; }

    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }

    public required string Password { get; init; }
    public required string PasswordConfirm { get; init; }

    public Employee ToEntity((string email, Role role, string passwordHash) data) =>
        new()
        {
            FirstName = FirstName.Trim(),
            MiddleName = StringHelpers.NormalizeOrNull(MiddleName),
            LastName = LastName.Trim(),

            Email = data.email,
            Role = data.role,
            PasswordHash = data.passwordHash,
        };
}
