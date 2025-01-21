namespace RecurrenceFinder.EnsembleSimilarity;

public interface ISimilarityCalculator
{
    double CalculateJaccardSimilarityCoefficient(string left, string right);
    HashSet<string> GenerateNgrams(string input, int chunkSize);

    /// <summary>
    /// Computes the Damerau-Levenshtein Distance between two strings, represented as arrays of integers, where each integer represents the code point of a
    /// character in the source string. Includes an optional threshold which can be used to indicate the maximum allowable distance.
    /// </summary>
    /// <param name="source">An array of the code points of the first string</param>
    /// <param name="target">An array of the code points of the second string</param>
    /// <param name="threshold">Maximum allowable distance</param>
    /// <see href="https://stackoverflow.com/a/9454016" />
    /// <returns>Int.MaxValue if threshold exceeded; otherwise the Damerau-Levenshtein distance between the strings</returns>
    int CalculateDamerauLevenshteinDistance(string source, string target, int threshold = int.MaxValue);
}

public class SimilarityCalculator : ISimilarityCalculator
{
    public double CalculateJaccardSimilarityCoefficient(string left, string right)
    {
        var leftTokens = left.Split(' ').ToHashSet(StringComparer.OrdinalIgnoreCase);
        var rightTokens = right.Split(' ').ToHashSet(StringComparer.OrdinalIgnoreCase);
        var union = leftTokens.Union(rightTokens).Count();

        if (union == 0)
        {
            return 0;
        }

        var intersectingTokens = Convert.ToDouble(leftTokens.Intersect(rightTokens).Count());
        return intersectingTokens / union;
    }

    public HashSet<string> GenerateNgrams(string input, int chunkSize)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i <= input.Length - chunkSize; i++)
        {
            result.Add(input.Substring(i, chunkSize));
        }
        return result;
    }

    public int CalculateDamerauLevenshteinDistance(string source, string target, int threshold)
    {
        var length1 = source.Length;
        var length2 = target.Length;

        // Return trivial case: difference in string lengths exceeds threshold
        if (Math.Abs(length1 - length2) > threshold)
        {
            return int.MaxValue;
        }

        // Ensure arrays [i] / length1 use shorter length
        if (length1 > length2)
        {
            (target, source) = (source, target);
            (length1, length2) = (length2, length1);
        }

        var maxi = length1;
        var maxj = length2;

        var dCurrent = new int[maxi + 1];
        var dMinus1 = new int[maxi + 1];
        var dMinus2 = new int[maxi + 1];

        for (var i = 0; i <= maxi; i++)
        {
            dCurrent[i] = i;
        }

        int jm1 = 0, im1 = 0, im2 = -1;

        for (var j = 1; j <= maxj; j++)
        {
            // Rotate
            (dMinus2, dMinus1, dCurrent) = (dMinus1, dCurrent, dMinus2);

            // Initialize
            var minDistance = int.MaxValue;
            dCurrent[0] = j;
            im1 = 0;
            im2 = -1;

            for (var i = 1; i <= maxi; i++)
            {
                var cost = source[im1] == target[jm1]
                    ? 0
                    : 1;

                var del = dCurrent[im1] + 1;
                var ins = dMinus1[i] + 1;
                var sub = dMinus1[im1] + cost;

                // Fastest execution for min value of 3 integers
                var min = del > ins
                    ? ins > sub
                        ? sub
                        : ins
                    : del > sub
                        ? sub
                        : del;

                if (i > 1
                    && j > 1
                    && source[im2] == target[jm1]
                    && source[im1] == target[j - 2])
                {
                    min = Math.Min(min, dMinus2[im2] + cost);
                }

                dCurrent[i] = min;
                if (min < minDistance)
                {
                    minDistance = min;
                }

                im1++;
                im2++;
            }
            jm1++;

            if (minDistance > threshold)
            {
                return int.MaxValue;
            }
        }

        var result = dCurrent[maxi];
        return result > threshold
            ? int.MaxValue
            : result;
    }
}