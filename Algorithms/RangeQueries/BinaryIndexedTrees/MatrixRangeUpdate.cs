namespace Algorithms.RangeQueries;

public struct MatrixRangeUpdate
{
    readonly long[,] tree;
    readonly int rows;
    readonly int cols;

    public MatrixRangeUpdate(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        tree = new long[rows, cols];
    }

    public MatrixRangeUpdate(long[,] matrix)
    {
        rows = matrix.GetLength(0);
        cols = matrix.GetLength(1);
        tree = (long[,])matrix.Clone();

        for (int i = rows - 1; i >= 1; i--)
        for (int j = cols - 1; j >= 1; j--)
            tree[i, j] -= tree[i - 1, j];

        for (int i = rows - 1; i >= 1; i--)
        for (int j = cols - 1; j >= 1; j--)
            tree[i, j] -= tree[i, j - 1];

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

    public void Add(int row1, int col1, int row2, int col2, long delta)
    {
        Add(row1, col1, delta);
        Add(row2, col1, -delta);
        Add(row1, col2, -delta);
        Add(row2, col2, delta);
    }

    void Add(int row, int col, long delta)
    {
        if (col < cols)
            for (int i = row; i < rows; i |= i + 1)
            for (int j = col; j < cols; j |= j + 1)
                tree[i, j] += delta;
    }

    public long this[int row, int col] {
        get
        {
            long sum = 0;
            for (int i = row; i >= 0; i -= ~i & -~i)
            for (int j = col; j >= 0; j -= ~j & -~j)
                sum += tree[i, j];
            return sum;
        }
        set => Add(row, col, row, col, value - this[row, col]);
    }
}