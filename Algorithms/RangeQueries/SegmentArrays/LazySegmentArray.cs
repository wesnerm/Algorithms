using static System.Math;

namespace Algorithms.RangeQueries;

public struct LazySegmentArray
{
    readonly long[] _sum;
    readonly long[] _min;
    readonly long[] _max;
    readonly long[] _lazy;
    readonly int _n;

    public LazySegmentArray(long[] array) : this(array.Length)
    {
        for (int i = 0; i < array.Length; i++)
            _sum[_n + i] = _min[_n + i] = _max[_n + i] = array[i];

        for (int i = _n - 1; i > 0; i--)
            UpdateNode(i);
    }

    public LazySegmentArray(int n)
    {
        _n = LeastPowerOfTwoGreaterOrEqualTo(n);
        int size = 2 * _n;
        _sum = new long[size];
        _min = new long[size];
        _max = new long[size];
        _lazy = new long[size];

        for (int i = 0; i < _n; i++) {
            _min[i] = long.MaxValue;
            _max[i] = long.MinValue;
        }
    }

    public static int LeastPowerOfTwoGreaterOrEqualTo(int n)
    {
        int bits = n;
        while ((bits & (bits - 1)) != 0) bits &= bits - 1;
        if (n > bits) bits <<= 1;
        return bits;
    }

    public void Add(int start, int end, int value)
    {
        if (value == 0) return;
        Add(1, start, end, value, 0, _n);
    }

    void Add(int node, int start, int end, long value, int nodeStart, int nodeLimit)
    {
        if (start >= nodeLimit || end < nodeStart)
            return;

        if (nodeStart >= start && nodeLimit <= end + 1) {
            LazyAdd(node, value, nodeStart, nodeLimit);
            return;
        }

        LazyPropagate(node, nodeStart, nodeLimit);
        int mid = (nodeStart + nodeLimit) >> 1;
        Add(node * 2, start, end, value, nodeStart, mid);
        Add(node * 2 + 1, start, end, value, mid, nodeLimit);
        UpdateNode(node);
    }

    void LazyAdd(int node, long value, int nodeStart, int nodeLimit)
    {
        _sum[node] += value * (nodeLimit - nodeStart);
        _min[node] += value;
        _max[node] += value;
        _lazy[node] += value;
    }

    public long GetMin(int start, int end) => GetMin(1, start, end, 0, _n);

    long GetMin(int node, int start, int end, int nodeStart, int nodeLimit)
    {
        if (nodeStart >= start && nodeLimit <= end + 1)
            return _min[node];
        if (start >= nodeLimit || end < nodeStart)
            return long.MaxValue;

        int mid = (nodeStart + nodeLimit) >> 1;
        LazyPropagate(node, nodeStart, nodeLimit);
        return Min(GetMin(node * 2, start, end, nodeStart, mid),
            GetMin(node * 2 + 1, start, end, mid, nodeLimit));
    }

    public long GetSum(int start, int end) => GetSum(1, start, end, 0, _n);

    long GetSum(int node, int start, int end, int nodeStart, int nodeLimit)
    {
        if (nodeStart >= start && nodeLimit <= end + 1)
            return _sum[node];
        if (start >= nodeLimit || end < nodeStart)
            return 0;

        int mid = (nodeStart + nodeLimit) >> 1;
        LazyPropagate(node, nodeStart, nodeLimit);
        return GetSum(node * 2, start, end, nodeStart, mid)
               + GetSum(node * 2 + 1, start, end, mid, nodeLimit);
    }

    void LazyPropagate(int node, int nodeStart, int nodeLimit)
    {
        if (_lazy[node] != 0) {
            int mid = (nodeStart + nodeLimit) >> 1;
            LazyAdd(node * 2, _lazy[node], nodeStart, mid);
            LazyAdd(node * 2 + 1, _lazy[node], mid, nodeLimit);
            _lazy[node] = 0;
        }
    }

    void UpdateNode(int node)
    {
        _min[node] = Min(_min[node * 2], _min[node * 2 + 1]);
        _max[node] = Max(_max[node * 2], _max[node * 2 + 1]);
        _sum[node] = _sum[node * 2] + _sum[node * 2 + 1];
    }
}