namespace Algorithms.Mathematics;

public struct Matrix
{
    public const long MOD = 1000 * 1000 * 1000 + 7;

    public long e11;
    public long e12;
    public long e21;
    public long e22;

    public Matrix(int m11, int m12, int m21, int m22)
    {
        e11 = m11;
        e12 = m12;
        e21 = m21;
        e22 = m22;
    }

    public static Matrix operator *(Matrix m1, Matrix m2)
    {
        var m = new Matrix
        {
            e11 = Add(Mult(m1.e11, m2.e11), Mult(m1.e12, m2.e21)),
            e12 = Add(Mult(m1.e11, m2.e12), Mult(m1.e12, m2.e22)),
            e21 = Add(Mult(m1.e21, m2.e11), Mult(m1.e22, m2.e21)),
            e22 = Add(Mult(m1.e21, m2.e12), Mult(m1.e22, m2.e22)),
        };
        return m;
    }

    public static Matrix operator +(Matrix m1, Matrix m2)
    {
        var m = new Matrix
        {
            e11 = Add(m1.e11, m2.e11),
            e12 = Add(m1.e12, m2.e12),
            e21 = Add(m1.e21, m2.e21),
            e22 = Add(m1.e22, m2.e22),
        };
        return m;
    }

    public void Apply(ref long x, ref long y)
    {
        long x2 = Add(Mult(e11, x), Mult(e12, y));
        long y2 = Add(Mult(e21, x), Mult(e22, y));
        x = x2;
        y = y2;
    }

    public Matrix Pow(long p)
    {
        Matrix b = this;
        var result = new Matrix(1, 0, 0, 1);
        while (p != 0) {
            if ((p & 1) != 0)
                result *= b;
            p >>= 1;
            b *= b;
        }

        return result;
    }

    public static long Add(long left, long right) => (left + right) % MOD;

    static long Mod(long a, long b, long c)
    {
        long x = 1, y = a;
        while (b > 0) {
            if (b % 2 == 1) x = Mul(x, y, c);
            y = Mul(y, y, c);
            b /= 2;
        }

        return x % c;
    }

    static long Mul(long a, long b, long c)
    {
        long x = 0, y = a % c;
        while (b > 0) {
            if (b % 2 == 1) x = (x + y) % c;
            y = y * 2 % c;
            b /= 2;
        }

        return x % c;
    }

    public static long Mult(long a, long b, long mod = MOD) => a * b % mod;

    public static long Pow(long n, long p, long mod)
    {
        long result = 1;
        long b = n;
        while (p > 0) {
            if ((p & 1) == 1) result = result * b % mod;
            p >>= 1;
            b = b * b % mod;
        }

        return result;
    }
}