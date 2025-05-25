using System.Runtime.CompilerServices;
using STType = long;

namespace Algorithms.RangeQueries;

[DebuggerDisplay("Length={Length}")]
public class SegmentTreeRU
{
    #region Variables

    public STType Sum;
    SegmentTreeRU Left, Right;
    public int Length;

    #endregion

    #region Constructor

    public SegmentTreeRU(STType[] array)
        : this(array, 0, array.Length) { }

    public SegmentTreeRU(STType[] array, int start, int length)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new SegmentTreeRU(array, start, half);
            Right = new SegmentTreeRU(array, start + half, length - half);
            Sum = DefaultValue;
        } else {
            Sum = array[start];
        }
    }

    public SegmentTreeRU(int length)
    {
        Length = length;
        Sum = DefaultValue;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new SegmentTreeRU(half);
            Right = (length & 1) == 0 ? Left : new SegmentTreeRU(length - half);
        }
    }

    public SegmentTreeRU(int length, STType defaultValue)
        : this(length) =>
        Sum = defaultValue;

    SegmentTreeRU(STType sum, SegmentTreeRU left, SegmentTreeRU right)
    {
        Left = left;
        Right = right;
        Length = left.Length + right.Length;
        Sum = sum;
    }

    #endregion

    #region Custom Operations

    const int DefaultValue = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static STType Combine(STType left, STType right) => left + right;

    #endregion

    #region Core Operations

    public STType Query(int start)
    {
        if (start >= Length || start < 0)
            return DefaultValue;
        if (start == 0 && Length == 1)
            return Sum;

        int mid = Left.Length;
        STType result = start < mid ? Left.Query(start) : Right.Query(start - mid);
        return Combine(result, Sum);
    }

    public void Add(int start, int end, STType value)
    {
        if (start >= Length || end < 0)
            return;

        if (start <= 0 && end >= Length - 1) {
            Sum = Combine(Sum, value);
            return;
        }

        int mid = Left.Length;
        Left.Add(start, end, value);
        Right.Add(start - mid, end - mid, value);
    }

    public void Cover(int start, int end, STType value)
    {
        if (start >= Length || end < 0) return;
        if (start <= 0 && end >= Length - 1) {
            Cover(value);
            return;
        }

        int mid = Left.Length;
        Propagate();
        Left.Cover(start, mid, value);
        Right.Cover(start - mid, end - mid, value);
    }

    public void Cover(STType value)
    {
        if (Left == null) {
            Sum = value;
            return;
        }

        Sum = DefaultValue;
        Left = new SegmentTreeRU(Left.Length, value);
        Right = new SegmentTreeRU(Right.Length, value);
    }

    void Propagate()
    {
        if (Sum != DefaultValue) {
            if (Left != null) Left.Sum = Combine(Left.Sum, Sum);
            if (Right != null) Right.Sum = Combine(Right.Sum, Sum);
            Sum = DefaultValue;
        }
    }

    #endregion

    #region Misc

    public override string ToString() => $"Sum={Sum} Length={Length}";

    public void FillTable(STType[] table, int start = 0, STType delta = DefaultValue)
    {
        STType sum = Combine(Sum, delta);
        if (Length == 1) {
            table[start] = sum;
        } else {
            Left.FillTable(table, start, sum);
            int rightStart = start + Left.Length;
            if (rightStart < table.Length) Right.FillTable(table, rightStart, sum);
        }
    }

    #endregion

    #region Properties

    public STType this[int index] {
        get => Query(index);
        set => Cover(index, index, value);
    }

    public STType[] Table {
        get
        {
            STType[] result = new STType[Length];
            FillTable(result);
            return result;
        }
    }

    #endregion
}