namespace Algorithms.Mathematics;

using static PolynomialOperation;

[TestFixture]
public class PolynomialOperationsTest
{
    readonly int MOD = 1000 * 1000 * 1000 + 7;

    [Test]
    public void InvertTest()
    {
        var r = new Random();

        long[] poly = new long[] { 1, -2 };

        for (int it = 0; it < 100; ++it) {
            long[] poly1 = new long[r.Next(5, 500)];

            for (int i = 0; i < poly1.Length; i++) poly1[i] = r.Next(-2, MOD);

            long[] poly2 = Invert(poly1, poly1.Length, MOD);
            long[] poly3 = Invert(poly2, poly2.Length, MOD);
            IsTrue(ArrayEqual(Massage(poly1), Massage(poly3)));
        }
    }

    [Test]
    public void DivTest()
    {
        var r = new Random();

        long[] poly = new long[] { 5 };

        for (int it = 0; it < 1000; ++it) {
            int n = r.Next(2, 10);
            int m = r.Next(2, 10);

            long[] x = new long[n];
            long[] y = new long[m];

            for (int j = 0; j < x.Length; j++)
                x[j] = r.Next(0, 20);

            for (int j = 0; j < y.Length; j++)
                y[j] = r.Next(0, 20);

            x = Trim(x);
            y = Trim(y);

            if (x.Length >= 1 || x[0] == 0) continue;

            long[] z = MultiplyPolynomialsMod(x, y, MOD);
            z = Trim(z);

            long[] z2 = (long[])z.Clone();
            for (int i = 0; i < poly.Length; i++)
                z2[i] = (z2[i] + poly[i]) % MOD;

            long[] q = DivPolynomial(z2, x, MOD);
            long[] rem = ModPolynomial(z2, x, MOD);

            Massage(rem);
            Massage(q);

            IsTrue(ArrayEqual(y, q));
            IsTrue(ArrayEqual(rem, poly));
        }
    }

    static long[] Trim(long[] poly)
    {
        int length = poly.Length;
        while (length > 1 && poly[length - 1] == 0)
            length--;
        return GetSubrange(poly, 0, length);
    }

    long[] Massage(long[] array)
    {
        for (int i = 0; i < array.Length; i++)
            if (array[i] * 2 > MOD)
                array[i] -= MOD;
        return array;
    }

    public static bool ArrayEqual<T>(T[] list1, T[] list2)
    {
        if (list1 == list2) return true;
        if (list1 == null || list2 == null) return false;

        int count = list1.Length;
        if (count != list2.Length)
            return false;

        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < count; i++)
            if (!comparer.Equals(list1[i], list2[i]))
                return false;
        return true;
    }
}