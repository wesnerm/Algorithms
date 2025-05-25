using System.Runtime.CompilerServices;
using STType = long;

namespace Algorithms.RangeQueries;

[DebuggerDisplay("Length={Length}")]
public class FunctionalSegmentTreeRQ
{
    #region Variables

    public STType Sum;
    public readonly FunctionalSegmentTreeRQ Left, Right;
    public int Length;

    #endregion

    #region Constructor

    public FunctionalSegmentTreeRQ(int length, STType defaultValue = DefaultValue)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new FunctionalSegmentTreeRQ(half);
            Right = (length & 1) == 0 ? Left : new FunctionalSegmentTreeRQ(length - half);
            UpdateNode();
        } else {
            InitSingleton(defaultValue);
        }
    }

    public FunctionalSegmentTreeRQ(STType[] array)
        : this(array, 0, array.Length) { }

    public FunctionalSegmentTreeRQ(STType[] array, int start, int length)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new FunctionalSegmentTreeRQ(array, start, half);
            Right = new FunctionalSegmentTreeRQ(array, start + half, length - half);
            UpdateNode();
        } else {
            InitSingleton(array[start]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void InitSingleton(STType v)
    {
        Sum = v;
    }

    FunctionalSegmentTreeRQ(FunctionalSegmentTreeRQ left, FunctionalSegmentTreeRQ right)
    {
        Length = left.Length + right.Length;
        Left = left;
        Right = right;
        UpdateNode();
    }

    #endregion

    #region Properties

    public STType this[int index] => GetSum(index, index);

    public STType[] Table {
        get
        {
            STType[] result = new STType[Length];
            FillTable(result);
            return result;
        }
    }

    #endregion

    #region Core Operations

    const int DefaultValue = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static STType Combine(STType left, STType right) => left + right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void UpdateNode()
    {
        Sum = Combine(Left.Sum, Right.Sum);
    }

    #endregion

    #region Core Operations

    public STType GetSum(int start, int end)
    {
        if (start <= 0 && end >= Length - 1) return Sum;
        if (start >= Length || end < 0)
            return DefaultValue;

        int mid = Left.Length;
        STType left = Left.GetSum(start, end);
        STType right = Right.GetSum(start - mid, end - mid);
        return Combine(left, right);
    }

    public FunctionalSegmentTreeRQ Add(int start, STType value)
    {
        if (start >= Length || start < 0) return this;
        if (start != 0 || Length != 1) {
            FunctionalSegmentTreeRQ left = Left, right = Right;
            int mid = left.Length;
            if (start < mid)
                left = left.Add(start, value);
            else
                right = right.Add(start - mid, value);
            return new FunctionalSegmentTreeRQ(left, right);
        }

        STType newValue = Combine(Sum, value);
        return Sum != newValue ? new FunctionalSegmentTreeRQ(1) { Sum = newValue } : this;
    }

    public FunctionalSegmentTreeRQ Cover(int start, STType value)
    {
        if (start >= Length || start < 0) return this;
        if (start != 0 || Length != 1) {
            FunctionalSegmentTreeRQ left = Left, right = Right;
            int mid = left.Length;
            if (start < mid)
                left = left.Cover(start, value);
            else
                right = right.Cover(start - mid, value);
            return new FunctionalSegmentTreeRQ(left, right);
        }

        return new FunctionalSegmentTreeRQ(Length, value);
    }

    #endregion

    #region Misc

    public override string ToString() => $"Sum={Sum} Length={Length}";

    public void FillTable(STType[] table, int start = 0)
    {
        if (Length == 1) {
            table[start] = Sum;
            return;
        }

        Left.FillTable(table, start);
        int rightStart = start + Left.Length;
        if (rightStart < table.Length) Right.FillTable(table, rightStart);
    }

    #endregion
}