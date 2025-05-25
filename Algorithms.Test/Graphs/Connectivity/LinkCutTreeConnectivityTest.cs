#region Copyright

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
//
// Copyright (C) 2005-2016, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

using Node = Algorithms.Graphs.LinkCutTreeConnectivity;

namespace Algorithms.Graphs;

[TestFixture]
public class LinkCutTreeConnectivityTest
{
    /// <summary>
    ///     Test for Access(Node x)
    /// </summary>
    [Test]
    [Ignore("Access is not yet implemented")]
    public void AccessTest()
    {
        // var obj = new LinkCutTreeConnectivity();
        // var expected = obj.Access();
        // var actual = default(Node);
        // Assert.AreEqual(expected, actual, "Access");
        Fail();
    }

    /// <summary>
    ///     Test for MakeRoot(Node x)
    /// </summary>
    [Test]
    [Ignore("MakeRoot is not yet implemented")]
    public void MakeRootTest()
    {
        // var obj = new LinkCutTreeConnectivity();
        // var expected = obj.MakeRoot();
        // var actual = default(void);
        // Assert.AreEqual(expected, actual, "MakeRoot");
        Fail();
    }

    /// <summary>
    ///     Test for Connected(Node x, Node y)
    /// </summary>
    [Test]
    [Ignore("Connected is not yet implemented")]
    public void ConnectedTest()
    {
        // var obj = new LinkCutTreeConnectivity();
        // var expected = obj.Connected();
        // var actual = default(bool);
        // Assert.AreEqual(expected, actual, "Connected");
        Fail();
    }

    /// <summary>
    ///     Test for Link(Node x, Node y)
    /// </summary>
    [Test]
    [Ignore("Link is not yet implemented")]
    public void LinkTest()
    {
        // var obj = new LinkCutTreeConnectivity();
        // var expected = obj.Link();
        // var actual = default(void);
        // Assert.AreEqual(expected, actual, "Link");
        Fail();
    }

    /// <summary>
    ///     Test for Cut(Node x, Node y)
    /// </summary>
    [Test]
    [Ignore("Cut is not yet implemented")]
    public void CutTest()
    {
        // var obj = new LinkCutTreeConnectivity();
        // var expected = obj.Cut();
        // var actual = default(void);
        // Assert.AreEqual(expected, actual, "Cut");
        Fail();
    }

    /// <summary>
    ///     Test for IsRoot()
    /// </summary>
    [Test]
    [Ignore("IsRoot is not yet implemented")]
    public void IsRootTest()
    {
        // var obj = new LinkCutTreeConnectivity();
        // var expected = obj.IsRoot();
        // var actual = default(bool);
        // Assert.AreEqual(expected, actual, "IsRoot");
        Fail();
    }

    /// <summary>
    ///     Test for Push()
    /// </summary>
    [Test]
    [Ignore("Push is not yet implemented")]
    public void PushTest()
    {
        // var obj = new LinkCutTreeConnectivity();
        // var expected = obj.Push();
        // var actual = default(void);
        // Assert.AreEqual(expected, actual, "Push");
        Fail();
    }

    public static bool Connected(bool[,] g, int u, int v, int p)
    {
        if (u == v)
            return true;
        for (int i = 0; i < g.Length; i++)
            if (i != p && g[u, i] && Connected(g, i, v, u))
                return true;
        return false;
    }

    // random test
    void LinkCutConnectivityTest(string[] args)
    {
        var rnd = new Random(1);
        for (int step = 0; step < 1000; step++) {
            int n = rnd.Next(50) + 1;
            bool[,] g = new bool[n, n];
            var nodes = new Node[n];
            for (int i = 0; i < n; i++)
                nodes[i] = new Node();
            for (int query = 0; query < 2000; query++) {
                int cmd = rnd.Next(10);
                int u = rnd.Next(n);
                int v = rnd.Next(n);
                Node x = nodes[u];
                Node y = nodes[v];
                if (cmd == 0) {
                    x.PrivateMakeRoot();
                    y.Access();
                    if ((y.Right == x && x.Left == null && x.Right == null) != g[u, v])
                        throw new Exception();
                    if (y.Right == x && x.Left == null && x.Right == null) {
                        x.Cut(y);
                        g[u, v] = g[v, u] = false;
                    }
                } else if (cmd == 1) {
                    if (x.Connected(y) != Connected(g, u, v, -1))
                        throw new Exception();
                } else {
                    x.Access();
                    if (x.Connected(y) != Connected(g, u, v, -1))
                        throw new Exception();
                    if (!x.Connected(y)) {
                        x.Link(y);
                        g[u, v] = g[v, u] = true;
                    }
                }
            }
        }
    }
    // private LinkCutTreeConnectivity sample;

    /*
    void LinkCutPathQueriesTest(string[] args)
    {
        Random rnd = new Random(1);
        for (int step = 0; step < 1000; step++)
        {
            int n = rnd.Next(50) + 1;
            bool[,] g = new bool[n, n];
            int[] val = new int[n];
            LinkCutPathQueries.Node[] nodes = new LinkCutPathQueries.Node[n];
            for (int i = 0; i < n; i++)
                nodes[i] = new LinkCutPathQueries.Node(0);
            for (int query = 0; query < 2_000; query++)
            {
                int cmd = rnd.Next(10);
                int u = rnd.Next(n);
                int v = rnd.Next(n);
                LinkCutPathQueries.Node x = nodes[u];
                LinkCutPathQueries.Node y = nodes[v];
                if (cmd == 0)
                {
                    MakeRoot(x);
                    Expose(y);
                    if (y.Right == x && x.Left == null && x.Right == null)
                    {
                        Cut(x, y);
                        g[u, v] = g[v, u] = false;
                    }
                }
                else if (cmd == 1)
                {
                    if (Connected(x, y))
                    {
                        List<int> path = new List<int>();
                        GetPathFromAtoB(g, u, v, -1, path);
                        int res = GetNeutralValue();
                        foreach (int i in path)
                            res = QueryOperation(res, val[i]);
                        if (Query(x, y) != res)
                            throw new Exception();
                    }
                }
                else if (cmd == 2)
                {
                    if (Connected(x, y))
                    {
                        List<int> path = new List<int>();
                        GetPathFromAtoB(g, u, v, -1, path);
                        int delta = rnd.Next(100) + 1;
                        foreach (int i in path)
                            val[i] = JoinValueWithDelta(val[i], delta);
                        Modify(x, y, delta);
                    }
                }
                else
                {
                    if (!Connected(x, y))
                    {
                        Link(x, y);
                        g[u, v] = g[v, u] = true;
                    }
                }
            }
        }
        Console.WriteLine("Test passed");
    }

    internal static bool GetPathFromAtoB(bool[,] tree, int u, int v, int p, List<int> path)
    {
        path.Add(u);
        if (u == v)
            return true;
        for (int i = 0; i < tree.Length; i++)
            if (i != p && tree[u, i] && GetPathFromAtoB(tree, i, v, u, path))
                return true;
        path.Remove(path.Count - 1);
        return false;
    }
    */
}