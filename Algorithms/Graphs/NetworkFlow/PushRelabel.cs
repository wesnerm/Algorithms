namespace Algorithms.Graphs;

// Adjacency list implementation of FIFO push relabel maximum flow
// with the gap relabeling heuristic.  This implementation is
// significantly faster than straight Ford-Fulkerson.  It solves
// random problems with 10000 vertices and 1000000 edges in a few
// seconds, though it is possible to construct test cases that
// achieve the worst-case.
//
// Running time:
//     O(|V|^3)
//
// INPUT: 
//     - graph, constructed using AddEdge()
//     - source
//     - sink
//
// OUTPUT:
//     - maximum flow value
//     - To obtain the actual flow values, look at all edges with
//       capacity > 0 (zero capacity edges are residual edges).

// This code performs maximum bipartite matching.
//
// Running time: O(|E| |V|) -- often much faster in practice
//
//   INPUT: w[i][j] = edge between row node i and column node j
//   OUTPUT: mr[i] = assignment for row node i, -1 if unassigned
//           mc[j] = assignment for column node j, -1 if unassigned
//           function returns number of matches made

public class PushRelabel
{
    readonly bool[] _active;
    readonly int[] _count;
    readonly int[] _dist;
    readonly long[] _excess;
    readonly List<Edge>[] _g;

    readonly int _n;
    readonly Queue<int> _q = new();

    public PushRelabel(int n)
    {
        _n = n;
        _g = new List<Edge>[n];
        _excess = new long[n];
        _dist = new int[n];
        _active = new bool[n];
        _count = new int[2 * n];

        for (int i = 0; i < n; i++)
            _g[i] = new List<Edge>();
    }

    public void AddEdge(int from, int to, int cap)
    {
        _g[from].Add(new Edge(from, to, cap, 0, _g[to].Count));
        if (from == to) _g[from][_g[from].Count - 1].Index++;
        _g[to].Add(new Edge(to, from, 0, 0, _g[from].Count - 1));
    }

    void Enqueue(int v)
    {
        if (!_active[v] && _excess[v] > 0) {
            _active[v] = true;
            _q.Enqueue(v);
        }
    }

    void Push(Edge e)
    {
        int amt = (int)Math.Min(_excess[e.From], e.Cap - e.Flow);
        if (_dist[e.From] <= _dist[e.To] || amt == 0) return;
        e.Flow += amt;
        _g[e.To][e.Index].Flow -= amt;
        _excess[e.To] += amt;
        _excess[e.From] -= amt;
        Enqueue(e.To);
    }

    void Gap(int k)
    {
        for (int v = 0; v < _n; v++) {
            if (_dist[v] < k) continue;
            _count[_dist[v]]--;
            _dist[v] = Math.Max(_dist[v], _n + 1);
            _count[_dist[v]]++;
            Enqueue(v);
        }
    }

    void Relabel(int v)
    {
        _count[_dist[v]]--;
        _dist[v] = 2 * _n;
        for (int i = 0; i < _g[v].Count; i++)
            if (_g[v][i].Cap - _g[v][i].Flow > 0)
                _dist[v] = Math.Min(_dist[v], _dist[_g[v][i].To] + 1);
        _count[_dist[v]]++;
        Enqueue(v);
    }

    void Discharge(int v)
    {
        for (int i = 0; _excess[v] > 0 && i < _g[v].Count; i++) Push(_g[v][i]);
        if (_excess[v] > 0) {
            if (_count[_dist[v]] == 1)
                Gap(_dist[v]);
            else
                Relabel(v);
        }
    }

    public long GetMaxFlow(int s, int t)
    {
        _count[0] = _n - 1;
        _count[_n] = 1;
        _dist[s] = _n;
        _active[s] = _active[t] = true;
        for (int i = 0; i < _g[s].Count; i++) {
            _excess[s] += _g[s][i].Cap;
            Push(_g[s][i]);
        }

        while (_q.Count > 0) {
            int v = _q.Dequeue();
            _active[v] = false;
            Discharge(v);
        }

        long totflow = 0;
        for (int i = 0; i < _g[s].Count; i++) totflow += _g[s][i].Flow;
        return totflow;
    }

    public class Edge
    {
        public readonly int Cap;
        public readonly int From;
        public readonly int To;
        public int Flow, Index;

        public Edge(int from, int to, int cap, int flow, int index)
        {
            From = from;
            To = to;
            Cap = cap;
            Flow = flow;
            Index = index;
        }
    }
}