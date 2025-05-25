namespace Algorithms.Graphs;

public class Graph
{
    public static Random random = new();

    public static readonly Graph K2 = Complete(2);
    readonly int m;
    readonly int n;
    public List<int>[] Nodes;

    public Graph(int n, int m)
    {
        this.n = n;
        this.m = m;
        Nodes = new List<int>[n];
        for (int i = 0; i < n; i++)
            Nodes[i] = new List<int>(m);
    }

    public List<int> this[int index] => Nodes[index];

    public static Graph CubeGraph(int m)
    {
        int n = 1 << m;
        var g = new Graph(n, m);

        for (int i = 0; i < n; i++)
        for (int j = 0; j < m; j++)
            g[i].Add(i ^ (1 << j));
        return g;
    }

    public static Graph RandomGraph2(int n, int degree)
    {
        var g = new Graph(n, degree);

        for (int i = 1; i < n; i++) {
            int j = random.Next(0, i);
            if (g.Nodes[j].Count >= degree) {
                i--;
                continue;
            }

            g.AddBidirectionalEdge(i, j);
        }

        return g;
    }

    public static Graph CrazyRandom(int n, int degree)
    {
        var list = new List<int>(n * degree * 2);

        for (int i = 0; i < n; i++)
        for (int j = 0; j < degree * 2; j++)
            list.Add(i);

        var g = new Graph(n, degree);
        int timer = list.Count * 2;
        while (list.Count > 0 && timer-- > 0) {
            int i = random.Next(0, list.Count);
            int j = (i + random.Next(0, list.Count - 1) + 1) % list.Count;

            if (list[i] == list[j])
                continue;

            g[list[i]].Add(list[j]);
            int v1 = list[list.Count - 1];
            int v2 = list[list.Count - 2];
            list[i] = v1;
            list[j] = v2;
            list.RemoveRange(list.Count - 2, 2);
        }

        if (timer < 0) return null;
        //var tarjan = new Tarjan(g.Nodes);
        //if (!tarjan.RunScc()) return null;

        return g;
    }

    public void AddUniRandomEdges()
    {
        // Add random edges 
        for (int i = 0; i < n; i++) {
            List<int> list = Nodes[i];
            while (list.Count < m) {
                // Excess edges
                if (list.Count >= n - 1) {
                    list.Add((i + random.Next(1, n)) % n);
                    continue;
                }

                int attach = random.Next(0, n - 1 - list.Count);
                int attach2 = attach;
                if (attach >= i) attach2++;
                foreach (int v in list)
                    if (attach >= v)
                        attach2++;

                if (attach2 > Nodes.Length)
                    attach2 = Nodes.Length - 1;
                list.Add(attach2);
            }
        }
    }

    public void AddBiRandomEdges()
    {
        // Add random edges 
        for (int i = 0; i + 1 < n; i++) {
            List<int> list = Nodes[i];
            int timer = 2 * m;
            while (list.Count < m && timer-- > 0) {
                int attach = random.Next(i + 1, n);
                if (Nodes[attach].Count >= m || list.Contains(attach))
                    continue;
                AddBidirectionalEdge(i, attach);
            }
        }

        AddUniRandomEdges();
    }

    public void ReduceDiameterToLogN()
    {
        int head = 0;
        int[] next = new int[n];

        next[n - 1] = -1;
        for (int i = 0; i + 1 < n; i++)
            next[i] = i + 1;

        while (head != -1) {
            int cur = head;
            int prev = -1;
            int headorig = head;
            while (cur != -1) {
                int tmp = cur;
                cur = next[cur] != -1 ? next[next[cur]] : -1;

                if (prev == -1)
                    head = next[tmp];
                else
                    next[prev] = next[tmp];

                next[tmp] = cur;
                Nodes[tmp].Add(cur != -1 ? cur : head != -1 && Nodes[tmp].Contains(headorig) ? head : headorig);
            }
        }
    }

