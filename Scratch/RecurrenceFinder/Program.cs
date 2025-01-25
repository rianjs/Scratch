// See https://aka.ms/new-console-template for more information

using RecurrenceFinder;
using RecurrenceFinder.Enrichment;
using RecurrenceFinder.Similarity;
using TransactionNormalizer = RecurrenceFinder.Similarity.TransactionNormalizer;


var path = Path.Combine("/", "Users/rianjs/Downloads/rian-txns.csv");
using var fileStream = File.OpenRead(path);
var txns = TransactionParser.GetTransactions(fileStream).ToList();

var txnNormalizer = new TransactionNormalizer();
var locationFinder = new LocationFinder();
var paymentProviderFinder = new PaymentProviderFinder();
var noiseAttenuator = new NoiseAttenuator();
var similarityFinder = new EnsembleSimilarityMatcher(new SimilarityCalculator());

var normalized = txns
    .Select(t => EnrichedTransaction.Empty() with
    {
        NormalizedDescription = txnNormalizer.Normalize(t.Description),
        Transaction = t,
    })
    .Select(et => paymentProviderFinder.Enrich(et))
    .Select(et => locationFinder.Enrich(et))
    .Select(et => noiseAttenuator.Enrich(et))
    .ToList();

var withAsterisks = normalized.Where(t => t.NormalizedDescription.Contains('*', StringComparison.Ordinal)).ToList();

withAsterisks.ForEach(t => Console.WriteLine($"{t.NormalizedDescription}\t{t.Transaction.Description}"));

var similarityLookup = new Dictionary<string, List<EnrichedTransaction>>(StringComparer.Ordinal);
for (var i = 0; i < normalized.Count; i++)
{
    var needle = normalized[i];
    if (!similarityLookup.TryGetValue(needle.NormalizedDescription, out var row))
    {
        row = [needle];
        similarityLookup[needle.NormalizedDescription] = row;
    }

    for (var j = i + 1; j < normalized.Count; j++)
    {
        var haystack = normalized[j];
        if (haystack.NormalizedDescription.Contains("PAPER STORE", StringComparison.Ordinal))
        {
            Console.WriteLine();
        }
        var score = similarityFinder.CalculateSimilarity(needle.NormalizedDescription, haystack.NormalizedDescription);
        if (score > 0.8d)
        {
            row.Add(haystack);
            normalized.RemoveAt(j);
        }
    }
}

var analyzer = new RecurrenceCalculator(3, 0.2);
var patterns = analyzer.AnalyzeTransactions(txns);

var limiter = patterns.Where(p => p is { IntervalConsistency: >= 0.8d, AmountConsistency: >= 0.8d });

foreach (var pattern in limiter)
{
    Console.WriteLine($"Found pattern for {pattern.Description}:");
    Console.WriteLine($"  Interval: {pattern.MeanInterval.TotalDays:F1} days");
    Console.WriteLine($"  Average Amount: ${pattern.MeanAmount:F2}");
    Console.WriteLine($"  Consistency Scores - Interval: {pattern.IntervalConsistency:P0}, Amount: {pattern.AmountConsistency:P0}");
    foreach (var txn in pattern.Transactions)
    {
        Console.WriteLine($"    {txn}");
    }
}

var nan = patterns.Where(p => double.IsNaN(p.IntervalConsistency)).ToList();
Console.Write(nan.Count);