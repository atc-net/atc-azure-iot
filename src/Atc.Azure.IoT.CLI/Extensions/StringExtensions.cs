namespace Atc.Azure.IoT.CLI.Extensions;

public static class StringExtensions
{
    public static Dictionary<string, string> ParseToDictionary(
        this string? input)
    {
        var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrEmpty(input))
        {
            return dictionary;
        }

        var pairs = input.Split(',');
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=');
            if (keyValue.Length == 2)
            {
                dictionary[keyValue[0]] = keyValue[1];
            }
        }

        return dictionary;
    }
}