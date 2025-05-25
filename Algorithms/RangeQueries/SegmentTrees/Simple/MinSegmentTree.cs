using static System.Math;

namespace Algorithms.RangeQueries;

public class MinSegmentTree
{
    public bool Covering;
    public MinSegmentTree Left, Right;
    public long Min = long.MaxValue, LazyMin = long.MaxValue;
    public int Start, End;

    public MinSegmentTree(int n)
        : this(null, 0, n - 1) { }

    public MinSegmentTree(long[] array)
        : this(array, 0, array.Length - 1) { }

    MinSegmentTree(long[] array, int start, int end)
    {
        Start = start;
        End = end;

        if (end > start) {
            int mid = (start + end) >> 1;
            Left = new MinSegmentTree(array, start, mid);
            Right = new MinSegmentTree(array, mid + 1, end);
            UpdateNode();
        } else {
            long v = array?[start] ?? long.MaxValue;
            Min = v;
        }
    }

    public long this[int index] => Query(index, index);
    public int Length => End - Start + 1;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public long[] Table {
        get
        {
            long[] result = new long[Length];
            FillTable(result);
            return result;
        }
    }

    public void FillTable(long[] table)
    {
        if (Start == End) {
            table[Start] = Min;
            return;
        }

        LazyPropagate();
        Left.FillTable(table);
        if (Right.Start < table.Length) Right.FillTable(table);
    }

    public long Query(int start, int end)
    {
        if (Start >= start && End <= end)
            return Min;
        if (start > End || end < Start)
            return long.MaxValue;

        LazyPropagate();
        return Min(Left.Query(start, end), Right.Query(start, end));
    }

    public void Add(int start, int end, long value)
    {
        if (start > End || end < Start)
            return;

        if (Start >= start && End <= end) {
            Add(value);
            return;
        }

        LazyPropagate();
        Left.Add(start, end, value);
        Right.Add(start, end, value);
        UpdateNode();
    }

    void Add(long value)
    {
        Min = Min(value, Min);
        LazyMin = Min(LazyMin, value);
    }

    void LazyPropagate()
    {
        if (Start == End)
            return;

        if (Covering) {
            Left.Cover(Min);
            Right.Cover(Min);
            LazyMin = long.MaxValue;
            Covering = false;
        }

        if (LazyMin != long.MaxValue) {
            long value = LazyMin;
            LazyMin = long.MaxValue;
            Left.Add(value);
            Right.Add(value);
        }
    }

    void UpdateNode()
    {
        MinSegmentTree left = Left;
        MinSegmentTree right = Right;
        Min = Min(left.Min, right.Min);
    }

    public void CoverInclusive(int start, int end, long value)
    {
        if (start > End || end < Start)
            return;

        if (Start >= start && End <= end) {
            Cover(value);
            return;
        }

        LazyPropagate();
        Left.CoverInclusive(start, end, value);
        Right.CoverInclusive(start, end, value);
        UpdateNode();
    }

    void Cover(long value)
    {
        Min = value;
        LazyMin = long.MaxValue;
        Covering = true;
    }

    public int FindFirstLE(int start, int end, long value)
    {
        if (start > End || end < Start || Min > value)
            return -1;

        if (Start == End)
            return Start;

        int result = Left.FindFirstLE(start, end, value);
        if (result != -1) return result;
        return Right.FindFirstLE(start, end, value);
    }

    public int FindLastLE(int start, int end, long value)
    {
        if (start > End || end < Start || Min > value)
            return -1;

        if (Start == End)
            return Start;

        int result = Right.FindLastLE(start, end, value);
        if (result != -1) return result;
        return Left.FindLastLE(start, end, value);
    }
}