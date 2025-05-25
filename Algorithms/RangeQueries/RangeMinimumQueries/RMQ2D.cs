namespace Algorithms.RangeQueries.RangeMinimumQueries;

public class RMQ2D
{
    readonly int[,,,] _rmq;
    int[][] A;

    public RMQ2D(int[][] A)
    {
        this.A = A;
        int n = A.Length;
        int m = A[0].Length;

        int[,,] rmq0 = new int[n + 1, Log2(m) + 1, m];
        for (int k = 1; k <= n; k++) {
            for (int i = 1; i <= m; i++)
                rmq0[k, i, 0] = A[k][i];
            for (int j = 1; 1 << j <= m; j++)
            for (int i = 0; i + (1 << j) - 1 <= m; i++)
                rmq0[k, i, j] = Math.Max(rmq0[k, i, j - 1], rmq0[k, i + (1 << (j - 1)), j - 1]);
        }

        _rmq = new int[m + 1, Log2(m) + 1, n + 1, Log2(n) + 1];
        for (int k = 1; k <= m; k++)
        for (int l = 0; k + (1 << l) - 1 <= m; l++) {
            for (int i = 1; i <= n; i++)
                _rmq[k, l, i, 0] = rmq0[i, k, l];
            for (int j = 1; 1 << j <= n; j++)
            for (int i = 0; i + (1 << j) - 1 <= n; i++)
                _rmq[k, l, i, j] = Math.Max(_rmq[k, l, i, j - 1], _rmq[k, l, i + (1 << (j - 1)), j - 1]);
        }
    }

    public int Query(int x1, int y1, int dx, int dy)
    {
        int lgdx = Log2(dx), lgdy = Log2(dy);
        int x2 = x1 + dx - (1 << lgdx);
        int y2 = y1 + dy - (1 << lgdy);
        int max = _rmq[y2, lgdy, x2, lgdx];
        max = Math.Max(max, _rmq[y1, lgdy, x1, lgdx]);
        max = Math.Max(max, _rmq[y1, lgdy, x2, lgdx]);
        max = Math.Max(max, _rmq[y2, lgdy, x1, lgdx]);
        return max;
    }
}