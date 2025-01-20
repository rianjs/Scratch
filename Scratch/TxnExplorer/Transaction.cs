namespace TxnExplorer;

// Date,Original Description,Amount,Transaction Type,Category,Account Name,Labels,Notes
public record Transaction
{
    public DateOnly Date { get; init; }
    public string Description { get; init; } = null!;
    public decimal Amount { get; init; }
    public string TransactionType { get; init; } = null!;
    public string Category { get; init; } = null!;
    public string AccountName { get; init; } = null!;
    public string? Labels { get; init; }
    public string? Notes { get; init; }
}