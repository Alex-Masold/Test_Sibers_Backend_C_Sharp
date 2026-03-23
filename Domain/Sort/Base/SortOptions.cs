namespace Domain.Sort.Base;

public class SortOptions<TField>
    where TField : struct, Enum
{
    public List<SortItem<TField>> Items { get; init; } = new();
}
