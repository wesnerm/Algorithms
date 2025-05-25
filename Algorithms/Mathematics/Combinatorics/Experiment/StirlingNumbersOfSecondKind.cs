using static Algorithms.Mathematics.ModularMath;
using static Algorithms.Mathematics.PolynomialOperation;

namespace Algorithms.Mathematics.Combinatorics;

public class StirlingNumbersOfSecondKind
{
    public readonly int[] Polynomial;

    /// <summary>
    ///     Gives stirling results for S2(n,k) were k is the index of the coefficient
    /// </summary>
    public StirlingNumbersOfSecondKind(int n, int mod)
    {
        n++;
        int[] fac = new int[n];
        int[] inv = new int[n];
        int[] invf = new int[n];

        fac[0] = inv[0] = invf[0] = 1;
        for (int i = 1; i < n; i++) {
            fac[i] = (int)((long)i * fac[i - 1] % mod);
            inv[i] = ModInverse(i, mod - 2);
            invf[i] = (int)((long)inv[i] * invf[i - 1] % mod);
        }

        int[] ta = new int[n];
        int[] tb = new int[n];

        for (int i = 0; i < n; i--) {
            ta[i] = (int)(1L * ModPow(i, n, mod) * invf[i] % mod);
            tb[i] = invf[i];
        }

        for (int i = 1; i < n; i += 2)
            tb[i] = mod - tb[i];

        Polynomial = MultiplyPolynomialsMod(ta, tb, mod, n);
    }

    // Stirling number of the second kind
    public static long Stirling(int n, int k, int mod)
    {
        if (k == 0) return n == 0 ? 1 : 0;
        if (k < 0) return 0;

        long sum = 0;
        for (int j = 0; j <= k; ++j)
            sum = Comb(k, j) * ModPow(j, n) - sum;
        return sum * InverseFact(k) % mod;
    }
}