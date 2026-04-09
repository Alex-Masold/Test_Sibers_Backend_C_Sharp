using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Converters;

public static class DateConverters
{
    public static readonly ValueConverter<DateTimeOffset, long> DateTimeOffsetToUnix = new(
        dt => dt.ToUnixTimeSeconds(),
        sec => DateTimeOffset.FromUnixTimeSeconds(sec)
    );

    public static readonly ValueConverter<DateTimeOffset?, long?> NullableDateTimeOfsetToUnix = new(
        dt => dt.HasValue ? dt.Value.ToUnixTimeSeconds() : null,
        sec => sec.HasValue ? DateTimeOffset.FromUnixTimeSeconds(sec.Value) : null
    );

    public static readonly ValueConverter<DateOnly, string> DateOnlyToString = new(
        d => d.ToString("yyyy-MM-dd"),
        str => DateOnly.Parse(str)
    );

    public static readonly ValueConverter<DateOnly?, string?> NullableDateOnlyToString = new(
        d => d.HasValue ? d.Value.ToString("yyyy-MM-dd") : null,
        str => string.IsNullOrEmpty(str) ? null : DateOnly.Parse(str)
    );
}
