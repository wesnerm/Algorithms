using T = long;

namespace Algorithms.RangeQueries;

public class MatrixSumIncremental
{
    readonly T[,] _matrix;

    public MatrixSumIncremental(T[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        _matrix = matrix;

        for (int i = 1; i < rows; i++)
        for (int j = 0; j < cols; j++)
            matrix[i, j] += matrix[i - 1, j];

        for (int i = 0; i < rows; i++)
        for (int j = 1; j < cols; j++)
            matrix[i, j] += matrix[i, j - 1];
    }

    public T this[int x, int y] => this[x, y, x, y];

    public T this[int x1, int y1, int x2, int y2] {
        get
        {
            T result = _matrix[x2, y2];

            if (x1 > 0)
                result -= _matrix[x1 - 1, y2];

            if (y1 > 0)
                result -= _matrix[x2, y1 - 1];

            if (x1 > 0 && y1 > 0)
                result += _matrix[x1 - 1, y1 - 1];

            return result;
        }
    }

    public T GetSum(int x, int y, int dx, int dy) => this[x, y, x + dx - 1, y + dy - 1];
}