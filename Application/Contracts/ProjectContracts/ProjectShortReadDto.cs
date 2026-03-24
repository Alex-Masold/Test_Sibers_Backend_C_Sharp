namespace Application.Contracts.ProjectContracts;

using Application.Contracts.Base;

public record ProjectShortReadDto : ShortDto
{
    public required string Name { get; init; }
}
