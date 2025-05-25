namespace Algorithms.RangeQueries;

using T = int;

public class RangeMinimumQuery
{
    readonly int[,] _table;
    public readonly T[] Array;
    public readonly int Length;

    public RangeMinimumQuery(T[] array)
    {
        Array = array;
        Length = array.Length;

        int n = array.Length;
        int lgn = Math.Max(1, Log2(n));
        _table = new int[lgn, n];

        _table[0, n - 1] = n - 1;
        for (int j = n - 2; j >= 0; j--)
            _table[0, j] = array[j] <= array[j + 1] ? j : j + 1;

        for (int i = 1; i < lgn; i++) {
            int curlen = 1 << i;
            for (int j = 0; j < n; j++) {
                int right = j + curlen;
                int pos1 = _table[i - 1, j];
                int pos2;
                _table[i, j] =
                    right >= n || array[pos1] <= array[pos2 = _table[i - 1, right]]
                        ? pos1
                        : pos2;
            }
        }
    }

    public int GetArgMin(int left, int right)
    {
        if (left == right) return left;
        int curlog = Log2(right - left + 1);
        int pos1 = _table[curlog - 1, left];
        int pos2 = _table[curlog - 1, right - (1 << curlog) + 1];
        return Array[pos1] <= Array[pos2] ? pos1 : pos2;
    }

    public T GetMin(int left, int right) => Array[GetArgMin(left, right)];
}