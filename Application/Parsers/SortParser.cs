using Domain.Sort.Base;

namespace Application.Parsers;

public static class SortParser<TField>
    where TField : struct, Enum
{
    public static List<SortItem<TField>> Parse(string? sortQuery)
    {
        var result = new List<SortItem<TField>>();
        if (string.IsNullOrWhiteSpace(sortQuery))
            return result;

        var stringSortItems = sortQuery.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var token in stringSortItems)
        {
            var trimmed = token.Trim();
            var descFlag = trimmed.StartsWith('-');

            var normalized = descFlag ? trimmed[1..] : trimmed;

            if (Enum.TryParse<TField>(normalized, ignoreCase: true, out var field))
            {
                result.Add(new SortItem<TField>(field, descFlag));
            }
        }

        return result;
    }
}
