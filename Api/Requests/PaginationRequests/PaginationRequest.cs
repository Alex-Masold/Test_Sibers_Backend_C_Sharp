using Application.Contracts;

namespace Api.Requests.PaginationRequests;

public record PaginationRequest
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public PagedDto ToDto() => new() { PageNumber = PageNumber, PageSize = PageSize };
}
