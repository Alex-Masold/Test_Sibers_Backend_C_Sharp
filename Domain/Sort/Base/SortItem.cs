namespace Domain.Sort.Base;

public record SortItem<TField>(TField Field, bool Desc) where TField : struct, Enum;