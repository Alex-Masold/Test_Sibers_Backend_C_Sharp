namespace Domain.Common;

public readonly struct Optional<T>
{
    public T? Value { get; }
    public bool HasValue { get; }

    private Optional(T? value, bool hasValue)
    {
        Value = value;
        HasValue = hasValue;
    }

    public static Optional<T> Of(T? value) => new(value, true);

    public static Optional<T> Undefined => new(default, false);

    public static implicit operator Optional<T>(T? value) => Of(value);
}
