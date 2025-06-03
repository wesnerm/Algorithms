using System.Numerics;

namespace Algorithms.Collections.Trees;

[TestFixture]
public class VanEmdeBoasDenseTest
{
    [Test]
    public void ComputeNodesAndDepth()
    {
        int[] T = new[]
        {
            1, 2, 4, 8, 10, 16, 32, 64, 100, 128, 256, 512, 1000, 1024, 2048, 4096, 8092, 10000, 16384, 32768, 65536,
            100000, 131072, 262144, 524288, 100000,
        };
        foreach (int m in T) {
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

        int m = BitOperations.Log2((uint)(M - 1)) + 1;
        int m2 = m;
        int loBits = m2 / 2;
        int hiBits = m2 - loBits;
        long hi = 1L << hiBits;
        long lo = 1L << loBits;

        long call = F1(lo, depth);
        if (depth == false)
            call *= hi + 1;
        return 1 + call;
    }

    [Test]
    public void InsertionTest()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        AreEqual(0, veb.Count);

        True(veb.Insert(2));
        AreEqual(1, veb.Count);
        AreEqual(2, veb.Min);
        AreEqual(2, veb.Max);
        AreEqual(-1, veb.Previous(2));
        AreEqual(-1, veb.Previous(1));
        AreEqual(-1, veb.Previous(0));
        AreEqual(2, veb.Previous(100));
        AreEqual(2, veb.Previous(100000));
        AreEqual(2, veb.Next(0));
        AreEqual(2, veb.Next(1));
        AreEqual(int.MaxValue, veb.Next(2));

        True(veb.Insert(100000));
        AreEqual(2, veb.Count);
        AreEqual(2, veb.Min);
        AreEqual(100000, veb.Max);
        AreEqual(2, veb.Previous(1000));
        AreEqual(100000, veb.Next(1000));

        False(veb.Insert(2));
        AreEqual(2, veb.Count);
        AreEqual(2, veb.Min);
        AreEqual(100000, veb.Max);
    }

    [Test]
    public void DeletionTest()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        int count = 0;
        for (int i = 3; i <= 3000000; i *= 10, count++)
            True(veb.Insert(i));
        AreEqual(count, veb.Count);

        for (int i = 1; i <= 1000000; i *= 10, count++)
            True(veb.Insert(i));
        AreEqual(count, veb.Count);

        AreEqual(1, veb.Min);
        AreEqual(3000000, veb.Max);

        False(veb.Delete(40000));
        False(veb.Insert(30000));
        True(veb.Delete(3));
        True(veb.Delete(1000000));
        count -= 2;
        AreEqual(count, veb.Count);

        AreEqual(30000, veb.Next(10000));
        AreEqual(100000, veb.Previous(300000));
        AreEqual(1, veb.Min);
        AreEqual(3000000, veb.Max);

        True(veb.Delete(1));
        count--;
        AreEqual(count, veb.Count);
        False(veb.Contains(1));

        True(veb.Delete(3000000));
        count--;
        AreEqual(count, veb.Count);
        AreEqual(300000, veb.Max);
    }

    [Test]
    public void CompleteDeletionTest()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        int count = 0;
        for (int i = 3; i <= 3000000; i *= 10, count++)
            True(veb.Insert(i));
        AreEqual(count, veb.Count);

        while (veb.Max != -1) {
            True(veb.Delete(veb.Max));
            count--;
            AreEqual(count, veb.Count);
        }

        AreEqual(0, veb.Count);
        AreEqual(-1, veb.Max);
        AreEqual(int.MaxValue, veb.Min);
    }

    [Test]
    public void CompleteDeletion2Test()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        int count = 0;
        for (int i = 3; i <= 3000000; i *= 10, count++)
            True(veb.Insert(i));
        AreEqual(count, veb.Count);

        while (veb.Min != int.MaxValue) {
            True(veb.Delete(veb.Min));
            count--;
            AreEqual(count, veb.Count);
        }

        AreEqual(0, veb.Count);
        AreEqual(-1, veb.Max);
        AreEqual(int.MaxValue, veb.Min);
    }

    [Test]
    public void TableTest()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        int count = 0;
        for (int i = 3; i <= 3000000; i *= 10, count++)
            True(veb.Insert(i));
        AreEqual(count, veb.Count);

        int[] table = veb.Table;
        AreEqual(table.Length, veb.Count);
    }

    [Test]
    public void FindNextTest()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        int count = 0;
        for (int i = 3; i <= 3000000; i *= 10, count++)
            True(veb.Insert(i));
        AreEqual(count, veb.Count);

        AreEqual(veb.Min, veb.Next(-1));
        AreEqual(veb.Max, veb.Previous(int.MaxValue));

        int x = -1;
        int count2 = 0;
        int prev = x;
        while ((x = veb.Next(x)) < int.MaxValue) {
            Less(prev, x);
            True(veb.Contains(x));
            prev = x;
            count2++;
        }

        True(count2 == count);
    }

    [Test]
    public void FindPrevTest()
    {
        var veb = new VanEmdeBoasDense(1000 * 1000 * 10);

        int count = 0;
        for (int i = 3; i <= 3000000; i *= 10, count++)
            True(veb.Insert(i));
        AreEqual(count, veb.Count);

        AreEqual(veb.Min, veb.Next(-1));
        AreEqual(veb.Max, veb.Previous(int.MaxValue));

        int x = int.MaxValue;
        int count2 = 0;
        int prev = x;
        while ((x = veb.Previous(x)) > -1) {
            Greater(prev, x);
            True(veb.Contains(x));
            prev = x;
            count2++;
        }

        True(count2 == count);
    }
}