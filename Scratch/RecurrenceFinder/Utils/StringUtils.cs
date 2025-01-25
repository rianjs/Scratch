namespace RecurrenceFinder.Utils;

public static class StringUtils
{
    public static string ReplaceWord(this string haystack, string needle, string replace)
    {
        if (string.IsNullOrEmpty(haystack) || string.IsNullOrEmpty(needle))
        {
            return haystack;
        }

        var words = haystack.Split(' ');
        for (var i = 0; i < words.Length; i++)
        {
            if (string.Equals(words[i], needle, StringComparison.Ordinal))
            {
                words[i] = replace;
            }
        }

        return string.Join(' ', words).Trim();
    }
}