#region Copyright
/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
//
// Copyright (C) 2005-2016, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////
#endregion

namespace Algorithms.Graphs;

using Node = LinkCutTreeLca;

[TestFixture]
public class LinkCutTreeLcaTest
{
    #region Variables
    // private LinkCutTreeLca sample;
    #endregion
    
    #region Tests
    
    /// <summary>
    /// Test for Access(Node x)
    /// </summary>
    [Test]
    [Ignore("Access is not yet implemented")]
    public void AccessTest()
    {
        // var obj = new LinkCutTreeLca();
        // var expected = obj.Access();
        // var actual = default(Node);
        // Assert.AreEqual(expected, actual, "Access");
        Assert.Fail();
    }
    
    /// <summary>
    /// Test for MakeRoot(Node x)
    /// </summary>
    [Test]
    [Ignore("MakeRoot is not yet implemented")]
    public void MakeRootTest()
    {
        // var obj = new LinkCutTreeLca();
        // var expected = obj.MakeRoot();
        // var actual = default(void);
        // Assert.AreEqual(expected, actual, "MakeRoot");
        Assert.Fail();
    }
    
    /// <summary>
    /// Test for Connected(Node x, Node y)
    /// </summary>
    [Test]
    [Ignore("Connected is not yet implemented")]
    public void ConnectedTest()
    {
        // var obj = new LinkCutTreeLca();
        // var expected = obj.Connected();
        // var actual = default(bool);
        // Assert.AreEqual(expected, actual, "Connected");
        Assert.Fail();
    }
    
    /// <summary>
    /// Test for Link(Node x, Node y)
    /// </summary>
    [Test]
    [Ignore("Link is not yet implemented")]
    public void LinkTest()
    {
        // var obj = new LinkCutTreeLca();
        // var expected = obj.Link();
        // var actual = default(void);
        // Assert.AreEqual(expected, actual, "Link");
        Assert.Fail();
    }
    
    /// <summary>
    /// Test for Cut(Node x)
    /// </summary>
    [Test]
    [Ignore("Cut is not yet implemented")]
    public void CutTest()
    {
        // var obj = new LinkCutTreeLca();
        // var expected = obj.Cut();
        // var actual = default(void);
        // Assert.AreEqual(expected, actual, "Cut");
        Assert.Fail();
    }
    
    /// <summary>
    /// Test for Lca(Node x, Node y)
    /// </summary>
    [Test]
    [Ignore("Lca is not yet implemented")]
    public void LcaTest()
    {
        // var obj = new LinkCutTreeLca();
        // var expected = obj.Lca();
        // var actual = default(Node);
        // Assert.AreEqual(expected, actual, "Lca");
        Assert.Fail();
    }
    
    /// <summary>
    /// Test for FindRoot(Node x)
    /// </summary>
    [Test]
    [Ignore("FindRoot is not yet implemented")]
    public void FindRootTest()
    {
        // var obj = new LinkCutTreeLca();
        // var expected = obj.FindRoot();
        // var actual = default(Node);
        // Assert.AreEqual(expected, actual, "FindRoot");
        Assert.Fail();
    }
    
    /// <summary>
    /// Test for IsRoot()
    /// </summary>
    [Test]
    [Ignore("IsRoot is not yet implemented")]
    public void IsRootTest()
    {
        // var obj = new LinkCutTreeLca();
        // var expected = obj.IsRoot();
        // var actual = default(bool);
        // Assert.AreEqual(expected, actual, "IsRoot");
        Assert.Fail();
    }

    #endregion

    public static bool Connected(bool[,] g, int u, int v, int p)
    {
        if (u == v)
            return true;
        for (int i = 0; i < g.Length; i++)
            if (i != p && g[u, i] && Connected(g, i, v, u))
                return true;
        return false;
    }

    [Test]
    public void RandomTest()
    {
        Random rnd = new Random(1);
        for (int step = 0; step < 1000; step++)
        {
            int n = rnd.Next(50) + 1;
            int[] p = new int[n];
            for (int i = 0; i < n; i++)
                p[i] = -1;
            Node[] nodes = new Node[n];
            for (int i = 0; i < n; i++)
                nodes[i] = new Node();
            for (int query = 0; query < 10000; query++)
            {
                int cmd = rnd.Next(10);
                int u = rnd.Next(n);
                Node x = nodes[u];
                if (cmd == 0)
                {
                    x.Access();
                    if ((x.Right != null) != (p[u] != -1))
                        throw new Exception();
                    if (x.Right != null)
                    {
                        x.Cut();
                        p[u] = -1;
                    }
                }
                else if (cmd == 1)
                {
                    int v = rnd.Next(n);
                    Node y = nodes[v];
                    if ((x.FindRoot() == y.FindRoot()) != (Root(p, u) == Root(p, v)))
                        throw new Exception();
                    if (x.FindRoot() == y.FindRoot())
                    {
                        Node lca = x.Lca(y);
                        int cur = u;
                        HashSet<int> path = new HashSet<int>();
                        for (; cur != -1; cur = p[cur])
                            path.Add(cur);
                        cur = v;
                        for (; cur != -1 && !path.Contains(cur); cur = p[cur])
                            ;
                        if (lca != nodes[cur])
                            throw new Exception();
                    }
                }
                else
                {
                    x.Access();
                    if ((x.Right == null) != (p[u] == -1))
                        throw new Exception();
                    if (x.Right == null)
                    {
                        int v = rnd.Next(n);
                        Node y = nodes[v];
                        if ((x.FindRoot() != y.FindRoot()) != (Root(p, u) != Root(p, v)))
                            throw new Exception();
                        if (x.FindRoot() != y.FindRoot())
                        {
                            x.Link(y);
                            p[u] = v;
                        }
                    }
                }
            }
        }
    }

    static int Root(int[] p, int u)
    {
        int root = u;
        while (p[root] != -1)
            root = p[root];
        return root;
    }
}