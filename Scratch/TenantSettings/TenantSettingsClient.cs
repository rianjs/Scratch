using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TenantSettings;

public class TenantSettingsClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOpts;
    private readonly ILogger<TenantSettingsClient> _log;

    public TenantSettingsClient(HttpClient http, JsonSerializerOptions jsonOpts, ILogger<TenantSettingsClient> log)
    {
        _http = http;
        _jsonOpts = jsonOpts;
        _log = log;
    }

    public async Task<TenantSettings> GetBaseSettingsAsync(CancellationToken ct)
    {
        var partialUrl = "/tenant-management/v1/settings";
        var uri = new Uri(partialUrl, UriKind.Relative);
        var req = new HttpRequestMessage(HttpMethod.Get, uri);

        var timer = Stopwatch.StartNew();
        try
        {
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(ct);
            timer.Stop();
            _log.LogInformation($"Base settings at {partialUrl} having {json.Length:N0} bytes in {timer.ElapsedMilliseconds:N0}ms");

            timer = Stopwatch.StartNew();
            var settings = JsonSerializer.Deserialize<TenantSettings>(json, _jsonOpts)!;
            timer.Stop();
            _log.LogInformation($"Base settings deserialized in {timer.ElapsedMilliseconds:N0}ms");
            return settings;
        }
        catch (Exception e)
        {
            var msg = e switch
            {
                HttpRequestException h => $"Request failed with status code: {h.StatusCode}",
                _ => "Request failed for an unknown reason",
            };
            _log.LogError(e, $"Base settings - Failed to retrieve {partialUrl} having 0 bytes - {msg}");
            throw;
        }
    }

    public Task<TenantSettings> PutBaseSettingAsync(TenantSetting s, CancellationToken ct)
    {
        var uri = $"/tenant-management/v1/settings/{s.Id}";
        return PutSettingAsync(s, uri, ct);
    }

    public Task<TenantSettings> PutTenantSettingAsync(string tenant, TenantSetting s, CancellationToken ct)
    {
        var uri = $"/tenant-management/v1/tenant/settings/{tenant}";
        return PutSettingAsync(s, uri, ct);
    }

    private async Task<TenantSettings> PutSettingAsync(TenantSetting s, string url, CancellationToken ct)
    {
        var req = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(s, _jsonOpts), Encoding.UTF8, "application/json"),
        };

        var timer = Stopwatch.StartNew();
        try
        {
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(ct);
            timer.Stop();
            _log.LogInformation($"Base settings at {req.RequestUri} having {json.Length:N0} bytes in {timer.ElapsedMilliseconds:N0}ms");

            timer = Stopwatch.StartNew();
            var settings = JsonSerializer.Deserialize<TenantSettings>(json, _jsonOpts)!;
            timer.Stop();
            _log.LogInformation($"Base settings deserialized in {timer.ElapsedMilliseconds:N0}ms");
            return settings;
        }
        catch (Exception e)
        {
            var msg = e switch
            {
                HttpRequestException h => $"Request failed with status code: {h.StatusCode}",
                _ => "Request failed for an unknown reason",
            };
            _log.LogError(e, $"Base settings - Failed to retrieve {req.RequestUri} having 0 bytes - {msg}");
            throw;
        }
    }

    public Task<bool> DeleteBaseSettingAsync(string settingId, CancellationToken ct)
    {
        var uri = $"/tenant-management/v1/settings/{settingId}";
        return DeleteSettingAsync(uri, ct);
    }

    public Task<bool> DeleteTenantSettingAsync(string settingId, CancellationToken ct)
    {
        var uri = $"/tenant-management/v1/tenant/settings/{settingId}";
        return DeleteSettingAsync(uri, ct);
    }

    private async Task<bool> DeleteSettingAsync(string url, CancellationToken ct)
    {
        var req = new HttpRequestMessage(HttpMethod.Delete, url);

        var timer = Stopwatch.StartNew();
        try
        {
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();
            timer.Stop();
            _log.LogInformation($"Base settings at {req.RequestUri} deleted in {timer.ElapsedMilliseconds:N0}ms");
            return true;
        }
        catch (Exception e)
        {
            var msg = e switch
            {
                HttpRequestException h => $"Request failed with status code: {h.StatusCode}",
                _ => "Request failed for an unknown reason",
            };
            _log.LogError(e, $"Base settings - Failed to retrieve {req.RequestUri} having 0 bytes - {msg}");
            throw;
        }
    }

    public async Task ReconcileAsync(CancellationToken ct)
    {
        var url = $"/tenant-management/v1/tenant/settings/reconcile?ignoreWarnings=false";
        var req = new HttpRequestMessage(HttpMethod.Post, url);

        var timer = Stopwatch.StartNew();
        try
        {
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();
            var response = await resp.Content.ReadAsStringAsync(ct);
            timer.Stop();
            _log.LogInformation($"Reconciled {url} in {timer.ElapsedMilliseconds:N0}ms with result: {response}");
        }
        catch (Exception e)
        {
            timer.Stop();
            var msg = e switch
            {
                HttpRequestException h => $"Request failed with status code: {h.StatusCode}",
                _ => "Request failed for an unknown reason",
            };
            _log.LogError(e, $"Failed to reconcile settings in {timer.ElapsedMilliseconds:N0}ms - {msg}");
            throw;
        }
    }

    public async Task<TenantSettings> GetTenantSettingsAsync(string fi, CancellationToken ct)
    {
        var partialUrl = $"/tenant-management/v1/tenant/settings/{fi}";
        var uri = new Uri(partialUrl, UriKind.Relative);
        var req = new HttpRequestMessage(HttpMethod.Get, uri);
        req.Headers.Add("X-Monit-Tenant", fi);

        var timer = Stopwatch.StartNew();
        try
        {
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(ct);
            timer.Stop();
            _log.LogInformation($"{fi} at {partialUrl} having {json.Length:N0} bytes in {timer.ElapsedMilliseconds:N0}ms");

            timer = Stopwatch.StartNew();
            var settings = JsonSerializer.Deserialize<TenantSettings>(json, _jsonOpts)!;
            timer.Stop();
            _log.LogInformation($"{fi} deserialized in {timer.ElapsedMilliseconds:N0}ms");
            return settings;
        }
        catch (Exception e)
        {
            var msg = e switch
            {
                HttpRequestException h => $"Request failed with status code: {h.StatusCode}",
                _ => "Request failed for an unknown reason",
            };
            _log.LogError(e, $"{fi} - Failed to retrieve {partialUrl} having 0 bytes - {msg}");
            throw;
        }
    }
}