using static System.Math;

namespace Algorithms.RangeQueries;

public class MaxSegmentTree
{
    public bool Covering;
    public MaxSegmentTree Left, Right;
    public long Max = long.MinValue, LazyMax = long.MinValue;
    public int Start, End;

    public MaxSegmentTree(int n)
        : this(null, 0, n - 1) { }

    public MaxSegmentTree(long[] array)
        : this(array, 0, array.Length - 1) { }

    MaxSegmentTree(long[] array, int start, int end)
    {
        Start = start;
        End = end;

        if (end > start) {
            int mid = (start + end) >> 1;
            Left = new MaxSegmentTree(array, start, mid);
            Right = new MaxSegmentTree(array, mid + 1, end);
            UpdateNode();
        } else {
            long v = array?[start] ?? long.MinValue;
            Max = v;
        }
    }

    public long this[int index] => QueryInclusive(index, index);
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
            table[Start] = Max;
            return;
        }

        LazyPropagate();
        Left.FillTable(table);
        if (Right.Start < table.Length) Right.FillTable(table);
    }

    public long QueryInclusive(int start, int end)
    {
        if (Start >= start && End <= end)
            return Max;
        if (start > End || end < Start)
            return long.MinValue;

        LazyPropagate();
        return Max(Left.QueryInclusive(start, end), Right.QueryInclusive(start, end));
    }

    public void UpdateInclusive(int start, int end, long value)
    {
        if (start > End || end < Start)
            return;

        if (Start >= start && End <= end) {
            Add(value);
            return;
        }

        LazyPropagate();
        Left.UpdateInclusive(start, end, value);
        Right.UpdateInclusive(start, end, value);
        UpdateNode();
    }

    void Add(long value)
    {
        Max = Max(value, Max);
        LazyMax = Max(LazyMax, value);
    }

    void LazyPropagate()
    {
        if (Start == End)
            return;

        if (Covering) {
            Left.Cover(Max);
            Right.Cover(Max);
            LazyMax = long.MinValue;
            Covering = false;
        }

        if (LazyMax != long.MinValue) {
            long value = LazyMax;
            LazyMax = long.MinValue;
            Left.Add(value);
            Right.Add(value);
        }
    }

    void UpdateNode()
    {
        MaxSegmentTree left = Left;
        MaxSegmentTree right = Right;
        Max = Max(left.Max, right.Max);
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
        Max = value;
        LazyMax = long.MinValue;
        Covering = true;
    }

    public int FindFirstGE(int start, int end, long value)
    {
        if (start > End || end < Start || Max < value)
            return -1;

        if (Start == End)
            return Start;

        int result = Left.FindFirstGE(start, end, value);
        if (result != -1) return result;
        return Right.FindFirstGE(start, end, value);
    }

    public int FindLastGE(int start, int end, long value)
    {
        if (start > End || end < Start || Max < value)
            return -1;

        if (Start == End)
            return Start;

        int result = Right.FindLastGE(start, end, value);
        if (result != -1) return result;
        return Left.FindLastGE(start, end, value);
    }
}