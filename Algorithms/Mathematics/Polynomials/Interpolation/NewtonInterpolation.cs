namespace Algorithms.Mathematics.Numerics;

public class NewtonInterpolation
{
    readonly int n;
    readonly double[] x;
    readonly double[] y;

    public NewtonInterpolation(IList<double> xvalues, IList<double> yvalues)
    {
        n = xvalues.Count;
        x = xvalues.ToArray();
        y = yvalues.ToArray();

        for (int j = 0; j < n; j++)
        for (int i = n - 1; i > j; i--)
            y[i] = (y[i] - y[i - 1]) / (x[i] - x[i - j - 1]);
    }

    public double Interpolate(double a, int n = 0)
    {
        if (n == 0) n = this.n;

        double sum = 0;
        double[] factors = new double[n];
        double f = factors[0] = 1;
        for (int i = 1; i < n; i++)
            factors[i] = f *= a - x[i - 1];
        for (int i = n - 1; i >= 0; i--)
            sum += factors[i] * y[i];
        return sum;
    }
}