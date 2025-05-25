namespace Algorithms.Graphs;

// SOURCE: https://www.hackerrank.com/rest/contests/w27/challenges/coprime-paths/hackers/uwi/download_solution
// SOURCE: http://codeforces.com/blog/entry/18369
// Euler trees are good for path operations, subtree sums, and lca operations

public class EulerTour
{
    public readonly int[] Begin;
    public readonly int[] Depth;
    public readonly int[] End;
    public readonly int[] Parent;
    public readonly int[] Trace;

    public EulerTour(List<int>[] g, int root, bool middle = false)
    {
        int n = g.Length;
        Trace = new int[2 * n - (middle ? 1 : 0)];
        Begin = new int[n];
        End = new int[n];
        Depth = new int[n];
        Parent = new int[n];
        int t = -1;

        for (int i = 0; i < n; i++)
            Begin[i] = -1;

        int[] stack = new int[n];
        int[] indices = new int[n];
        int sp = 0;
        stack[sp++] = root;
        Parent[root] = -1;

        while (sp > 0) {
        outer:
            int current = stack[sp - 1], index = indices[sp - 1];
            if (index == 0) {
                ++t;
                Trace[t] = current;
                Begin[current] = t;
                Depth[current] = sp - 1;
            }

            List<int> children = g[current];
            while (index < children.Count) {
                int child = children[index++];
                if (Begin[child] == -1) {
                    if (middle && Trace[t] != current) Trace[++t] = current;
                    indices[sp - 1] = index;
                    stack[sp] = child;
                    indices[sp] = 0;
                    Parent[child] = current;
                    sp++;
                    goto outer;
                }
            }

            indices[sp - 1] = index;
            if (index == children.Count) {
                sp--;
                if (!middle || Trace[t] != current) Trace[++t] = current;
                End[current] = t;
            }
        }
    }

    public int this[int index] => Trace[index];

    public bool IsBegin(int trace) => Begin[Trace[trace]] == trace;

    public bool IsEnd(int trace) => End[Trace[trace]] == trace;
}