namespace Algorithms.RangeQueries;

using T = System.Int64;

public class RangeAggregateQuery
{
    readonly T[,] _table;
    readonly int _n;
    readonly T[] _array;
    Func<T, T, T> _func;

    public RangeAggregateQuery(T[] array, Func<T, T, T> f)
    {
        _func = f;
        _array = array;
        _n = array.Length;

        int n = array.Length;
        int lgn = Log2(n);
        _table = new T[lgn, n];

        _table[0, n - 1] = array[n - 1];
        for (int j = n - 2; j >= 0; j--)
            _table[0, j] = f(array[j], array[j + 1]);

        for (int i = 1; i < lgn; i++)
        {
            int curlen = 1 << i;
            for (int j = 0; j < n; j++)
            {
                int right = j + curlen;
                var v = _table[i - 1, j];
                _table[i, j] = right >= n ? v : f(v, _table[i - 1, right]);
            }
        }
    }

    public T GetAggregate(int left, int right)
    {
        if (left == right) return _array[left];
        int curlog = Log2(right - left + 1);
        T pos1 = _table[curlog - 1, left];
        T pos2 = _table[curlog - 1, right - (1 << curlog) + 1];
        return _func(pos1, pos2);
    }
}
