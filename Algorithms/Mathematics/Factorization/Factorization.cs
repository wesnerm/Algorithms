using static System.Math;

namespace Algorithms.Mathematics;

public class Factorization
{
    public const int LargestPrimeInt32 = int.MaxValue;
    public const uint LargestPrimeUInt32 = 0xFFFFFFFB;
    public const long LargestPrimeInt64 = 0x7FFFFFFFFFFFFFE7;
    public const ulong LargestPrimeUInt64 = 0xFFFFFFFFFFFFFFC5;

    public static int[] Primes1000 =
    {
        2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
        31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
        73, 79, 83, 89, 97, 101, 103, 107, 109, 113,
        127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
        179, 181, 191, 193, 197, 199, 211, 223, 227, 229,
        233, 239, 241, 251, 257, 263, 269, 271, 277, 281,
        283, 293, 307, 311, 313, 317, 331, 337, 347, 349,
        353, 359, 367, 373, 379, 383, 389, 397, 401, 409,
        419, 421, 431, 433, 439, 443, 449, 457, 461, 463,
        467, 479, 487, 491, 499, 503, 509, 521, 523, 541,
        547, 557, 563, 569, 571, 577, 587, 593, 599, 601,
        607, 613, 617, 619, 631, 641, 643, 647, 653, 659,
        661, 673, 677, 683, 691, 701, 709, 719, 727, 733,
        739, 743, 751, 757, 761, 769, 773, 787, 797, 809,
        811, 821, 823, 827, 829, 839, 853, 857, 859, 863,
        877, 881, 883, 887, 907, 911, 919, 929, 937, 941,
        947, 953, 967, 971, 977, 983, 991, 997, 1009, 1013,
        1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069,
        1087, 1091, 1093, 1097, 1103, 1109, 1117, 1123, 1129, 1151,
        1153, 1163, 1171, 1181, 1187, 1193, 1201, 1213, 1217, 1223,
        1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291,
        1297, 1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373,
        1381, 1399, 1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451,
        1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511,
        1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583,
        1597, 1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657,
        1663, 1667, 1669, 1693, 1697, 1699, 1709, 1721, 1723, 1733,
        1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789, 1801, 1811,
        1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889,
        1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987,
        1993, 1997, 1999, 2003, 2011, 2017, 2027, 2029, 2039, 2053,
        2063, 2069, 2081, 2083, 2087, 2089, 2099, 2111, 2113, 2129,
        2131, 2137, 2141, 2143, 2153, 2161, 2179, 2203, 2207, 2213,
        2221, 2237, 2239, 2243, 2251, 2267, 2269, 2273, 2281, 2287,
        2293, 2297, 2309, 2311, 2333, 2339, 2341, 2347, 2351, 2357,
        2371, 2377, 2381, 2383, 2389, 2393, 2399, 2411, 2417, 2423,
        2437, 2441, 2447, 2459, 2467, 2473, 2477, 2503, 2521, 2531,
        2539, 2543, 2549, 2551, 2557, 2579, 2591, 2593, 2609, 2617,
        2621, 2633, 2647, 2657, 2659, 2663, 2671, 2677, 2683, 2687,
        2689, 2693, 2699, 2707, 2711, 2713, 2719, 2729, 2731, 2741,
        2749, 2753, 2767, 2777, 2789, 2791, 2797, 2801, 2803, 2819,
        2833, 2837, 2843, 2851, 2857, 2861, 2879, 2887, 2897, 2903,
        2909, 2917, 2927, 2939, 2953, 2957, 2963, 2969, 2971, 2999,
        3001, 3011, 3019, 3023, 3037, 3041, 3049, 3061, 3067, 3079,
        3083, 3089, 3109, 3119, 3121, 3137, 3163, 3167, 3169, 3181,
        3187, 3191, 3203, 3209, 3217, 3221, 3229, 3251, 3253, 3257,
        3259, 3271, 3299, 3301, 3307, 3313, 3319, 3323, 3329, 3331,
        3343, 3347, 3359, 3361, 3371, 3373, 3389, 3391, 3407, 3413,
        3433, 3449, 3457, 3461, 3463, 3467, 3469, 3491, 3499, 3511,
        3517, 3527, 3529, 3533, 3539, 3541, 3547, 3557, 3559, 3571,
        3581, 3583, 3593, 3607, 3613, 3617, 3623, 3631, 3637, 3643,
        3659, 3671, 3673, 3677, 3691, 3697, 3701, 3709, 3719, 3727,
        3733, 3739, 3761, 3767, 3769, 3779, 3793, 3797, 3803, 3821,
        3823, 3833, 3847, 3851, 3853, 3863, 3877, 3881, 3889, 3907,
        3911, 3917, 3919, 3923, 3929, 3931, 3943, 3947, 3967, 3989,
        4001, 4003, 4007, 4013, 4019, 4021, 4027, 4049, 4051, 4057,
        4073, 4079, 4091, 4093, 4099, 4111, 4127, 4129, 4133, 4139,
        4153, 4157, 4159, 4177, 4201, 4211, 4217, 4219, 4229, 4231,
        4241, 4243, 4253, 4259, 4261, 4271, 4273, 4283, 4289, 4297,
        4327, 4337, 4339, 4349, 4357, 4363, 4373, 4391, 4397, 4409,
        4421, 4423, 4441, 4447, 4451, 4457, 4463, 4481, 4483, 4493,
        4507, 4513, 4517, 4519, 4523, 4547, 4549, 4561, 4567, 4583,
        4591, 4597, 4603, 4621, 4637, 4639, 4643, 4649, 4651, 4657,
        4663, 4673, 4679, 4691, 4703, 4721, 4723, 4729, 4733, 4751,
        4759, 4783, 4787, 4789, 4793, 4799, 4801, 4813, 4817, 4831,
        4861, 4871, 4877, 4889, 4903, 4909, 4919, 4931, 4933, 4937,
        4943, 4951, 4957, 4967, 4969, 4973, 4987, 4993, 4999, 5003,
        5009, 5011, 5021, 5023, 5039, 5051, 5059, 5077, 5081, 5087,
        5099, 5101, 5107, 5113, 5119, 5147, 5153, 5167, 5171, 5179,
        5189, 5197, 5209, 5227, 5231, 5233, 5237, 5261, 5273, 5279,
        5281, 5297, 5303, 5309, 5323, 5333, 5347, 5351, 5381, 5387,
        5393, 5399, 5407, 5413, 5417, 5419, 5431, 5437, 5441, 5443,
        5449, 5471, 5477, 5479, 5483, 5501, 5503, 5507, 5519, 5521,
        5527, 5531, 5557, 5563, 5569, 5573, 5581, 5591, 5623, 5639,
        5641, 5647, 5651, 5653, 5657, 5659, 5669, 5683, 5689, 5693,
        5701, 5711, 5717, 5737, 5741, 5743, 5749, 5779, 5783, 5791,
        5801, 5807, 5813, 5821, 5827, 5839, 5843, 5849, 5851, 5857,
        5861, 5867, 5869, 5879, 5881, 5897, 5903, 5923, 5927, 5939,
        5953, 5981, 5987, 6007, 6011, 6029, 6037, 6043, 6047, 6053,
        6067, 6073, 6079, 6089, 6091, 6101, 6113, 6121, 6131, 6133,
        6143, 6151, 6163, 6173, 6197, 6199, 6203, 6211, 6217, 6221,
        6229, 6247, 6257, 6263, 6269, 6271, 6277, 6287, 6299, 6301,
        6311, 6317, 6323, 6329, 6337, 6343, 6353, 6359, 6361, 6367,
        6373, 6379, 6389, 6397, 6421, 6427, 6449, 6451, 6469, 6473,
        6481, 6491, 6521, 6529, 6547, 6551, 6553, 6563, 6569, 6571,
        6577, 6581, 6599, 6607, 6619, 6637, 6653, 6659, 6661, 6673,
        6679, 6689, 6691, 6701, 6703, 6709, 6719, 6733, 6737, 6761,
        6763, 6779, 6781, 6791, 6793, 6803, 6823, 6827, 6829, 6833,
        6841, 6857, 6863, 6869, 6871, 6883, 6899, 6907, 6911, 6917,
        6947, 6949, 6959, 6961, 6967, 6971, 6977, 6983, 6991, 6997,
        7001, 7013, 7019, 7027, 7039, 7043, 7057, 7069, 7079, 7103,
        7109, 7121, 7127, 7129, 7151, 7159, 7177, 7187, 7193, 7207,
        7211, 7213, 7219, 7229, 7237, 7243, 7247, 7253, 7283, 7297,
        7307, 7309, 7321, 7331, 7333, 7349, 7351, 7369, 7393, 7411,
        7417, 7433, 7451, 7457, 7459, 7477, 7481, 7487, 7489, 7499,
        7507, 7517, 7523, 7529, 7537, 7541, 7547, 7549, 7559, 7561,
        7573, 7577, 7583, 7589, 7591, 7603, 7607, 7621, 7639, 7643,
        7649, 7669, 7673, 7681, 7687, 7691, 7699, 7703, 7717, 7723,
        7727, 7741, 7753, 7757, 7759, 7789, 7793, 7817, 7823, 7829,
        7841, 7853, 7867, 7873, 7877, 7879, 7883, 7901, 7907, 7919,
    };

