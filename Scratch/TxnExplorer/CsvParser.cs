namespace TxnExplorer;

public static class CsvParser
{
    private const int _dateCol = 0;
    private const int _origDescCol = 1;
    private const int _amountCol = 2;
    private const int _txnTypeCol = 3;
    private const int _catCol = 4;
    private const int _acctNameCol = 5;
    private const int _labelCol = 6;
    private const int _notesCol = 7;

    public static IEnumerable<Transaction> Parse(IEnumerable<string> lines, bool skipFirstLine)
        => lines.Skip(skipFirstLine ? 1 : 0).Select(l => Parse(l));

    public static Transaction Parse(string line)
    {
        var parts = line.Split(',', StringSplitOptions.TrimEntries);
        var desc = parts[_origDescCol];
        if (desc.Contains('\"', StringComparison.Ordinal))
        {

        }
        return new Transaction
        {
            Date = DateOnly.Parse(parts[_dateCol]),
            Description = parts[_origDescCol],
            Amount = decimal.Parse(parts[_amountCol]),
            TransactionType = parts[_txnTypeCol],
            Category = parts[_catCol],
            AccountName = parts[_acctNameCol],
            Labels = string.IsNullOrWhiteSpace(parts[_labelCol]) ? null : parts[_labelCol],
            Notes = string.IsNullOrWhiteSpace(parts[_notesCol]) ? null : parts[_notesCol],
        };
    }
}