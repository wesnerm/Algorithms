namespace Algorithms.Mathematics;

public class DirichletConvolution { }

/*    Prefix sum of multiplicative Func<long, long>tions :
        p_f : the prefix sum of f (x) (1 <= x <= th).
        p_g : the prefix sum of g (x) (0 <= x <= N).
        p_c : the prefix sum of f * g (x) (0 <= x <= N).
        th : the thereshold, generally should be n ^ (2 / 3).
*/

// https://codeforces.com/blog/entry/53925  -- Mobius function
// https://codeforces.com/blog/entry/54150  -- Dirichlet function

// Multiplicative functions
// 1. constant function  I(p^k) = 1
// 2. identity function  Id(p^k) = p^k
// 3. power function  Id_a(p^k) = p^ak
// 4. unit function  eps(p^k) = [p^k == 1]
// 5. divisor function  sigma_a(p^k) = sum(i=0, k, p^ai)
// 6. moebius function  mobius(p^k) = [k==0] - [k==1]
// 7. totient function  totient(p^k) = p^k - p^(k-1)

public class PrefixMultiplication
{
    readonly Func<long, long> csum;
    readonly Func<long, long> fsum;
    readonly Func<long, long> gsum;
    Dictionary<long, long> _cache;
    long inv;
    long n, threshold;

    public PrefixMultiplication(Func<long, long> p_f, Func<long, long> p_g, Func<long, long> p_c)
    {
        fsum = p_f;
        gsum = p_g;
        csum = p_c;
    }

    long Calc(long x)
    {
        if (x <= threshold) return fsum(x);

        long answer = 0;
        if (_cache.TryGetValue(x, out answer))
            return answer;

        for (long i = 2, last; i <= x; i = last + 1) {
            // Does floor(x*i/x) work? No.. because that would just be i.
            long x_i = x / i;
            last = x / x_i;
            answer += (gsum(last) - gsum(i - 1)) * Calc(x_i);
        }

        /*

        // Optimized version of above loop that performs half as many calculations
        if (x>=2)
        {
            long prev = gsum(1);
            for (long i = 2, last; i <= x; i = last + 1)
            {
                var x_i = x / i;
                // if (x_i < i) break;
                last = x / x_i;
                var next = gsum(last);
                answer += (next - prev) * Calc(x_i);
                prev = next;
            }
        }
        */

        answer = csum(x) - answer;
        answer = answer / inv;
        return _cache[x] = answer;
    }

    public long Solve(long n, long threshold)
    {
        if (n <= 0) return 0;
        this.n = n;
        this.threshold = threshold;
        inv = gsum(1);
        return Calc(n);
    }
}