using System.ComponentModel.DataAnnotations;
using Application.Contracts.LoginContracts;

namespace Api.Requests.AuthRequests;

public class AuthenticationRequest
{
    [EmailAddress(ErrorMessage = "Incorrect Email")]
    public required string Email { get; init; }

    public LoginDto ToDto() => new()
    {
        Email = Email
    };
}
