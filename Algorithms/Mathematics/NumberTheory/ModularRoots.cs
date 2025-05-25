using static Algorithms.Mathematics.ModularMath;
using static Algorithms.Mathematics.FactorizationSingle;
using static Algorithms.Mathematics.NumberTheory;

namespace Algorithms.Mathematics;

public static class ModularRoots
{
    // https://en.wikipedia.org/wiki/Primitive_root_modulo_n
    // https://www.hackerrank.com/contests/infinitum17/challenges/primitive-problem
    // http://www.apfloat.org/prim.html

    public static int PrimitiveRoot(int n)
    {
        long s = TotientFunction(n);
        List<long> primes = PrimeFactors(s).Keys.ToList();

        for (int a = 2; a < n; a++) {
            bool good = true;
            foreach (long prime in primes) {
                long pow = ModularMath.ModPow(a, s / prime, n);
                if (pow == 1) {
                    good = false;
                    break;
                }
            }

            if (good)
                return a;
        }

        return -1;
    }

    public static int NthRootOfUnity(int modulus, int n)
    {
        // http://math.stackexchange.com/questions/1437624/number-theoretic-transform-ntt-example-not-working-out
        int primitiveRoot = PrimitiveRoot(modulus);
        long totient = TotientFunction(modulus);
        return (int)ModPow(primitiveRoot, totient / n, modulus);
    }

    public static long NumberOfPrimitiveRoots(int modulus)
    {
        if (modulus <= 1) return 0;
        return TotientFunction(TotientFunction(modulus));
    }

    public static bool IsPrimitive(long g, long n)
    {
        /* isprimitive(g,n) - Test whether g is primitive - generates the group of units mod n.*/
        if (Gcd(g, n) != 1)
            return false; // Not in the group of units
        long order = TotientFunction(n);
        if (CarmichaelLambda(n) != order)
            return false; // Group of units isn't cyclic
        List<long> orderfacts = Factorize(order);
        long oldfact = 1;
        foreach (long fact in orderfacts)
            if (fact != oldfact) {
                if (ModularMath.ModPow(g, order / fact, n) == 1)
                    return false;
                oldfact = fact;
            }

        return true;
    }

    public static int[] QuadraticFormulaMod(int b, int c, int m)
    {
        long sqrt = SqrtModOfPrime((int)((1L * b * b + 4 * (m - c)) % m), m);
        if (sqrt < 0) return Array.Empty<int>();

        long r1 = (m - b + sqrt) * ((m + 1L) >> 1) % m;
        if (sqrt == 0) return new[] { (int)r1 };

        long r2 = r1 - sqrt;
        if (r2 < 0) r2 += m;
        return new[] { (int)r1, (int)r2 };
    }

    public static int QuadraticFormulaMod2(int b, int c, int m)
    {
        long sqrt = SqrtModOfPrime((int)((1L * b * b + 4 * (m - c)) % m), m);
        return sqrt < 0 ? -1 : (int)((m - b + sqrt) * ((m + 1L) >> 1) % m);
    }

    public static long ModPow(long n, long p, long mod)
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

    /* sqrtmod(a,n) - Compute sqrt(a) mod n using various algorithms.
    Currently n must be prime, but will be extended to general n(when I get the time).*/
    public static long SqrtModOfPrime(long a, long n)
    {
        if (a == 0) return 0;

        // Error:  Currently can only compute sqrtmod(a, n) for prime n.");
        if (!MillerRabin.IsPrime(n)) return -1;

        // "*** Error ***:  a is not quadratic residue, so sqrtmod(a,n) has no answer.");
        if (ModularMath.ModPow(a, (n - 1) / 2, n) != 1) return -1;
        return TSRsqrtmod(a, n - 1, n);
    }

    /* TSRsqrtmod(a,grpord,p) - Compute sqrt(a) mod n using Tonelli-Shanks-RESSOL algorithm.
    Here integers mod n must form a cyclic group of order grpord.*/
    static long TSRsqrtmod(long a, long grpord, long p)
    {
        // Rewrite group order as non2*(2^pow2)
        int ordpow2 = 0;
        long non2 = grpord;
        while ((non2 & 0x01) != 1) {
            ordpow2 += 1;
            non2 /= 2;
        }

        // Find 2-primitive g (i.e. non-QR)
        long g;
        for (g = 2; g < grpord - 1; g++)
            if (ModularMath.ModPow(g, grpord / 2, p) != 1)
                break;

        g = ModularMath.ModPow(g, non2, p);

        // Tweak a by appropriate power of g, so result is (2^ordpow2)-residue
        long gpow = 0;
        long atweak = a;

        for (int pow2 = 0; pow2 <= ordpow2; pow2++)
            if (ModularMath.ModPow(atweak, non2 * (1 << (ordpow2 - pow2)), p) != 1) {
                gpow += 1 << (pow2 - 1);
                atweak = atweak * ModularMath.ModPow(g, 1 << (pow2 - 1), p) % p;
                // Assert: atweak now is (2**pow2)-residue
            }

        // Now a*(g**powg) is in cyclic group of odd order non2 - can sqrt directly
        long d = ModInverse(2, non2);
        long tmp = ModularMath.ModPow(a * ModularMath.ModPow(g, gpow, p) % p, d, p); // sqrt(a*(g**gpow))
        return tmp * ModInverse(ModularMath.ModPow(g, gpow / 2, p), p) % p; // sqrt(a*(g**gpow))/g**(gpow/2)
    }
}