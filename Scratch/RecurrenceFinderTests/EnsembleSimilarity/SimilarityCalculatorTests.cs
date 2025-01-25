using NUnit.Framework.Interfaces;
using RecurrenceFinder.Similarity;

namespace RecurrenceFinderTests.EnsembleSimilarity;

public class SimilarityCalculatorTests
{
    private static readonly SimilarityCalculator _simCalc = new();
    [Test, TestCaseSource(nameof(LevenshteinTestCases), null)]
    public int CalculateLevenshtein(string a, string b)
    {
        return _simCalc.CalculateDamerauLevenshteinDistance(a, b, int.MaxValue);
    }

    public static IEnumerable<ITestCaseData> LevenshteinTestCases()
    {
        yield return new TestCaseData("", "")
            .Returns(0)
            .SetName("Empty strings return 0");

        yield return new TestCaseData("", "test")
            .Returns(4)
            .SetName("Empty source string returns target length");

        yield return new TestCaseData("test", "")
            .Returns(4)
            .SetName("Empty target string returns source length");

        yield return new TestCaseData("test", "test")
            .Returns(0)
            .SetName("Identical strings return 0");

        yield return new TestCaseData("test", "tent")
            .Returns(1)
            .SetName("Single character difference returns 1");

        yield return new TestCaseData("test", "tset")
            .Returns(1)
            .SetName("Transposed characters return 1");

        yield return new TestCaseData("hello", "world")
            .Returns(4)
            .SetName("Different strings return max edit distance");

        yield return new TestCaseData("kitten", "sitting")
            .Returns(3)
            .SetName("kitten -> sitting = 3");

        yield return new TestCaseData("saturday", "sunday")
            .Returns(3)
            .SetName("saturday -> sunday = 3");

        yield return new TestCaseData("cake", "bake")
            .Returns(1)
            .SetName("cake -> bake = 1");

        yield return new TestCaseData("Test", "test")
            .Returns(1)
            .SetName("Case sensitivity matters");

        yield return new TestCaseData("test case", "testcase")
            .Returns(1)
            .SetName("Space differences count");
    }
}