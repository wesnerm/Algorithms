namespace Algorithms.Graphs;

public class StoerWagnerMinEdgeCut
{
    public long BestWeight;

    public StoerWagnerMinEdgeCut(long[,] g)
    {
        int n = g.GetLength(0);
        int[] v = new int[n];
        bool[] a = new bool[n];
        long[] w = new long[n];
        BestWeight = long.MaxValue;

        for (int i = 0; i < n; i++)
            v[i] = i;

        while (n > 1) {
            a[v[0]] = true;
            for (int i = 1; i < n; i++) {
                a[v[i]] = false;
                w[i] = g[v[0], v[i]];
            }

            for (int i = 1, prev = v[0]; i < n; i++) {
                int k = -1;
                for (int j = 1; j < n; j++) {
                    if (a[v[j]]) continue;
                    if (k < 0 || w[j] > w[k])
                        k = j;
                }

                a[v[k]] = true;

                if (i + 1 == n) {
                    BestWeight = Math.Min(w[k], BestWeight);

                    for (int j = 0; j < n; j++) {
                        g[prev, v[j]] += g[v[k], v[j]];
                        g[v[j], prev] = g[prev, v[j]];
                    }

                    n--;
                    v[k] = v[n];
                    break;
                }

                prev = v[k];

                for (int j = 1; j < n; j++) {
                    if (a[v[j]]) continue;
                    w[j] += g[v[k], v[j]];
                }
            }
        }
    }
}