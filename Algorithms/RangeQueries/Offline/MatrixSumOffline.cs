namespace Algorithms.RangeQueries;

[DebuggerStepThrough]
public class MatrixSumOffline
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public readonly long[,] A;

    public readonly int Rows, Cols;
    public int D; // 1=difference, 0=function, -1=summation

    public MatrixSumOffline(int rows, int cols, int deriv = 0)
    {
        A = new long[rows, cols];
        Rows = rows;
        Cols = cols;
        deriv = 0;
    }

    public MatrixSumOffline(long[,] array, bool clone = true)
    {
        A = clone ? (long[,])array.Clone() : array;
        Rows = A.GetLength(0);
        Cols = A.GetLength(1);
    }

    public MatrixSumOffline(int[,] array) : this(array.GetLength(0), array.GetLength(1))
    {
        for (int r = 0; r < Rows; r++)
        for (int c = 0; c < Cols; c++)
            A[r, c] = array[r, c];
    }

    public MatrixSumOffline(long[][] array) : this(array.Length, array[0].Length)
    {
        for (int r = 0; r < Rows; r++)
        for (int c = 0; c < Cols; c++)
            A[r, c] = array[r][c];
    }

    public long this[int r, int c] {
        get => D == 0 ? A[r, c] : QueryInclusive(r, c, r, c);
        set
        {
            if (D != 0) FindDerivative(0);
            A[r, c] = value;
        }
    }

    public void FindDerivative(int d)
    {
        for (; d < D; D--) // Integrate
        {
            for (int i = 1; i <= Rows; i++)
            for (int j = 1; j <= Cols; j++)
                A[i, j] += A[i - 1, j];

            for (int i = 1; i <= Rows; i++)
            for (int j = 1; j <= Cols; j++)
                A[i, j] += A[i, j - 1];
        }

        for (; d > D; D++) // Differentiate
        {
            for (int i = Rows - 1; i >= 1; i--)
            for (int j = Cols - 1; j >= 1; j--)
                A[i, j] -= A[i - 1, j];

            for (int i = Rows - 1; i >= 1; i--)
            for (int j = Cols - 1; j >= 1; j--)
                A[i, j] -= A[i, j - 1];
        }
    }

    public void AddInclusive(int r1, int c1, int r2, int c2, long v)
    {
        if (D != 1) FindDerivative(1);

        A[r1, c1] += v;
        if (r2 + 1 < Rows) A[r2 + 1, c1] -= v;
        if (c2 + 1 < Cols) A[r1, c2 + 1] -= v;
        if (r2 + 1 < Rows && c2 + 1 < Cols) A[r2 + 1, c2 + 1] += v;
    }

    public long QueryInclusive(int r1, int c1, int r2, int c2)
    {
        if (D != -1) FindDerivative(-1);

        long result = A[r2, c2];
        if (r1 >= 0) result -= A[r1, c2];
        if (c1 >= 0) result -= A[r2, c1];
        if (r1 >= 0 && c1 >= 0) result += A[r1, c1];
        return result;
    }
}