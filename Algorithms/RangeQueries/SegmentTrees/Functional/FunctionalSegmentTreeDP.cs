using System.Runtime.CompilerServices;
using STType = long;

namespace Algorithms.RangeQueries;

[DebuggerDisplay("Length={Length}")]
public partial class FunctionalSegmentTreeDP
{
    #region Variables

    public STType Sum, Delta = DefaultValue; // IMPORTANT!
    public FunctionalSegmentTreeDP Left, Right;
    public int Length;

    #endregion

    #region Constructor

    public FunctionalSegmentTreeDP(int length, STType defaultValue = DefaultValue)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new FunctionalSegmentTreeDP(half);
            Right = (length & 1) == 0 ? Left : new FunctionalSegmentTreeDP(length - half);
            UpdateNode();
        } else {
            InitSingleton(defaultValue);
        }
    }

    public FunctionalSegmentTreeDP(STType[] array)
        : this(array, 0, array.Length) { }

    public FunctionalSegmentTreeDP(STType[] array, int start, int length)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new FunctionalSegmentTreeDP(array, start, half);
            Right = new FunctionalSegmentTreeDP(array, start + half, length - half);
            UpdateNode();
        } else {
            InitSingleton(array[start]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void InitSingleton(STType v)
    {
        Sum = v;
        SetMinMax(v);
    }

    FunctionalSegmentTreeDP(FunctionalSegmentTreeDP left, FunctionalSegmentTreeDP right)
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
        if (start <= 0 && end >= Length) return Sum;
        if (start >= Length || end < 0)
            return DefaultValue;

        int mid = Left.Length;
        STType left = Left.GetSum(start, end);
        STType right = Right.GetSum(start - mid, end - mid);
        return Combine(Delta, Combine(left, right));
    }

    public FunctionalSegmentTreeDP Add(int start, int end, STType value)
    {
        if (start >= Length || end < 0)
            return this;

        if (start <= 0 && end >= Length - 1)
            return Add(value);

        int mid = Left.Length;
        return UpdateNode(
            Left.Add(start, end, value),
            Right.Add(start - mid, end - mid, value));
    }

    public FunctionalSegmentTreeDP Cover(int start, int end, STType value)
    {
        if (start >= Length || end < 0)
            return this;

        if (start <= 0 && end >= Length - 1)
            return Cover(value);

        DeltaPropagate();
        int mid = Left.Length;
        return UpdateNode(
            Left.Cover(start, end, value),
            Right.Cover(start - mid, end - mid, value));
    }

    public FunctionalSegmentTreeDP Cover2(int start, int end, STType value)
    {
        if (start >= Length || end < 0)
            return this;

        if (start <= 0 && end >= Length - 1)
            return Cover(value);

        int mid = Left.Length;
        return UpdateNode(
            Left.Cover2(start, end, value - Delta),
            Right.Cover2(start - mid, end - mid, value - Delta));
    }

    void DeltaPropagate()
    {
        if (Length <= 1 || Delta == DefaultValue)
            return;

        Left = Left.Add(Delta);
        Right = Right.Add(Delta);
        Delta = DefaultValue;
    }

    FunctionalSegmentTreeDP Add(STType value)
    {
        var node = (FunctionalSegmentTreeDP)MemberwiseClone();
        node.Delta = Combine(node.Delta, value);
        node.Sum = Combine(node.Sum, CombineLength(value, node.Length));
        node.AddMinMax(value);
        return node;
    }

    FunctionalSegmentTreeDP Cover(STType value) =>
        Length != 1 || value != Sum
            ? new FunctionalSegmentTreeDP(Length, value)
            : this;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void UpdateNode()
    {
        Sum = Combine(Combine(Left.Sum, Right.Sum), CombineLength(Delta, Length));
        UpdateMinMax();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    FunctionalSegmentTreeDP UpdateNode(FunctionalSegmentTreeDP left, FunctionalSegmentTreeDP right) =>
        left != Left || right != Right ? new FunctionalSegmentTreeDP(left, right) : this;

    #endregion

    #region Misc

    public override string ToString() => $"Sum={Sum} Length={Length}";

    public void FillTable(STType[] table, int start = 0)
    {
        if (Length == 1) {
            table[start] = Sum;
            return;
        }

        DeltaPropagate();
        Left.FillTable(table, start);
        int rightStart = start + Left.Length;
        if (rightStart < table.Length) Right.FillTable(table, rightStart);
    }

    #endregion
}