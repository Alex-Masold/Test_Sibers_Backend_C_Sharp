using Domain.Filters.Base;

namespace Domain.Filters;

public record ProjectFilter
{
    public string? Name { get; init; }
    public string? CompanyOrdering { get; init; }
    public string? CompanyExecuting { get; init; }

    public RangeFilter<int>? Priority { get; init; }
    public RangeFilter<DateOnly>? StartDate { get; init; }
    public RangeFilter<DateOnly>? EndDate { get; init; }

    public int? ManagerId { get; init; }
    public int? RelatedEmployeeId { get; init; }
}
