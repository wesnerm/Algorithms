using System.Runtime.CompilerServices;
using STType = long;

namespace Algorithms.RangeQueries;

[DebuggerDisplay("Length={Length}")]
public class SegmentTreeRQ
{
    #region Variables

    public STType Sum;
    readonly SegmentTreeRQ Left, Right;
    public int Length;

    #endregion

    #region Constructor

    public SegmentTreeRQ(STType[] array)
        : this(array, 0, array.Length) { }

    public SegmentTreeRQ(STType[] array, int start, int length)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new SegmentTreeRQ(array, start, half);
            Right = new SegmentTreeRQ(array, start + half, length - half);
            UpdateNode();
        } else {
            InitSingleton(array[start]);
        }
    }

    public SegmentTreeRQ(int length, STType defaultValue = DefaultValue)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new SegmentTreeRQ(half, defaultValue);
            Right = (length & 1) == 0 ? Left : new SegmentTreeRQ(length - half, defaultValue);
            UpdateNode();
        } else {
            InitSingleton(defaultValue);
        }
    }

    SegmentTreeRQ(SegmentTreeRQ left, SegmentTreeRQ right)
    {
        Left = left;
        Right = right;
        Length = left.Length + right.Length;
        UpdateNode();
    }

    #endregion

    #region Custom Operations

    const int DefaultValue = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static STType Combine(STType left, STType right) => left + right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void InitSingleton(STType v)
    {
        Sum = v;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void UpdateNode()
    {
        Sum = Combine(Left.Sum, Right.Sum);
    }

    #endregion

    #region Core Operations

    public STType Query(int start, int end)
    {
        if (start <= 0 && end >= Length) return Sum;
        if (start >= Length || end < 0)
            return DefaultValue;

        int mid = Left.Length;
        STType left = Left.Query(start, end);
        STType right = Right.Query(start - mid, end - mid);
        return Combine(left, right);
    }

    public void Add(int start, STType value)
    {
        if (start >= Length || start < 0) return;
        if (start != 0 || Length != 1) {
            SegmentTreeRQ left = Left, right = Right;
            int mid = left.Length;
            if (start < mid)
                left.Add(start, value);
            else
                right.Add(start - mid, value);
            UpdateNode();
            return;
        }

        Sum = Combine(Sum, value);
    }

    public void Cover(int start, STType value)
    {
        if (start >= Length || start < 0) return;
        if (start == 0 && Length == 1) {
            Sum = value;
            return;
        }

        SegmentTreeRQ left = Left, right = Right;
        int mid = left.Length;
        if (start < mid)
            left.Cover(start, value);
        else
            right.Cover(start - mid, value);
        UpdateNode();
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

    #region Search

    public int FindGreaterOrEqual(int k) => FindGreater(k - 1);

    public int FindGreater(STType k, int start = 0)
    {
        if (k >= Sum) return -1;
        if (Length == 1) return start;
        STType leftSum = Left.Sum;
        return k < leftSum
            ? Left.FindGreater(k, start)
            : Right.FindGreater(k - leftSum, start + Left.Length);
    }

    public int Next(STType k, int start = 0)
    {
        if (k >= start + Length - 1 || Sum <= 0) return -1;
        if (Length == 1) return start;
        int result = Left.Next(k);
        if (result < 0) result = Right.Next(k, start + Left.Length);
        return result;
    }

    public int Previous(STType k, int start = 0)
    {
        if (k <= start || Sum <= 0) return -1;
        if (Length == 1) return start;

        int result = Right.Previous(k, start + Left.Length);
        if (result < 0) result = Left.Previous(k);
        return result;
    }

    #endregion

    #region Properties

    public STType this[int index] {
        get => Query(index, index);
        set => Cover(index, value);
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