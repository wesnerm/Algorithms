namespace Algorithms.Mathematics;

public class CharacteristicPolynomial
{
    public readonly long[,] Poly;
    public readonly int Size;

    public CharacteristicPolynomial(long[,] a, int n, long mod)
    {
        long[,] M = (long[,])a.Clone();

        long pSquare = mod * mod;

        // Hessenberg reduction
        for (int s = 1; s < n - 1; ++s) {
            for (int r_nz = s; r_nz < n; ++r_nz)
                if (M[r_nz, s - 1] != 0) {
                    for (int c = s - 1; c < n; ++c)
                        Swap(ref M[s, c], ref M[r_nz, c]);
                    for (int r = 0; r < n; ++r)
                        Swap(ref M[r, s], ref M[r, r_nz]);
                    break;
                }

            if (M[s, s - 1] == 0)
                continue;

            long inv = ModPow(M[s, s - 1], mod - 2, mod);
            for (int rNz = s + 1; rNz < n; ++rNz)
                if (M[rNz, s - 1] != 0) {
                    long coefPlus = inv * M[rNz, s - 1] % mod;
                    long coefMinus = (mod - coefPlus) % mod;
                    for (int c = s - 1; c < n; ++c)
                        M[rNz, c] = (M[rNz, c] + coefMinus * M[s, c]) % mod;

                    for (int r = 0; r < n; ++r) {
                        M[r, s] += coefPlus * M[r, rNz];
                        if (M[r, s] >= pSquare)
                            M[r, s] -= pSquare;
                    }

                    M[s, s] %= mod;
                }

            for (int r = 0; r < n; ++r)
                M[r, s] %= mod;
        }

        long[,] poly = new long[a.GetLength(0), n + 1];
        poly[0, 0] = 1;

        for (int s = 0; s < n; ++s) {
            for (int i = 0; i < n; ++i) {
                poly[s + 1, i] = M[s, s] * poly[s, i] % mod;
                if (i > 0)
                    poly[s + 1, i] = (poly[s + 1, i] + mod - poly[s, i - 1]) % mod;
            }

            long prod = 1;
            for (int r = s; r >= 1; --r) {
                prod = prod * (mod - M[r, r - 1]) % mod;
                long coeff = prod * M[r - 1, s] % mod;
                for (int i = 0; i < r; ++i) {
                    poly[s + 1, i] += coeff * poly[r - 1, i];
                    if (poly[s + 1, i] >= pSquare)
                        poly[s + 1, i] -= pSquare;
                }
            }

            for (int i = 0; i < n; ++i)
                poly[s + 1, i] %= mod;
        }

        int res;
        for (res = 0; res < n; ++res)
            if (poly[n, res] != 0)
                break;

        Poly = poly;
        Size = n - res;
    }
}