namespace Algorithms.Mathematics;

public static partial class Permutations
{
    #region Combinations

    /// <summary>
    ///     Combinations with replacement at most k.
    /// </summary>
    /// <param name="k">The number of items to choose.</param>
    /// <param name="n">The number of different items.</param>
    /// <param name="kmax">The maximum repetition.</param>
    /// <returns>Returns a table with values for [0..n, 0..m] </returns>
    public static long[,] CombinationsWithReplacementAtMostK(int n, int k, int kmax)
    {
        long[,] dp = new long[k + 1, n + 1];

        for (int j = 0; j <= n; j++) {
            dp[0, j] = 1;
            dp[1, j] = 1 + j;
        }

        for (int i = 2; i <= k; i++) {
            dp[i, 1] = dp[i - 1, 1] + (i <= kmax ? 1 : 0);

            for (int j = 2; j <= n; j++) {
                int prev = i - kmax - 1;
                long tmp = dp[i, j - 1] - (prev < 0 ? 0 : dp[prev, j - 1]);

                long result = tmp + dp[i - 1, j];
                dp[i, j] = result % MOD;
            }
        }

        for (int j = 0; j <= n; j++)
        for (int i = k; i > 0; i--)
            dp[i, j] = Fix(dp[i, j] - dp[i - 1, j]);

        return dp;
    }

    public static long[] CombinationsWithReplacementAtMostK2(int n, int k, int kmax)
    {
        long[] dp = new long[k + 1];
        long[] dp2 = new long[k + 1];

        for (int i = 0; i <= k; i++)
            dp2[i] = 1;

        for (int j = 1; j <= n; j++) {
            dp[0] = 1;
            dp[1] = j + dp[0];
            for (int i = 2; i <= k; i++) {
                int prev = i - kmax - 1;
                long tmp = dp2[i] - (prev < 0 ? 0 : dp2[prev]);
                long result = tmp + dp[i - 1];
                dp[i] = result % MOD;
            }

            Swap(ref dp2, ref dp);
        }

        for (int i = k; i > 0; i--)
            dp2[i] = Fix(dp2[i] - dp2[i - 1]);

        return dp2;
    }

    /// <summary>
    ///     Produces combinations with replacement at most k using polynomial arithmetic.
    /// </summary>
    /// <param name="m">The number of different items.</param>
    /// <param name="n">The maximum number of repetitions.</param>
    /// <param name="multiply">The multiply.</param>
    /// <returns>an array that returns the result for the length of each combination</returns>
    public static long[] CombinationsWithReplacementAtMostK(int m, int n,
        Func<long[], long[], long[]> multiply)
    {
        long[] poly = new long[n + 1];

        for (int i = 0; i <= n; i++)
            poly[i] = 1;

        return Polynomial.PolyPow(poly, m, multiply);
    }

    /// <summary>
    ///     Beggars combination.
    ///     This produces ways to assign n coins to k beggars
    /// </summary>
    /// <param name="n">The n.</param>
    /// <param name="k">The k.</param>
    /// <returns></returns>
    public static long BeggarsCombinations(int n, int k) => Combinations(n + k - 1, k - 1);

    /// <summary>
    ///     Combinationse with replacement.
    /// </summary>
    /// <param name="n">The number of different items</param>
    /// <param name="k">The number of items to choose</param>
    /// <returns></returns>
    public static long CombinationsWithReplacement(int n, int k) =>
        // Also known as multiset coefficient or multiset number
        // http://www.mathsisfun.com/combinatorics/combinations-permutations.html
        // Repetition is Allowed: such as coins in your pocket(5,5,5,10,10)
        // No Repetition: such as lottery numbers(2, 14, 15, 27, 30, 33)
        Combinations(n + k - 1, k);

    public static long SumChooseOtoN(int n, int k) =>
        // Hockey-stick identity
        Combinations(n + 1, k + 1);

    public static long SumChoose(int nStart, int nEnd, int k) =>
        Combinations(nEnd + 1, k + 1) - Combinations(nStart, k + 1);

    // Sum{k=0}^r \binom(m,k) \binom(n,r-k)
    public static long Vandermonde(int n, int m, int r) => Combinations(n + m, r);

    // Sum{k=0}^n \binom(m,k) \binom(n,k)
    public static long Vandermonde(int n, int m) => Combinations(n + m, n); // = Combinations(n + m, m)

