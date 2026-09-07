using Application.Contracts.EmployeeContracts;

namespace Api.Requests.EmployeeRequests;

public record ChangePasswordRequest
{
    public string? CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
    public required string ConfirmNewPassword { get; init; }

    public ChangePasswordDto ToDto() =>
        new()
        {
            CurrentPassword = CurrentPassword,
            NewPassword = NewPassword,
            ConfirmNewPassword = ConfirmNewPassword,
        };
}
