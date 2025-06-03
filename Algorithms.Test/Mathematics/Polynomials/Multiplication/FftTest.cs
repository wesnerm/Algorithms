using System.Numerics;

namespace Algorithms.Mathematics.Multiplication;

[TestFixture]
public class FftTest : FftTestBase
{
    public override int MOD => 998244353;

    protected override long[] Multiply(long[] a, long[] b)
    {
        int maxSize = a.Length + b.Length - 1;
        var c = new FastFourierTransformMod(BitOperations.Log2(BitOperations.RoundUpToPowerOf2((uint)maxSize)), MOD);;
        return c.Multiply(a, b, maxSize);
    }
}