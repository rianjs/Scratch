using System.Text.RegularExpressions;

namespace RecurrenceFinder.Similarity;

public interface ITransactionNormalizer
{
    string Normalize(string input);
}

public partial class TransactionNormalizer : ITransactionNormalizer
{
    private const RegexOptions _normalizerOpts = RegexOptions.IgnoreCase | RegexOptions.Singleline;

    [GeneratedRegex(@"(?<!\d)\d{1,4}(?!\d)", _normalizerOpts)]
    private static partial Regex DigitsMatch();

    [GeneratedRegex(@"\s+", _normalizerOpts)]
    private static partial Regex ConsolidateSpacesMatch();

    private readonly Dictionary<string, string> _cache = new(StringComparer.Ordinal);

    public string Normalize(string input)
    {
        if (_cache.TryGetValue(input, out var cached))
        {
            return cached;
        }

        // Basic normalization
        var normalized = input
            // .Replace('-', ' ')
            .Replace("-", "")
            .Replace(',', ' ')
            .Replace('#', ' ')
            .Replace("\'", "")
            .ToUpperInvariant();

        // Remove common payment suffixes
        // normalized = TxnTypeMatch().Replace(normalized, "");

        // Remove digits except trailing 4
        normalized = DigitsMatch().Replace(normalized, "");

        // Remove multiple spaces
        normalized = ConsolidateSpacesMatch().Replace(normalized, " ").Trim();

        _cache[input] = normalized;
        return normalized;
    }
}