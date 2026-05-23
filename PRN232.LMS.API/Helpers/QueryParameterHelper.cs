namespace PRN232.LMS.API.Helpers;

public static class QueryParameterHelper
{
    public static List<string> ParseCommaSeparatedValues(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }
}
