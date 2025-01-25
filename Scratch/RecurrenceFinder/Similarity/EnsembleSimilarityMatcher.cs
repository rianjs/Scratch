namespace RecurrenceFinder.Similarity;

public interface IEnsembleSimilarityMatcher
{
    /// <summary>
    /// Compares strings or tokens (such as ngrams) using a weighted similarity algorithm.
    /// </summary>
    /// <param name="left">Normalized comparison token</param>
    /// <param name="right">Normalized comparison token</param>
    /// <returns>Typically values > ~0.8 are considered "the same"</returns>
    double CalculateSimilarity(string left, string right);
}

public class EnsembleSimilarityMatcher : IEnsembleSimilarityMatcher
{
    private readonly ISimilarityCalculator _calc;

    private readonly int _ngramChunkLength;
    private readonly double _levenshteinWeight;
    private readonly double _wordWeight;
    private readonly double _ngramWeight;

    public EnsembleSimilarityMatcher(ISimilarityCalculator calc, int ngramChunkLength = 3, double levenshteinWeight = 0.4d, double wordWeight = 0.4d, double ngramWeight = 0.2d)
    {
        _calc = calc;
        _ngramChunkLength = ngramChunkLength;

        var weightSum = levenshteinWeight + wordWeight + ngramWeight;
        if (Math.Abs(weightSum - 1.0d) > 1e-10)
        {
            var msg = $"Weights should add up to 1.0 {levenshteinWeight} + {wordWeight} + {ngramWeight} = {weightSum}";
            throw new ArgumentException(msg);
        }

        _levenshteinWeight = levenshteinWeight;
        _wordWeight = wordWeight;
        _ngramWeight = ngramWeight;
    }

    public double CalculateSimilarity(string left, string right)
    {
        var levenScore = _calc.CalculateLevenshteinSimilarityRatio(left, right);
        var wordScore = _calc.CalculateJaccardSimilarityCoefficient(left, right);
        var ngramScore = CalculateNgramScore(left, right);

        return _levenshteinWeight * levenScore
            + _wordWeight * wordScore
            + _ngramWeight * ngramScore;
    }

    private double CalculateNgramScore(string left, string right)
    {
        var leftNgrams = _calc.GenerateNgrams(left, _ngramChunkLength);
        var rightNgrams = _calc.GenerateNgrams(right, _ngramChunkLength);
        return _calc.CalculateJaccardSimilarityCoefficient(leftNgrams, rightNgrams);
    }
}