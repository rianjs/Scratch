using System.Text.Json;
using Monit.Dotnet.Utils.Serialization;
using Monit.FinancialData.Utils.Accounting;
using Monit.Insights.Utils.Sync;

var jsonOpts = JsonSerializerUtils.GetJsonSerializerSettings();
var syncInfo = new SyncInfo
{
    Action = SyncAction.Cache,
    CompanyId = Guid.NewGuid().ToString(),
    AccountingPlatform = AccountingPlatform.QuickBooksOnline,
    FinancialInstitution = "monit",
    ProviderAlias = "monit",
    CodatTenantId = "",
    PlatformLedgerId = "some-ledger-id",
    DataProviderReadySignal = true,
};

var serialized = JsonSerializer.Serialize(syncInfo, jsonOpts);
Console.WriteLine(serialized);