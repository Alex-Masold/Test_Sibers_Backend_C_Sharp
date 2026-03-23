using Domain.Models;

namespace Domain.Filters;

public record EmployeeFilter
{
    public string? SearchQuery { get; init; }
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }

    public Role? Role { get; init; }

    public int? RelatedProjectId { get; init; }
}
