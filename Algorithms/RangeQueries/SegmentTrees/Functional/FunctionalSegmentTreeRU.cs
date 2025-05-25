using System.Runtime.CompilerServices;
using STType = long;

namespace Algorithms.RangeQueries;

[DebuggerDisplay("Length={Length}")]
public class FunctionalSegmentTreeRU
{
    #region Variables

    public readonly STType Sum;
    readonly FunctionalSegmentTreeRU Left, Right;
    public int Length;

    #endregion

    #region Constructor

    public FunctionalSegmentTreeRU(STType[] array)
        : this(array, 0, array.Length) { }

    public FunctionalSegmentTreeRU(STType[] array, int start, int length)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new FunctionalSegmentTreeRU(array, start, half);
            Right = new FunctionalSegmentTreeRU(array, start + half, length - half);
            Sum = DefaultValue;
        } else {
            Sum = array[start];
        }
    }

    public FunctionalSegmentTreeRU(int length)
    {
        Length = length;
        Sum = DefaultValue;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new FunctionalSegmentTreeRU(half);
            Right = (length & 1) == 0 ? Left : new FunctionalSegmentTreeRU(length - half);
        }
    }

    public FunctionalSegmentTreeRU(int length, STType defaultValue)
        : this(length) =>
        Sum = defaultValue;

    FunctionalSegmentTreeRU(STType sum, FunctionalSegmentTreeRU left, FunctionalSegmentTreeRU right)
    {
        Left = left;
        Right = right;
        Length = left != null ? left.Length + right.Length : 1;
        Sum = sum;
    }

    #endregion

    #region Properties

    public STType this[int index] => GetSum(index);

    public STType[] Table {
        get
        {
            STType[] result = new STType[Length];
            FillTable(result);
            return result;
        }
    }

    #endregion

    #region Custom Operations

    const int DefaultValue = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static STType Combine(STType left, STType right) => left + right;

    #endregion

    #region Core Operations

    public STType GetSum(int start)
    {
        if (start >= Length || start < 0)
            return DefaultValue;
        if (Length == 1)
            return Sum;

        int mid = Left.Length;
        STType result = start < mid ? Left.GetSum(start) : Right.GetSum(start - mid);
        return Combine(result, Sum);
    }

    public FunctionalSegmentTreeRU Add(int start, int end, STType value)
    {
        if (start >= Length || end < 0)
            return this;

        if (start <= 0 && end >= Length - 1)
            return new FunctionalSegmentTreeRU(Combine(Sum, value), Left, Right);

        int mid = Left.Length;
        return new FunctionalSegmentTreeRU(Sum,
            Left.Add(start, end, value),
            Right.Add(start - mid, end - mid, value));
    }

    public FunctionalSegmentTreeRU Cover(int start, int end, STType value)
    {
        if (start >= Length || end < 0) return this;

        if (start <= 0 && end >= Length - 1)
            return new FunctionalSegmentTreeRU(Length, value);

        int mid = Left.Length;
        return new FunctionalSegmentTreeRU(Sum,
            Left.Cover(start, end, value - Sum),
            Right.Cover(start - mid, end - mid, value - Sum));
    }

    #endregion

    #region Misc

    public override string ToString() => $"Sum={Sum} Length={Length}";

    public void FillTable(STType[] table, int start = 0, STType delta = DefaultValue)
    {
        STType sum = Combine(Sum, delta);
        if (Length == 1) {
            table[start] = sum;
            return;
        }

        Left.FillTable(table, start, sum);
        int rightStart = start + Left.Length;
        if (rightStart < table.Length) Right.FillTable(table, rightStart, sum);
    }

    #endregion
}