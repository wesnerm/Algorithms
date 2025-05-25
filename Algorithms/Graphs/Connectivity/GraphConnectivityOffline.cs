using Algorithms.Collections;

namespace Algorithms.Graphs;

// SOURCE: https://www.hackerrank.com/rest/contests/university-codesprint-3/challenges/simple-tree-counting/hackers/Gennady/download_solution
// REFERENCE: http://codeforces.com/blog/entry/22031
// ALTERNATIVE ALGO: https://ideone.com/WAe2wk doesn't use dsu
// ALTERNATIVE ALGO: https://cp-algorithms.com/data_structures/deleting_in_log_n.html
// ALTERNATIVE ALGO: https://codeforces.com/blog/entry/15296
// DYN CON CONTEST: https://codeforces.com/gym/100551

public class GraphConnectivityOffline
{
    readonly List<Query> Queries = new();
    Action<Query, UnionFindPersistent> Action;
    int[] match;

    public void ClearEdges()
    {
        Queries.Clear();
    }

    public Query Connect(int x, int y, bool connect = true)
    {
        var query = new Query(Queries.Count, x, y, connect);
        Queries.Add(query);
        return query;
    }

    public void Solve(
        UnionFindPersistent ds,
        Action<Query, UnionFindPersistent> action)
    {
        Action = action;

        var edgeMap = new Dictionary<long, int>();
        foreach (Query q in Queries) {
            long code = q.Code;
            if (edgeMap.ContainsKey(code) == false)
                edgeMap[code] = edgeMap.Count;
        }

        int[] active = new int[edgeMap.Count];
        match = new int[edgeMap.Count];

        int n = active.Length;
        for (int i = 0; i < n; i++) {
            active[i] = -1;
            match[i] = n;
        }

        int count = Queries.Count;
        for (int i = 0; i < count; i++) {
            Query q = Queries[i];
            long code = q.Code;
            int edge = edgeMap[code];
            int activeIndex = active[edge];
            if (activeIndex >= 0)
                match[activeIndex] = i;

            if (q.Insert) {
                match[i] = active.Length;
                active[edge] = i;
            } else {
                match[i] = activeIndex;
                active[edge] = -1;
            }
        }

        Solve(0, active.Length - 1, ds);
    }

    void Solve(int left, int right, UnionFindPersistent dsOriginal)
    {
        UnionFindPersistent ds = dsOriginal;

        if (left == right) {
            Query q = Queries[left];
            if (q.Insert) // alternatively, match[i] > right
                ds = ds.Union(q.X, q.Y);
            Action?.Invoke(q, ds);
            q.Action?.Invoke(q, ds);
            return;
        }

        int mid = (left + right) >> 1;

        for (int i = mid + 1; i <= right; i++) {
            Query q = Queries[i];
            if (match[i] < left)
                ds = ds.Union(q.X, q.Y);
        }

        Solve(left, mid, ds);

        ds = dsOriginal;

        for (int i = left; i <= mid; i++) {
            Query q = Queries[i];
            if (match[i] > right)
                ds = ds.Union(q.X, q.Y);
        }

        Solve(mid + 1, right, ds);
    }

    public class Query
    {
        public Action<Query, UnionFindPersistent> Action;
        public int Index, X, Y;
        public bool Insert = true;

        public Query(int i, int x, int y, bool insert)
        {
            X = x <= y ? x : y;
            Y = x <= y ? y : x;
            Insert = insert;
            Index = i;
        }

        public long Code => ((X * 1L) << 24) ^ Y;

        public override string ToString()
        {
            string insert = Insert ? "Insert" : "Delete";
            return $"#{Index} {insert} {X}-{Y}";
        }
    }
}