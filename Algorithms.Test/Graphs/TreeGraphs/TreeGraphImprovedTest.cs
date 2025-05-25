namespace Algorithms.Graphs;

[TestFixture]
class TreeGraphTmprovedTest
{
    TreeGraphImproved Sample()
    {
        // graph G {
        //    1--9
        //    4--8
        //    9--7
        //    2--4
        //    3--6
        //    1--2
        //    9--10
        //    2--3
        //    4--5
        // }

        return new TreeGraphImproved(new[]
        {
            new List<int>(),
            new List<int> { 2, 9 },
            new List<int> { 1, 3, 4 },
            new List<int> { 2, 6 },
            new List<int> { 2, 5, 8 },
            new List<int> { 4 },
            new List<int> { 3 },
            new List<int> { 9 },
            new List<int> { 4 },
            new List<int> { 1, 7, 10 },
            new List<int> { 9 },
        }, 1);
    }

    [Test]
    public void SizesTest()
    {
        TreeGraphImproved tree = Sample();
        int[] sizes = tree.Trace.Select((_, i) => tree.Size(i)).ToArray();
        AreEqual(new[] { 0, 10, 6, 2, 3, 1, 1, 1, 1, 3, 1 }, sizes);
    }

    [Test]
    public void TraceTest()
    {
        TreeGraphImproved tree = Sample();
        for (int i = 1; i < tree.TreeSize; i++)
            AreEqual(i, tree.Index(tree.Trace[i]));
    }

    [Test]
    public void BeginTest()
    {
        TreeGraphImproved tree = Sample();
        for (int i = 1; i <= 10; i++)
            AreEqual(i, tree.Trace[tree.Index(i)]);
    }

    [Test]
    public void ParentTest()
    {
        TreeGraphImproved tree = Sample();
        AreEqual(-1, tree.Parent(1));
        AreEqual(1, tree.Parent(2));
        AreEqual(1, tree.Parent(9));
        AreEqual(9, tree.Parent(7));
        AreEqual(9, tree.Parent(10));
        AreEqual(2, tree.Parent(4));
        AreEqual(2, tree.Parent(3));
        AreEqual(4, tree.Parent(8));
        AreEqual(4, tree.Parent(5));
        AreEqual(3, tree.Parent(6));
    }

    [Test]
    public void HeadTest()
    {
        TreeGraphImproved tree = Sample();
        AreEqual(1, tree.Head(1));
        AreEqual(1, tree.Head(2));
        AreEqual(1, tree.Head(4));
        AreEqual(3, tree.Head(3));
        AreEqual(3, tree.Head(6));
        AreEqual(9, tree.Head(9));
        IsTrue(tree.Head(7) == 7 || tree.Head(7) == 9);
        IsTrue(tree.Head(10) == 10 || tree.Head(10) == 9);
        IsTrue((tree.Head(10) == 10) ^ (tree.Head(7) == 7));
    }

    [Test]
    public void LcaTest()
    {
        TreeGraphImproved tree = Sample();
        AreEqual(9, tree.Lca(7, 10));
        AreEqual(1, tree.Lca(6, 7));
        AreEqual(2, tree.Lca(2, 6));
        AreEqual(2, tree.Lca(6, 2));
        AreEqual(2, tree.Lca(8, 3));
        AreEqual(4, tree.Lca(5, 8));
        AreEqual(9, tree.Lca(9, 9));
        AreEqual(1, tree.Lca(10, 4));
        AreEqual(2, tree.Lca(2, 5));
    }

    [Test]
    public void DistanceTest()
    {
        TreeGraphImproved tree = Sample();
        AreEqual(2, tree.Distance(7, 10));
        AreEqual(5, tree.Distance(6, 7));
        AreEqual(2, tree.Distance(2, 6));
        AreEqual(2, tree.Distance(6, 2));
        AreEqual(3, tree.Distance(8, 3));
        AreEqual(2, tree.Distance(5, 8));
        AreEqual(0, tree.Distance(9, 9));
        AreEqual(4, tree.Distance(10, 4));
        AreEqual(2, tree.Distance(2, 5));
    }

    [Test]
    public void AncestorTest()
    {
        TreeGraphImproved tree = Sample();
        AreEqual(1, tree.Ancestor(1, 0));
        AreEqual(-1, tree.Ancestor(1, 1));
        AreEqual(-1, tree.Ancestor(1, 2));
        AreEqual(6, tree.Ancestor(6, 0));
        AreEqual(3, tree.Ancestor(6, 1));
        AreEqual(2, tree.Ancestor(6, 2));
        AreEqual(1, tree.Ancestor(6, 3));
        AreEqual(-1, tree.Ancestor(6, 4));
        AreEqual(1, tree.Ancestor(10, 2));
        AreEqual(9, tree.Ancestor(10, 1));
    }

