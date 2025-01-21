namespace RecurrenceFinder;

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