    //http://research.nii.ac.jp/graphgolf/2015/candar15/graphgolf2015-mizuno.pdf

    public void AddBidirectionalEdge(int i, int j)
    {
        if (i == j) return;
        if (!Nodes[i].Contains(j)) Nodes[i].Add(j);
        if (!Nodes[j].Contains(i)) Nodes[j].Add(i);
    }

    public static Graph Complete(int n)
    {
        // Degree n-1
        // Diameter 1
        var g = new Graph(n, n - 1);
        for (int i = 0; i < n; i++)
        for (int j = i + 1; j < n; j++)
            g.AddBidirectionalEdge(i, j);
        return g;
    }

    public static Graph Graph33(int n)
    {
        // Degree and diameter 3
        int[] array = null;
        switch (n) {
            case 8:
                array = new[] { 6, 3, 4, 1, 2, 7, 0, 5 };
                break;
            case 10:
                array = new[] { 7, 6, 5, 9, 8, 2, 1, 0, 3, 4 };
                break;
            case 12:
                array = new[] { 10, 3, 8, 1, 6, 11, 4, 9, 2, 7, 0, 5 };
                break;
            case 14:
                array = new[]
                {
                    5, 10, 7, 12, 9, 0, 11,
                    2, 13, 4, 1, 6, 3, 8,
                };
                break;
            case 18:
                array = new[]
                {
                    14, 5, 9, 16, 12, 1,
                    11, 15, 13, 2, 17, 6,
                    4, 8, 0, 7, 3, 10,
                };
                break;
        }

        if (array == null) return null;
        Graph g = CycleGraph(n, 3, true);
        for (int i = 0; i < array.Length; i++)
            g.AddBidirectionalEdge(i, array[i]);
        return g;
    }

    public static Graph CycleGraph(int n, int m = 1, bool bidrectional = false)
    {
        var g = new Graph(n, m);
        g[n - 1].Add(0);
        for (int i = 1; i < n; i++)
            g[i - 1].Add(i);

        if (bidrectional) {
            g[0].Add(n - 1);
            for (int i = 1; i < n; i++)
                g[i].Add(i - 1);
        }

        return g;
    }

    public static Graph G8(int k)
    {
        // New order = 8 * k
        // New degree = k + 2
        // Diameter = 2

        var g8 = new Graph(8 * k, k + 2);

        for (int i = 0; i < k; i++) {
            g8.AddBidirectionalEdge(i * 8 + 0, i * 8 + 2);
            g8.AddBidirectionalEdge(i * 8 + 0, i * 8 + 3);
            g8.AddBidirectionalEdge(i * 8 + 0, i * 8 + 4);
            g8.AddBidirectionalEdge(i * 8 + 1, i * 8 + 2);
            g8.AddBidirectionalEdge(i * 8 + 1, i * 8 + 3);
            g8.AddBidirectionalEdge(i * 8 + 1, i * 8 + 4);
            g8.AddBidirectionalEdge(i * 8 + 2, i * 8 + 5);
            g8.AddBidirectionalEdge(i * 8 + 3, i * 8 + 6);
            g8.AddBidirectionalEdge(i * 8 + 4, i * 8 + 7);
            g8.AddBidirectionalEdge(i * 8 + 5, i * 8 + 6);
            g8.AddBidirectionalEdge(i * 8 + 5, i * 8 + 7);
            g8.AddBidirectionalEdge(i * 8 + 6, i * 8 + 7);
        }

        for (int i = 0; i < k; i++)
        for (int j = 0; j < k; j++) {
            if (i == j) continue;
            g8.AddBidirectionalEdge(i * 8 + 0, j * 8 + 1);
            g8.AddBidirectionalEdge(i * 8 + 1, j * 8 + 0);
            g8.AddBidirectionalEdge(i * 8 + 2, j * 8 + 6);
            g8.AddBidirectionalEdge(i * 8 + 3, j * 8 + 7);
            g8.AddBidirectionalEdge(i * 8 + 4, j * 8 + 5);
            g8.AddBidirectionalEdge(i * 8 + 5, j * 8 + 4);
            g8.AddBidirectionalEdge(i * 8 + 6, j * 8 + 2);
            g8.AddBidirectionalEdge(i * 8 + 7, j * 8 + 3);
        }

        return g8;
    }

