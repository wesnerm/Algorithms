namespace Algorithms.Mathematics;

public static class ContinuedFractions
{
    public static void Mediant(long n1, long d1, long n2, long d2,
        out long n, out long d)
    {
        n = n1 + n2;
        d = d1 + d2;
    }

    // https://en.wikipedia.org/wiki/Farey_sequence
    public static IEnumerable<Tuple<long, long>> FareyFunction(long n, bool descending = false)
    {
        // Print the nth Farey sequence, either ascending or descending.
        long a = descending ? 1 : 0;
        long b = 1;
        long c = descending ? n - 1 : 1;
        long d = n;

        yield return Tuple.Create(a, b);
        while (descending ? a > 0 : c <= n) {
            long k = (n + b) / d;
            long nc = k * c - a;
            long nd = k * d - b;
            a = c;
            b = d;
            c = nc;
            d = nd;
            yield return Tuple.Create(a, b);
        }
    }

    // I think this is for building pi but I forgot
    public static long[] Build(long[] c)
    {
        if (c.Length == 0) return new long[] { 1, 0 };
        long p0 = 1, p1 = c[0];
        for (int i = 1; i < c.Length; i++) {
            if ((double)p1 * c[i] + p0 >= 1e18) return null;
            long p2 = p1 * c[i] + p0;
            p0 = p1;
            p1 = p2;
        }

        long q0 = 0, q1 = 1;
        for (int i = 1; i < c.Length; i++) {
            if ((double)q1 * c[i] + q0 >= 1e18) return null;
            long q2 = q1 * c[i] + q0;
            q0 = q1;
            q1 = q2;
        }

        return new[] { p1, q1 };
    }

    public static List<long> GetContinuedFraction(double d)
    {
        var list = new List<long>();
        for (int i = 0; i < 14; i++) {
            long whole = (long)d;
            list.Add(whole);
            d -= whole;
            if (d == 0) break;
            d = 1 / d;
        }

        while (list.Count > 0 && list[list.Count - 1] == 0)
            list.Remove(list[list.Count - 1]);

        return list;
    }

    public static void ConvertToFraction(List<long> cf,
        out List<long> ns, out List<long> ds)
    {
        ns = new List<long>();
        ds = new List<long>();

        ns.Add(1);
        ds.Add(0);
        ns.Add(cf[0]);
        ds.Add(1);

        for (int i = 1; i + 1 < cf.Count; i++) {
            long c = cf[i];
            ns.Add(c * ns[i] + ns[i - 1]);
            ds.Add(c * ds[i] + ds[i - 1]);
        }

#if DEBUG
        var vs = new List<double>();
        for (int i = 0; i < ns.Count; i++)
            if (ds[i] == 0)
                vs.Add(double.MaxValue);
            else
                vs.Add(1d * ns[i] / ds[i]);
#endif
    }

    public static IEnumerable<Tuple<long, long>> ConvertToFraction(List<long> cf)
    {
        return ConvertToFraction(i => i < cf.Count ? cf[(int)i] : 0);
    }

    public static IEnumerable<Tuple<long, long>> ConvertToFraction(Func<long, long> cf)
    {
        long ns0 = 0;
        long ds0 = 1;
        long ns1 = 1;
        long ds1 = 0;

        for (long i = 0;; i++) {
            long c = cf(i);
            if (c == 0) break;
            long ns = c * ns1 + ns0;
            long ds = c * ds1 + ds0;
            yield return new Tuple<long, long>(ns, ds);
            ns0 = ns1;
            ds0 = ds1;
            ns1 = ns;
            ds1 = ds;
        }
    }

    /// <summary>
    ///     This gives the farey sequence from 1/N to (N-1)/N...
    /// </summary>
    /// <param name="n">The n.</param>
    /// <param name="action">The action.</param>
    public static void FareySequence(int n, Action<int, int> action)
    {
        int a = 0, b = 1, c = 1, d = n;
        while (d > 1) {
            int k = (n + b) / d;
            int aa = a, bb = b;
            a = c;
            b = d;
            c = k * c - aa;
            d = k * d - bb;
            action(a, b);
        }
    }

    public static IEnumerable<Tuple<int, int>> FareySequence(int n)
    {
        int a = 0, b = 1, c = 1, d = n;
        while (d > 1) {
            int k = (n + b) / d;
            int aa = a, bb = b;
            a = c;
            b = d;
            c = k * c - aa;
            d = k * d - bb;
            yield return new Tuple<int, int>(a, b);
        }
    }
}