    public static long NumberOfKTuplesOfPositiveIntegersWhoseSumIs(int n, int k) =>
        // https://en.wikipedia.org/wiki/Stars_and_bars_(combinatorics)
        Combinations(n - 1, k - 1);

    /// <summary>
    ///     Combinations without replacement.
    /// </summary>
    /// <param name="n">The n.</param>
    /// <param name="k">The k.</param>
    /// <returns></returns>
    public static long Combinations(long n, long k)
    {
        if (k <= 0) {
            if (k == 0) return 1;
            long combo = Combinations(-k - 1, n - k);
            if ((n - k) % 2 != 0)
                combo *= -1;
            return combo;
        }

        if (k + k > n) {
            if (k > n) return 0;
            return Combinations(n, n - k);
        }

        long result = 1L;
        long top = n - (k - 1);
        long bottom = 1L;

        while (bottom <= k) {
            result = result * top / bottom;
            bottom++;
            top++;
        }

        return result;
    }

    public static long[,] NumberOfWaysOfGroupingObjects(int blacks, int whites)
    {
        // Project Euler 181

        long[,] f = new long[blacks + 1, whites + 1];
        f[0, 0] = 1;

        for (int b = 0; b <= blacks; b++)
        for (int w = 0; w <= whites; w++) {
            if (b + w <= 0) continue;
            for (int b2 = b; b2 <= blacks; b2++)
            for (int w2 = w; w2 <= whites; w2++)
                f[b2, w2] += f[b2 - b, w2 - w];
        }

        return f;
    }

