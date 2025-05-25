namespace Algorithms.Graphs;

[TestFixture]
class TreeGraphTest
{
    TreeGraph Sample()
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

        return new TreeGraph(new[]
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
    public void DisjointSetUnion()
    {
        var set = new HashSet<int>();
        TreeGraph tree = Sample();

        int[][] result = new int[11][];

        tree.DisjointSetUnion(v => set.Add(v), v => set.Remove(v),
            v => {
                set.Add(v);
                result[v] = set.OrderBy(x => x).ToArray();
            });

        AreEqual(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, result[1]);
        AreEqual(new[] { 2, 3, 4, 5, 6, 8 }, result[2]);
        AreEqual(new[] { 3, 6 }, result[3]);
        AreEqual(new[] { 4, 5, 8 }, result[4]);
        AreEqual(new[] { 5 }, result[5]);
        AreEqual(new[] { 6 }, result[6]);
        AreEqual(new[] { 7 }, result[7]);
        AreEqual(new[] { 8 }, result[8]);
        AreEqual(new[] { 7, 9, 10 }, result[9]);
        AreEqual(new[] { 10 }, result[10]);
    }

    [Test]
    public void SizesTest()
    {
        TreeGraph tree = Sample();
        AreEqual(new[] { 0, 10, 6, 2, 3, 1, 1, 1, 1, 3, 1 }, tree.Sizes);
    }

    [Test]
    public void TraceTest()
    {
        TreeGraph tree = Sample();
        for (int i = 1; i < tree.TreeSize; i++)
            AreEqual(i, tree.Begin[tree.Trace[i]]);
    }

    [Test]
    public void BeginTest()
    {
        TreeGraph tree = Sample();
        for (int i = 1; i <= 10; i++)
            AreEqual(i, tree.Trace[tree.Begin[i]]);
    }

    [Test]
    public void ParentTest()
    {
        TreeGraph tree = Sample();
        AreEqual(-1, tree.Parent[1]);
        AreEqual(1, tree.Parent[2]);
        AreEqual(1, tree.Parent[9]);
        AreEqual(9, tree.Parent[7]);
        AreEqual(9, tree.Parent[10]);
        AreEqual(2, tree.Parent[4]);
        AreEqual(2, tree.Parent[3]);
        AreEqual(4, tree.Parent[8]);
        AreEqual(4, tree.Parent[5]);
        AreEqual(3, tree.Parent[6]);
    }

    [Test]
    public void HeadTest()
    {
        TreeGraph tree = Sample();
        AreEqual(1, tree.Head[1]);
        AreEqual(1, tree.Head[2]);
        AreEqual(1, tree.Head[4]);
        AreEqual(3, tree.Head[3]);
        AreEqual(3, tree.Head[6]);
        AreEqual(9, tree.Head[9]);
        IsTrue(tree.Head[7] == 7 || tree.Head[7] == 9);
        IsTrue(tree.Head[10] == 10 || tree.Head[10] == 9);
        IsTrue((tree.Head[10] == 10) ^ (tree.Head[7] == 7));
    }

    [Test]
    public void LcaTest()
    {
        TreeGraph tree = Sample();
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
        TreeGraph tree = Sample();
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
        TreeGraph tree = Sample();
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

    void IntersectCore(Func<TreeGraph, int, int, int, int> func)
    {
        TreeGraph tree = Sample();
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
            if (t.Begin[result] < t.Begin[lca2])
                result = lca2;
            if (t.Begin[result] < t.Begin[lca3])
                result = lca3;
            return result;
        });
    }

    [Test]
    public void AdvanceTest()
    {
        TreeGraph tree = Sample();

        for (int i = 0; i < tree.Trace.Length; i++) {
            int p = tree.Parent[i];
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
        TreeGraph tree = Sample();
        AreEqual(new[] { 7, 9 }, HldCore(tree, 7, 6, 1));
        AreEqual(new[] { 1, 2, 3, 6 }, HldCore(tree, 7, 6, -1));
        AreEqual(new[] { 1, 2, 3, 6 }, HldCore(tree, 6, 7, 1));
        AreEqual(new[] { 7, 9 }, HldCore(tree, 6, 7, -1));
        AreEqual(new[] { 9, 10 }, HldCore(tree, 10, 5, 1));
        AreEqual(new[] { 1, 2, 4, 5 }, HldCore(tree, 10, 5, -1));
    }

    int[] HldCore(TreeGraph g, int u, int v, int dir)
    {
        var hashSet = new HashSet<int>();
        foreach (TreeGraph.Segment seg in g.Query(u, v)) {
            if (seg.Dir != dir) continue;
            for (int i = seg.HeadIndex; i <= seg.NodeIndex; i++)
                hashSet.Add(g.Trace[i]);
        }

        return hashSet.OrderBy(x => x).ToArray();
    }

    public static int DistanceSlow(TreeGraph tree, int x, int y)
    {
        int distance = 0;
        int[] head = tree.Head;
        int[] begin = tree.Begin;
        int[] parent = tree.Parent;
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

    public static int LcaSlow(TreeGraph tree, int x, int y)
    {
        int[] head = tree.Head;
        int[] begin = tree.Begin;
        int[] parent = tree.Parent;
        int diff = begin[y] - begin[x];
        if (diff >= 0) {
            if (diff < tree.Sizes[x]) return x;
        } else {
            if (-diff < tree.Sizes[y]) return y;
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