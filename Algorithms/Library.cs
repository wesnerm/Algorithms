using System.Runtime.CompilerServices;
using static System.Math;
using static System.Array;
using System.Numerics;

public static class Library
{
    #region Variables

    public const int MOD = 1000000007;
    const int FactCache = 1000;
    const long BIG = long.MaxValue >> 15;

    #endregion

    #region Mod Math

    static int[]? _inverse;

    public static long Inverse(long n)
    {
        long result;

        if (_inverse == null)
            _inverse = new int[1000];

        if (n >= 0 && n < _inverse.Length && (result = _inverse[n]) != 0)
            return result - 1;

        result = InverseDirect((int)n);
        if (n >= 0 && n < _inverse.Length)
            _inverse[n] = (int)(result + 1);
        return result;
    }

    public static int InverseDirect(int a, int mod = MOD)
    {
        int b = mod, p = 1, q = 0;
        while (b > 0) {
            int c = a / b;
            int d = a;
            a = b;
            b = d % b;
            d = p;
            p = q;
            q = d - c * q;
        }

        return p < 0 ? p + mod : p;
    }

    public static int InverseDirectOld(int a, int mod = MOD)
    {
        int t = 0, r = mod, t2 = 1, r2 = a;
        while (r2 != 0) {
            int q = r / r2;
            t -= q * t2;
            r -= q * r2;

            if (r != 0) {
                q = r2 / r;
                t2 -= q * t;
                r2 -= q * r;
            } else {
                r = r2;
                t = t2;
                break;
            }
        }

        return r <= 1 ? t >= 0 ? t : t + mod : -1;
    }

    public static long Mult(long left, long right) => left * right % MOD;

    public static long Div(long left, long divisor) => left * Inverse(divisor) % MOD;

    public static long Fix(long n) => (n %= MOD) >= 0 ? n : n + MOD;

    public static long ModPow(long n, long p, long mod = MOD)
    {
        long b = n;
        long result = 1;
        while (p != 0) {
            if ((p & 1) != 0)
                result = result * b % mod;
            p >>= 1;
            b = b * b % mod;
        }

        return result;
    }

    static List<long> _fact;

    public static long Fact(int n)
    {
        if (_fact == null) _fact = new List<long>(FactCache) { 1 };
        for (int i = _fact.Count; i <= n; i++)
            _fact.Add(Mult(_fact[i - 1], i));
        return _fact[n];
    }

    static long[] _ifact = new long[0];

    public static long InverseFact(int n)
    {
        long result;
        if (n < _ifact.Length && (result = _ifact[n]) != 0)
            return result;

        long inv = Inverse(Fact(n));
        if (n >= _ifact.Length) Resize(ref _ifact, _fact.Capacity);
        _ifact[n] = inv;
        return inv;
    }

    public static long Fact(int n, int m)
    {
        long fact = Fact(n);
        if (m < n) fact = fact * InverseFact(n - m) % MOD;
        return fact;
    }

    public static long Comb(int n, int k)
    {
        if (k <= 1) return k == 1 ? n : k == 0 ? 1 : 0;
        return Mult(Mult(Fact(n), InverseFact(k)), InverseFact(n - k));
    }

    public static long Combinations(long n, int k)
    {
        if (k <= 0) return k == 0 ? 1 : 0; // Note: n<0 -> 0 unless k=0
        if (k + k > n) return Combinations(n, (int)(n - k));

        long result = InverseFact(k);
        for (long i = n - k + 1; i <= n; i++) result = result * i % MOD;
        return result;
    }

    #endregion

    #region Common

    public static void Swap<T>(ref T a, ref T b)
    {
        T tmp = a;
        a = b;
        b = tmp;
    }

    public static T[] GetRange<T>(T[] x, int start, int count, bool extend = false)
    {
        if (!extend && (start < 0 || start + count > x.Length)) throw new ArgumentOutOfRangeException();
        var result = new T[count];
        Copy(x, Max(0, start), result, Max(0, -start), Min(count, x.Length - start));
        return result;
    }

    public static int Bound<T>(T[] array, T value, bool upper = false)
        where T : IComparable<T>
    {
        int left = 0;
        int right = array.Length - 1;

        while (left <= right) {
            int mid = left + ((right - left) >> 1);
            int cmp = value.CompareTo(array[mid]);
            if (cmp > 0 || (cmp == 0 && upper))
                left = mid + 1;
            else
                right = mid - 1;
        }

        return left;
    }

    public static int Gcd(int n, int m)
    {
        while (true) {
            if (m == 0) return n >= 0 ? n : -n;
            n %= m;
            if (n == 0) return m >= 0 ? m : -m;
            m %= n;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long HighestOneBit(long x)
    {
        return x & (1L << BitOperations.Log2((ulong)x));
    }

    #endregion
}