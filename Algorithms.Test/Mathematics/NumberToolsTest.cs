namespace Algorithms.Mathematics;

using static NumberTheory;

[TestFixture]
public class NumberToolsTest
{
    /// <summary>
    ///     Test for AreClose(double d1, double d2)
    /// </summary>
    [Test]
    [Ignore("AreClose is not yet implemented")]
    public void AreClose()
    {
        // NumberTools obj = new NumberTools();
        // bool expected = obj.AreClose(d1,d2);
        // bool actual = default(bool);
        // Assert.AreEqual(expected, actual, "AreClose");
        Fail("AreClose is inconclusive");
    }

    /// <summary>
    ///     Test for Chop(double d)
    /// </summary>
    [Test]
    [Ignore("Chop is not yet implemented")]
    public void Chop()
    {
        // NumberTools obj = new NumberTools();
        // double expected = obj.Chop(d);
        // double actual = default(double);
        // Assert.AreEqual(expected, actual, "Chop");
        Fail("Chop is inconclusive");
    }

    [Test]
    public void ExtendedGCD()
    {
        int x = 7;
        int y = 23;
        int gcd = ExtendedGcd(x, y, out int xF, out int yF);

        AreEqual(x, 7);
        AreEqual(y, 23);
        AreEqual(xF, 10);
        AreEqual(yF, -3);
        AreEqual(gcd, 1);
    }

    /// <summary>
    ///     Test for GCD(int u, int v)
    /// </summary>
    [Test]
    [Ignore("GCD is not yet implemented")]
    public void GCD()
    {
        // NumberTools obj = new NumberTools();
        // int expected = obj.GCD(u,v);
        // int actual = default(int);
        // Assert.AreEqual(expected, actual, "GCD");
        Fail("GCD is inconclusive");
    }

    /// <summary>
    ///     Test for IsDivisible(double d, double mod)
    /// </summary>
    [Test]
    [Ignore("IsDivisible is not yet implemented")]
    public void IsDivisible()
    {
        // NumberTools obj = new NumberTools();
        // bool expected = obj.IsDivisible(d,mod);
        // bool actual = default(bool);
        // Assert.AreEqual(expected, actual, "IsDivisible");
        Fail("IsDivisible is inconclusive");
    }

    /// <summary>
    ///     Test for IsOne(double d)
    /// </summary>
    [Test]
    [Ignore("IsOne is not yet implemented")]
    public void IsOne()
    {
        // NumberTools obj = new NumberTools();
        // bool expected = obj.IsOne(d);
        // bool actual = default(bool);
        // Assert.AreEqual(expected, actual, "IsOne");
        Fail("IsOne is inconclusive");
    }

    /// <summary>
    ///     Test for IsZero(double d)
    /// </summary>
    [Test]
    [Ignore("IsZero is not yet implemented")]
    public void IsZero()
    {
        // NumberTools obj = new NumberTools();
        // bool expected = obj.IsZero(d);
        // bool actual = default(bool);
        // Assert.AreEqual(expected, actual, "IsZero");
        Fail("IsZero is inconclusive");
    }

    [Test]
    public void ModulusInverseTest()
    {
        AreEqual(ModulusInverse(3, 19), 13, "3^-1 mod 19 = 13");
        AreEqual(ModulusInverse(7, 23), 10, "7^-1 mod 23 = 10");
    }

    [Test]
    public void ModulusLogTest()
    {
        AreEqual(ModulusLog(5, 2, 13), 9, "log2 5 mod 13 = {0}");
    }

    [Test]
    public void ModulusPowTest()
    {
        AreEqual(ModulusPow(13, 11, 19), 2, "13^11 mod 19 = {0}");
        AreEqual(ModulusPow(2, 10001, 11), 2, "2^10001 mod 11 = {0}");
        AreEqual(ModulusPow(2, 245, 35), 32, "2^245 mod 35 = {0}");
        AreEqual(ModulusInverse(2, 23), 12, "23^-1 mod 23 = {0}");
        AreEqual(ModulusPow(2, -1, 23), 12, "23^-1 mod 23 = {0}");
    }

    [Test]
    public void ModulusRootTest()
    {
        AreEqual(ModulusRoot(2, 11, 19), 13, "11th root of 2 in Z19 = 13");
    }

    /// <summary>
    ///     Test for QuadraticRoots(double a, double b, double c)
    /// </summary>
    [Test]
    [Ignore("QuadraticRoots is not yet implemented")]
    public void QuadraticRoots()
    {
        // NumberTools obj = new NumberTools();
        // double[] expected = obj.QuadraticRoots(a,b,c);
        // double[] actual = default(double[]);
        // Assert.AreEqual(expected, actual, "QuadraticRoots");
        Fail("QuadraticRoots is inconclusive");
    }

    [Test]
    public void CombinationsTest()
    {
        AreEqual(1, Combinations(5, 0));
        AreEqual(5, Combinations(5, 1));
        AreEqual(5 * 4 / 2, Combinations(5, 2));
        AreEqual(5 * 4 / 2, Combinations(5, 3));
        AreEqual(5, Combinations(5, 4));
        AreEqual(1, Combinations(5, 5));
        AreEqual(0, Combinations(5, 6));
        AreEqual(1, Combinations(-10, 0));
        AreEqual(1, Combinations(0, 0));
        AreEqual(0, Combinations(0, 1));
        AreEqual(1, Combinations(1, 1));
        AreEqual(1, Combinations(1, 0));
    }
}