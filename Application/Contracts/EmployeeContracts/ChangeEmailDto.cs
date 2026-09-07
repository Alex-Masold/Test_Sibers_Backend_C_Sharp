namespace Application.Contracts.EmployeeContracts;

public record ChangeEmailDto
{
    public required string NewEmail { get; init; }
}
