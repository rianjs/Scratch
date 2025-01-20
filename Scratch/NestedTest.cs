namespace Scratch;

public record BalanceLine
{
    public string? AccountId { get; init; }
    public List<BalanceLine>? Items { get; init; }
}

public static class BalanceLineExtensions
{
    public static IEnumerable<BalanceLine> Flatten(this BalanceLine balanceLine)
    {
        yield return balanceLine;

        foreach (var item in balanceLine.Items ?? Enumerable.Empty<BalanceLine>())
        {
            foreach (var flattenedItem in item.Flatten())
            {
                yield return flattenedItem;
            }
        }
    }
}

public class NestedTest
{
    public void Go()
    {
        var rootBalanceLine = new BalanceLine
        {
            AccountId = "Root",
            Items = new List<BalanceLine>
            {
                new()
                {
                    AccountId = "Child1",
                    Items = new List<BalanceLine>
                    {
                        new() { AccountId = "Grandchild1" },
                        new() { AccountId = "Grandchild2" }
                    }
                },
                new()
                {
                    AccountId = "Child2",
                    Items = new List<BalanceLine>
                    {
                        new() { AccountId = "Grandchild3" },
                        new()
                        {
                            AccountId = "Grandchild4", Items = new()
                            {
                                new() { AccountId = "Great grandchild" },
                            }
                        },
                    }
                }
            }
        };

        var flattenedBalanceLines = rootBalanceLine.Flatten().ToList();
        foreach (var balanceLine in flattenedBalanceLines)
        {
            Console.WriteLine(balanceLine.AccountId);
        }
    }
}