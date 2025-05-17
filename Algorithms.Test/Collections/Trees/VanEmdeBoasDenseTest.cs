using static System.Math;

namespace Algorithms.Collections.Trees;

[TestFixture]
public class VanEmdeBoasDenseTest
{
    [Test]
    public void ComputeNodesAndDepth()
    {
        var T = new int[] { 1, 2, 4, 8, 10, 16, 32, 64, 100, 128, 256, 512, 1000, 1024, 2048, 4096, 8092, 10000, 16384, 32768, 65536, 100000, 131072, 262144, 524288, 100000 };
        foreach (var m in T)
        {
            long numberOfNodes = F1(m, false);
            long depth = F1(m, true);
            double rat = numberOfNodes * 1d / m;
            Console.WriteLine($"{m,9} -> {numberOfNodes,8} {rat:N2}  (D={depth}) ");
        }
    }

    public long F1(long M, bool depth)
    {
        if (M <= 1)
            return 1;

        int m = Log2(M - 1) + 1;
        int m2 = m;
        int loBits = m2 / 2;
        int hiBits = m2 - loBits;
        long hi = 1L << hiBits;
        long lo = 1L << loBits;

        var call = F1(lo, depth);
        if (depth == false)
            call *= (hi + 1);
        return 1 + call;
    }

    [Test]
    public void InsertionTest()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        Assert.AreEqual(0, veb.Count);

        Assert.True(veb.Insert(2));
        Assert.AreEqual(1, veb.Count);
        Assert.AreEqual(2, veb.Min);
        Assert.AreEqual(2, veb.Max);
        Assert.AreEqual(-1, veb.Previous(2));
        Assert.AreEqual(-1, veb.Previous(1));
        Assert.AreEqual(-1, veb.Previous(0));
        Assert.AreEqual(2, veb.Previous(100));
        Assert.AreEqual(2, veb.Previous(100000));
        Assert.AreEqual(2, veb.Next(0));
        Assert.AreEqual(2, veb.Next(1));
        Assert.AreEqual(int.MaxValue, veb.Next(2));
        
        Assert.True(veb.Insert(100000));
        Assert.AreEqual(2, veb.Count);
        Assert.AreEqual(2, veb.Min);
        Assert.AreEqual(100000, veb.Max);
        Assert.AreEqual(2, veb.Previous(1000));
        Assert.AreEqual(100000, veb.Next(1000));

        Assert.False(veb.Insert(2));
        Assert.AreEqual(2, veb.Count);
        Assert.AreEqual(2, veb.Min);
        Assert.AreEqual(100000, veb.Max);
    }

    [Test]
    public void DeletionTest()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        int count = 0;
        for (int i = 3; i <= 3000000; i *= 10, count++)
            Assert.True(veb.Insert(i));
        Assert.AreEqual(count, veb.Count);

        for (int i = 1; i <= 1000000; i *= 10, count++)
            Assert.True(veb.Insert(i));
        Assert.AreEqual(count, veb.Count);

        Assert.AreEqual(1, veb.Min);
        Assert.AreEqual(3000000, veb.Max);

        Assert.False(veb.Delete(40000));
        Assert.False(veb.Insert(30000));
        Assert.True(veb.Delete(3));
        Assert.True(veb.Delete(1000000));
        count -= 2;
        Assert.AreEqual(count, veb.Count);

        Assert.AreEqual(30000, veb.Next(10000));
        Assert.AreEqual(100000, veb.Previous(300000));
        Assert.AreEqual(1, veb.Min);
        Assert.AreEqual(3000000, veb.Max);

        Assert.True(veb.Delete(1));
        count--;
        Assert.AreEqual(count, veb.Count);
        Assert.False(veb.Contains(1));

        Assert.True(veb.Delete(3000000));
        count--;
        Assert.AreEqual(count, veb.Count);
        Assert.AreEqual(300000, veb.Max);
    }

    [Test]
    public void CompleteDeletionTest()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        int count = 0;
        for (int i = 3; i <= 3000000; i *= 10, count++)
            Assert.True(veb.Insert(i));
        Assert.AreEqual(count, veb.Count);

        while (veb.Max != -1)
        {
            Assert.True(veb.Delete(veb.Max));
            count--;
            Assert.AreEqual(count, veb.Count);
        }

        Assert.AreEqual(0, veb.Count);
        Assert.AreEqual(-1, veb.Max);
        Assert.AreEqual(int.MaxValue, veb.Min);
    }

    [Test]
    public void CompleteDeletion2Test()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        int count = 0;
        for (int i = 3; i <= 3000000; i *= 10, count++)
            Assert.True(veb.Insert(i));
        Assert.AreEqual(count, veb.Count);

        while (veb.Min != int.MaxValue)
        {
            Assert.True(veb.Delete(veb.Min));
            count--;
            Assert.AreEqual(count, veb.Count);
        }

        Assert.AreEqual(0, veb.Count);
        Assert.AreEqual(-1, veb.Max);
        Assert.AreEqual(int.MaxValue, veb.Min);
    }

    [Test]
    public void TableTest()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        int count = 0;
        for (int i = 3; i <= 3000000; i *= 10, count++)
            Assert.True(veb.Insert(i));
        Assert.AreEqual(count, veb.Count);

        var table = veb.Table;
        Assert.AreEqual(table.Length, veb.Count);

    }

    [Test] 
    public void FindNextTest()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        int count = 0;
        for (int i = 3; i <= 3000000; i *= 10, count++)
            Assert.True(veb.Insert(i));
        Assert.AreEqual(count, veb.Count);

        Assert.AreEqual(veb.Min, veb.Next(-1));
        Assert.AreEqual(veb.Max, veb.Previous(int.MaxValue));

        int x = -1;
        int count2 = 0;
        int prev = x;
        while ((x = veb.Next(x)) < int.MaxValue)
        {
            Assert.Less(prev, x);
            Assert.True(veb.Contains(x));
            prev = x;
            count2++;
        }

        Assert.True(count2 == count);
    }

    [Test]
    public void FindPrevTest()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        int count = 0;
        for (int i = 3; i <= 3000000; i *= 10, count++)
            Assert.True(veb.Insert(i));
        Assert.AreEqual(count, veb.Count);

        Assert.AreEqual(veb.Min, veb.Next(-1));
        Assert.AreEqual(veb.Max, veb.Previous(int.MaxValue));

        int x = int.MaxValue;
        int count2 = 0;
        int prev = x;
        while ((x = veb.Previous(x)) >-1)
        {
            Assert.Greater(prev, x);
            Assert.True(veb.Contains(x));
            prev = x;
            count2++;
        }

        Assert.True(count2 == count);

    }

}
