namespace RecurrenceFinder;

using System;
using System.Collections.Generic;
using System.Linq;

public class RecurrenceCalculator
{
    private readonly TimeSpan[] _commonIntervals =
    [
        TimeSpan.FromDays(7),      // Weekly
        TimeSpan.FromDays(14),     // Bi-weekly
        TimeSpan.FromDays(30.44),  // Monthly
        TimeSpan.FromDays(91.31),  // Quarterly
        TimeSpan.FromDays(182.62), // Semi-annually
        TimeSpan.FromDays(365.25), // Annually
    ];

    private readonly int _matchThreshold;
    private readonly double _deviationCeiling;
    public RecurrenceCalculator(int matchThreshold, double deviationCeiling)
    {
        _matchThreshold = matchThreshold;
        _deviationCeiling = deviationCeiling;
    }

    public List<RecurringPattern> AnalyzeTransactions(List<Transaction> transactions)
    {
        var patterns = transactions
            .OrderBy(t => t.Date)
            .GroupBy(t => t.OriginalDescription)
            .Where(g => g.Count() >= _matchThreshold)
            .SelectMany(g => IdentifyPatterns(g.ToList()))
            .OrderByDescending(p => p.IntervalConsistency * p.AmountConsistency)
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

            if (matchingTransactions.Count >= _matchThreshold)
            {
                var meanAmount = matchingTransactions.Average(t => t.Amount);

                // Check if amounts are within acceptable variation
                if (AreAmountsConsistent(matchingTransactions, meanAmount))
                {
                    patterns.Add(new RecurringPattern
                    {
                        Description = sortedTransactions[0].OriginalDescription,
                        OriginalDescriptions = sortedTransactions
                            .Select(t => t.OriginalDescription)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
                            .ToList(),
                        MeanInterval = interval,
                        MeanAmount = meanAmount,
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
        var meanInterval = TimeSpan.FromDays(intervals.Average(i => i.TotalDays));

        // Check each common interval to see if it matches the pattern
        foreach (var commonInterval in _commonIntervals)
        {
            // Calculate how well this interval matches the observed intervals
            var meanDeviation = intervals
                .Select(interval => Math.Abs((interval.TotalDays - commonInterval.TotalDays) / commonInterval.TotalDays))
                .Average();

            if (meanDeviation < _deviationCeiling)
            {
                result.Add(commonInterval);
            }
        }

        // If no common intervals match well, add the averaged actual interval
        if (result.Count == 0)
        {
            result.Add(meanInterval);
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

    /// <summary>
    /// Returns the interval tolerance for a interval. Longer intervals allow for more tolerance, e.g. 1 day for weekly,
    /// 2 days for bi-weekly, etc.
    /// </summary>
    private double GetIntervalTolerance(TimeSpan interval)
        => interval.TotalDays switch
        {
            <= 7 => 1,
            <= 14 => 2,
            <= 31 => 3,
            <= 100 => 7,
            <= 200 => 10,
            _ => 15,
        };

    private bool AreAmountsConsistent(List<Transaction> transactions, decimal meanAmount)
    {
        var tolerance = GetAmountThreshold(meanAmount);
        return transactions.All(t => Math.Abs(t.Amount - meanAmount) <= tolerance);
    }

    private decimal GetAmountThreshold(decimal amount)
        => amount switch
        {
            < 100m => 10m,     // Up to $100: within $10 (~10% max variation)
            < 500m => 25m,     // $100-500: within $25 (~5-25% variation)
            < 1000m => 50m,    // $500-1000: within $50 (~5-10% variation)
            < 2500m => 100m,   // $1000-2500: within $100 (~4-10% variation)
            < 5000m => 250m,   // $2500-5000: within $250 (~5-10% variation)
            < 10000m => 350m,  // $5000-10000: within $350 (~3.5-7% variation)
            _ => 500m,         // $10000+: within $500 (<5% variation)
        };

    private double CalculateIntervalConsistency(List<Transaction> transactions)
    {
        if (transactions.Count <= 1)
        {
            return 0;
        }

        var intervals = new List<double>(transactions.Count);
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
