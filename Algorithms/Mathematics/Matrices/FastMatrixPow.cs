using static Algorithms.Mathematics.Matrices.MatrixOperationsMod;
using T = long;

namespace Algorithms.Mathematics.Matrices;

public struct FastMatrixPow
{
    const int shift = 4;
    const long mask = (1L << shift) - 1;

    readonly T[][][,] _cache;
    readonly long _mod;
    readonly T[,] _tmp;
    readonly int _n;

    public FastMatrixPow(T[,] a, long mod)
    {
        _n = a.GetLength(0);
        _cache = new T[64 / shift][][,];
        _mod = mod;
        _tmp = new T[_n, _n];

        for (int j = 0; j < _cache.Length; j++) {
            long[][,] t = _cache[j] = new T[mask + 1][,];
            t[1] = j == 0 ? (T[,])a.Clone() : Mult(_cache[j - 1][1], _cache[j - 1][mask], mod);
            for (int i = 2; i < t.Length; i++)
                t[i] = Mult(t[1], t[i - 1], mod);
        }
    }

    public T[,] Pow(long p, T[,] buffer = null)
    {
        T[,] result = buffer ?? new T[_n, _n];

        if (p == 0) {
            if (buffer != null) Array.Clear(buffer, 0, buffer.Length);
            for (int i = result.GetLength(0) - 1; i >= 0; i--)
                result[i, i] = 1;
            return result;
        }

        int asst = 0;
        int bit = 0;
        while (p > 0) {
            if ((p & mask) != 0) {
                long[,] tmp = asst++ > 0
                    ? Mult(result, _cache[bit][p & mask], _mod, _tmp)
                    : _cache[bit][p & mask];
                Array.Copy(tmp, 0, result, 0, tmp.Length);
            }

            p >>= shift;
            bit++;
        }

        return result;
    }
}