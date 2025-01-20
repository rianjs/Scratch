// See https://aka.ms/new-console-template for more information

using System.Net;
using TxnExplorer;

var l = long.Parse("+123");

var parentDir = Path.Combine("/", "Users/rianjs/Documents");
var inFile = Path.Combine(parentDir, "rian-txns.csv");
var contents = File.ReadAllLines(inFile);
var txns = CsvParser.Parse(contents, skipFirstLine: true).ToList();

var withAsterisks = txns
    .Where(t => t.Description.Contains('*', StringComparison.Ordinal))
    .ToList();

Console.WriteLine(withAsterisks.Count);