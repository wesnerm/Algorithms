namespace Algorithms.RangeQueries;

public struct FenwickTree
{
    public readonly long[] A;

    public FenwickTree(long[] a) : this(a.Length)
    {
        int n = a.Length;
        Array.Copy(a, 0, A, 1, n);
        for (int k = 2, h = 1; k <= n; k *= 2, h *= 2)
        for (int i = k; i <= n; i += k)
            A[i] += A[i - h];
    }

    public FenwickTree(long size) => A = new long[size + 1];

    public long this[int index] => SumInclusive(index, index);

    public int Length => A.Length - 1;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public long[] Table {
        get
        {
            long[] ret = (long[])A.Clone();
            for (int i = 1; i < ret.Length; i++) {
                int i2 = i + (i & -i);
                if (i2 < ret.Length)
                    ret[i2] -= ret[i];
            }

            return ret;
        }
    }

    public void Clear()
    {
        Array.Clear(A, 0, A.Length);
    }

    public void Add(int i, long val)
    {
        if (val == 0) return;
        for (i++; i < A.Length; i += i & -i)
            A[i] += val;
    }

    public long SumInclusive(int i)
    {
        long sum = 0;
        for (i++; i > 0; i -= i & -i)
            sum += A[i];
        return sum;
    }

    public long SumInclusive(int i, int j) => SumInclusive(j) - SumInclusive(i - 1);

    public int GetIndexGreater(long x)
    {
        int i = 0, n = A.Length - 1;
        for (int bit = HighestOneBit(n); bit != 0; bit >>= 1) {
            int t = i | bit;

            // if (t <= n && A[t] < x) for greater or equal 
            if (t <= n && A[t] <= x) {
                i = t;
                x -= A[t];
            }
        }

        // return i <= n ? i : -1; // for greater or equal
        return i < n ? i : -1;
    }

    public int Next(int x) => GetIndexGreater(SumInclusive(x));

    public int Previous(int x)
    {
        long count = SumInclusive(x - 1);
        return count > 0 ? GetIndexGreater(count - 1) : -1;
    }
}