    // To use,
    // M[x] = 0, if not-squarefree
    // M[1] = 1, if even number of prime divisors
    // M[x] = -1, if odd
    // To compute counts[x] from left, sum(j=1..N/x, counts[x*j] * M(j)) -- O(N/x)
    // To compute counts[x] from right, sum(d|x, counts[x] * M(x/d))     -- O(tot(x))
    // -- To avoid division, process divisors from left & right

    public static sbyte[] MobiusTable(int n, int[] factors)
    {
        sbyte[] mobius = new sbyte[n + 1];
        mobius[1] = 1;
        for (int i = 2; i <= n; i++) {
            int j = i / factors[i];
            mobius[i] = (sbyte)(factors[j] != factors[i] ? -mobius[j] : 0);
        }

        return mobius;
    }

    public static sbyte[] FastMobiusTable(int n, int[] factors)
    {
        n = (n + 4) & ~3; // Align upward to multiple of 4 
        sbyte[] mobius = new sbyte[n + 1];
        mobius[1] = 1;
        mobius[2] = -1;
        for (int i = 3; i <= n; i += 2) {
            int j = i / factors[i];
            mobius[i] = (sbyte)(factors[j] != factors[i] ? -mobius[j] : 0);
            mobius[i + 1] = (sbyte)((i & 3) != 3 ? -mobius[(i + 1) >> 1] : 0);
        }

        return mobius;
    }

