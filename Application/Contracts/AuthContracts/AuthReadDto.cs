namespace Application.Contracts.AuthContracts;

public class AuthReadDto
{
    public required string AccessToken { get; init; }

    public static AuthReadDto From(string accessToken) => new() { AccessToken = accessToken };
}
