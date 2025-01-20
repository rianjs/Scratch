using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Monit.Dotnet.Utils.Serialization;
using TenantSettings;
using Environment = TenantSettings.Environment;

var envs = new List<Environment>
{
    new()
    {
        Label = "dev",
        Url = "https://api.dev.monitapp.io/tenant-management",
        Jwt = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJYX2o5TFczcTM0dFMzVjd3b25zSk9RQ1ZSVGJYNjdJS19iaVZ2b1Y2NExZIn0.eyJleHAiOjE3MzQ2OTkwOTMsImlhdCI6MTczNDY5MTg5MywiYXV0aF90aW1lIjoxNzM0NjkxODkzLCJqdGkiOiI3MzIwMTQ0Ny01ZjNkLTRjZDctYTJhMC04OGQzZGYzNzFiMjMiLCJpc3MiOiJodHRwczovL3Nzby5kZXYubW9uaXRhcHAuaW8vcmVhbG1zL2RldiIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiI2MDU2NGEwZC04NTI2LTQ2MzUtOWU3YS02ODAwYjM5NjcxMTAiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJtb25pdC1hZG1pbi1wb3J0YWwiLCJzaWQiOiJlNjhlOTQ4My05YmMyLTRlNGItYWFjYy1lMGZiMzk2ZjA5ZGMiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHBzOi8vYXBpLmRldi5tb25pdGFwcC5pbyIsImh0dHA6Ly9sb2NhbGhvc3Q6MzAwMSJdLCJyZWFsbV9hY2Nlc3MiOnsicm9sZXMiOlsiZGVmYXVsdC1yb2xlcy1kZXYiLCJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIiwic3VwZXJ1c2VyIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJuYW1lIjoiUmlhbiBTdG9ja2Jvd2VyIiwicHJlZmVycmVkX3VzZXJuYW1lIjoicmlhbkBtb25pdGFwcC5pbyIsImdpdmVuX25hbWUiOiJSaWFuIiwiZmFtaWx5X25hbWUiOiJTdG9ja2Jvd2VyIiwiZW1haWwiOiJyaWFuQG1vbml0YXBwLmlvIn0.dPsTzCbO3roiiKMw56OwJ3N8FEaINtsXNTPGJfePOsLZbctBuaGEW-VwuTAQE-V7B5oCH5P1xy8kusAxvS8zjPWsXzVH3ggn-gpYJEOpfNkLw8MQj4wCfTDfTPspBX9w65L6_iGErEJVF4gjKNg7ve3ObtH3cXmZJO-44CXzyYOH2elc2TKY8k6ma_koR-V_mxVTw_DF-iNp3MnpONb6aGHjj97bvP2SevB6B5XY9TlAtUB_chbo5VzYKt7pgvKcqMwaauFZB1M5mddJKhNMaZIxmya6n5cwIiRjO-4tcZr-MdhbyUxOL0xyQg0zLq_7bv11pczb7CctSkBAqcEe_w",
        Tenants = new List<string>
        {
            "cfg-test", "devin-digital-garden", "elevate-test", "monit", "monit-theme-test", "narmi-sandbox-fi", "northeast-mn-test", "primary-cif-test",
            "rori-digital-garden", "tdbank-test", "thrive-test",
        },
    },
    new()
    {
        Label = "xbx",
        Url = "https://api.xbox.monitapp.io/tenant-management",
        Jwt = "",
        Tenants = new List<string>
        {
            "Apiture", "banno-test", "capone", "cfg", "elevate", "elevate-dev", "elevate-stg", "ephrata-test", "foo-test", "monit", "narmi-sandbox-fi",
            "northeast-mn", "northeast-mn-test", "palmetto-test", "pinnacle-test", "primis", "rori-digital-garden", "silverline-test", "tabbank-test", "tdamcb",
            "thrive-test", "tiburon-test",
        },
    },
    new()
    {
        Label = "prd",
        Url = "https://api.monitapp.io/tenant-management",
        Jwt = "",
        Tenants = new List<string>
        {
            "bank-of-tn", "Barlow", "capone", "cbb", "cfg", "cynergy", "digital-garden-demo", "Eastern", "eastern-test", "ecredable", "elevate",
            "first-internet-bank", "first-internet-bank-test", "first-regional-demo", "forum-cu", "fvcbank", "monit", "MONIT-APP-DFT-BANK", "monit-playground",
            "monit-test", "narmi-demo", "narmi-demo-sales", "narmi-demo-sales-c", "nayaone", "northeast-mn", "nwsb", "orionfcu", "pccsc", "pinnacle", "primis",
            "qtwo-demo", "sbdfl", "silverline-test", "stlouisbank", "tabbank", "tabbank-test", "td-test", "tdamcb", "tdbank-test", "texas-security",
            "thrive-dev-test", "thrive-sales-demo", "thrive-test", "transpecos",
        },
    },
};

// Foreach env + tenant
//  1) Add the new setting (btxn-lookback-months = 39) to the base set iff it doesn't exist
//  2) Reconcile the setting to propagate it
//  3) Delete the old setting (btxn-lookback-days) from the base set
//  4) Delete the old setting from each tenant

const string comment = "Changing btxn-lookback-days to btxn-lookback-months - https://monitproduct.atlassian.net/browse/OP-56";
var log = new Logger<TenantSettingsClient>(new LoggerFactory());
var jsonOpts = JsonSerializerUtils.GetJsonSerializerSettings();
var ct = CancellationToken.None;
const string toDelete = "btxn-lookback-days";
var defaultSetting = new TenantSetting
{
    Id = "btxn-lookback-months",
    Description = "Lookback window for calculating BTIs",
    Type = "PlatformBehavior",
    CanModify = true,
    ValueType = "Integer",
    Value = "0",
    Comment = "Initial value for setting - https://monitproduct.atlassian.net/browse/OP-56",
};

foreach (var env in envs)
{
    using var http = new HttpClient { BaseAddress = new Uri(env.Url, UriKind.Absolute), };
    http.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
    http.DefaultRequestHeaders.Add("Authorization", $"Bearer {env.Jwt}");
    var tsClient = new TenantSettingsClient(http, jsonOpts, log);

    var baseSettings = await tsClient.GetBaseSettingsAsync(ct);
    if (baseSettings.Settings.Any(s => string.Equals(s.Id, toDelete, StringComparison.OrdinalIgnoreCase)))
    {
        // PUT the new, DELETE the old
        var newBaseResp = await tsClient.PutBaseSettingAsync(defaultSetting, ct);
        var baseDelResp = await tsClient.DeleteBaseSettingAsync(toDelete, ct);
    }

    foreach (var tenant in env.Tenants)
    {
        var tenantSettings = await tsClient.GetTenantSettingsAsync(tenant, ct);
        if (tenantSettings.Settings.Any(s => string.Equals(s.Id, toDelete, StringComparison.OrdinalIgnoreCase)) == false)
        {
            continue;
        }

        // PUT the new
        // DELETE the old
    }
}