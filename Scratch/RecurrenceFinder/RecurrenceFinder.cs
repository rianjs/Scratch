namespace RecurrenceFinder;

using System;
using System.Collections.Generic;
using System.Linq;

public record Transaction
{
    public DateOnly Date { get; init; }
    public required string OriginalDescription { get; init; }
    public required decimal Amount { get; init; }
    public required string DebitCredit { get; init; }
    public required string Category { get; init; }
    public required string Account { get; init; }
    public string? Labels { get; init; }
    public string? Notes { get; init; }

    public override string ToString()
    {
        var descLimit = Math.Min(16, OriginalDescription.Length);
        return $"{Date:O} - {Amount:C} - {OriginalDescription[..descLimit]} ({Category})";
    }
}

public record RecurringPattern
{
    public string Description { get; init; }
    public TimeSpan AverageInterval { get; init; }
    public decimal AverageAmount { get; init; }
    public List<Transaction> Transactions { get; init; }
    public double IntervalConsistency { get; init; } // 0 to 1, higher is more consistent
    public double AmountConsistency { get; init; }   // 0 to 1, higher is more consistent
}

public class RecurringTransactionAnalyzer
{
    private readonly decimal[] _amountThresholds = [100m, 1000m, 10000m];
    private readonly decimal[] _variationThresholds = [10m, 50m, 500m];

    private readonly TimeSpan[] _commonIntervals =
    [
        TimeSpan.FromDays(7),      // Weekly
        TimeSpan.FromDays(14),     // Bi-weekly
        TimeSpan.FromDays(30.44),  // Monthly
        TimeSpan.FromDays(91.31),  // Quarterly
        TimeSpan.FromDays(182.62), // Semi-annually
        TimeSpan.FromDays(365.25), // Annually
    ];

    public List<RecurringPattern> AnalyzeTransactions(List<Transaction> transactions)
    {
        var patterns = transactions
            .OrderBy(t => t.Date)
            .GroupBy(t => t.OriginalDescription)
            .Where(g => g.Count() >= 3)
            .SelectMany(g => IdentifyPatterns(g.ToList()))
            .OrderByDescending(p => p.IntervalConsistency * Convert.ToDouble(p.AmountConsistency))
            .ToList();

        return patterns;
    }

    private List<RecurringPattern> IdentifyPatterns(List<Transaction> transactions)
    {
        var patterns = new List<RecurringPattern>();
        var sortedTransactions = transactions.OrderBy(t => t.Date).ToList();

        // Calculate intervals between transactions
        var intervals = new List<TimeSpan>();
        for (var i = 1; i < sortedTransactions.Count; i++)
        {
            var daysBetween = sortedTransactions[i].Date.DayNumber - sortedTransactions[i - 1].Date.DayNumber;
            intervals.Add(TimeSpan.FromDays(daysBetween));
        }

        // Get potential intervals that match this transaction set
        var potentialIntervals = FindCommonIntervals(intervals);

        foreach (var interval in potentialIntervals)
        {
            // Group transactions that fit this interval pattern
            var matchingTransactions = FindTransactionsMatchingInterval(sortedTransactions, interval);

            if (matchingTransactions.Count >= 3)
            {
                var averageAmount = matchingTransactions.Average(t => t.Amount);

                // Check if amounts are within acceptable variation
                if (AreAmountsConsistent(matchingTransactions, averageAmount))
                {
                    patterns.Add(new RecurringPattern
                    {
                        Description = sortedTransactions[0].OriginalDescription,
                        AverageInterval = interval,
                        AverageAmount = averageAmount,
                        Transactions = matchingTransactions,
                        IntervalConsistency = CalculateIntervalConsistency(matchingTransactions),
                        AmountConsistency = CalculateAmountConsistency(matchingTransactions)
                    });
                }
            }
        }

        return patterns;
    }

