using Application.Contracts.AuthContracts;

namespace Api.Requests.AuthRequests;

public class AuthenticationRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }

    public LoginDto ToDto() => new() { Email = Email, Password = Password };
}
