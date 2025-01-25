namespace RecurrenceFinder.Enrichment;

public class PaymentProviderFinder
{
    private readonly IReadOnlyList<MatchRule> _matchRules;

    public PaymentProviderFinder()
    {
        var rules = new List<MatchRule>
        {
            new()
            {
                Provider = "SQUARE",
                Strings = ["SQ *", "SQU*", "SQUARE*"],
                ClearableTokens = ["SQ", "GOSQ COM", "GOSQ.COM", "GOSQ"],
            },
            new()
            {
                Provider = "APPLE PAY",
                Strings = ["APLPAY", "APL*"],
                ClearableTokens = ["APL"],
            },
            new()
            {
                Provider = "TOAST",
                Strings = ["TST*"],
                ClearableTokens = ["TST"],
            },
            new()
            {
                Provider = "PAYPAL",
                Strings = ["PP*", "PAYPAL *", "PAYPAL"],
                ClearableTokens = ["PP", "PAYPAL", "INST", "XFER", "TRANSFER"],
            },
            new()
            {
                Provider = "SHOPIFY",
                Strings = ["SP *", "SP*", "SHOP*"],
                ClearableTokens = ["SP", "SHOPIFY"],
            },
            new()
            {
                Provider = "CLOVER",
                Strings = ["CLV*", "CLV_", "CLOVER*"],
                ClearableTokens = ["CLOVER"],
            },
            new()
            {
                Provider = "KLARNA",
                Strings = ["KLN*", "KLARNA*"],
                ClearableTokens = ["KLARNA"],
            },
            new()
            {
                Provider = "AFFIRM",
                Strings = ["AFF*", "AFFIRM*"],
                ClearableTokens = ["AFFIRM"],
            },
            new()
            {
                Provider = "AFTERPAY",
                Strings = ["APY*", "AFTERPAY*"],
                ClearableTokens = ["AFFIRM"],
            },
            new()
            {
                Provider = "ZIP (QUADPAY)",
                Strings = ["ZIP*", "QUADPAY*"],
                ClearableTokens = ["ZIP", "QUADPAY"],
            },
            new()
            {
                Provider = "FIRST SOURCE PAYMENTS",
                Strings = ["FSP*"],
                ClearableTokens = [],
            },
            new()
            {
                Provider = "PRIORITY PAYMENT SYSTEMS",
                Strings = ["PTI*"],
                ClearableTokens = [],
            },
            new()
            {
                Provider = "BRAINTREE PAYMENT",
                Strings = ["BT*"],
                ClearableTokens = [],
            },
            new()
            {
                Provider = "INTEGRATED CREDIT PROCESSING",
                Strings = ["ICP*"],
                ClearableTokens = [],
            },
            new()
            {
                Provider = "DIGITAL RIVER",
                Strings = ["DRI*"],
                ClearableTokens = [],
            },
            new()
            {
                Provider = "AMAZON PAYMENTS",
                Strings = ["AMZ*"],
                ClearableTokens = ["AMAZON PAYMENTS"],
            },
            new()
            {
                Provider = "ADYEN",
                Strings = ["ADY*"],
                ClearableTokens = [],
            },
            new()
            {
                Provider = "STRIPE",
                Strings = ["STL*"],
                ClearableTokens = [],
            },
            new()
            {
                Provider = "WEPAY",
                Strings = ["WPY*"],
                ClearableTokens = [],
            },
            new()
            {
                Provider = "TELEFLORA",
                Strings = ["TFL*"],
                ClearableTokens = [],
            },
            new()
            {
                Provider = "VERIZON CONNECTED SERVICES",
                Strings = ["VSC*", "VZWRLSS*", "VZC*"],
                ClearableTokens = [],
            },
            new()
            {
                Provider = "BLUESNAP",
                Strings = ["BB*", "BB *", "BLUESNAP*"],
                ClearableTokens = ["BB"],
            },
            new()
            {
                Provider = "FLIPDISH",
                Strings = ["+35316972801",],
                ClearableTokens = ["35316972801"],
            },
            new()
            {
                Provider = "VENMO",
                Strings = ["VENMO"],
                ClearableTokens = [],
            },
            new()
            {
                Provider = "",
                Strings = ["ACH"],
                ClearableTokens = [],
                Rail = "ACH",
            },
        };

        rules.TrimExcess();
        _matchRules = rules;
    }

    public EnrichedTransaction Enrich(EnrichedTransaction rawTxn)
    {
        var providers = new List<string>();
        var toClear = new List<string>();
        foreach (var rule in _matchRules)
        {
            var isMatch = rule.Strings.Any(r => rawTxn.NormalizedDescription.Contains(r, StringComparison.Ordinal));
            if (!isMatch)
            {
                continue;
            }

            providers.Add(rule.Provider);
            toClear.AddRange(rule.Strings);
            toClear.AddRange(rule.ClearableTokens);
        }

        if (toClear.Count == 0)
        {
            return rawTxn;
        }

        var asTokens = rawTxn.NormalizedDescription
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Where(t => !toClear.Any(d => string.Equals(t, d, StringComparison.Ordinal)))
            .Where(t => !string.IsNullOrEmpty(t));
        var normalizedDesc = string.Join(' ', asTokens);

        return rawTxn with
        {
            NormalizedDescription = normalizedDesc,
            Providers = providers,
        };
    }

    private record MatchRule
    {
        public required string Provider { get; init; }

        /// <summary>
        /// Prefixes should also be cleared from the beginning of strings in addition to any additional ClearableTokens
        /// </summary>
        public required List<string> Strings { get; init; }

        /// <summary>
        /// Break the word into tokens, then clear a token whenever there's a match with an element of this collection.
        /// </summary>
        public required List<string> ClearableTokens { get; init; }

        public string? Rail { get; init; }
    }
}