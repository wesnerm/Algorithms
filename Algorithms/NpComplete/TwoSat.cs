namespace Algorithms.NpComplete;

// REVIEW: https://github.com/kth-competitive-programming/kactl/blob/master/content/graph/2sat.h

public class TwoSat
{
    List<int>[] g;
    int n;
    int time;
    int[] val, comp;
    int[] values; // 0 = false, 1 = true
    List<int> z;

    public TwoSat(int n = 0)
    {
        this.n = n;
        g = new List<int>[2 * n];
    }

    public int AddVar()
    {
        int var = n++;
        Ensure(ref g, 2 * n);
        g[var] = new List<int>();
        return var;
    }

    static void Ensure<T>(ref T[] array, int n)
    {
        if (n >= array.Length)
            Array.Resize(ref array, Math.Max(n + 1, array.Length * 2));
    }


    public void Either(int f, int j)
    {
        f *= 2;
        j *= 2;
        f = Math.Max(f, ~f);
        j = Math.Max(j, ~j);
        g[f].Add(j ^ 1);
        g[j].Add(f ^ 1);
    }

    public void SetValue(int x)
    {
        Either(x, x);
    }

    public void AtMostOne(params int[] vars)
    {
        if (vars.Length <= 1) return;
        int cur = ~vars[0];
        for (int i = 2; i < vars.Length; i++) {
            int next = AddVar();
            Either(cur, ~vars[i]);
            Either(next, ~vars[i]);
            Either(cur, next);
            cur = ~next;
        }

        Either(cur, ~vars[1]);
    }

    int Dfs(int i)
    {
        int low = val[i] = ++time;
        z.Add(i);

        foreach (int e in g[i])
            if (comp[e] == 0) {
                int tmp = val[e];
                if (tmp == 0) tmp = Dfs(e);
                low = Math.Min(low, tmp);
            }

        if (low == val[i]) {
            int x;
            do {
                x = z[z.Count - 1];
                z.RemoveAt(z.Count - 1);
                comp[x] = low;
                if (values[x >> 1] == -1)
                    values[x >> 1] = x & 1;
            } while (x != i);
        }

        return val[i] = low;
    }

    public int[] Solve()
    {
        val = new int[2 * n];
        comp = new int[2 * n];
        values = new int[n];
        for (int i = 0; i < n; i++)
            values[i] = -1;

        for (int i = 0; i < 2 * n; i++)
            if (comp[i] == 0)
                Dfs(i);

        for (int i = 0; i < n; i++)
            if (comp[2 * i] == comp[2 * i + 1])
                return null;

        return values;
    }
}