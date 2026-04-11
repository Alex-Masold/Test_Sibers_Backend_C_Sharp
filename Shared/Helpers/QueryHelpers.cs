namespace Shared.Helpers;

public static class QueryHelpers
{
    public static string NormalizeSearch(string input)
    {
        var escaped = input
            .Trim()
            .ToLowerInvariant()
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");

        return $"%{escaped}%";
    }
}
