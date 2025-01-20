using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using System.Text.Json;


namespace FdsDownloader;

public class FdsClient
{
    private readonly HttpClient _http;
    private readonly ILogger<FdsClient> _log;

    public FdsClient(HttpClient http, ILogger<FdsClient> log)
    {
        _http = http;
        _log = log;
    }

    public async Task<string> GetJournalEntriesAsync(string fi, string companyId, DateOnly inclusiveStart, DateOnly exclusiveEnd, string jwt, CancellationToken ct)
    {
        var partialUrl = $"financial-data/v1/accounting/journal-entries/{companyId}?inclusiveStart={inclusiveStart:O}&exclusiveEnd={exclusiveEnd:O}";
        var uri = new Uri(partialUrl, UriKind.Relative);

        var req = new HttpRequestMessage(HttpMethod.Get, uri);
        req.Headers.Add("X-Monit-Tenant", fi);
        req.Headers.Add("Authorization", $"Bearer {jwt}");

        string json;
        var timer = Stopwatch.StartNew();
        try
        {
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();

            json = await resp.Content.ReadAsStringAsync(ct);
            timer.Stop();
            _log.LogInformation($"{fi}::{companyId} at {partialUrl} having {json.Length:N0} bytes in {timer.ElapsedMilliseconds:N0}ms");

            timer = Stopwatch.StartNew();
            var formatted = GetFormattedJson(json);
            timer.Stop();
            _log.LogInformation($"{fi}::{companyId} deserialized to JsonDocument in {timer.ElapsedMilliseconds:N0}ms");
            return formatted;
        }
        catch (Exception e)
        {
            var msg = e switch
            {
                HttpRequestException h => $"Request failed with status code: {h.StatusCode}",
                _ => "Request failed for an unknown reason",
            };
            _log.LogError(e, $"{fi}::{companyId} - Failed to retrieve {partialUrl} having 0 bytes - {msg}");
            return default;
        }
    }

    private static string GetFormattedJson(string rawJson)
    {
        var options = new JsonWriterOptions { Indented = true, };

        using var jsonDocument = JsonDocument.Parse(rawJson);
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, options);
        jsonDocument.WriteTo(writer);
        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }
}