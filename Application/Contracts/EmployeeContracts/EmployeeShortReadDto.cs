namespace Application.Contracts.EmployeeContracts;

using Application.Contracts.Base;

public record EmployeeShortReadDto : ShortDto
{
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }
}
