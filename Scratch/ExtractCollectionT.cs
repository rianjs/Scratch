using System.Text.Json;

namespace Scratch;

public class ExtractCollectionT
{
    private readonly JsonSerializerOptions _jsonOpts;

    public ExtractCollectionT(JsonSerializerOptions jsonOpts)
    {
        _jsonOpts = jsonOpts;
    }

    public IEnumerable<T> Extract<T>(string json, string arrayName) // "results"
    {
        using var document = JsonDocument.Parse(json);

        // Get the root element
        var root = document.RootElement;

        // Get the "results" array
        if (root.TryGetProperty(arrayName, out var resultsElement) && resultsElement.ValueKind == JsonValueKind.Array)
        {
            // Iterate through each item in the "results" array
            foreach (var item in resultsElement.EnumerateArray())
            {
                var result = JsonSerializer.Deserialize<T>(item.GetRawText(), _jsonOpts);
                yield return result!;
            }
        }
        else
        {
            throw new JsonException("The JSON does not contain a 'results' array.");
        }
    }
}