using static Algorithms.Mathematics.Combinatorics.BernoulliNumbers;

namespace Algorithms.Mathematics.Combinatorics;

[TestFixture]
public class BernoulliNumbersTests
{
    const int MOD = 1000 * 1000 * 1000 + 7;

    static long Fix(long v, long MOD)
    {
        v %= MOD;
        if (v < 0)
            v += MOD;
        if (v + v <= MOD)
            return v;
        return v - MOD;
    }

    [Test]
    public void BernoulliNaiveTest()
    {
        long[] b = BernoulliNaive(50, MOD);
        AreEqual(1, Fix(b[0], MOD));
        AreEqual(1, Fix(b[1] * 2, MOD)); // Can be + or -; newer users -
        AreEqual(1, Fix(b[2] * 6, MOD));
        AreEqual(0, b[3]);
        AreEqual(-1, Fix(b[4] * 30, MOD));
        AreEqual(1, Fix(b[6] * 42, MOD));
    }

    [Test]
    public void BernoulliTest()
    {
        long[] b = BernoulliNaive(51, MOD);
        long[] b2 = Bernoulli(51);

        AreEqual(b.Length, b2.Length);
        for (int i = 0; i < 50; i++)
            AreEqual(b[i], b2[i]);
    }

    [Test]
    public void FaulhabersTableTest()
    {
        long[] bernoulli = BernoulliNaive(52, MOD);
        long[] f = FaulhabersTableForFixedN(bernoulli, 40, 50, MOD);
        long[] test = FaulHabersTable2(bernoulli, 40, 50);
        AreEqual(f, test);
    }

    [Test]
    public void FaulhabersTest()
    {
        long[] bernoulli = BernoulliNaive(52, MOD);
        long[] f = Enumerable.Range(0, 51)
            .Select(x => Faulhabers(bernoulli, 40, x, MOD))
            .ToArray();
        long[] test = FaulHabersTable2(bernoulli, 40, 50);

        AreEqual(test, f);
    }

    long[] FaulHabersTable2(long[] bernoulli, int n, int P)
    {
        long[] F = new long[P + 1];
        for (int i = 0; i <= P; i++)
            F[i] = Faulhabers2(bernoulli, n, i);
        return F;
    }

    long Faulhabers2(long[] bernoulli, int n, int p)
    {
        long sum = 0;
        for (int i = 1; i <= n; i++)
            sum = (sum + ModPow(i, p)) % MOD;
        return sum;
    }
}