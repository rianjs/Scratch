// See https://aka.ms/new-console-template for more information

using System.Net.Mime;
using Microsoft.Net.Http.Headers;
using FdsDownloader;
using Microsoft.Extensions.Logging;

const string jwt = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJ2SlE4SGFhbDh1OFRwc3Bjd1JsZmJUWnM2MlJpRjNqOVdQc2JBWDFSay1JIn0.eyJleHAiOjE3MzEzNTI2MDcsImlhdCI6MTczMTM1MjMwNywiYXV0aF90aW1lIjoxNzMxMzM5NDg4LCJqdGkiOiI0M2NlYmZlZi01MmNiLTQwZjctYmM2Mi03MGM3OTNiZTY1ZGMiLCJpc3MiOiJodHRwczovL3Nzby5tb25pdGFwcC5pby9yZWFsbXMvcHJkIiwiYXVkIjoiYWNjb3VudCIsInN1YiI6IjFlZDI0OWQzLTc0YzYtNDVkZC04Y2EyLTlhNWU2YmQ0MjQ4ZSIsInR5cCI6IkJlYXJlciIsImF6cCI6Im1vbml0LWFkbWluLXBvcnRhbCIsIm5vbmNlIjoiODRlNWMyYjAtYjVjNC00ZDg4LThhZTUtYzQ2OWQwMjFmMThiIiwic2Vzc2lvbl9zdGF0ZSI6ImE2N2YyNjQ0LTU1OGUtNGYyZS1iNzY2LWU0ZjhkYWMzNDQ1NCIsImFjciI6IjAiLCJhbGxvd2VkLW9yaWdpbnMiOlsiaHR0cHM6Ly9hcGkubW9uaXRhcHAuaW8iXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm1vbml0OmFkbWluIiwib2ZmbGluZV9hY2Nlc3MiLCJ1bWFfYXV0aG9yaXphdGlvbiIsImRlZmF1bHQtcm9sZXMtcHJkIiwic3VwZXJ1c2VyIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCIsInNpZCI6ImE2N2YyNjQ0LTU1OGUtNGYyZS1iNzY2LWU0ZjhkYWMzNDQ1NCIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJuYW1lIjoiUmlhbiBTdG9ja2Jvd2VyIiwicHJlZmVycmVkX3VzZXJuYW1lIjoicmlhbkBtb25pdGFwcC5pbyIsImdpdmVuX25hbWUiOiJSaWFuIiwiZmFtaWx5X25hbWUiOiJTdG9ja2Jvd2VyIiwiZW1haWwiOiJyaWFuQG1vbml0YXBwLmlvIn0.lU__MwA0JmbW0yPJvsDKn9tijZf81L01KRwne0zRZ4OlTjpunqP8y6tuxo7flMaKbTf54PIpaas5Bdh6RNlAlLLNYy46SbV9OsSl8-QxjLBaoR4xrhNGPFts3K0cUrxlILKqzaWXyUbhp1IctLMpojpzmiUTz-KRKfYxFbMQpQuu-cwb2Mgq4UJr2BGD7EXZfJmMlINFGxzbeWvj_s_9gG-OUyTd4GkzjJrVlRV0sYSNkzrMH47DrBwxHacPG7wA7gxd_3ukfT1uh6Tr6alQR-ngXmjcMWlIe_-K99qC4N7j8BydmXTibgCwk-MTk4S18lCfsJxqd8OhlmiZTD4dDKGYjUSZHbzPdXPHP06HzFU35oOPtt_hzV2H8Qjf-gwJsGuLZp2bCfSP393JyMt2TrCFLnkWMPwgpAL4Svxhy0hm0j0kE8KzgS8Q3Am-pv2YU9zp_XnlMHjD-9UnBcpppOgRKoRmd7f2MEfoMkRM2maLTAR9i3a0ONHeLVYeYiARQyd0KmWek2AiSEtoYh0G2JGhCsHHknrj9ofRWLTnl7JI70GdwUIdJDbblsekruqvWAay1SOyzp5JeFx1ZRdN6BXaRslB0c9kLTt1SR8XGc26fMjWInRq28b5LoD9qyDDcoQact3MCCDx6sKudxk-43Y9iXz9TMIAoPC5muC86-w";
var companies = File.ReadLines("companies.txt")
    .Select(l => l.Trim())
    .Where(l => Guid.TryParse(l, out _))
    .ToList();
var inclusiveSearchStart = new DateOnly(2024, 05, 01);
var exclusiveSearchEnd = inclusiveSearchStart.AddMonths(1);
const string tenant = "tdamcb";

var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://api.monitapp.io", UriKind.Absolute),
};
httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
var client = new FdsClient(httpClient, new Logger<FdsClient>(new LoggerFactory()));

foreach (var company in companies)
{
    var path = Path.Combine("/", $"Users/rianjs/Downloads/exploration/{company}.json");
    var json = await client.GetJournalEntriesAsync(tenant, company, inclusiveSearchStart, exclusiveSearchEnd, jwt, CancellationToken.None);
    if (string.IsNullOrWhiteSpace(json))
    {
        continue;
    }
    File.WriteAllText(path, json);
}