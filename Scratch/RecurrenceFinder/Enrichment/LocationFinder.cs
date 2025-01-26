using System.Collections.Frozen;
using RecurrenceFinder.Utils;

namespace RecurrenceFinder.Enrichment;

public class LocationFinder
{
    private readonly FrozenDictionary<string, KeyValuePair<string, string>> _loc;
    private readonly int _tokenLimit;

    // TODO: Use the location libs from benchmarking to try to get _all_ cities and towns
    public LocationFinder()
    {
        // TODO: Swap this with the Benchmarking utils method
        _loc = new Dictionary<string, KeyValuePair<string, string>>(StringComparer.Ordinal)
        {
            ["NEW YORK NY"] = new("NEW YORK", "NY"),
            ["LOS ANGELES CA"] = new("LOS ANGELES", "CA"),
            ["CHICAGO IL"] = new("CHICAGO", "IL"),
            ["HOUSTON TX"] = new("HOUSTON", "TX"),
            ["PHOENIX AZ"] = new("PHOENIX", "AZ"),
            ["PHILADELPHIA PA"] = new("PHILADELPHIA", "PA"),
            ["SAN ANTONIO TX"] = new("SAN ANTONIO", "TX"),
            ["SAN DIEGO CA"] = new("SAN DIEGO", "CA"),
            ["DALLAS TX"] = new("DALLAS", "TX"),
            ["SAN JOSE CA"] = new("SAN JOSE", "CA"),
            ["AUSTIN TX"] = new("AUSTIN", "TX"),
            ["JACKSONVILLE FL"] = new("JACKSONVILLE", "FL"),
            ["FORT WORTH TX"] = new("FORT WORTH", "TX"),
            ["COLUMBUS OH"] = new("COLUMBUS", "OH"),
            ["SAN FRANCISCO CA"] = new("SAN FRANCISCO", "CA"),
            ["CHARLOTTE NC"] = new("CHARLOTTE", "NC"),
            ["INDIANAPOLIS IN"] = new("INDIANAPOLIS", "IN"),
            ["SEATTLE WA"] = new("SEATTLE", "WA"),
            ["DENVER CO"] = new("DENVER", "CO"),
            ["WASHINGTON DC"] = new("WASHINGTON", "DC"),
            ["BOSTON MA"] = new("BOSTON", "MA"),
            ["EL PASO TX"] = new("EL PASO", "TX"),
            ["DETROIT MI"] = new("DETROIT", "MI"),
            ["NASHVILLE TN"] = new("NASHVILLE", "TN"),
            ["PORTLAND OR"] = new("PORTLAND", "OR"),
            ["MEMPHIS TN"] = new("MEMPHIS", "TN"),
            ["OKLAHOMA CITY OK"] = new("OKLAHOMA CITY", "OK"),
            ["LAS VEGAS NV"] = new("LAS VEGAS", "NV"),
            ["LOUISVILLE KY"] = new("LOUISVILLE", "KY"),
            ["BALTIMORE MD"] = new("BALTIMORE", "MD"),
            ["MILWAUKEE WI"] = new("MILWAUKEE", "WI"),
            ["ALBUQUERQUE NM"] = new("ALBUQUERQUE", "NM"),
            ["TUCSON AZ"] = new("TUCSON", "AZ"),
            ["FRESNO CA"] = new("FRESNO", "CA"),
            ["SACRAMENTO CA"] = new("SACRAMENTO", "CA"),
            ["MESA AZ"] = new("MESA", "AZ"),
            ["KANSAS CITY MO"] = new("KANSAS CITY", "MO"),
            ["ATLANTA GA"] = new("ATLANTA", "GA"),
            ["OMAHA NE"] = new("OMAHA", "NE"),
            ["COLORADO SPRINGS CO"] = new("COLORADO SPRINGS", "CO"),
            ["RALEIGH NC"] = new("RALEIGH", "NC"),
            ["VIRGINIA BEACH VA"] = new("VIRGINIA BEACH", "VA"),
            ["LONG BEACH CA"] = new("LONG BEACH", "CA"),
            ["MIAMI FL"] = new("MIAMI", "FL"),
            ["OAKLAND CA"] = new("OAKLAND", "CA"),
            ["MINNEAPOLIS MN"] = new("MINNEAPOLIS", "MN"),
            ["TULSA OK"] = new("TULSA", "OK"),
            ["ARLINGTON TX"] = new("ARLINGTON", "TX"),
            ["TAMPA FL"] = new("TAMPA", "FL"),
            ["NEW ORLEANS LA"] = new("NEW ORLEANS", "LA"),
            ["WICHITA KS"] = new("WICHITA", "KS"),
            ["CLEVELAND OH"] = new("CLEVELAND", "OH"),
            ["BAKERSFIELD CA"] = new("BAKERSFIELD", "CA"),
            ["AURORA CO"] = new("AURORA", "CO"),
            ["ANAHEIM CA"] = new("ANAHEIM", "CA"),
            ["HONOLULU HI"] = new("HONOLULU", "HI"),
            ["SANTA ANA CA"] = new("SANTA ANA", "CA"),
            ["RIVERSIDE CA"] = new("RIVERSIDE", "CA"),
            ["CORPUS CHRISTI TX"] = new("CORPUS CHRISTI", "TX"),
            ["LEXINGTON KY"] = new("LEXINGTON", "KY"),
            ["STOCKTON CA"] = new("STOCKTON", "CA"),
            ["HENDERSON NV"] = new("HENDERSON", "NV"),
            ["SAINT PAUL MN"] = new("SAINT PAUL", "MN"),
            ["CINCINNATI OH"] = new("CINCINNATI", "OH"),
            ["ST LOUIS MO"] = new("ST LOUIS", "MO"),
            ["PITTSBURGH PA"] = new("PITTSBURGH", "PA"),
            ["GREENSBORO NC"] = new("GREENSBORO", "NC"),
            ["LINCOLN NE"] = new("LINCOLN", "NE"),
            ["ANCHORAGE AK"] = new("ANCHORAGE", "AK"),
            ["PLANO TX"] = new("PLANO", "TX"),
            ["ORLANDO FL"] = new("ORLANDO", "FL"),
            ["IRVINE CA"] = new("IRVINE", "CA"),
            ["NEWARK NJ"] = new("NEWARK", "NJ"),
            ["DURHAM NC"] = new("DURHAM", "NC"),
            ["CHULA VISTA CA"] = new("CHULA VISTA", "CA"),
            ["TOLEDO OH"] = new("TOLEDO", "OH"),
            ["FORT WAYNE IN"] = new("FORT WAYNE", "IN"),
            ["ST PETERSBURG FL"] = new("ST PETERSBURG", "FL"),
            ["LAREDO TX"] = new("LAREDO", "TX"),
            ["JERSEY CITY NJ"] = new("JERSEY CITY", "NJ"),
            ["CHANDLER AZ"] = new("CHANDLER", "AZ"),
            ["MADISON WI"] = new("MADISON", "WI"),
            ["BUFFALO NY"] = new("BUFFALO", "NY"),
            ["LUBBOCK TX"] = new("LUBBOCK", "TX"),
            ["SCOTTSDALE AZ"] = new("SCOTTSDALE", "AZ"),
            ["RENO NV"] = new("RENO", "NV"),
            ["GLENDALE AZ"] = new("GLENDALE", "AZ"),
            ["GILBERT AZ"] = new("GILBERT", "AZ"),
            ["WINSTON SALEM NC"] = new("WINSTON SALEM", "NC"),
            ["NORTH LAS VEGAS NV"] = new("NORTH LAS VEGAS", "NV"),
            ["NORFOLK VA"] = new("NORFOLK", "VA"),
            ["CHESAPEAKE VA"] = new("CHESAPEAKE", "VA"),
            ["GARLAND TX"] = new("GARLAND", "TX"),
            ["IRVING TX"] = new("IRVING", "TX"),
            ["HIALEAH FL"] = new("HIALEAH", "FL"),
            ["FREMONT CA"] = new("FREMONT", "CA"),
            ["BOISE ID"] = new("BOISE", "ID"),
            ["RICHMOND VA"] = new("RICHMOND", "VA"),
            ["BATON ROUGE LA"] = new("BATON ROUGE", "LA"),
            ["SPOKANE WA"] = new("SPOKANE", "WA"),
            ["DES MOINES IA"] = new("DES MOINES", "IA"),
            ["TACOMA WA"] = new("TACOMA", "WA"),
            ["SAN BERNARDINO CA"] = new("SAN BERNARDINO", "CA"),
            ["MODESTO CA"] = new("MODESTO", "CA"),
            ["CAMBRIDGE MA"] = new("CAMBRIDGE", "MA"),
            ["PALO ALTO CA"] = new("PALO ALTO", "CA"),
            ["BETHESDA MD"] = new("BETHESDA", "MD"),
            ["ANN ARBOR MI"] = new("ANN ARBOR", "MI"),
            ["PRINCETON NJ"] = new("PRINCETON", "NJ"),
            ["WHITE PLAINS NY"] = new("WHITE PLAINS", "NY"),
            ["BELLEVUE WA"] = new("BELLEVUE", "WA"),
            ["STAMFORD CT"] = new("STAMFORD", "CT"),
            ["TYSONS VA"] = new("TYSONS", "VA"),
            ["BROOKLINE MA"] = new("BROOKLINE", "MA"),
            ["FORT LAUDERDALE FL"] = new("FORT LAUDERDALE", "FL"),
            ["GREENWICH CT"] = new("GREENWICH", "CT"),
            ["ALEXANDRIA VA"] = new("ALEXANDRIA", "VA"),
            ["BOULDER CO"] = new("BOULDER", "CO"),
            ["SANTA MONICA CA"] = new("SANTA MONICA", "CA"),
            ["MOUNTAIN VIEW CA"] = new("MOUNTAIN VIEW", "CA"),
            ["SUNNYVALE CA"] = new("SUNNYVALE", "CA"),
            ["PASADENA CA"] = new("PASADENA", "CA"),
            ["AL"] = new("", "AL"),
            ["AK"] = new("", "AK"),
            ["AZ"] = new("", "AZ"),
            ["AR"] = new("", "AR"),
            ["CA"] = new("", "CA"),
            ["CO"] = new("", "CO"),
            ["CT"] = new("", "CT"),
            ["DE"] = new("", "DE"),
            ["FL"] = new("", "FL"),
            ["GA"] = new("", "GA"),
            ["HI"] = new("", "HI"),
            ["ID"] = new("", "ID"),
            ["IL"] = new("", "IL"),
            ["IN"] = new("", "IN"),
            ["IA"] = new("", "IA"),
            ["KS"] = new("", "KS"),
            ["KY"] = new("", "KY"),
            ["LA"] = new("", "LA"),
            ["ME"] = new("", "ME"),
            ["MD"] = new("", "MD"),
            ["MA"] = new("", "MA"),
            ["MI"] = new("", "MI"),
            ["MN"] = new("", "MN"),
            ["MS"] = new("", "MS"),
            ["MO"] = new("", "MO"),
            ["MT"] = new("", "MT"),
            ["NE"] = new("", "NE"),
            ["NV"] = new("", "NV"),
            ["NH"] = new("", "NH"),
            ["NJ"] = new("", "NJ"),
            ["NM"] = new("", "NM"),
            ["NY"] = new("", "NY"),
            ["NC"] = new("", "NC"),
            ["ND"] = new("", "ND"),
            ["OH"] = new("", "OH"),
            ["OK"] = new("", "OK"),
            ["OR"] = new("", "OR"),
            ["PA"] = new("", "PA"),
            ["RI"] = new("", "RI"),
            ["SC"] = new("", "SC"),
            ["SD"] = new("", "SD"),
            ["TN"] = new("", "TN"),
            ["TX"] = new("", "TX"),
            ["UT"] = new("", "UT"),
            ["VT"] = new("", "VT"),
            ["VA"] = new("", "VA"),
            ["WA"] = new("", "WA"),
            ["WV"] = new("", "WV"),
            ["WI"] = new("", "WI"),
            ["WY"] = new("", "WY"),
            ["DC"] = new("", "DC"),
            ["AS"] = new("", "AS"),
            ["GU"] = new("", "GU"),
            ["MP"] = new("", "MP"),
            ["PR"] = new("", "PR"),
            ["VI"] = new("", "USVI"),
            ["USVI"] = new("", "USVI"),
            ["BVI"] = new("", "BVI"),
            ["ALLEGHENY PA"] = new("ALLEGHENY", "PA"),
            ["BROOKLYN NY"] = new("BROOKLYN", "NY"),
            ["CAMDEN NJ"] = new("CAMDEN", "NJ"),
            ["CANTON OH"] = new("CANTON", "OH"),
            ["CITRUS HEIGHTS CA"] = new("CITRUS HEIGHTS", "CA"),
            ["DALY CITY CA"] = new("DALY CITY", "CA"),
            ["DULUTH MN"] = new("DULUTH", "MN"),
            ["ERIE PA"] = new("ERIE", "PA"),
            ["FALL RIVER MA"] = new("FALL RIVER", "MA"),
            ["FEDERAL WAY WA"] = new("FEDERAL WAY", "WA"),
            ["FLINT MI"] = new("FLINT", "MI"),
            ["GARY IN"] = new("GARY", "IN"),
            ["HAMMOND IN"] = new("HAMMOND", "IN"),
            ["LIVONIA MI"] = new("LIVONIA", "MI"),
            ["NIAGARA FALLS NY"] = new("NIAGARA FALLS", "NY"),
            ["NORWALK CA"] = new("NORWALK", "CA"),
            ["PARMA OH"] = new("PARMA", "OH"),
            ["PORTSMOUTH VA"] = new("PORTSMOUTH", "VA"),
            ["READING PA"] = new("READING", "PA"),
            ["ROANOKE VA"] = new("ROANOKE", "VA"),
            ["SCRANTON PA"] = new("SCRANTON", "PA"),
            ["SOMERVILLE MA"] = new("SOMERVILLE", "MA"),
            ["ST JOSEPH MO"] = new("ST JOSEPH", "MO"),
            ["TRENTON NJ"] = new("TRENTON", "NJ"),
            ["UTICA NY"] = new("UTICA", "NY"),
            ["WILMINGTON DE"] = new("WILMINGTON", "DE"),
            ["YOUNGSTOWN OH"] = new("YOUNGSTOWN", "OH"),

            // Testing purposes
            ["STOW MA"] = new("STOW", "MA"),
            ["MAYNARD MA"] = new("MAYNARD", "MA"),
        }.ToFrozenDictionary();

        _tokenLimit = _loc.Keys.Max(key => key.Count(c => c == ' ')) + 1;
    }

    public EnrichedTransaction Enrich(EnrichedTransaction et)
    {
        var desc = et.NormalizedDescription;

        var bestMatch = FindBestMatch(desc);
        if (bestMatch is null)
        {
            return et;
        }

        var normalizedDescription = TrimLocation(desc, bestMatch.Value).Trim();
        return et with
        {
            City = bestMatch.Value.Key,
            StateProvince = bestMatch.Value.Value,
            NormalizedDescription = normalizedDescription,
        };
    }

    private KeyValuePair<string, string>? FindBestMatch(string input)
    {
        KeyValuePair<string, string>? bestMatch = null;
        for (var start = input.Length - 1; start >= 0; start--)
        {
            var sequence = input[start..];
            if (_loc.TryGetValue(sequence, out var location))
            {
                bestMatch = location;
            }
        }
        return bestMatch;
    }

    private string TrimLocation(string input, KeyValuePair<string, string> loc)
    {
        var buffer = string.IsNullOrEmpty(loc.Key) || string.IsNullOrEmpty(loc.Value)
            ? 0
            : 1;
        var trimLength = loc.Key.Length + loc.Value.Length + buffer;
        var substring = input[..^trimLength];
        return substring;
    }
}