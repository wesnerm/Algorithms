namespace Algorithms.Mathematics;

public class SquareFreeNumbers
{
    readonly int[] primes;

    public SquareFreeNumbers(long max)
    {
        int sqrt = (int)MathUtil.Sqrt(max) + 1;
        primes = Factorization.GetPrimes(sqrt);
    }

    public long GetSquareFreeNumbers(long n) => n - NumberOfSquaredNumbers(0, n);

    long NumberOfSquaredNumbers(int index, long n)
    {
        if (index == primes.Length) return 0;

        long p = primes[index];
        long sqr = p * p;
        if (sqr > n)
            return 0;

        long result = n / sqr;
        result -= NumberOfSquaredNumbers(index + 1, result);
        result += NumberOfSquaredNumbers(index + 1, n);
        return result;
    }
}

public class SquareFreeNumbers2
{
    readonly sbyte[] mobius;

    public SquareFreeNumbers2(long max)
    {
        int limit = (int)MathUtil.Sqrt(max) + 1;

        int[] factors = Factorization.PrimeFactorsUpTo(limit + 4);
        mobius = Factorization.MobiusTable(limit, factors);
    }

    public long GetSquareFreeNumbers(long n)
    {
        long limit = MathUtil.Sqrt(n) + 1;
        long result = n;
        for (long i = 2; i < limit; i++)
            result += mobius[i] * (n / (i * i));
        return result;
    }
}

public class SquareFreeNumbers3
{
    public long GetSquareFreeNumbers(long n)
    {
        long result = n;

        var sieve = new SegmentedSieve();
        sieve.MobiusSieve(2, (int)MathUtil.Sqrt(n), (lo, hi, mobius) => {
            long sum = 0;
            for (long i = lo; i <= hi; i++) {
                sbyte m = mobius[i - lo];
                if (m != 0) sum += m * (n / (i * i));
            }

            result += sum;
        });

        return result;
    }
}