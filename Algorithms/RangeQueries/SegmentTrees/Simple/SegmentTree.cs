using System.Runtime.CompilerServices;

namespace Algorithms.RangeQueries;

using STType = long;

[DebuggerDisplay("Length = {Length}")]
public partial class SegmentTree
{
    #region Variables

    public STType Sum, LazyValue = DefaultValue; // IMPORTANT!
    public SegmentTree Left, Right;
    public int Length;
    public byte Covering;

    #endregion

    #region Constructor

    public SegmentTree(STType[] array)
        : this(array, 0, array.Length) { }

    public SegmentTree(int length, STType defaultValue = DefaultValue)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new SegmentTree(half);
            Right = new SegmentTree(length - half);
            UpdateNode();
        } else {
            Sum = defaultValue;
            SetMinMax(defaultValue);
        }
    }

    SegmentTree(STType[] array, int start, int length)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new SegmentTree(array, start, half);
            Right = new SegmentTree(array, start + half, length - half);
            UpdateNode();
        } else {
            Sum = array[start];
            SetMinMax(Sum);
        }
    }

    #endregion

    #region Properties

    public STType this[int index] {
        get => GetSum(index, index);
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

    #region Partials

    partial void AddMinMax(STType value);
    partial void SetMinMax(STType value);
    partial void UpdateMinMax();

    #endregion

    #region Core Operations

    const int DefaultValue = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static STType Combine(STType left, STType right) => left + right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static STType CombineLength(STType v, STType length) => v * length;

    public STType GetSum(int start, int end)
    {
        if (start <= 0 && end >= Length - 1) return Sum;
        if (start >= Length || end < 0)
            return DefaultValue;

        LazyPropagate();
        int mid = Left.Length;
        STType left = Left.GetSum(start, end);
        STType right = Right.GetSum(start - mid, end - mid);
        return Combine(left, right);
    }

    public void Add(int start, int end, STType value)
    {
        if (start >= Length || end < 0)
            return;

        if (start <= 0 && end >= Length - 1) {
            Add(value);
            return;
        }

        LazyPropagate();
        int mid = Left.Length;
        Left.Add(start, end, value);
        Right.Add(start - mid, end - mid, value);
        UpdateNode();
    }

    public void Cover(int start, int end, STType value)
    {
        if (start >= Length || end < 0)
            return;

        if (start <= 0 && end >= Length - 1) {
            Cover(value);
            return;
        }

        LazyPropagate();
        int mid = Left.Length;
        Left.Cover(start, end, value);
        Right.Cover(start - mid, end - mid, value);
        UpdateNode();
    }

    void LazyPropagate()
    {
        if (Length <= 1)
            return;

        STType value = LazyValue;
        if (Covering != 0) {
            if ((Covering & 1) != 0) {
                Left.Cover(value);
                Right.Cover(value);
            }

            LazyValue = DefaultValue;
            Covering = 0;
        } else if (value != DefaultValue) {
            Left.Add(value);
            Right.Add(value);
            LazyValue = DefaultValue;
        }
    }

    void Add(STType value)
    {
        LazyValue = Combine(LazyValue, value);
        Sum = Combine(Sum, CombineLength(value, Length));
        AddMinMax(value);
    }

    void Cover(STType value)
    {
        LazyValue = value;
        Sum = CombineLength(value, Length);
        Covering |= 1;
        SetMinMax(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void UpdateNode()
    {
        Sum = Combine(Left.Sum, Right.Sum);
        UpdateMinMax();
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

        LazyPropagate();
        Left.FillTable(table, start);
        int rightStart = start + Left.Length;
        if (rightStart < table.Length) Right.FillTable(table, rightStart);
    }

    #endregion
}