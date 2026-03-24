namespace Shared.Helpers;

public static class QueryHelpers
{
    public static string NormalizeSearch(string input) => input.Trim().ToLower();
}
