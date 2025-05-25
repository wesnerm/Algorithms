using System.Runtime.CompilerServices;

namespace Algorithms.RangeQueries.RangeMinimumQueries;

using STType = long;

public class DisjointSparseTable
{
    readonly STType[] A;
    readonly int maxlev;
    readonly int size;
    int n;
    STType[][] table;

    public DisjointSparseTable(STType[] A)
    {
        this.A = A;
        maxlev = Log2(n - 1) + 1;
        size = 1 << maxlev;

        for (int i = 0; i < maxlev; i++)
            table[i] = new STType[n];

        Build(0, 0, size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static STType Op(STType a, STType b) => a + b;

    void Build(int level, int l, int r)
    {
        int m = (l + r) >> 1;

        table[level][m] = A[m];
        for (int i = m - 1; i >= l; i--)
            table[level][i] = Op(table[level][i + 1], A[i]);

        if (m + 1 < r) {
            table[level][m + 1] = A[m + 1];
            for (int i = m + 2; i < r; i++)
                table[level][i] = Op(table[level][i - 1], A[i]);
        }

        if (l + 1 != r) {
            Build(level + 1, l, m);
            Build(level + 1, m, r);
        }
    }

    public STType Query(int x, int y)
    {
        if (x == y)
            return A[x];

        int k2 = Log2(x ^ y);
        int lev = maxlev - 1 - k2;
        STType ans = table[lev][x];
        if ((y & ((1 << k2) - 1)) != 0)
            ans = Op(ans, table[lev][y]);
        return ans;
    }

    void BuildH()
    {
        n = 2 << Log2(n - 1);
        for (int h = 1; h < table.Length; h++) {
            int range = n >> (h - 1);
            int half = range >> 1;
            for (int i = half; i < n; i += range) {
                table[h][i - 1] = A[i - 1];
                for (int j = i - 2; j >= i - half; j--)
                    table[h][j] = Op(table[h][j + 1], A[j]);
                table[h][i] = A[i];
                for (int j = i + 1; j < i + half; j++)
                    table[h][j] = Op(table[h][j - 1], A[j]);
            }
        }
    }

    public STType QueryH(int l, int r)
    {
        STType result;
        if (l == r) {
            result = A[l];
        } else {
            int h = Log2(l ^ r);
            result = Op(table[h][l], table[h][r]);
        }

        return result;
    }
}