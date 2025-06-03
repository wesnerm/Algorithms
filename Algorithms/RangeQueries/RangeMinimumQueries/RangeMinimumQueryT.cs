using System.Numerics;

namespace Algorithms.RangeQueries;

public class RangeMinimumQuery<T>
{
    readonly IList<T> _array;
    readonly Comparison<T> _compare;
    readonly int _n;
    readonly int[,] _table;

    public RangeMinimumQuery(IList<T> array, Comparison<T> compare = null)
    {
        compare = compare ?? Comparer<T>.Default.Compare;
        _array = array;
        _compare = compare;
        _n = array.Count;

        int n = array.Count;
        int lgn = Log2(n);
        _table = new int[lgn, n];

        _table[0, n - 1] = n - 1;
        for (int j = n - 2; j >= 0; j--)
            _table[0, j] = compare(array[j], array[j + 1]) <= 0 ? j : j + 1;

        for (int i = 1; i < lgn; i++) {
            int curlen = 1 << i;
            for (int j = 0; j < n; j++) {
                int right = j + curlen;
                int pos1 = _table[i - 1, j];
                int pos2;
                _table[i, j] =
                    right >= n || compare(array[pos1], array[pos2 = _table[i - 1, right]]) <= 0
                        ? pos1
                        : pos2;
            }
        }
    }

    public int GetArgMin(int left, int right)
    {
        if (left == right) return left;
        int curlog = BitOperations.Log2((ulong)(right - left + 1));
        int pos1 = _table[curlog - 1, left];
        int pos2 = _table[curlog - 1, right - (1 << curlog) + 1];
        return _compare(_array[pos1], _array[pos2]) <= 0 ? pos1 : pos2;
    }

    public T GetMin(int left, int right) => _array[GetArgMin(left, right)];

    private static int Log2(long size) => size > 0 ? BitOperations.Log2((ulong)size) : -1;

}