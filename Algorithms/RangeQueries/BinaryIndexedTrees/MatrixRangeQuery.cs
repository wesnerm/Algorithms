namespace Algorithms.RangeQueries;

public struct MatrixRangeQuery
{
    readonly long[,] tree;
    readonly int rows;
    readonly int cols;

    public MatrixRangeQuery(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        tree = new long[rows, cols];
    }

    public MatrixRangeQuery(long[,] matrix)
    {
        rows = matrix.GetLength(0);
        cols = matrix.GetLength(1);
        tree = new long[rows, cols];

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++) {
            int r2 = r | (r + 1);
            int c2 = c | (c + 1);
            long v = tree[r, c];
            if (r2 < rows) tree[r2, c] += v;
            if (c2 < cols) tree[r, c2] += v;
            if (r2 < rows && c2 < cols) tree[r2, c2] -= v;
        }
    }

    public void Add(int row, int col, long delta)
    {
        for (int i = row; i < rows; i += ~i & -~i)
        for (int j = col; j < cols; j += ~j & -~j)
            tree[i, j] += delta;
    }

    public long SumRegion(int row1, int col1, int row2, int col2) =>
        Sum(row2, col2) + Sum(row1 - 1, col1 - 1) - Sum(row1 - 1, col2) - Sum(row2, col1 - 1);

    public long Sum(int row, int col)
    {
        long sum = 0;
        for (int i = row; i >= 0; i -= ~i & -~i)
        for (int j = col; j >= 0; j -= ~j & -~j)
            sum += tree[i, j];
        return sum;
    }
}