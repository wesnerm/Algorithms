namespace Algorithms.Mathematics.Multiplication;

using static KaratsubaMultiplication;
using static Polynomial;

[TestFixture]
public class GeneratingFunctionsTest
{
    [Test]
    public void MultiplyPolyTest()
    {
        long[] p = MultiplyPolynomials(
            [1L, 2L], [3L, 4L]);
        Console.WriteLine(string.Join(",", p));
        AreEqual(new[] { 3L, 10L, 8L }, p);
    }

    [Test]
    public void KaratsubaTest()
    {
        TestKaratsuba(new[] { 1L, 2L }, new[] { 3L, 4L });
    }

    [Test]
    public void Karatsuba2Test()
    {
        long[] x = new[] { 1L, 2L, 3L };
        long[] y = new[] { 4L, 5L };
        TestKaratsuba(x, y);
    }

    [Test]
    public void Karatsuba3Test()
    {
        long[] x = new[] { 4L, 2L };
        long[] y = new[] { 4L, 5L };
        TestKaratsuba(x, y);
    }

    [Test]
    public void Karatsuba5Test()
    {
        long[] x = new[] { 1L, -2L, 3L, -4L, 5L };
        long[] y = new[] { 6L, -7L, 8L, 0, 9L, 10L, -12L };
        TestKaratsuba(x, y);
    }

    [Test]
    public void Karatsuba6Test()
    {
        long[] x = new[] { 1L, 2L, 3L, 4L, 5L };
        long[] y = new[] { 1L, 2L, 3L, 4L, 5L };
        //var x = new[] { 1L, 2L, 3L, };
        //var y = new[] { 1L, 2L, 3L, };
        TestKaratsuba(x, y);
    }

    [Test]
    public void Karatsuba7Test()
    {
        long[] x = new[] { 1L, 2L, -3L, 4L, -5L, 6L, 10L, 20L, 30L };
        long[] y = new[] { 1L, -2L, 3L, -4L, 5L, 6L, 7L, 8L, 9L };
        TestKaratsuba(x, y);
    }

    [Test]
    public void Karatsuba8Test()
    {
        long[] x = new[] { 1L, 2L, 3L, 4L };
        long[] y = new[] { 1L, 2L, 3L, 4L };
        TestKaratsuba(x, y);
    }

    void TestKaratsuba(long[] x, long[] y)
    {
        long[] pm = MultiplyPolynomialsMod(x, y, MOD);
        long[] pk = KaratsubaFast(x, y);

        Trim(ref pk);
        Console.WriteLine(string.Join(",", pm));
        Console.WriteLine(string.Join(",", pk));

        for (int i = 0; i < pm.Length; i++) {
            pm[i] = (pm[i] + MOD) % MOD;
            pk[i] = (pk[i] + MOD) % MOD;
        }

        AreEqual(pm, pk);
    }

    void Trim(ref long[] x)
    {
        int len = x.Length;
        while (len > 0 && x[len - 1] == 0) len--;
        Array.Resize(ref x, len);
    }
}