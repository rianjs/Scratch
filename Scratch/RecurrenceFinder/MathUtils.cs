namespace RecurrenceFinder;

public static class MathUtils
{
    public static double Clamp(this double value, double min, double max)
        => Math.Max(min, Math.Min(max, value));

    public static decimal Clamp(this decimal value, decimal min, decimal max)
        => Math.Max(min, Math.Min(max, value));
}