    // TODO: Need to review this-- d doesn't change
    public static Func<int, int> InclusionExclusion(
        int[] mobius, int[] leastPrimeFactor, int[] pfreq)
    {
        Func<int, int, int, int> dfs = null;
        dfs = (cur, n, d) => {
            int result = 0;
            if (n == 1) {
                if (d > 0) result = mobius[cur] * pfreq[cur];
                pfreq[cur] += d;
                if (d < 0) result = mobius[cur] * pfreq[cur];
                return result;
            }

            int lpf = leastPrimeFactor[n];
            result += dfs(cur, n / lpf, d);
            result += dfs(cur / lpf, n / lpf, d);
            return result;
        };

        return x => dfs(x, x, 1);
    }

    public static IEnumerable<int> DistinctPrimeFactorsOf(int[] table, int n)
    {
        int prev = 0;
        int k = n;
        while (k > 1) {
            int next = table[k];
            if (next != prev) yield return next;
            k /= next;
            prev = next;
        }
    }

    public static IEnumerable<int> MultiplePrimeFactorsOf(int[] table, int n)
    {
        int prev = 0;
        int k = n;
        while (k > 1) {
            int next = table[k];
            yield return next;
            k /= next;
            prev = next;
        }
    }

    public static long LargestPrimeFactor(List<int> primes, long n)
    {
        long largest = 0;
        long sqrt = (long)Ceiling(Sqrt(n));
        foreach (int p in primes) {
            if (p > sqrt) break;
            if (n % p == 0) {
                while (n % p == 0)
                    n /= p;
                largest = p;
                sqrt = (long)Ceiling(Sqrt(n));
                if (n == 1) break;
            }
        }

        return Max(largest, n);
    }

    /// <summary>
    ///     Returns a list of booleans indicating which numbers are prime.
    /// </summary>
    /// <returns>A boolean array indicating the primeness of each number</returns>
    public static BitArray GetPrimeBitArray(int max)
    {
        var isPrime = new BitArray(max + 1, true)
        {
            [0] = false,
            [1] = false,
        };

        // Should be 4
        for (int i = 4; i <= max; i += 2)
            isPrime[i] = false;

        int limit = (int)Sqrt(max) + 1;
        //for (int i = 3; i <= max; i += 2)
        for (int i = 3; i < limit; i += 2) {
            if (!isPrime[i]) continue;
            // NOTE: Squaring causes overflow
            for (long j = (long)i * i; j <= max; j += i + i)
                isPrime[(int)j] = false;
        }

        return isPrime;
    }

