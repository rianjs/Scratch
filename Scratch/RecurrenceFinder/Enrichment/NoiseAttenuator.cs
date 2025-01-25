using System.Text.RegularExpressions;

namespace RecurrenceFinder.Enrichment;

public partial class NoiseAttenuator
{
    private const RegexOptions _normalizerOpts = RegexOptions.IgnoreCase | RegexOptions.Singleline;

    [GeneratedRegex(@"\b(DEBIT|DBT|DBT PURCHASE|CREDIT|PMT|PAYMENT|ACH)\b", _normalizerOpts)]
    private static partial Regex NoisyMatches();

    private static readonly char[] _separators = new[]{' ', '*'};
    public EnrichedTransaction Enrich(EnrichedTransaction et)
    {
        // if (et.Transaction.Description.Contains("venmo", StringComparison.OrdinalIgnoreCase))
        // {
        //     Console.WriteLine("jere");
        // }

        var eventuallyNormalized = NoisyMatches().Replace(et.NormalizedDescription, "");
        var split = eventuallyNormalized
            .Split(_separators, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Distinct(StringComparer.Ordinal);

        eventuallyNormalized = string.Join(' ', split);
        return et with { NormalizedDescription = eventuallyNormalized };
    }

    // Maybe we need an amplifier to add back in check numbers and what else?
}