using Application.Contracts.LoginContracts;

namespace Api.Requests.AuthRequests;

public class AuthenticationRequest
{
    public required string Email { get; init; }

    public LoginDto ToDto() => new() { Email = Email };
}
