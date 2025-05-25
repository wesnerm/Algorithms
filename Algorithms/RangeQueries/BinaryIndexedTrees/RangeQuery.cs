namespace Algorithms.RangeQueries;

public struct RangeQuery
{
    readonly long[] a1, a2;

    public RangeQuery(int n)
    {
        a1 = new long[n + 1];
        a2 = new long[n + 1];
    }

    public int Length => a1.Length;

    public long this[int index] => SumInclusive(index, index);

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public long[] Table {
        get
        {
            long[] table = new long[a1.Length - 1];
            for (int i = 0; i < table.Length; i++) table[i] = this[i];
            return table;
        }
    }

    public void Clear()
    {
        Array.Clear(a1, 0, a1.Length);
        Array.Clear(a2, 0, a2.Length);
    }

    public void AddInclusive(int left, int right, long v)
    {
        if (v == 0 || left > right) return;
        Add(a1, left, v);
        Add(a1, right + 1, -v);
        Add(a2, left, -v * (left - 1)); // TODO: Test this out for zero case
        Add(a2, right + 1, v * right);
    }

    public long SumInclusive(int i) => SumInclusive(a1, i) * i + SumInclusive(a2, i);

    public long SumInclusive(int i, int j) => SumInclusive(j) - SumInclusive(i - 1);

    void Add(long[] A, int i, long val)
    {
        for (i++; i < A.Length; i += i & -i) A[i] += val;
    }

    long SumInclusive(long[] A, int i)
    {
        long sum = 0;
        for (i++; i > 0; i -= i & -i) sum += A[i];
        return sum;
    }
}