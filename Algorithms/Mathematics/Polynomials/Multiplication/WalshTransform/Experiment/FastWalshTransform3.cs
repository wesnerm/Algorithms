namespace Algorithms.Mathematics.Multiplication;

public static class FastWalshTransform3
{
    // Fast Walsh-Hadamard Transformation

    public static readonly Ternary w = new(0, 1);
    public static readonly Ternary w2 = new(MOD - 1, MOD - 1);

    public static void Fwt3(Ternary[] a, int n)
    {
        for (int d = 1; d < n; d *= 3)
        for (int i = 0; i < n; i += d * 3)
        for (int j = 0; j < d; j++) {
            int ij = i + j;
            Ternary x = a[ij];
            Ternary y = a[ij + d];
            Ternary z = a[ij + 2 * d];
            a[ij] = x + y + z;
            a[ij + d] = x + y * w + z * w2;
            a[ij + 2 * d] = x + y * w2 + z * w;
        }
    }

    public static void InverseFwt3(Ternary[] a, int n)
    {
        for (int d = 1; d < n; d *= 3)
        for (int i = 0; i < n; i += d * 3)
        for (int j = 0; j < d; j++) {
            int ij = i + j;
            Ternary x = a[ij];
            Ternary y = a[ij + d];
            Ternary z = a[ij + 2 * d];
            a[ij] = x + y + z;
            a[ij + d] = x + y * w2 + z * w;
            a[ij + 2 * d] = x + y * w + z * w2;
        }

        long inv = Inverse(n);
        for (int i = 0; i < n; i++) a[i] = inv * a[i];
    }

    // a, b are two polynomials and n is size which is power of two
    public static void Convolution3(Ternary[] a, Ternary[] b, int n)
    {
        Fwt3(a, n);
        Fwt3(b, n);
        for (int i = 0; i < n; i++)
            a[i] = a[i] * b[i];
        InverseFwt3(a, n);
    }

    /// An algebraic extended number system, where w^3 = 1
    /// A number is a+bw, a and b are in modular field
    /// multiplication rule is special w*w = - w - 1 (as w^2 + w + 1 = 0)
    public struct Ternary
    {
        readonly long a;
        readonly long b;

        public Ternary(long a, long b = 0)
        {
            this.a = a;
            this.b = b;
        }

        public static implicit operator Ternary(long d) => new(d);

        public static Ternary operator +(Ternary y, Ternary x)
        {
            long a = y.a + x.a;
            long b = y.b + x.b;
            if (a >= MOD) a -= MOD;
            if (b >= MOD) b -= MOD;
            return new Ternary(a, b);
        }

        public static Ternary operator *(long k, Ternary y) => new(y.a * k % MOD, y.b * k % MOD);

        public static Ternary operator *(Ternary y, Ternary x)
        {
            long t = (MOD - y.b) * x.b;
            return new Ternary(
                (y.a * x.a + t) % MOD,
                (y.a * x.b + y.b * x.a + t) % MOD);
        }

        public override string ToString() => $"{a} + {b}w";
    }
}