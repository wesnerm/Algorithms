namespace Algorithms.RangeQueries;

using System.Numerics;
using RMQT = long;

public class RangeMaximumQuery
{
    readonly RMQT[] _array;
    readonly int _n;
    readonly int[,] _table;

    public RangeMaximumQuery(RMQT[] array)
    {
        _array = array;
        _n = array.Length;

        int n = array.Length;
        int lgn = BitOperations.Log2((uint)n);
        _table = new int[Math.Max(1, lgn), n];

        _table[0, n - 1] = n - 1;
        for (int j = n - 2; j >= 0; j--)
            _table[0, j] = array[j] >= array[j + 1] ? j : j + 1;

        for (int i = 1; i < lgn; i++) {
            int curlen = 1 << i;
            for (int j = 0; j < n; j++) {
                int right = j + curlen;
                int pos1 = _table[i - 1, j];
                int pos2;
                _table[i, j] =
                    right >= n || array[pos1] >= array[pos2 = _table[i - 1, right]]
                        ? pos1
                        : pos2;
            }
        }
    }

    public int GetArgMax(int left, int right)
    {
        if (left == right) return left;
        int curlog = Log2(right - left + 1);
        int pos1 = _table[curlog - 1, left];
        int pos2 = _table[curlog - 1, right - (1 << curlog) + 1];
        return _array[pos1] >= _array[pos2] ? pos1 : pos2;
    }

    public RMQT GetMax(int left, int right) => _array[GetArgMax(left, right)];

    private static int Log2(long size) => size > 0 ? BitOperations.Log2((ulong)size) : -1;

}