#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2005, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Mathematics;

public static class Statistics
{
    public class SampleStatistics
    {
        public const double Z90 = 1.645;
        public const double Z95 = 1.960;
        public const double Z98 = 2.326;
        public const double Z99 = 2.576;

        public readonly double Average;
        public readonly double N;
        public readonly double Sum;
        public readonly double SumSquared;

        public SampleStatistics(double[] array)
        {
            N = array.Length;
            Sum = array.Sum();
            Average = Sum / N;
            SumSquared = array.Sum(x => (x - Average) * (x - Average));
        }

        public double Stdev => Math.Sqrt(Variance);
        public double Variance => SumSquared / (N - 1);
        public double StandardError => Stdev / Math.Sqrt(N);

        public double StdevP => Math.Sqrt(VarianceP);
        public double VarianceP => SumSquared / N;

        public double Upper95ConfidenceLevel => Average + Z95 * StandardError;
        public double Lower95ConfidenceLevel => Average - Z95 * StandardError;
        public double Upper99ConfidenceLevel => Average + Z99 * StandardError;
        public double Lower99ConfidenceLevel => Average - Z99 * StandardError;
    }

    #region Methods

    public static float Sum(params float[] numbers)
    {
        double sum = 0;
        for (int i = 0; i < numbers.Length; i++)
            sum += numbers[i];
        return (float)sum;
    }

    public static double Average(params double[] numbers) => Sum(numbers) / numbers.Length;

    public static double Sum(params double[] numbers)
    {
        double sum = 0;
        foreach (double t in numbers)
            sum += t;
        return sum;
    }

    public static double SumSquared(params double[] numbers)
    {
        double sum = 0;
        foreach (double n in numbers)
            sum += n * n;
        return sum;
    }

    public static int Sum(params int[] numbers)
    {
        int sum = 0;
        foreach (int t in numbers)
            sum += t;
        return sum;
    }

    [DebuggerStepThrough]
    public static double Variance(double[] x) => Covariance(x, x);

    [DebuggerStepThrough]
    public static double Stdev(double[] x) => Math.Sqrt(Variance(x));

    [DebuggerStepThrough]
    public static double RSquared(double[] x, double[] y)
    {
        double cov = Covariance(x, y);
        return cov * cov / (Variance(x) * Variance(y));
    }

    [DebuggerStepThrough]
    public static double Correlation(double[] x, double[] y) => Math.Sqrt(RSquared(x, y));

    public static double Covariance(double[] x, double[] y)
    {
        int n = x.Length;
        if (n != y.Length)
            throw new ArgumentException();

        double sumxy = 0;
        double sumx = Sum(x);
        double sumy = x == y ? sumx : Sum(y);
        double averagex = sumx / n;
        double averagey = sumy / n;

        for (int i = 0; i < n; i++)
            sumxy += (x[i] - averagex) * (y[i] - averagey);

        return sumxy / n;
    }

    public static void FindLine(double[] x, double[] y, out double slope, out double bias)
    {
        int n = x.Length;
        if (n != y.Length)
            throw new ArgumentException();

        double sumx = Sum(x);
        double sumy = Sum(y);
        double averagex = sumx / n;
        double averagey = sumy / n;
        double wnum = 0;
        double den = 0;
        double sumx2 = 0;
        double sumxy = 0;

        for (int i = 0; i < n; i++) {
            double diffx = x[i] - averagex;
            double diffy = y[i] - averagey;

            den += diffx * diffx;
            wnum += diffx * diffy;
            sumx2 += x[i] * x[i];
            sumxy += x[i] * y[i];
        }

        bias = (sumx2 * sumy - sumx * sumxy) / den;
        slope = wnum / den;
    }

    public static IEnumerable<T> Mode<T>(this IEnumerable<T> sequence)
    {
        int modeCount = 0;
        var dict = new Dictionary<T, int>();
        foreach (T v in sequence) {
            int count = dict.ContainsKey(v)
                ? ++dict[v]
                : dict[v] = 1;
            if (count > modeCount)
                modeCount = count;
        }

        foreach (KeyValuePair<T, int> pair in dict)
            if (pair.Value == modeCount)
                yield return pair.Key;
    }

    public static double Median(this double[] array, bool presorted = false)
    {
        if (!presorted) {
            presorted = true;
            double prev = double.NegativeInfinity;
            foreach (double v in array)
                if (v < prev) {
                    presorted = false;
                    break;
                }
        }

        if (!presorted)
            array = (double[])array.Clone();

        return (array[array.Length / 2] + array[(array.Length - 1) / 2]) / 2.0;
    }

    public static long PairwiseManhattanSumX(int[] array)
    {
        Array.Sort(array);
        long result = 0, sum = 0;
        int n = array.Length;
        for (long i = 0; i < n; i++) {
            result += array[i] * i - sum;
            sum += array[i];
        }

        return result;
    }

    // SOURCE: https://littledice.me/2014/03/19/calculating-probabilities-in-c/

    public static double Z(double score, double average, double standardDeviation)
    {
        if (standardDeviation == 0) return 0;

        return (score - average) / standardDeviation;
    }

    public static double StandardNormalPdf(double x)
    {
        double exponent = -1 * (0.5 * Math.Pow(x, 2));
        double numerator = Math.Pow(Math.E, exponent);
        double denominator = Math.Sqrt(2 * Math.PI);
        return numerator / denominator;
    }

    public static double Integral(Func<double, double> f, double a, double b)
    {
        double sum = f(a) + 3 * f((2 * a + b) / 3) + 3 * f((a + 2 * b) / 3) + f(b);
        double integral = (b - a) / 8 * sum;
        return integral;
    }

    public static double ProbabilityLessThanX(double x)
    {
        double integral = Integral(StandardNormalPdf, 0, x);
        return integral + 0.5;
    }

    // SOURCE: http://pedanticidiot.blogspot.com/2009/07/z-score-from-percentile-in-c.html
    public static double ComputeZScore(double percentile, double du)
    {
        double v1 = 1 / Math.Sqrt(2 * Math.PI);
        double val = percentile / v1;
        double integral = 0;

        //5 standard deviations to the left and right of the mean
        for (double x = -5; x < 5; x += du) {
            integral += Math.Pow(Math.E, -.5 * Math.Pow(x, 2)) * du;

            if (integral > val)
                return x;
        }

        return -5;
    }

    #endregion
}