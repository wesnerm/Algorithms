using Algorithms.Mathematics.Multiplication.FFT;
using Algorithms.Mathematics.Multiplication.NTT;
using System.Numerics;
using System.Reflection.Metadata;

namespace Algorithms.Mathematics.Multiplication;

[TestFixture]
public class NttCrtTest : FftTestBase
{
    protected override long[] Multiply(long[] a, long[] b)
    {
        int maxSize = a.Length + b.Length - 1;
        var nttBase = new NttMB((int)BitOperations.RoundUpToPowerOf2((uint)maxSize));
        var c = new NttCrt(nttBase);
        return c.Multiply(a, b, maxSize, 0);
    }
}