    public static Graph Product(Graph g1, Graph g2, bool strong = false)
    {
        var g = new Graph(g1.n * g2.n, g1.m + g2.m + (strong ? g1.m * g2.m : 0));
        int f = g2.n;

        for (int i = 0; i < g1.n; i++) {
            List<int> iedges = g1[i];
            for (int j = 0; j < g2.n; j++) {
                List<int> jedges = g2[j];

                foreach (int e1 in iedges)
                    g[i * f + j].Add(e1 * f + j);

                foreach (int e2 in jedges)
                    g[i * f + j].Add(i * f + e2);

                if (strong)
                    foreach (int e1 in iedges)
                    foreach (int e2 in jedges)
                        g[i * f + j].Add(e1 * f + e2);
            }
        }

        // Preserves diameter
        // Order(g) = Order(g1) * Order(g2)

        // Weak
        // Degree(g) = Degree(g1)+Degree(g2)
        // Diameter(g) = Diameter(g1) + Diameter(g2)

        // Strong
        // if (strong) Degree(g) = Degree(g1)*Degree(g2) + Degree(g1) + Degree(g2)
        // Diameter(g) = Max(Diameter(g1), Diameter(g2))

        // G(n/2,2) * C(2) -> G(n,5)   // C(2) diameter=1
        // G(n/3,2) * C(3) -> G(n/3,5) // C(2) diameter=2 
        // Question: Is C(3) degree 1
        return g;
    }

    public static List<int>[] RandomTree(int n)
    {
        List<int>[] g = CreateEmptyGraph(n);

        var r = new Random();
        for (int i = 1; i < n; i++) {
            int u = i;
            int v = r.Next(i);
            g[u].Add(v);
            g[v].Add(u);
        }

        return g;
    }

    public static List<int>[] RandomLongTree(int n)
    {
        List<int>[] g = CreateEmptyGraph(n);

        int i = 1;
        for (; i < n >> 1; i++) {
            int u = i;
            int v = i - 1;
            g[u].Add(v);
            g[v].Add(u);
        }

        var r = new Random();
        for (; i < n; i++) {
            int u = i;
            int v = r.Next(i);
            g[u].Add(v);
            g[v].Add(u);
        }

        return g;
    }

    public static List<int>[] RandomAugmentations(List<int>[] g, int n)
    {
        List<int>[] g2 = CreateEmptyGraph(n);
        int i = 0;
        for (; i < g.Length; i++)
            g2[i].AddRange(g[i]);

        var r = new Random();
        for (; i < n; i++) {
            int u = i;
            int v = r.Next(i);
            g2[u].Add(v);
            g2[v].Add(u);
        }

        return g2;
    }

    public static List<int>[] StarTree(int n)
    {
        List<int>[] g = CreateEmptyGraph(n);

        for (int i = 1; i < n; i++) {
            int u = i;
            int v = 0;
            g[u].Add(v);
            g[v].Add(u);
        }

        return g;
    }

    public static List<int>[] BalancedTree(int n)
    {
        List<int>[] g = CreateEmptyGraph(n);

        for (int i = 1; i < n; i++) {
            int u = i;
            int v = (i - 1) >> 1;
            g[u].Add(v);
            g[v].Add(u);
        }

        return g;
    }

    public static List<int>[] CreateEmptyGraph(int n)
    {
        var g = new List<int>[n];
        for (int i = 0; i < n; i++)
            g[n] = new List<int>();
        return g;
    }
}