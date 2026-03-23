namespace Application.Contracts.LoginContracts;

public record LoginDto
{
    public required string Email { get; init; }
}
