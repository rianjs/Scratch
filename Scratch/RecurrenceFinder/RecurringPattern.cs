namespace RecurrenceFinder;

public record RecurringPattern
{
    public required string Description { get; init; }
    public required List<string> OriginalDescriptions { get; init; }
    public TimeSpan MeanInterval { get; init; }
    public decimal MeanAmount { get; init; }
    public required List<Transaction> Transactions { get; init; }

    /// <summary> 0 to 1; closer to 1 is more consistent </summary>
    public double IntervalConsistency { get; init; }

    /// <summary> 0 to 1; closer to 1 is more consistent </summary>
    public double AmountConsistency { get; init; }
}