    public static BitArray GetPrimeBitArray(long lo, long hi)
    {
        if (lo == 0 && hi < int.MaxValue)
            return GetPrimeBitArray((int)hi);

        int range = (int)(hi - lo + 1);
        var check = new BitArray(range, true);

        // Mark all numbers less than 2 as composite
        for (long i = lo; i < 2; i++)
            check[(int)(i - lo)] = false;

        // Mark all even numbers as composite
        for (long i = Max(lo + (lo & 1), 4); i <= hi; i += 2)
            check[(int)(i - lo)] = false;

        int sqrt = (int)Ceiling(Sqrt(hi));
        BitArray primes = GetPrimeBitArray(sqrt);
        for (int i = 3; i <= sqrt; i += 2)
            if (primes[i]) {
                // Use longs here to avoid overflow
                long start = Max(lo, i);
                start -= start % i;
                if (start < lo || start == i) start += i;
                for (long j = start; j <= hi; j += i)
                    check[(int)(j - lo)] = false;
            }

        return check;
    }

    public static bool[] GetPrimeSet(int max)
    {
        int limit = (int)Sqrt(max) + 1;
        max++;

        bool[] isPrime = new bool[max];
        for (int i = 3; i < isPrime.Length; i += 2)
            isPrime[i] = true;
        isPrime[2] = true;

        for (int i = 3; i < limit; i += 2) {
            if (!isPrime[i]) continue;
            // NOTE: Squaring causes overflow
            for (long j = (long)i * i; j < max; j += i + i)
                isPrime[j] = false;
        }

        return isPrime;
    }

    public static int[] GetPrimes(int max)
    {
        bool[] isPrime = GetPrimeSet(max);
        int count = 1;
        for (int i = 3; i <= max; i += 2)
            if (isPrime[i])
                count++;

        int[] primes = new int[count];
        int p = 0;
        primes[p++] = 2;
        for (int i = 3; i <= max; i += 2)
            if (isPrime[i])
                primes[p++] = i;
        return primes;
    }

    // TODO: Looks redundant with totient function
    /// <summary>
    /// </summary>
    /// <param name="primes"> a sorted list of primes</param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static long NumberOfNoncoprimes(IList<int> primes, int n) => n - TotientFromPrimes(primes, n);

    public static int TotientFunction(int[] table, int n)
    {
        int prev = 0;
        int result = n;
        for (int k = n; k > 1; k /= prev) {
            int next = table[k];
            if (next != prev) result -= result / next;
            prev = next;
        }

        return result;
    }

    public static int TotientFromPrimes(IEnumerable<int> primes, int n)
    {
        int result = n;
        foreach (int p in primes)
            result -= result / p;
        return result;
    }

    public static long NumberUpToKCoprimeWithP(int[] table, int k, int p)
    {
        int totient = TotientFunction(table, p);
        int wholes = k / p;
        int frac = k % p;
        long parts = table[p] > frac ? frac : k - ScanNonCoprime(table, frac, p);
        return wholes * totient + parts;
    }

    static int ScanNonCoprime(int[] table, int frac, int p, int f2 = 1)
    {
        if (p == 1) return frac;

        int f = table[p];
        if (f > frac) return frac;
        do
            p /= f;
        while (p != 1 && table[p] == f);

        int result = 0;
        int f2New = f * f2;
        if (f2New <= frac) {
            result += frac / f2New;
            result += ScanNonCoprime(table, frac, p, f2);
            result -= ScanNonCoprime(table, frac, p, f2New);
        }

        return result;
    }

    public static int EnumerateFactors(int[] factors, int n, int max, Action<int> action = null, int f = 1)
    {
        if (f > max)
            return 0;

        if (n == 1) {
            action?.Invoke(f);
            return 1;
        }

        int p = factors[n];
        int c = 1;
        int next = n / p;
        while (next > 1 && factors[next] == p) {
            c++;
            next = next / p;
        }

        int result = EnumerateFactors(factors, next, max, action, f);
        while (c-- > 0) {
            f *= p;
            result += EnumerateFactors(factors, next, max, action, f);
        }

        return result;
    }

    public static int EnumerateFactors(int[] factors, int n1, int n2, int max, Action<int> action = null, int f = 1)
    {
        if (f > max)
            return 0;

        if (n1 == 1 && n2 == 1) {
            action?.Invoke(f);
            return 1;
        }

        int p1 = factors[n1];
        int p2 = factors[n2];
        int p = n1 == 1 ? p2 : n2 == 1 ? p1 : Min(p1, p2);

        int c = 0;
        int next1 = n1;
        int next2 = n2;

        while (next1 > 1 && factors[next1] == p) {
            c++;
            next1 /= p;
        }

        while (next2 > 1 && factors[next2] == p) {
            c++;
            next2 /= p;
        }

        int result = EnumerateFactors(factors, next1, next2, max, action, f);
        while (c-- > 0) {
            if (f * 1L * p > max)
                break;

            f *= p;
            result += EnumerateFactors(factors, next1, next2, max, action, f);
        }

        return result;
    }