    public static long MultinomialCoefficient(params long[] coefficients)
    {
        long n = coefficients.Sum();
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Calculates the factorial of n mod p
    ///     O(p logp(n))
    ///     see: http://e-maxx.ru/algo/modular_factorial
    /// </summary>
    /// <param name="n">The n.</param>
    /// <param name="p">The p.</param>
    /// <returns></returns>
    public static int FactMod(int n, int p)
    {
        int res = 1;
        while (n > 1) {
            res = res * (n / p % 2 != 0 ? p - 1 : 1) % p;
            for (int i = 2; i <= n % p; ++i)
                res = res * i % p;
            n /= p;
        }

        return res % p;
    }

    public static int FindDerangement(int n)
    {
        long sum = 0;
        long np = Fact(n);
        for (int i = n; i >= 0; i--) {
            long factor = np * InverseFact(i) % MOD;
            sum = factor - sum;
        }

        return (int)Fix(sum);
    }

    public static int Derangements(int n, int mod)
    {
        if (n < 2) return n == 0 ? 1 : 0;
        long a = 0, b = 1;
        for (int i = 3; i <= n; i++) {
            long c = (b + a) * (i - 1) % mod;
            a = b;
            b = c;
        }

        return (int)b;
    }

    public static int[] DerangementsTable(int n, int mod) => BuildFactorialTable(n, mod, 1, 0);

    public static int[] FactorialTable(int n, int mod) => BuildFactorialTable(n, mod, 1, 1);

    static int[] BuildFactorialTable(int n, int mod, int a, int b)
    {
        int[] dp = new int[Math.Max(n + 1, 2)];
        dp[0] = a;
        dp[1] = b;
        for (int i = 2; i <= n; i++)
            dp[i] = (dp[i - 1] + dp[i - 2]) * (i - 1) % mod;
        return dp;
    }

    static long CompositionOfOddParts(int n, int k)
    {
        // n and k need to be same parity
        if (((n ^ k) & 1) != 0) return 0;
        return Comb((n - k) / 2 + k - 1, k - 1);
    }

    static long CompositionIntoEvenParts(int n)
    {
        if ((n & 1) == 1) return 0;
        if (n == 0) return 1;
        return ModPow(2, n / 2 - 1);
    }

    static long WeakComposition(int n, int k) => Comb(n + k - 1, k - 1);

    static long Compositions(int n) => ModPow(2, n - 1);

    static long Compositions(int n, int k)
    {
        if (n < 1 || k < 1) return 0;
        return Comb(n - 1, k - 1);
    }

    public static long Comb(int n, int k)
    {
        if (k <= 1) return k == 1 ? n : k == 0 ? 1 : 0;
        if (k + k > n) return Comb(n, n - k);
        return Mult(Mult(Fact(n), InverseFact(k)), InverseFact(n - k));
    }

    static List<long> _fact;
    static List<long> _ifact;

    public static long Fact(int n)
    {
        if (_fact == null) _fact = new List<long>(100) { 1 };
        for (int i = _fact.Count; i <= n; i++)
            _fact.Add(Mult(_fact[i - 1], i));
        return _fact[n];
    }

    public static long InverseFact(int n)
    {
        if (_ifact == null) _ifact = new List<long>(100) { 1 };
        for (int i = _ifact.Count; i <= n; i++)
            _ifact.Add(Div(_ifact[i - 1], i));
        return _ifact[n];
    }

    public static long ChooseOddEvenNeutral(long n, int odd, int even, int either = 0)
    {
        long[] A = new long[odd + 1];
        long[] B = new long[even + 1];

        for (int i = 0; i <= odd; ++i)
            A[i] = ((i & 1) == 0 ? 1 : -1) * Comb(odd, i);

        for (int i = 0; i <= even; ++i)
            B[i] = Comb(even, i);

        long[] C = new long[A.Length + B.Length - 1];
        for (int i = 0; i <= odd; i++)
        for (int j = 0; j <= even; j++)
            C[i + j] = (C[i + j] + A[i] * B[j]) % MOD;

        long answer = 0;
        long combined = odd + even + either;
        for (int i = odd + even; i >= 0; i--)
            answer = (answer + C[i] * ModPow(combined - 2 * i, n)) % MOD;
        answer *= ModPow(Inverse(2), odd + even);
        return Fix(answer);
    }

    public static long ChooseOddEven(long n, int odd, int even)
    {
        long[] A = new long[odd + 1];
        long[] B = new long[even + 1];

        for (int i = 0; i <= odd; ++i)
            A[i] = ((i & 1) == 0 ? 1 : -1) * Comb(odd, i);

        for (int i = 0; i <= even; ++i)
            B[i] = Comb(even, i);

        long[] C = new long[A.Length + B.Length - 1];
        for (int i = 0; i <= odd; i++)
        for (int j = 0; j <= even; j++)
            C[i + j] = (C[i + j] + A[i] * B[j]) % MOD;

        long answer = 0;
        int combined = odd + even;
        for (int i = combined; i >= 0; i--)
            answer = (answer + C[i] * ModPow(combined - 2 * i, n)) % MOD;
        answer *= ModPow(Inverse(2), combined);
        return Fix(answer);
    }

    public static long SumChooseOddEvenNeutral(long n, int odd, int even, int either = 0)
    {
        long[] A = new long[odd + 1];
        long[] B = new long[even + 1];

        for (int i = 0; i <= odd; ++i)
            A[i] = (i % 2 == 0 ? 1 : -1) * Comb(odd, i);

        for (int i = 0; i <= even; ++i)
            B[i] = Comb(even, i);

        long[] C = new long[A.Length + B.Length - 1];
        for (int i = 0; i <= odd; i++)
        for (int j = 0; j <= even; j++) {
            long tmp = C[i + j] + A[i] * B[j] % MOD;
            if (tmp >= MOD) tmp -= MOD;
            C[i + j] = tmp;
        }

        long ans = 0;
        for (int i = 0; i <= odd + even; ++i) {
            int x = odd + even + either - 2 * i;
            int num = MOD + 1 - ModPow(x, n + 1);
            int den = MOD + 1 - x;
            long r = Div(num % MOD, den % MOD);
            ans = (ans + C[i] * r % MOD) % MOD;
        }

        ans *= ModPow((MOD + 1) >> 1, odd + even);
        return Fix(ans);
    }

    #endregion

    #region Helpers

    const int MOD = 1000 * 1000 * 1000 + 7;

    public static int Mult(long left, long right) => (int)(left * right % MOD);

    public static long Div(long left, long divisor) =>
        left % divisor == 0
            ? left / divisor
            : Mult(left, Inverse(divisor));

    public static int Inverse(long n) => ModPow(n, MOD - 2);

    public static long Fix(long n) => (n % MOD + MOD) % MOD;

    public static int ModPow(long n, long p)
    {
        long b = n;
        int result = 1;
        while (p != 0) {
            if ((p & 1) != 0)
                result = Mult(result, b);
            p >>= 1;
            b = Mult(b, b);
        }

        return result;
    }

    #endregion
}