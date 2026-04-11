namespace Application.Contracts.AuthContracts;

public record LoginDto
{
    public required string Email { get; init; }
}
