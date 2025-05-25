using Algorithms.Collections;
using static System.Math;

namespace Algorithms.Graphs;

public class BridgesAndCuts
{
    public BridgesAndCuts(List<int>[] graph)
    {
        _graph = graph;
        var builder = new Builder(_graph);
        CutPoints = builder.CutPoints;
        Bridges = builder.Bridges;
    }

    long Combine(int x, int y)
    {
        if (x > y) {
            int tmp = x;
            x = y;
            y = tmp;
        }

        return ((long)x << 32) + y;
    }

    public UnionFind GetComponents(bool avoidBridges = true, bool avoidCuts = true)
    {
        int n = _graph.Length;
        var ds = new UnionFind(n);
        var hs = new HashSet<long>();

        if (avoidBridges)
            foreach (Bridge bridge in Bridges)
                hs.Add(Combine(bridge.U, bridge.V));

        for (int i = 0; i < n; i++) {
            if (avoidCuts && CutPoints.Contains(i)) continue;
            foreach (int e in _graph[i]) {
                if (e < i || (avoidCuts && CutPoints.Contains(e)) || (avoidBridges && hs.Contains(Combine(i, e))))
                    continue;
                ds.Union(i, e);
            }
        }

        return ds;
    }

    public struct Builder
    {
        readonly int[] low;
        readonly int[] num;
        readonly List<int>[] _graph;
        int curnum;

        public HashSet<int> CutPoints;
        public List<Bridge> Bridges;

        public Builder(List<int>[] graph)
        {
            int n = graph.Length;
            _graph = graph;
            low = new int[n];
            num = new int[n];

            CutPoints = new HashSet<int>();
            Bridges = new List<Bridge>();

            for (int i = 0; i < n; i++)
                num[i] = -1;

            curnum = 0;
            for (int i = 0; i < n; i++)
                if (num[i] == -1)
                    Dfs(i);
        }

        void Dfs(int u, int p = -1)
        {
            low[u] = num[u] = curnum++;
            int cnt = 0;
            bool found = false;

            List<int> neighbors = _graph[u];
            for (int i = 0; i < neighbors.Count; i++) {
                int v = neighbors[i];
                if (num[v] == -1) {
                    Dfs(v, u);
                    low[u] = Min(low[u], low[v]);
                    cnt++;
                    found = found || low[v] >= num[u];
                    if (low[v] > num[u]) Bridges.Add(new Bridge(u, v));
                } else if (p != v) {
                    low[u] = Min(low[u], num[v]);
                }
            }

            if (found && (p != -1 || cnt > 1))
                CutPoints.Add(u);
        }
    }

    public class Bridge
    {
        public int U, V;

        public Bridge(int u, int v)
        {
            U = u;
            V = v;
        }

        public override string ToString() => $"{U}--{V}";
    }

    #region Using

    readonly List<int>[] _graph;
    public HashSet<int> CutPoints;
    public List<Bridge> Bridges;

    #endregion
}