    public static int[][] DivisorsUpTo(int n)
    {
        n++;
        short[] counts = new short[n];
        int[][] divisors = new int[n][];
        for (int i = 1; i < n; i++)
        for (int j = i; j < n; j += i)
            counts[j]++;

        for (int i = 1; i < n; i++)
            divisors[i] = new int[counts[i]];

        for (int i = 1; i < n; i++)
        for (int j = i; j < n; j += i)
            divisors[j][--counts[j]] = i;
        return divisors;
    }

    public static int[][] PrimeDivisorsUpTo(int n)
    {
        n++;
        short[] counts = new short[n];
        bool[] prime = new bool[n];
        int[][] divisors = new int[n][];
        for (int i = 2; i < n; i++) {
            if (counts[i] != 0) continue;
            prime[i] = true;
            for (int j = i; j < n; j += i)
                counts[j]++;
        }

        for (int i = 1; i < n; i++)
            divisors[i] = new int[counts[i]];

        for (int i = 1; i < n; i++) {
            if (!prime[i]) continue;
            for (int j = i; j < n; j += i)
                divisors[j][--counts[j]] = i;
        }

        return divisors;
    }

    /// <summary>
    ///     Tau Function.
    /// </summary>
    /// <returns></returns>
    public static int[] CountOfDivisors(int n)
    {
        n++;
        int[] divisors = new int[n];
        int[] multiplicities = new int[divisors.Length];

        for (int i = 0; i < n; i++)
            divisors[i] = 1;

        for (long i = 2; i < n; i++) {
            if (multiplicities[i] != 0) continue;

            for (long k = i, p = 1; k < n; k = k * i, p++)
            for (long j = k; j < n; j += k)
                multiplicities[j] = (int)p;

            for (long j = i; j < n; j += i)
                divisors[j] *= 2 * multiplicities[j] + 1;
        }

        return divisors;
    }

    public static int[] CountOfDivisorsAlt(int n)
    {
        n++;
        int[] counts = new int[n];
        for (int i = 1; i < n; i++)
        for (int j = i; j < n; j += i)
            counts[j]++;
        return counts;
    }

    //Check out  https://math.stackexchange.com/questions/22721/is-there-a-formula-to-calculate-the-sum-of-all-proper-divisors-of-a-number
    // for calculating bulk sod with generalized pentagonal numbers

    static long SumOfDivisors(long z)
    {
        long s = z * z;
        long p = 2;
        long d;
        for (d = 1; d != 0; d++) {
            long n = z / d - z / (d + 1);
            if (n <= 1) {
                p = d;
                break;
            }

            long a = z % d;
            s -= (2 * a + (n - 1) * d) * n / 2;
        }

        for (d = 2; d <= z / p; d++)
            s -= z % d;

        return s;
    }

    static long DivisorFunction(long prime, long pow, long k)
    {
        // Divisor function is multiplicative on powers of primes.

        // An alternative approach is to factorize a number and then compute
        // sum and counts from the number using the multiplicative properties of the divisor 
        // function on powers of primes

        // k=0, Number of Divisors
        // k=1, Sum Of Divisors

        if (k == 0) return pow + 1;
        if (k == 1 && pow == 1) return prime + 1;
        return (long)((Pow(prime, (pow + 1) * k) - 1) / (Pow(prime, k) - 1));
    }

    // SOD(p^a) = (p^(a+1) - 1) / (p-1)
    // DivisorFunction0 = 2 

    /// <summary>
    ///     Returns a list of prime factors for each number up to n.
    ///     Space and time complexity is O(n lg(n)).
    /// </summary>
    /// <param name="n">The n.</param>
    /// <returns></returns>
    public static int[] PrimeFactorsUpTo(int n)
    {
        int[] factors = new int[n + 1];

        for (int i = 2; i <= n; i += 2)
            factors[i] = 2;

        int sqrt = (int)Sqrt(n);
        for (int i = 3; i <= sqrt; i += 2) {
            if (factors[i] != 0) continue;
            for (int j = i * i; j <= n; j += i + i)
                if (factors[j] == 0)
                    factors[j] = i;
        }

        for (int i = 3; i <= n; i += 2)
            if (factors[i] == 0)
                factors[i] = i;

        return factors;
    }

    public static void GenerateFactors(int[] factors, int n, List<int> result)
    {
        result.Clear();
        result.Add(1);
        int prev = -1;
        int start = 0;
        while (n > 1) {
            int f = factors[n];
            if (f != prev) {
                start = 0;
                prev = f;
            }

            n /= f;
            int end = result.Count;
            for (; start < end; start++)
                result.Add(result[start] * f);
        }

        result.Sort();
    }
}