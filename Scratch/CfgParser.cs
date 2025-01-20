using System.Text.RegularExpressions;

namespace Scratch;

public partial class CfgParser
{
    private static readonly Regex _braceExtractor = BraceExtractor();
    [GeneratedRegex(@"(?<=\{)(.*?)(?=\})", RegexOptions.Multiline | RegexOptions.Compiled)]
    private static partial Regex BraceExtractor();

//     const string test = @"{companyName=UNIT2 TESTING FIRST BUSINESS COM, householdId=null, persistentId=854861399108621653, relationshipManager=null}
// {companyName=kira\ dosa in	c, householdId=null, persistentId=923361184985515882, relationshipManager=E019405}";

    public IEnumerable<CfgBusiness> Parse(string s)
    {
        //{companyName=UNIT2 TESTING FIRST BUSINESS COM, householdId=null, persistentId=854861399108621653, relationshipManager=null}
        // {companyName=kira\ dosa in	c, householdId=null, persistentId=923361184985515882, relationshipManager=E019405}
        var objects = _braceExtractor.Matches(s);

        return objects.Select(o => o.Value)
            .Select(b => Normalize(b))
            .Select(b => ParseBiz(b));
    }


    private static readonly Regex _toEmpty = ToEmptyNormalization();
    [GeneratedRegex(@"[\t\n\r]+", RegexOptions.Multiline | RegexOptions.Compiled)]
    private static partial Regex ToEmptyNormalization();

    private string Normalize(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        var partial = raw.Replace("\\ ", " ");
        return _toEmpty.Replace(partial, string.Empty);
    }

    private CfgBusiness ParseBiz(string serialized)
    {
        var propertiesQuery = serialized.Split(',', StringSplitOptions.TrimEntries)
            .Select(properties => properties.Split('=', StringSplitOptions.TrimEntries))
            .Select (kvp => new KeyValuePair<string?, string?>(kvp[0], kvp[1]));

        var business = new CfgBusiness();
        foreach (var kvp in propertiesQuery)
        {
            if (string.Equals("companyName", kvp.Key, StringComparison.OrdinalIgnoreCase))
            {
                business = business with { CompanyName = kvp.Value };
            }
            else if (string.Equals("householdId", kvp.Key, StringComparison.OrdinalIgnoreCase))
            {
                business = business with { HouseholdId = kvp.Value };
            }
            else if (string.Equals("persistentId", kvp.Key, StringComparison.OrdinalIgnoreCase))
            {
                business = business with { PersistentId = kvp.Value };
            }
            else if (string.Equals("relationshipManager", kvp.Key, StringComparison.OrdinalIgnoreCase))
            {
                business = business with { RelationshipManager = kvp.Value };
            }
        }

        return business;
    }
}

public record CfgBusiness
{
    public string? CompanyName { get; init; }
    public string? HouseholdId { get; init; }
    public string? PersistentId { get; init; }
    public string? RelationshipManager { get; init; }
}