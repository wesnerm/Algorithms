namespace Algorithms.Mathematics;

#if false
[TestFixture]
public class MultipointInterpolationTests
{
    void CheckEqual(IList<long> list1, IList<long> list2)
    {
        Assert.AreEqual(list1.Count, list2.Count);
        Assert.AreEqual(list1, list2);
    }

    const int M = 100000007;

    [Test]
    public void DivModTest()
    {
        var rnd = new Random(123456);
        for (int n = 2; n < 1000; ++n)
        {
            int m = rnd.Next(1, n + 1);
            long[] p = new long[n];
            long[] q = new long[m];
            for (int i = 0; i < p.Length; ++i) p[i] = rnd.Next(1, M);
            for (int i = 0; i < q.Length; ++i) q[i] = rnd.Next(1, M);
            var pq1 = divmod(p, q, M);
            CheckEqual(p, add(mul(pq1.fst, q, M), pq1.snd, M));
            var pq2 = divmod_n(p, q, M);
            CheckEqual(p, add(mul(pq2.fst, q, M), pq2.snd, M));
            CheckEqual(pq1.fst, pq2.fst);
            CheckEqual(pq1.snd, pq2.snd);
        }
    }

    [Test]
    public void MulTest()
    {
        var rnd = new Random(123456);
        for (int n = 2; n < 1000; ++n)
        {
            long[] p = new long[n];
            long[] q = new long[n];
            for (int i = 0; i < p.Length; ++i) p[i] = rnd.Next(1, M);
            for (int i = 0; i < q.Length; ++i) q[i] = rnd.Next(1, M);
            var pq1 = mul(p, q, M);
            var pq2 = mul_n(p, q, M);
            CheckEqual(pq1, pq2);
        }
    }

    [Test]
    public void EvaluateTest()
    {
        var rnd = new Random(123456);
        for (int n = 2; n < 1000; ++n)
        {
            long[] p = new long[n];
            for (int i = 0; i < p.Length; ++i) p[i] = rnd.Next(1, M);
            int m = rnd.Next(1, 2 * n + 1);
            long[] x = new long[m];
            for (int i = 0; i < x.Length; ++i) x[i] = rnd.Next(1, M);
            long[] y = evaluate(p, x, M);
            long[] z = new long[m];
            for (int i = 0; i < m; ++i) z[i] = eval(p, x[i], M);
            for (int i = 0; i < x.Length; ++i)
                Assert.AreEqual(y[i], eval(p, x[i], M));
        }
    }

    [Test]
    public void InterpolationTest()
    {
        var rnd = new Random(123456);
        for (int n = 2; n < 100000; n *= 2)
        {
            var xi = new HashSet<long>();
            while (xi.Count() < n) xi.Add(rnd.Next(M));
            long[] x = xi.ToArray();
            long[] y = new long[x.Length];
            for (int i = 0; i < y.Length; ++i) y[i] = rnd.Next(1, M);
            Console.Write($"{n} "); ;
            long[] q = interpolate(x, y, M);
            Console.Write($"tick() ");
            for (int i = 0; i < x.Length; ++i)
                AreEqual(y[i], eval(q, x[i], M));
            long[] r = interpolate_n(x, y, M);
            Console.WriteLine($"tick()");
            for (int i = 0; i < x.Length; ++i)
                AreEqual(y[i], eval(r, x[i], M));
            CheckEqual(q, r);
        }
    }
}
#endif