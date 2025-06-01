namespace Algorithms.Mathematics.Multiplication.WalshTransform;

[TestFixture]
public class FastWalshTransformTest
{
    public int[] A2 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
    public int[] B2 = new[] { 1, 2, 4, 8, 16, 32, 64, 128 };

    public int[] A3 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    public int[] B3 = new[] { 1, 2, 4, 8, 16, 32, 64, 128, 256 };

    [Test]
    public void TestWalsh2D()
    {
        int[] sol = Mult2(A2, B2);
        int[] sol2 = A2.ToArray();
        FastWalshTransform.Convolution(sol2, B2, sol2.Length);
        AreEqual(sol, sol2);
    }

    public static int ConvertNumber(int x)
    {
        int xx = x;
        int result = 0;

        while (xx != 0) {
            int b = xx & -xx;
            xx -= b;
            int b2 = Log2(b);
            result += 1 << (b2 * 2);
        }

        return result;
    }

    static int[] Mult2(int[] d1, int[] d2)
    {
        if (d1 == null || d2 == null)
            return d1 ?? d2;

        int[] d = new int[d1.Length];
        int maxnum = d1.Length;
        for (int i = 0; i < maxnum; i++) {
            long r = d1[i];
            if (r != 0)
                for (int j = 0; j < maxnum; j++)
                    if (d2[j] != 0)
                        d[i ^ j] += (int)(1L * r * d2[j]);
        }

        return d;
    }

    static int Combine(int a, int b)
    {
        const int mask1 = 0x11111111;
        const int mask2 = 0x4444444;

        int x = (a & (mask1 * 3)) + (b & (mask1 * 3));
        int y = (a & (mask2 * 3)) + (b & (mask2 * 3));
        return 0
               | (x - (((x + mask1) & (mask1 * 4)) >> 2) * 3)
               | (y - (((y + mask2) & (mask2 * 4)) >> 2) * 3);
    }
}