namespace Application.Contracts;

public record PagedDto
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
};
