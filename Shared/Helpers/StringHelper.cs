namespace Shared.Helpers;

public static class StringHelpers
{
    public static string? NormalizeOrNull(string? str) =>
        string.IsNullOrWhiteSpace(str) ? null : str.Trim();
}
