using static System.Math;

namespace Algorithms.Mathematics;

public class SegmentedSieve
{
    public int[] Primes;
    public bool[] Range;

    public void Run(int lo, int hi, Action<int, int> action, int bucketSize = 1000000)
    {
        int size = Min(hi - lo + 1, bucketSize);
        for (int start = lo; start <= hi; start += size) {
            int newLo = start;
            int newHi = Min(start + size - 1, hi);
            action(newLo, newHi);
        }
    }

    public void PrimeSieve(int start, int end, Action<int, int, bool[]> action, int bucketSize = 1000000)
    {
        int sqrt = (int)Sqrt(end);
        Primes = GetPrimes(sqrt);
        int size = Min(end - start + 1, bucketSize);
        bool[] range = new bool[size];
        Run(start, end, (lo, hi) => {
            GetPrimeSet(lo, hi, range, Primes);
            action(lo, hi, range);
        }, bucketSize);
    }

    public void MobiusSieve(int start, int end, Action<int, int, sbyte[]> action, int bucketSize = 1000000)
    {
        Primes = GetPrimes(end);
        int rangeSize = Min(end - start + 1, bucketSize);
        sbyte[] mobiusStore = new sbyte[rangeSize];
        Run(start, end, (lo, hi) => {
            int[] primes = Primes;
            int size = hi - lo + 1;

            sbyte[] mobius = mobiusStore;
            for (int i = 0; i < size; i++)
                mobius[i] = 1;

            for (int i = 0; i < primes.Length; i++) {
                int p = primes[i];
                for (long q = Adjust(p, lo); q <= hi; q += p)
                    mobius[q - lo] *= -1;
                for (long r = Adjust(p * p, lo), q = r; q <= hi; q += r)
                    mobius[q - lo] = 0;
            }

            action(lo, hi, mobius);
        }, rangeSize);
    }

    public static bool[] GetPrimeSet(int max)
    {
        int limit = (int)Sqrt(max) + 1;
        max++;

        bool[] isPrime = new bool[max];
        for (int i = 3; i < isPrime.Length; i += 2)
            isPrime[i] = true;
        if (2 < isPrime.Length)
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

    public static bool[] GetPrimeSet(long lo, long hi, bool[] range = null, int[] primes = null)
    {
        if (primes == null) {
            int sqrt = (int)Ceiling(Sqrt(hi));
            primes = GetPrimes(sqrt);
        }

        int size = (int)(hi - lo + 1);
        if (range == null)
            range = new bool[size];
        else
            Array.Clear(range, 0, size);

        if (lo <= 2 && 2 <= hi)
            range[2 - lo] = true;

        for (long i = Max(lo | 1, 3); i <= hi; i += 2)
            range[i - lo] = true;

        for (int ip = 1; ip < primes.Length; ip++) {
            int p = primes[ip];
            long start = Max(lo, p * p);
            if (start > hi) break;
            start -= start % p;
            if (start < lo || start == p) start += p;
            if ((start & 1) == 0) start += p;
            for (long j = start; j <= hi; j += p + p)
                range[j - lo] = false;
        }

        return range;
    }

    public static long Adjust(int p, long lo)
    {
        long start = lo;
        start -= start % p;
        if (start < lo) start += p;
        return start;
    }
}