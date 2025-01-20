using System.Text;

namespace RecurrenceFinder;

public static class TransactionParser
{
    // Overload for convenience when working with files
    public static IEnumerable<Transaction> GetTransactions(Stream stream, bool skipHeaderRow = true)
    {
        var reader = new StreamReader(stream);
        return GetTransactions(reader, skipHeaderRow);
    }

    public static IEnumerable<Transaction> GetTransactions(TextReader reader, bool skipHeaderRow = true)
    {
        var started = false;
        while (reader.ReadLine() is { } line)
        {
            if (!started)
            {
                started = true;
                if (skipHeaderRow)
                {
                    continue;
                }
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            yield return ParseTransactionLine(line);
        }
    }

    private static Transaction ParseTransactionLine(string line)
    {
        var fields = new List<string>();
        var currentField = new StringBuilder(line.Length);
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            switch (line[i])
            {
                // Handle escaped quotes ("") within quoted fields, and skip the next quote
                case '"' when inQuotes && i + 1 < line.Length && line[i + 1] == '"':
                    currentField.Append('"');
                    i++;
                    break;
                case '"':
                    inQuotes = !inQuotes;
                    break;
                case ',' when !inQuotes:
                    fields.Add(currentField.ToString().Trim());
                    currentField.Clear();
                    break;
                default:
                    currentField.Append(line[i]);
                    break;
            }
        }

        // Add the last field
        fields.Add(currentField.ToString().Trim());

        var amount = decimal.Parse(fields[2]);
        var isDebit = string.Equals(fields[3], "debit", StringComparison.OrdinalIgnoreCase);
        var t = new Transaction
        {
            Date = DateOnly.Parse(fields[0]),
            OriginalDescription = fields[1],
            Amount = isDebit
                ? -amount
                : amount,
            DebitCredit = fields[3],
            Category = fields[4],
            Account = fields[5],
            Labels = fields.Count >= 6
                ? fields[6]
                : null,
            Notes = fields.Count >= 7
                ? fields[7]
                : null,
        };

        return t;
    }
}