namespace Algorithms.Mathematics.Multiplication;

[TestFixture]
public class KaratsubaMultiplicationModTest : FftTestBase
{
    public override int MOD => 1000000007;

    protected override long[] Multiply(long[] a, long[] b) => KaratsubaMultiplicationMod.Karatsuba(a, b, 0, MOD);

}