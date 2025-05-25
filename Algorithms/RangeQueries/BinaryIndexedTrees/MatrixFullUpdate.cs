namespace Algorithms.RangeQueries;

public struct MatrixFullUpdate
{
    readonly long[,] t0;
    readonly long[,] t1;
    readonly int rows;
    readonly int cols;

    public MatrixFullUpdate(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        t0 = new long[rows, cols];
        t1 = new long[rows, cols];
    }

    public MatrixFullUpdate(long[,] matrix)
    {
        rows = matrix.GetLength(0);
        cols = matrix.GetLength(1);
        t0 = (long[,])matrix.Clone();
        t1 = new long[rows, cols];

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++) {
            int r2 = r | (r + 1);
            int c2 = c | (c + 1);
            long v = t0[r, c];
            if (r2 < rows) t0[r2, c] += v;
            if (c2 < cols) t0[r, c2] += v;
            if (r2 < rows && c2 < cols) t0[r2, c2] -= v;
        }
    }

    public void Add(int row1, int col1, int row2, int col2, long v)
    {
        Add(t0, row1, col1, v);
        Add(t0, row2 + 1, col1, -v);
        Add(t0, row1, col2 + 1, -v);
        Add(t0, row2 + 1, col2 + 1, v);
        Add(t1, row1, col1, -v * (row1 - 1) * (col1 - 1));
        Add(t1, row1, col2 + 1, v * (row1 - 1) * col2);
        Add(t1, row2 + 1, col1, v * row2 * (col1 - 1));
        Add(t1, row2 + 1, col1 + 1, -v * row2 * col2);
    }

    public long Query(int row1, int col1, int row2, int col2)
    {
        col1--;
        col2--;
        return Sum(row2, col2) + Sum(row1, col1) - Sum(row2, col1) - Sum(row1, col2);
    }

    void Add(long[,] t, int row, int col, long delta)
    {
        if (col < cols)
            for (int i = row; i < rows; i |= i + 1)
            for (int j = col; j < cols; j |= j + 1)
                t0[i, j] += delta;
    }

    long Sum(int row, int col) => Sum(t0, row, col) + Sum(t1, row, col) * row * col;

    static long Sum(long[,] t, int row, int col)
    {
        long sum = 0;
        if (col >= 0)
            for (int i = row; i >= 0; i -= ~i & -~i)
            for (int j = col; j >= 0; j -= ~j & -~j)
                sum += t[i, j];
        return sum;
    }

    public long this[int row, int col] {
        get => Query(row, col, row, col);
        set => Add(row, col, row, col, value - this[row, col]);
    }
}