namespace PRN232.LMS.API.Helpers;

public static class FieldSelectionHelper
{
    public static List<string> ParseFields(string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
        {
            return [];
        }

        return fields
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static List<string> GetInvalidFields<T>(
        IEnumerable<string> requestedFields,
        IReadOnlyDictionary<string, Func<T, object?>> selectors)
    {
        return requestedFields
            .Where(field => !selectors.ContainsKey(field))
            .ToList();
    }

    public static object Apply<T>(
        IEnumerable<T> items,
        IReadOnlyList<string> requestedFields,
        IReadOnlyDictionary<string, Func<T, object?>> selectors)
    {
        var materializedItems = items.ToList();
        if (requestedFields.Count == 0)
        {
            return materializedItems;
        }

        return materializedItems
            .Select(item => requestedFields.ToDictionary(
                field => field,
                field => selectors[field](item),
                StringComparer.OrdinalIgnoreCase))
            .ToList();
    }
}
