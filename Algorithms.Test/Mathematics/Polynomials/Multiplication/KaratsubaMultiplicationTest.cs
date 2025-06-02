namespace Algorithms.Mathematics.Multiplication;

[TestFixture]
public class KaratsubaMultiplicationTest : FftTestBase
{
    protected override long[] Multiply(long[] a, long[] b) => KaratsubaMultiplication.Karatsuba(a, b, 0);

}