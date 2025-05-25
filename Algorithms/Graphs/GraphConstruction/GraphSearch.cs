namespace Algorithms.Graphs;

public class GraphSearch
{
    public static int[] TopologicalSort(int[,] g)
    {
        int n = g.GetLength(0);
        int[] indegree = new int[n];
        for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            if (g[i, j] > 0)
                indegree[j]++;

        int[] ret = new int[n];
        int p = 0;
        int q = 0;

        for (int i = 0; i < n; i++) {
            bool good = true;
            for (int j = 0; j < n; j++)
                if (g[j, i] > 0) {
                    good = false;
                    break;
                }

            if (good) ret[q++] = i;
        }

        for (; p < q; p++) {
            int cur = ret[p];
            for (int i = 0; i < n; i++)
                if (g[cur, i] > 0) {
                    indegree[i]--;
                    if (indegree[i] == 0) ret[q++] = i;
                }
        }

        for (int i = 0; i < n; i++)
            if (indegree[i] > 0)
                return null;
        return ret;
    }

    public static List<int> LexicographicTopologicalSort(List<int>[] g)
    {
        int n = g.Length;
        int[] counts = new int[n];
        var queue = new SortedSet<int>();
        for (int i = 0; i < n; i++)
            foreach (int v in g[i])
                counts[v]++;

        for (int i = 0; i < n; i++) {
            if (counts[i] != 0) continue;
            // Isolated vertex
            if (g[i].Count == 0) continue;
            queue.Add(i);
        }

        var result = new List<int>(n);

        while (queue.Count > 0) {
            int min = queue.Min();
            queue.Remove(min);

            foreach (int v in g[min])
                if (--counts[v] == 0)
                    queue.Add(v);

            result.Add(min);
        }

        for (int i = 0; i < n; i++)
            if (counts[i] > 0)
                return null;

        return result;
    }

    public static List<int> TopologicalSort(List<int>[] g)
    {
        int n = g.Length;
        int[] counts = new int[n];
        var queue = new Queue<int>();
        for (int i = 0; i < n; i++)
            foreach (int v in g[i])
                counts[v]++;

        for (int i = 0; i < n; i++) {
            if (counts[i] != 0) continue;
            // Isolated vertex
            if (g[i].Count == 0) continue;
            queue.Enqueue(i);
        }

        var result = new List<int>(n);

        while (queue.Count > 0) {
            int min = queue.Dequeue();

            foreach (int v in g[min])
                if (--counts[v] == 0)
                    queue.Enqueue(v);

            result.Add(min);
        }

        for (int i = 0; i < n; i++)
            if (counts[i] > 0)
                return null;

        return result;
    }

    public static int[] ComputeDistance(IList<int>[] g, int start)
    {
        int n = g.Length;
        int[] d = new int[n];
        for (int i = 0; i < n; i++)
            d[i] = n + 3;

        int[] q = new int[n];
        int p = 0;
        q[p++] = start;
        d[start] = 0;
        for (int r = 0; r < p; r++) {
            int cur = q[r];
            foreach (int e in g[cur])
                if (d[e] > d[cur] + 1) {
                    d[e] = d[cur] + 1;
                    q[p++] = e;
                }
        }

        return d;
    }

    public class DepthFirstSearch
    {
        readonly List<int>[] g;
        readonly int[] indices;
        readonly int[] stack;
        readonly int[] timestamp;
        int time;

        public DepthFirstSearch(List<int>[] graph)
        {
            g = new List<int>[graph.Length];
            timestamp = new int[graph.Length];
            stack = new int[graph.Length];
            indices = new int[graph.Length];
        }

        /// <summary>
        ///     Performs Dfs but non-strictly. Uses very little memory.
        /// </summary>
        public void DfsIterativeFastNonStrict(int u, int p, Action<int> action)
        {
            int stackSize = 0;
            time++;

            stack[stackSize++] = u;
            timestamp[u] = time;
            if (p >= 0) timestamp[p] = time;

            while (stackSize > 0) {
                int pop = stack[--stackSize];
                action(pop);
                foreach (int v in g[pop]) {
                    if (timestamp[v] >= time) continue;
                    timestamp[v] = time;
                    stack[stackSize++] = v;
                }
            }
        }

        /// <summary>
        ///     Dfs -- Strict and elegant preorder traversal
        /// </summary>
        public void DfsIterative(int u, Func<int, int, bool> visit)
        {
            int stackSize = 0;
            time++;
            timestamp[u] = time;
            if (visit(u, 0)) {
                indices[stackSize] = -1;
                stack[stackSize++] = u;
            }

            while (stackSize > 0) {
                int pop = stack[--stackSize];
                int depth = stackSize;
                List<int> children = g[pop];
                for (int i = indices[pop] + 1; i < children.Count; i++) {
                    int child = children[i];
                    if (timestamp[child] < time) {
                        timestamp[child] = time;
                        if (visit(child, depth + 1)) {
                            indices[pop] = i;
                            stack[stackSize++] = pop;
                            indices[child] = -1;
                            stack[stackSize++] = child;
                            break;
                        }
                    }
                }
            }
        }

        public void DfsIterativeInorder(int u, Func<int, int, bool> visit, Action<int, int, int, int, bool> inorder)
        {
            int stackSize = 0;
            time++;
            timestamp[u] = time;
            if (visit(u, 0)) {
                indices[stackSize] = -1;
                stack[stackSize++] = u;
            }

            while (stackSize > 0) {
                int pop = stack[--stackSize];
                List<int> children = g[pop];
                int i = indices[pop] + 1;
                int depth = stackSize;
                bool first = i == 0;

                for (; i < children.Count; i++) {
                    int child = children[i];
                    if (timestamp[child] < time) {
                        timestamp[child] = time;
                        if (visit(child, depth + 1)) {
                            indices[pop] = i;
                            stack[stackSize++] = pop;
                            indices[child] = -1;
                            stack[stackSize++] = child;
                            break;
                        }
                    }
                }

                // for preorder, first == true
                // for postorder, child has value -1
                // if no children, one inorder call combines preorder and postorder
                // for inorder, all other calls to inorder
                // for trees, produces a euler tour with one node per leaf -- and outer and inner nodes for parents

                inorder(pop, i, i < children.Count ? stack[stackSize - 1] : -1, depth, first);
            }
        }
    }

    public struct FindPath
    {
        public List<int> Path;
        public IList<int>[] Graph;

        public FindPath(IList<int>[] g, int v, int to, bool bfs = true)
        {
            Path = new List<int>();
            Graph = g;
            if (bfs)
                Bfs(v, to);
            else
                Dfs(v, to, -1);
        }

        bool Dfs(int v, int to, int p)
        {
            if (v == to) {
                Path.Add(to);
                return true;
            }

            Path.Add(v);
            foreach (int u in Graph[v])
                if (p != u && Dfs(u, to, v))
                    return true;
            Path.RemoveAt(Path.Count - 1);
            return false;
        }

        bool Bfs(int v, int to)
        {
            var queue = new Queue<int>();
            int[] parent = new int[Graph.Length];
            for (int i = 0; i < parent.Length; i++)
                parent[i] = -1;

            queue.Enqueue(v);

            while (queue.Count > 0) {
                int pop = queue.Dequeue();
                if (pop == to) {
                    for (int p = to; p >= 0; p = parent[p])
                        Path.Add(p);
                    Path.Reverse();
                    return true;
                }

                foreach (int c in Graph[pop]) {
                    if (parent[c] >= 0) continue;
                    parent[c] = pop;
                    queue.Enqueue(c);
                }
            }

            return false;
        }
    }
}