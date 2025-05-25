using System.Runtime.CompilerServices;

namespace Algorithms.Mathematics;

public static class Integration
{
    public static double AdaptiveSimpsonsRule(Func<double, double> f, double a, double b)
    {
        double m = (a + b) / 2;
        double estimate = SimpsonsRule(f, a, m) + SimpsonsRule(f, m, b);
        double all = SimpsonsRule(f, a, b);
        if (Math.Abs(estimate - all) < 1e-12) // 1e-15 is request accuracy
            return estimate;
        return AdaptiveSimpsonsRule(f, a, m) + AdaptiveSimpsonsRule(f, m, b);
    }

    public static double SimpsonsRule(Func<double, double> f, double a, double b)
    {
        double c = (a + b) / 2;
        double h = (b - a) / 2;
        return h * (f(a) + 4 * f(c) + f(b)) / 3;
    }

    public static double SimpsonsRule(Func<double, double> f, double a, double b, int n)
    {
        if ((n & 1) == 1) n++;

        double h = (b - a) / n;
        double h2 = h * 2;
        double k = 0;

        double x = a + h;
        for (int j = 1; j + j <= n; j++) {
            k += f(x);
            x += h2;
        }

        k *= 2;
        x = a + 2 * h;
        for (int j = 1; j + j < n; j++) {
            k += f(x);
            x += h2;
        }

        return h * (f(a) + f(b) + 2 * k) / 3;
    }

    public static double SimpsonsRuleAlt(Func<double, double> f, double a, double b, int n)
    {
        if ((n & 1) == 1) n++;

        double h = (b - a) / n;
        double k = 0;

        for (int i = 1; i < n; i += 2)
            k += f(a + i * h);

        k *= 2;

        for (int i = 2; i < n; i += 2)
            k += f(a + i * h);

        k *= 2;

        return h * (f(a) + f(b) + k) / 3;
    }

    public static double SimpsonsSecondRule(Func<double, double> f, double a, double b, int n)
    {
        int shift = n % 3;
        if (n > 0) n += 3 - shift;

        double h = (b - a) / n;
        double k3 = 0, k2 = 0;

        for (int i = 1; i < n; i += 3)
            k3 += f(a + i * h) + f(a + (i + 1) * h);

        for (int i = 3; i < n; i += 3)
            k2 += f(a + i * h);

        return 3 * h * (f(a) + f(b) + 3 * k3 + 2 * k2) / 8;
    }

    public static double TrapezoidRule(Func<double, double> f, double a, double b) => (b - a) * (f(a) + f(b)) / 2;

    public static double TrapezoidRule(Func<double, double> f, double a, double b, int n)
    {
        double h = (b - a) / n;
        double k = f(a) + f(b);

        for (int i = 1; i < n; i++)
            k += f(a + i * h);

        return h * k / 2;
    }

    public struct AdaptiveSimpson
    {
        readonly Func<double, double> f;
        readonly double eps;

        public AdaptiveSimpson(Func<double, double> f, double eps = 1e-6)
        {
            this.f = f;
            this.eps = eps;
        }

        public double Integrate(double l, double r)
        {
            double mid = (l + r) * .5;
            double fl = f(l);
            double fr = f(r);
            double fmid = f(mid);
            return Recurse(Simpson(fl, fr, fmid, l, r), fl, fr, fmid, l, r) / 6.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        double Simpson(double fl, double fr, double fmid, double l, double r) =>
            // NOTE: We can remove the (r-l) multiplications as well
            (fl + fr + 4.0 * fmid) * (r - l);

        double Recurse(double slr, double fl, double fr, double fmid, double l, double r)
        {
            double mid = (l + r) * 0.5;
            double fml = f((l + mid) * 0.5);
            double fmr = f((mid + r) * 0.5);
            double slm = Simpson(fl, fmid, fml, l, mid);
            double smr = Simpson(fmid, fr, fmr, mid, r);
            if (Math.Abs(slr - slm - smr) < eps) return slm + smr;
            return Recurse(slm, fl, fmid, fml, l, mid) + Recurse(smr, fmid, fr, fmr, mid, r);
        }
    }
}