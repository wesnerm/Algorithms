#region Copyright

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
//
// Copyright (C) 2005-2016, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Mathematics;

[TestFixture]
public class FactorizationTest
{
    /// <summary>
    ///     Test for NumberOfNoncoprimes(IList<int> primes, long n, int start = 0, int factor = 1)
    /// </summary>
    [Test]
    public void NumberOfNoncoprimesTest()
    {
        int[] table = Factorization.PrimeFactorsUpTo(1000);

        for (int i = 1; i <= 100; i++) {
            List<int> primes = Factorization.DistinctPrimeFactorsOf(table, i).ToList();
            AreEqual(i - Factorization.TotientFunction(table, i),
                Factorization.NumberOfNoncoprimes(primes, i));
        }
    }

    /// <summary>
    ///     Test for TotientFromPrimes(IEnumerable<int> primes, int n)
    /// </summary>
    [Test]
    [Ignore("TotientFromPrimes is not yet implemented")]
    public void TotientFromPrimesTest()
    {
        // var obj = new Factorization();
        // var expected = obj.TotientFromPrimes();
        // var actual = default(int);
        // Assert.AreEqual(expected, actual, "TotientFromPrimes");
        Fail();
    }

    /// <summary>
    ///     Test for Degree(long n, long factor)
    /// </summary>
    [Test]
    [Ignore("Degree is not yet implemented")]
    public void DegreeTest()
    {
        // var obj = new Factorization();
        // var expected = obj.Degree();
        // var actual = default(int);
        // Assert.AreEqual(expected, actual, "Degree");
        Fail();
    }

    /// <summary>
    ///     Test for NumberUpToKCoprimeWithP(int[] table, int n, int p)
    /// </summary>
    [Test]
    [Ignore("NumberUpToKCoprimeWithP is not yet implemented")]
    public void NumberUpToKCoprimeWithPTest()
    {
        // var obj = new Factorization();
        // var expected = obj.NumberUpToKCoprimeWithP();
        // var actual = default(long);
        // Assert.AreEqual(expected, actual, "NumberUpToKCoprimeWithP");
        Fail();
    }

    /// <summary>
    ///     Test for PollardsRhoFactorization(long n, long seed=2)
    /// </summary>
    [Test]
    [Ignore("PollardsRhoFactorization is not yet implemented")]
    public void PollardsRhoFactorizationTest()
    {
        // var obj = new Factorization();
        // var expected = obj.PollardsRhoFactorization();
        // var actual = default(long);
        // Assert.AreEqual(expected, actual, "PollardsRhoFactorization");
        Fail();
    }

    /// <summary>
    ///     Test for CountDivisors(long rParam)
    /// </summary>
    [Test]
    [Ignore("CountDivisors is not yet implemented")]
    public void CountDivisorsTest()
    {
        // var obj = new Factorization();
        // var expected = obj.CountDivisors();
        // var actual = default(long);
        // Assert.AreEqual(expected, actual, "CountDivisors");
        Fail();
    }
}