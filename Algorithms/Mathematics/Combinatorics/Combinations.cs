namespace Algorithms.Mathematics.Combinatorics;

public class Combinations
{
    public int[] fact, ifact;
    public int MOD;

    public Combinations(int n, int MOD)
    {
        this.MOD = MOD;

        n++;
        fact = new int[n];
        ifact = new int[n];

        fact[0] = ifact[0] = 1;
        for (long i = 1; i < fact.Length; i++)
            fact[i] = (int)(fact[i - 1] * i % MOD);

        ifact[ifact.Length - 1] = (int)ModPow(fact[fact.Length - 1], MOD - 2, MOD);
        for (long i = ifact.Length - 1; i > 0; i--)
            ifact[i - 1] = (int)(ifact[i] * i % MOD);
    }

    public long Fact(long i) => fact[i];

    public long InverseFact(long i) => ifact[i];

    public long Comb(long n, long k) =>
        !unchecked((ulong)k > (ulong)n) ? (long)fact[n] * ifact[k] % MOD * ifact[n - k] % MOD : 0;

    public long CombUnchecked(long n, long k) => (long)fact[n] * ifact[k] % MOD * ifact[n - k];

    public long Inverse(long i) => i > 1 ? (long)ifact[i] * fact[i - 1] : i;

    public int[] InverseTable()
    {
        int[] inverse = new int[fact.Length];
        for (int i = 1; i < fact.Length; i++)
            inverse[i] = (int)Inverse(i);
        return inverse;
    }
}