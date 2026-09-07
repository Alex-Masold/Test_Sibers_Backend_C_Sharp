using System.ComponentModel.DataAnnotations;
using Application.Contracts.EmployeeContracts;

namespace Api.Requests.EmployeeRequests;

public record ChangeEmailRequest
{
    [EmailAddress(ErrorMessage = "Incorect Email")]
    public required string NewEmail { get; init; }

    public ChangeEmailDto ToDto() => new() { NewEmail = NewEmail };
}
