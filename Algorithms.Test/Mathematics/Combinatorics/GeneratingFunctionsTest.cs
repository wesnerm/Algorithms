namespace Algorithms.Mathematics;

using static Algorithms.Mathematics.Combinatorics.KaratsubaMultiplication;
using static Polynomials;

[TestFixture]
public class GeneratingFunctionsTest
{
    #region Variables
    // private FastFourierTransform sample;
    #endregion

    [Test]
    public void MultiplyPolyTest()
    {
        var p = MultiplyPolynomials(
            new[] {1L, 2L}, new[] {3L, 4L});
        Console.WriteLine(string.Join(",", p));
        Assert.AreEqual(new[] {3L, 10L, 8L}, p);
    }

    [Test]
    public void KaratsubaTest()
    {
        TestKaratsuba(new[] { 1L, 2L }, new[] { 3L, 4L });
    }

    [Test]
    public void Karatsuba2Test()
    {
        var x = new[] {1L, 2L, 3L};
        var y = new[] {4L, 5L } ;
        TestKaratsuba(x, y);
    }

    [Test]
    public void Karatsuba3Test()
    {
        var x = new[] { 4L, 2L };
        var y = new[] { 4L, 5L };
        TestKaratsuba(x, y);
    }

    [Test]
    public void Karatsuba5Test()
    {
        var x = new[] { 1L, -2L, 3L, -4L, 5L };
        var y = new[] { 6L, -7L, 8L, 0, 9L, 10L, -12L };
        TestKaratsuba(x, y);
    }

    [Test]
    public void Karatsuba6Test()
    {
        var x = new[] { 1L, 2L, 3L, 4L, 5L, };
        var y = new[] { 1L, 2L, 3L, 4L, 5L, };
        //var x = new[] { 1L, 2L, 3L, };
        //var y = new[] { 1L, 2L, 3L, };
        TestKaratsuba(x, y);
    }

    [Test]
    public void Karatsuba7Test()
    {
        var x = new[] { 1L, 2L, -3L, 4L, -5L, 6L, 10L, 20L, 30L };
        var y = new[] { 1L, -2L, 3L, -4L, 5L, 6L, 7L, 8L, 9L, };
        TestKaratsuba(x, y);
    }

    [Test]
    public void Karatsuba8Test()
    {
        var x = new[] { 1L, 2L, 3L, 4L, };
        var y = new[] { 1L, 2L, 3L, 4L, };
        TestKaratsuba(x, y);
    }

    void TestKaratsuba(long[] x, long[] y)
    {
        var pm = MultiplyPolynomialsMod(x, y, MOD);
        var pk = KaratsubaFast(x, y);

        Trim(ref pk);
        Console.WriteLine(string.Join(",", pm));
        Console.WriteLine(string.Join(",", pk));
        Assert.AreEqual(pm, pk);
    }

    void Trim(ref long[] x)
    {
        int len = x.Length;
        while (len > 0 && x[len - 1] == 0) len--;
        Array.Resize(ref x, len);
    }

}