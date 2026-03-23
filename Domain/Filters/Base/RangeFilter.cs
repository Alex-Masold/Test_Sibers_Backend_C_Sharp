namespace Domain.Filters.Base;

public record RangeFilter<T>
    where T : struct
{
    public T? Min { get; init; }
    public T? Max { get; init; }
}

