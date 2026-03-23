using System.ComponentModel.DataAnnotations;
using Application.Contracts;

namespace Api.Requests.PaginationRequests;

public record PaginationRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "the page number must not be less than 1")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;

    public PagedDto ToDto() => new() { PageNumber = PageNumber, PageSize = PageSize };
}
