using static Algorithms.Mathematics.ModularMath;

namespace Algorithms.Mathematics.Numerics;

public class NewtonInterpolationMod
{
    readonly int MOD;
    readonly int n;
    readonly long[] x;
    readonly long[] y;

    public NewtonInterpolationMod(IList<long> xvalues, IList<long> yvalues, int mod)
    {
        n = xvalues.Count;
        x = xvalues.ToArray();
        y = yvalues.ToArray();
        MOD = mod;
    }

    public NewtonInterpolationMod(Func<long, long> f, int degree, int mod)
    {
        long sumDegree = degree + 1;
        long[] xs = new long[sumDegree + 1];
        long[] ys = new long[sumDegree + 1];
        xs[0] = 0;
        ys[0] = f(0);

        for (int x = 1; x <= sumDegree; x++) {
            xs[x] = x;
            ys[x] = f(x) + ys[x - 1];
        }

        n = xs.Length;
        x = xs;
        y = ys;
        MOD = mod;
        Init();
    }

    void Init()
    {
        for (int j = 0; j < n; j++)
        for (int i = n - 1; i > j; i--)
            y[i] = Div(y[i] - y[i - 1], x[i] - x[i - j - 1]);
    }

    public unsafe long Interpolate(long a, int n = 0)
    {
        if (n == 0) n = this.n;
        long sum = 0;
        long* factors = stackalloc long[n];
        long f = factors[0] = 1;
        for (int i = 1; i < n; i++) {
            long amx = a - x[i - 1];
            if ((ulong)amx >= (ulong)MOD) amx %= MOD;
            factors[i] = f = Mult(f, amx, MOD);
        }

        for (int i = n - 1; i >= 0; i--)
            sum += Mult(factors[i], y[i], MOD);
        return sum % MOD;
    }

    public long Div(long x, long y) => Mult(x, ModInverse(y, MOD), MOD);

    public static Func<long, long> SumPolynomial(Func<long, long, long> f, int degree, int mod)
    {
        long[] xs = new long[degree + 1];
        var ys = new NewtonInterpolationMod[degree + 1];

        for (int x = 0; x < degree; x++) {
            int xx = x;
            xs[x] = xx;
            ys[x] = new NewtonInterpolationMod(y => f(xx, y), degree, mod);
        }

        return y => {
            long[] zs = new long[degree + 1];
            for (int yy = 0; yy < degree; yy++)
                zs[yy] = ys[yy].Interpolate(yy);

            var newt = new NewtonInterpolationMod(xs, zs, mod);
            return newt.Interpolate(y);
        };
    }
}