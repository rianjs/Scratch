namespace RecurrenceFinder.EnsembleSimilarity;

public partial class EnsembleSimilarityMatcher
{
    private readonly ITransactionNormalizer _txnNormalizer;
    private readonly ISimilarityCalculator _calc;

    private readonly int _ngramChunkLength;
    private readonly double _levenshteinWeight;
    private readonly double _wordWeight;
    private readonly double _ngramWeight;

    public EnsembleSimilarityMatcher(ITransactionNormalizer txnNormalizer, ISimilarityCalculator calc, int ngramChunkLength = 3, double levenshteinWeight = 0.4d, double wordWeight = 0.4d, double ngramWeight = 0.2d)
    {
        _txnNormalizer = txnNormalizer;
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
        var leftNormal = _txnNormalizer.Normalize(left);
        var rightNormal = _txnNormalizer.Normalize(right);

        var levenScore = LevenshteinSimilarity(leftNormal, rightNormal);
        var wordScore = _calc.CalculateJaccardSimilarityCoefficient(leftNormal, rightNormal);
        var ngramScore = NgramSimilarity(leftNormal, rightNormal, _ngramChunkLength);

        return _levenshteinWeight * levenScore
            + _wordWeight * wordScore
            + _ngramWeight * ngramScore;
    }

    private double NgramSimilarity(string left, string right, int n)
    {
        var ngrams1 = _calc.GenerateNgrams(left, n);
        var ngrams2 = _calc.GenerateNgrams(right, n);

        var union = ngrams1.Union(ngrams2).Count();
        if (union == 0)
        {
            return 0;
        }

        var intersection = Convert.ToDouble(ngrams1.Intersect(ngrams2).Count());
        return intersection / union;
    }

    private double LevenshteinSimilarity(string left, string right)
    {
        var distance = _calc.CalculateDamerauLevenshteinDistance(left, right);
        var maxLength = Math.Max(left.Length, right.Length);
        return 1 - Convert.ToDouble(distance) / maxLength;
    }
}