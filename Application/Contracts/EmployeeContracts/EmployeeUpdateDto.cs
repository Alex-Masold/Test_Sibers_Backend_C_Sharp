using Application.Contracts.Base;
using Domain.Common;
using Domain.Models;

namespace Application.Contracts.EmployeeContracts;

public record EmployeeUpdateDto : IUpdateDto<Employee>
{
    public string? FirstName { get; init; }
    public Optional<string?> MiddleName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }

    public Role? Role { get; init; }

    private string? Normalize(string? str)
    {
        return string.IsNullOrWhiteSpace(str) ? null : str.Trim();
    }

    public bool ApplyTo(Employee employee)
    {
        var changed = false;

        if (FirstName is not null)
        {
            var normalized = FirstName.Trim();
            if (!string.Equals(employee.FirstName, normalized, StringComparison.Ordinal))
            {
                employee.FirstName = normalized;
                changed = true;
            }
        }

        if (MiddleName.HasValue)
        {
            var normalized = Normalize(MiddleName.Value);
            if (!string.Equals(employee.MiddleName, normalized, StringComparison.Ordinal))
            {
                employee.MiddleName = normalized;
                changed = true;
            }
        }

        if (LastName is not null)
        {
            var normalized = LastName.Trim();
            if (!string.Equals(employee.LastName, normalized, StringComparison.Ordinal))
            {
                employee.LastName = normalized;
                changed = true;
            }
        }

        if (Email is not null)
        {
            var normalized = Email.Trim();
            if (!string.Equals(employee.Email, normalized, StringComparison.Ordinal))
            {
                employee.Email = normalized;
                changed = true;
            }
        }

        if (Role is not null && employee.Role != Role.Value)
        {
            employee.Role = Role.Value;
            changed = true;
        }

        return changed;
    }
}
