using static Algorithms.Mathematics.PolynomialOperation;

namespace Algorithms.Mathematics.Combinatorics;

// Presentation of the log(N) optimization:
// https://discuss.codechef.com/t/chefknn-editorial/18179

public class StirlingNumbersOfFirstKind
{
    readonly int mod;
    readonly bool signed;
    public int[] Polynomial;

    /// <summary>
    ///     Gives stirling results for S1(n,k) were k is the index of the coefficient
    /// </summary>
    public StirlingNumbersOfFirstKind(int n, bool signed, int mod)
    {
        this.mod = mod;
        this.signed = signed;
        Polynomial = ComputeNaive(0, n - 1);
    }

    int[] ComputeNaive(int start, int end)
    {
        if (start == end)
            return new[] { signed ? -start : start, 1 };

        int mid = (start + end) >> 1;
        int[] poly1 = ComputeNaive(start, mid);
        int[] poly2 = ComputeNaive(mid + 1, end);
        return MultiplyPolynomialsMod(poly1, poly2, mod);
    }

    int[] Compute(int n)
    {
        if (n == 1)
            return new[] { 1, 1 };

        int[] stirling = Compute(n >> 1);

        int[] aux = new int[((n + 1) >> 1) + 1];
        int[] shift = ShiftStirling(stirling, aux, n >> 1);

        if ((n & 1) == 1) {
            for (int i = 0; i <= (n >> 1) + 1; i++) aux[i] = 0;
            for (int i = 0; i <= n >> 1; i++) {
                aux[i] = (int)((aux[i] + (long)shift[i] * n) % mod);
                aux[i + 1] = aux[i + 1] + shift[i];
                aux[i + 1] = aux[i + 1] >= mod ? aux[i + 1] - mod : aux[i + 1];
            }

            for (int i = 0; i <= (n >> 1) + 1; i++)
                shift[i] = aux[i];
        }

        return MultiplyPolynomialsMod(stirling, shift, n + 1);
    }

    int[] ShiftStirling(int[] stirling, int[] aux, int n)
    {
        int[] shift = new int[n + 1];
        for (int i = 0; i <= n; i++)
            shift[i] = (int)(Fact(i) * stirling[i] % mod);

        for (int i = 0, val = 1; i <= n; i++, val = (int)((long)val * n % mod))
            aux[n - i] = (int)(val * InverseFact(i) % mod);

        shift = MultiplyPolynomialsMod(shift, aux, n + 1);
        for (int i = 0; i <= n; i++)
            shift[i] = (int)(shift[i + n] * InverseFact(i) % mod);

        return shift;
    }

    // Stirling number of the first unsigned
    public static IEnumerable<int[]> Stirling1(int n, int mod)
    {
        int[] s = new[] { 0, 1 };
        yield return s;
        for (long k = 2; k <= n; k++) {
            int[] s2 = new int[k + 1];
            for (long j = k; j > 0; --j)
                s2[j] = (int)((s[j - 1] + (k - 1L) * s[j]) % mod);
            yield return s = s2;
        }
    }
}