using System.Text;
using System.Text.Json;
using Monit.Dotnet.Utils.Serialization;

namespace B62Compression;

public class Program
{
    public static void Main(string[] args)
    {
        var jsonOpts = JsonSerializerUtils.GetFormattedJsonSettings();

        var files = new List<string>
        {
            Path.Combine("/", "Users/rianjs/Downloads/5056-2024-txns.json.gz"),
        };

        foreach (var file in files)
        {
            Console.WriteLine(file);
            var formatted = GetString(File.ReadAllText(file), jsonOpts);
            var newFilename = file.Substring(0, file.Length - ".gz.b64".Length);
            File.WriteAllText(newFilename, formatted);
            Console.WriteLine(formatted);
            Console.WriteLine();
        }

        // var s =
        //     "H4sIAAAAAAAAA9XWW2+bMBQH8K9i8RyQuabkKU0ybZF6kZJ2D5v6YLCbWLMNsk1WVPW7z+Q2lUupijZtUp7+HBP/fDgkz5ZAnFgTa4YYEikBknBEBRUbkCWMbpCmmTBhnkltjSxNOVEa8dys8KAX2DCyXXjnwYkLJwF0vLHrxzD+ZkoLReSKMLKrbmtN4MjKkdTiVehWYYkSRpQ1+f4wsiRJCd2dgmeLKlWQBdLk/H2+7Xrm7rgRe1WcI4pvxeGKKBgbWYhnhdCLwgReCB1o9qEzjdjlPv8dGpUu1GlVtdXy5nAyc6pLcEVNvaLC7Ot49ZM5J3aJsSTKLLN4leFpaooZ5U6amTqKzYXlzVcbQnd83t0drW77iJgixoFKdbsjElcbhC+jNnLQTg7+cbLnDyF7cQs5tF2/lzyGTuTXyBe95MVyuV4BG6w5YgzMDnAF1kTuaNp5BCnCU0ypks4m2+maf0DLQ/NEf9Dvxk4c1vjH7E83/GIIeH9cdXBkPn3gyL0YO2H9IT+n3egVxRvyE5XgXlCzPWVYHfDXTj/8uDNqb2z0jsZGbbMc/Z1Z9oNB5KCdHPSRYzdwwvrDfAq7yTOkfhAN5kiRDq7aZvk02ZelpqrpjQZ5w3Zv2Of146jZ4lPY7b1G0rQWrEulCe/qcA044OV0HMoWYO+sen7Q8nt0DN9qaKkoJmDOiqSDl2SJU+WyGmrT2jI1tc2+DnhFdfbVg33ssG10w97R/YI4Zdr8/1pzqrfgSuMOPBWP2XSrNk1v/D95V+QJXBNMEfgssyJ/CyvJE68qG+IAvkv88PILOi5oO/oKAAA=";
        // Console.WriteLine(GetString(s, jsonOpts));
    }

    private static string GetString(string jsonGzipB64String, JsonSerializerOptions jsonOpts)
    {
        var compressedBytes = Convert.FromBase64String(jsonGzipB64String);
        var rawJson = CompressionUtils.DecompressGzipString(compressedBytes, Encoding.UTF8);
        var formattedJson = JsonSerializer.Serialize(JsonDocument.Parse(rawJson), jsonOpts);
        return formattedJson;
    }
}