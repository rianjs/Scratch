namespace Scratch;

public static class Extensions
{
    public static DateTime ConvertToTimeZone(this DateTime dt, string sourceTz, string destTz)
    {
        var safe = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
        var sourceTzi = TimeZoneInfo.FindSystemTimeZoneById(sourceTz);
        var destTzi = TimeZoneInfo.FindSystemTimeZoneById(destTz);
        return TimeZoneInfo.ConvertTime(safe, sourceTzi, destTzi);
    }

    public static DateTimeOffset ToDateTimeOffset(this DateTime dt, string sourceTz)
    {
        var safe = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
        var sourceTzi = TimeZoneInfo.FindSystemTimeZoneById(sourceTz);
        // var intermediate = TimeZoneInfo.ConvertTime(safe, sourceTzi, sourceTzi)
        var offset = sourceTzi.IsAmbiguousTime(dt)
            ? sourceTzi.GetAmbiguousTimeOffsets(dt).First()
            : sourceTzi.GetUtcOffset(dt);
        return new DateTimeOffset(safe, offset);
    }

    public static string GetIanaTimeZone(string tzId)
        => GetIanaName(TimeZoneInfo.FindSystemTimeZoneById(tzId));

    public static string GetIanaName(TimeZoneInfo tzi)
    {
        if (tzi.HasIanaId)
        {
            return tzi.Id;
        }

        return TimeZoneInfo.TryConvertWindowsIdToIanaId(tzi.StandardName, out var maybeIana)
            ? maybeIana
            : string.Empty;
    }

    public static string GetLocalSystemIanaTimeZone()
        => GetIanaName(TimeZoneInfo.Local);

    private static readonly Lazy<Dictionary<string, TimeZoneInfo>> _zonesByIdentifiers = new(InitZonesById, LazyThreadSafetyMode.PublicationOnly);
    private static Dictionary<string, TimeZoneInfo> InitZonesById()
    {
        var byIdQuery = TimeZoneInfo.GetSystemTimeZones()
            .Select(tzi => new KeyValuePair<string, TimeZoneInfo>(tzi.Id, tzi));
        var byStandardNameQuery = TimeZoneInfo.GetSystemTimeZones()
            .Select(tzi => new KeyValuePair<string, TimeZoneInfo>(tzi.StandardName, tzi));
        var byDisplayNameQuery = TimeZoneInfo.GetSystemTimeZones()
            .Select(tzi => new KeyValuePair<string, TimeZoneInfo>(tzi.DisplayName, tzi));
        var byDaylightNameQuery = TimeZoneInfo.GetSystemTimeZones()
            .Select(tzi => new KeyValuePair<string, TimeZoneInfo>(tzi.DaylightName, tzi));

        var foo = byIdQuery
            .Concat(byStandardNameQuery)
            .Concat(byDisplayNameQuery)
            .Concat(byDaylightNameQuery)
            .GroupBy(tzi => tzi.Key)
            .ToList();
        var bar = foo.Where(i => i.Count() > 1).ToList();

        var aggregate = byIdQuery
            .Concat(byStandardNameQuery)
            .Concat(byDisplayNameQuery)
            .Concat(byDaylightNameQuery)
            .GroupBy(tzi => tzi.Key)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.First().Value, StringComparer.OrdinalIgnoreCase);
        aggregate.TrimExcess();
        return aggregate;
    }

    public static TimeZoneInfo? FindTimeZoneInfo(string tzId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(tzId);
        }
        catch (Exception)
        {
            return _zonesByIdentifiers.Value.GetValueOrDefault(tzId);
        }
    }

    // public static Dictionary<string, List<TimeZoneInfo>> ByAbbrevs()
    // {
    //     typeof(TimeZoneInfo).GetField("_abbrev", BindingFlags.NonPublic | BindingFlags.Instance)
    // }
}