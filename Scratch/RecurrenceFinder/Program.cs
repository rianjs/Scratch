// See https://aka.ms/new-console-template for more information

using RecurrenceFinder;

Console.WriteLine("Hello, World!");

var path = Path.Combine("/", "Users/rianjs/Downloads/rian-txns.csv");
using var fileStream = File.OpenRead(path);
var txns = TransactionParser.GetTransactions(fileStream).ToList();

var analyzer = new RecurringTransactionAnalyzer();
var patterns = analyzer.AnalyzeTransactions(txns);

var limiter = patterns.Where(p => p is { IntervalConsistency: >= 0.8d, AmountConsistency: >= 0.8d });

foreach (var pattern in limiter)
{
    Console.WriteLine($"Found pattern for {pattern.Description}:");
    Console.WriteLine($"  Interval: {pattern.AverageInterval.TotalDays:F1} days");
    Console.WriteLine($"  Average Amount: ${pattern.AverageAmount:F2}");
    Console.WriteLine($"  Consistency Scores - Interval: {pattern.IntervalConsistency:P0}, Amount: {pattern.AmountConsistency:P0}");
    foreach (var txn in pattern.Transactions)
    {
        Console.WriteLine($"    {txn}");
    }
}

var nan = patterns.Where(p => double.IsNaN(p.IntervalConsistency)).ToList();
Console.Write(nan.Count);