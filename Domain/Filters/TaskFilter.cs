using Domain.Filters.Base;
using Domain.Models;

namespace Domain.Filters;

public record TaskFilter
{
    public string? Title { get; init; }
    public string? Comment { get; init; }

    public WorkTaskStatus? Status { get; init; }

    public RangeFilter<int>? Priority { get; init; }
    public RangeFilter<DateTime>? CreatedAt { get; init; }
    public RangeFilter<DateTime>? UpdatedAt { get; init; }

    public int? AuthorId { get; init; }
    public int? ExecutorId { get; init; }
    public int? ProjectId { get; init; }
    public int? ProjectManagerId { get; init; }
}
