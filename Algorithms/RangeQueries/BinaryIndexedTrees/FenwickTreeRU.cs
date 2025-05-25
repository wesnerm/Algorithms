namespace Algorithms.RangeQueries;

public struct FenwickTreeRU
{
    public readonly long[] A;

    public FenwickTreeRU(int size) => A = new long[size + 1];

    public int Length => A.Length - 1;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public long[] Table {
        get
        {
            long[] table = new long[Length];
            for (int i = 0; i < table.Length; i++) table[i] = Query(i);
            return table;
        }
    }

    public void Clear()
    {
        Array.Clear(A, 0, A.Length);
    }

    public long Query(int i)
    {
        long sum = 0;
        for (i++; i > 0; i -= i & -i) sum += A[i];
        return sum;
    }

    public void AddInclusive(int left, int right, long delta)
    {
        for (int i = left + 1; i < A.Length; i += i & -i) A[i] += delta;
        for (int i = right + 2; i < A.Length; i += i & -i) A[i] -= delta;
    }
}