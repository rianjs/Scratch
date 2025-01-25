namespace RecurrenceFinder;

public record EnrichedTransaction
{
    public required string NormalizedDescription { get; init; }
    public required Transaction Transaction { get; init; }
    public required List<string> Providers { get; init; }
    public required string City { get; init; }
    public required string StateProvince { get; init; }

    public static EnrichedTransaction Empty()
        => new()
        {
            NormalizedDescription = "",
            Transaction = Transaction.Empty(),
            City = "",
            StateProvince = "",
            Providers = [],
        };
}

public record Transaction
{
    public DateOnly Date { get; init; }
    public required string Description { get; init; }
    public required decimal Amount { get; init; }
    public required string DebitCredit { get; init; }
    public required string Category { get; init; }
    public required string Account { get; init; }
    public string? Labels { get; init; }
    public string? Notes { get; init; }

    public static Transaction Empty()
    {
        return new Transaction
        {
            Date = DateOnly.MinValue,
            Description = "",
            Account = "",
            Amount = 0M,
            Category = "",
            DebitCredit = "",
            Labels = "",
            Notes = "",
        };
    }

    public override string ToString()
    {
        var descLimit = Math.Min(16, Description.Length);
        return $"{Date:O} - {Amount:C} - {Description[..descLimit]} ({Category})";
    }
}