    void IntersectCore(Func<TreeGraphImproved, int, int, int, int> func)
    {
        TreeGraphImproved tree = Sample();
        AreEqual(9, func(tree, 1, 7, 10));
        AreEqual(9, func(tree, 9, 7, 10));
        AreEqual(9, func(tree, 9, 9, 9));
        AreEqual(9, func(tree, 9, 9, 10));
        AreEqual(2, func(tree, 9, 4, 3));
        AreEqual(2, func(tree, 4, 9, 3));
        AreEqual(2, func(tree, 4, 3, 9));
        AreEqual(2, func(tree, 1, 3, 8));
        AreEqual(2, func(tree, 1, 6, 5));
        AreEqual(4, func(tree, 2, 5, 8));
        AreEqual(2, func(tree, 10, 4, 6));
    }

    [Test]
    public void IntersectTest()
    {
        IntersectCore((t, p1, p2, p3) => t.Intersect(p1, p2, p3));
    }

    [Test]
    public void Intersect2Test()
    {
        IntersectCore((t, p1, p2, p3) => {
            int lca1 = t.Lca(p1, p2);
            int lca2 = t.Lca(p2, p3);
            int lca3 = t.Lca(p1, p3);
            int result = lca1;
            if (t.Index(result) < t.Index(lca2))
                result = lca2;
            if (t.Index(result) < t.Index(lca3))
                result = lca3;
            return result;
        });
    }

    [Test]
    public void AdvanceTest()
    {
        TreeGraphImproved tree = Sample();

        for (int i = 0; i < tree.Trace.Length; i++) {
            int p = tree.Parent(i);
            AreEqual(p, tree.Advance(i, i));

            if (p < 0) continue;
            AreEqual(i, tree.Advance(p, i));
            AreEqual(p, tree.Advance(i, p));
        }

        AreEqual(9, tree.Advance(7, 3));
        AreEqual(2, tree.Advance(3, 7));
        AreEqual(3, tree.Advance(6, 1));
        AreEqual(2, tree.Advance(1, 6));
        AreEqual(9, tree.Advance(1, 10));
        AreEqual(4, tree.Advance(2, 8));
    }

    //[Test]
    //public void IntersectFastTest()
    //{
    //    IntersectCore((t, p1, p2, p3) => t.IntersectFast(p1, p2, p3));
    //}

    [Test]
    public void HldTest()
    {
        TreeGraphImproved tree = Sample();
        AreEqual(new[] { 7, 9 }, HldCore(tree, 7, 6, 1));
        AreEqual(new[] { 1, 2, 3, 6 }, HldCore(tree, 7, 6, -1));
        AreEqual(new[] { 1, 2, 3, 6 }, HldCore(tree, 6, 7, 1));
        AreEqual(new[] { 7, 9 }, HldCore(tree, 6, 7, -1));
        AreEqual(new[] { 9, 10 }, HldCore(tree, 10, 5, 1));
        AreEqual(new[] { 1, 2, 4, 5 }, HldCore(tree, 10, 5, -1));
    }

    int[] HldCore(TreeGraphImproved g, int u, int v, int dir)
    {
        var hashSet = new HashSet<int>();
        foreach (TreeGraphImproved.Segment seg in g.Query(u, v)) {
            if (seg.Dir != dir) continue;
            for (int i = seg.HeadIndex; i <= seg.NodeIndex; i++)
                hashSet.Add(g.Trace[i]);
        }

        return hashSet.OrderBy(x => x).ToArray();
    }

    public static int DistanceSlow(TreeGraphImproved tree, int x, int y)
    {
        int distance = 0;
        int[] head = tree.Trace.Select((_, i) => tree.Head(i)).ToArray();
        int[] begin = tree.Trace.Select((_, i) => tree.Index(i)).ToArray();
        int[] parent = tree.Trace.Select((_, i) => tree.Parent(i)).ToArray();
        for (int rx = head[x], ry = head[y]; rx != ry;)
            if (begin[rx] > begin[ry]) {
                distance += 1 + begin[x] - begin[rx];
                x = parent[rx];
                rx = head[x];
            } else {
                distance += 1 + begin[y] - begin[ry];
                y = parent[ry];
                ry = head[y];
            }

        return distance + Math.Abs(begin[x] - begin[y]);
    }

    public static int LcaSlow(TreeGraphImproved tree, int x, int y)
    {
        int[] head = tree.Trace.Select((_, i) => tree.Head(i)).ToArray();
        int[] begin = tree.Trace.Select((_, i) => tree.Index(i)).ToArray();
        int[] parent = tree.Trace.Select((_, i) => tree.Parent(i)).ToArray();
        int[] sizes = tree.Trace.Select((_, i) => tree.Size(i)).ToArray();
        int diff = begin[y] - begin[x];
        if (diff >= 0) {
            if (diff < sizes[x]) return x;
        } else {
            if (-diff < sizes[y]) return y;
        }

        for (int rx = head[x], ry = head[y]; rx != ry;)
            if (begin[rx] > begin[ry]) {
                x = parent[rx];
                rx = head[x];
            } else {
                y = parent[ry];
                ry = head[y];
            }

        return begin[x] > begin[y] ? y : x;
    }
}