namespace Application.Contracts.EmployeeContracts;

public record ChangePasswordDto
{
    public string? CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
    public required string ConfirmNewPassword { get; init; }
}
