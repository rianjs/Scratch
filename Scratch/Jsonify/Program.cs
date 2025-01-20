using System.Text.Json;





var jsonOpts = Monit.Dotnet.Utils.Serialization.JsonSerializerUtils.GetFormattedJsonSettings();
var parentDir = Path.Combine("/", "Users/rianjs/Downloads");
var inPath = Path.Combine(parentDir, "2024-06-18 output.csv");
var outPath = Path.Combine(parentDir, "2024-06-18 output.csv.json");

var csv = File.ReadAllText(inPath);
var asJson = JsonSerializer.Serialize(csv, jsonOpts);
File.WriteAllText(outPath, asJson);