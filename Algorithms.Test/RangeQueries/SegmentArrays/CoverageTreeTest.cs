namespace Algorithms.RangeQueries;

[TestFixture]
public class CoverageTreeTest
{
    [Test]
    public void SortAndRemoveDupesTest()
    {
        AreEqual(new int [0],
            CoverageTree.SortAndRemoveDupes(new int[] { }));

        AreEqual(new[] { 1 },
            CoverageTree.SortAndRemoveDupes(new[] { 1, 1, 1, 1 }));

        AreEqual(new[] { 1, 2, 3, 4 },
            CoverageTree.SortAndRemoveDupes(new[] { 3, 2, 1, 2, 4, 3 }));
    }

    [Test]
    public void CoverageTreeBasicTest()
    {
        var cov = new CoverageTree(15);
        cov.UpdateIndex(5, 10, 1);
        AreEqual(6, cov.QueryAll());
        AreEqual(3, cov.QueryX(8, 11));

        var cov2 = new CoverageTree(15);
        cov2.UpdateX(5, 10, 1);
        AreEqual(5, cov2.QueryAll());
        AreEqual(2, cov2.QueryX(8, 11));
    }

    [Test]
    public void NextTest()
    {
        var cov = new CoverageTree(15);
        cov.UpdateIndex(5, 10, 1);
        AreEqual(5, cov.NextIndex(0));
        AreEqual(6, cov.NextIndex(5));
        AreEqual(10, cov.NextIndex(9));
        AreEqual(15, cov.NextIndex(10));
        AreEqual(1, cov.NextIndex(0, false));
        AreEqual(11, cov.NextIndex(4, false));
        AreEqual(11, cov.NextIndex(5, false));
        AreEqual(11, cov.NextIndex(7, false));
        AreEqual(12, cov.NextIndex(11, false));
    }

    [Test]
    public void PreviousTest()
    {
        var cov = new CoverageTree(15);
        cov.UpdateIndex(5, 10, 1);
        AreEqual(-1, cov.PreviousIndex(0));
        AreEqual(-1, cov.PreviousIndex(1));
        AreEqual(-1, cov.PreviousIndex(5));
        AreEqual(5, cov.PreviousIndex(6));
        AreEqual(9, cov.PreviousIndex(10));
        AreEqual(10, cov.PreviousIndex(15));
        AreEqual(-1, cov.PreviousIndex(0, false));
        AreEqual(3, cov.PreviousIndex(4, false));
        AreEqual(4, cov.PreviousIndex(5, false));
        AreEqual(4, cov.PreviousIndex(7, false));
        AreEqual(4, cov.PreviousIndex(11, false));
        AreEqual(11, cov.PreviousIndex(12, false));
    }

    [Test]
    public void StateTest()
    {
        var cov = new CoverageTree(15);
        cov.UpdateIndex(5, 10, 1);

        for (int i = 0; i < 15; i++) AreEqual(i >= 5 && i <= 10, cov[i]);
    }

    [Test]
    public void RectangleUnionTest()
    {
        var list = new List<int[]>
        {
            new[] { 0, 1, 3, 2 },
            new[] { 0, 3, 3, 4 },
            new[] { 1, 2, 2, 5 },
        };

        AreEqual(8, CoverageTree.UnionArea(list));
    }

    [Test]
    public void SizeCheckTest()
    {
        var list = new List<int>();
        for (int i = 0; i < 50; i++) {
            var tree = new CoverageTree(list.ToArray());

            for (int j = 0; j < i; j++)
                tree.UpdateIndex(j, j, i);

            list.Add(i);
        }
    }
}