    private List<TimeSpan> FindCommonIntervals(List<TimeSpan> intervals)
    {
        var result = new List<TimeSpan>();
        var averageInterval = TimeSpan.FromDays(intervals.Average(i => i.TotalDays));

        // Check each common interval to see if it matches the pattern
        foreach (var commonInterval in _commonIntervals)
        {
            // Calculate how well this interval matches the observed intervals
            var deviations = intervals.Select(interval =>
                Math.Abs((interval.TotalDays - commonInterval.TotalDays) / commonInterval.TotalDays));

            // If the average deviation is less than 20%, consider this a potential match
            if (deviations.Average() < 0.2)
            {
                result.Add(commonInterval);
            }
        }

        // If no common intervals match well, add the averaged actual interval
        if (!result.Any())
        {
            result.Add(averageInterval);
        }

        return result;
    }

    private List<Transaction> FindTransactionsMatchingInterval(List<Transaction> transactions, TimeSpan targetInterval)
    {
        var result = new List<Transaction>();
        var toleranceDays = GetIntervalTolerance(targetInterval);

        result.Add(transactions[0]);
        var lastMatchDate = transactions[0].Date;

        foreach (var transaction in transactions.Skip(1))
        {
            var intervalDays = transaction.Date.DayNumber - lastMatchDate.DayNumber;
            var interval = TimeSpan.FromDays(intervalDays);
            var intervalDifference = Math.Abs((interval - targetInterval).TotalDays);

            if (intervalDifference <= toleranceDays)
            {
                result.Add(transaction);
                lastMatchDate = transaction.Date;
            }
        }

        return result;
    }

    private double GetIntervalTolerance(TimeSpan interval)
    {
        // Allow more tolerance for longer intervals: 1 day for weekly, 2 days for bi-weeklly, etc.
        return interval.TotalDays switch
        {
            <= 7 => 1,
            <= 14 => 2,
            <= 32 => 3,
            <= 100 => 7,
            <= 200 => 10,
            _ => 15,
        };
    }

    private bool AreAmountsConsistent(List<Transaction> transactions, decimal averageAmount)
    {
        var threshold = GetAmountThreshold(averageAmount);
        return transactions.All(t => Math.Abs(t.Amount - averageAmount) <= threshold);
    }

    private decimal GetAmountThreshold(decimal amount)
    {
        for (var i = 0; i < _amountThresholds.Length; i++)
        {
            if (amount < _amountThresholds[i])
            {
                return _variationThresholds[i];
            }
        }

        return _variationThresholds.Last();
    }

    private double CalculateIntervalConsistency(List<Transaction> transactions)
    {
        if (transactions.Count <= 1)
        {
            return 0;
        }

        var intervals = new List<double>();
        for (var i = 1; i < transactions.Count; i++)
        {
            var intervalDays = transactions[i].Date.DayNumber - transactions[i - 1].Date.DayNumber;
            var interval = TimeSpan.FromDays(intervalDays);
            intervals.Add(interval.TotalDays);
        }

        var avgInterval = intervals.Average();
        var maxDeviation = intervals.Max(i => Math.Abs(i - avgInterval));

        // Convert to a 0-1 scale where 1 is perfect consistency
        var ratio = maxDeviation / avgInterval;
        var clamped = ratio.Clamp(0, 1);
        return 1 - clamped;
    }

    private double CalculateAmountConsistency(List<Transaction> transactions)
    {
        if (transactions.Sum(t => t.Amount) == 0m)
        {
            // Filters out test deposits and purchase + refund pairs
            return 0;
        }

        var avgAmount = transactions.Average(t => t.Amount);
        var maxDeviation = transactions.Max(t => Math.Abs(t.Amount - avgAmount));

        // Convert to a 0-1 scale where 1 is perfect consistency
        var ratio = Convert.ToDouble(maxDeviation) / Convert.ToDouble(avgAmount);
        var clamped = ratio.Clamp(0, 1);
        return 1d - clamped;
    }
}
