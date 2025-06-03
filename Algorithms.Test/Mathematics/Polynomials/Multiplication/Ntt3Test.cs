using Algorithms.Mathematics.Multiplication.FFT;
using Algorithms.Mathematics.Multiplication.NTT;
using System.Numerics;
using System.Reflection.Metadata;

namespace Algorithms.Mathematics.Multiplication;

[TestFixture]
public class Ntt3Test : FftTestBase
{
    public override int MOD => 998244353;

    protected override long[] Multiply(long[] a, long[] b)
    {
        int maxSize = a.Length + b.Length - 1;
        var c = new Ntt3((int)BitOperations.RoundUpToPowerOf2((uint)maxSize), MOD);;
        return c.Multiply(a, b, maxSize);
    }
}