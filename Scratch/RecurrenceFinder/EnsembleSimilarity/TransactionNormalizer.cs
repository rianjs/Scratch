using System.Text.RegularExpressions;

namespace RecurrenceFinder.EnsembleSimilarity;

public interface ITransactionNormalizer
{
    string Normalize(string input);
}

public partial class TransactionNormalizer : ITransactionNormalizer
{
    private const RegexOptions _normalizerOpts = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.NonBacktracking;

    [GeneratedRegex(@"\b(DEBIT|CREDIT|PMT|PAYMENT|ACH)\b", _normalizerOpts)]
    private static partial Regex TxnTypeMatch();

    [GeneratedRegex(@"\d+(?!\d{0,3}$)", _normalizerOpts)]
    private static partial Regex DigitsMatch();

    [GeneratedRegex(@"\s+", _normalizerOpts)]
    private static partial Regex ConsolidateSpacesMatch();

    private readonly Dictionary<string, string> _cache = new(StringComparer.OrdinalIgnoreCase);

    public string Normalize(string input)
    {
        if (_cache.TryGetValue(input, out var cached))
        {
            return cached;
        }

        // Basic normalization
        var normalized = input.ToUpperInvariant()
            .Replace("*", " ")
            .Replace("-", " ")
            .Replace(".", " ");

        // Remove common payment suffixes
        normalized = TxnTypeMatch().Replace(normalized, "");

        // Remove digits except trailing 4
        normalized = DigitsMatch().Replace(normalized, "");

        // Remove multiple spaces
        normalized = ConsolidateSpacesMatch().Replace(normalized, " ").Trim();

        _cache[input] = normalized;
        return normalized;
    }
}