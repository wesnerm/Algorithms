using System.Runtime.CompilerServices;
using STType = long;

namespace Algorithms.RangeQueries;

[DebuggerDisplay("Length={Length}")]
public partial class FunctionalSegmentTree
{
    #region Variables

    public STType Sum, LazyValue = DefaultValue; // IMPORTANT!
    public FunctionalSegmentTree Left, Right;
    public int Length;

    #endregion

    #region Constructor

    public FunctionalSegmentTree(int length, STType defaultValue = DefaultValue)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new FunctionalSegmentTree(half);
            Right = (length & 1) == 0 ? Left : new FunctionalSegmentTree(length - half);
            UpdateNode();
        } else {
            InitSingleton(defaultValue);
        }
    }

    public FunctionalSegmentTree(STType[] array)
        : this(array, 0, array.Length) { }

    public FunctionalSegmentTree(STType[] array, int start, int length)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new FunctionalSegmentTree(array, start, half);
            Right = new FunctionalSegmentTree(array, start + half, length - half);
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

    FunctionalSegmentTree(FunctionalSegmentTree left, FunctionalSegmentTree right)
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
        if (start <= 0 && end >= Length - 1) return Sum;
        if (start >= Length || end < 0)
            return DefaultValue;

        LazyPropagate();
        int mid = Left.Length;
        STType left = Left.GetSum(start, end);
        STType right = Right.GetSum(start - mid, end - mid);
        return Combine(left, right);
    }

    public FunctionalSegmentTree Add(int start, int end, STType value)
    {
        if (start >= Length || end < 0)
            return this;

        if (start <= 0 && end >= Length - 1)
            return Add(value);

        LazyPropagate();
        int mid = Left.Length;
        return UpdateNode(
            Left.Add(start, end, value),
            Right.Add(start - mid, end - mid, value));
    }

    public FunctionalSegmentTree Cover(int start, int end, STType value)
    {
        if (start >= Length || end < 0)
            return this;

        if (start <= 0 && end >= Length - 1)
            return Cover(value);

        LazyPropagate();
        int mid = Left.Length;
        return UpdateNode(
            Left.Cover(start, end, value),
            Right.Cover(start - mid, end - mid, value));
    }

    void LazyPropagate()
    {
        if (Length <= 1)
            return;

        STType value = LazyValue;
        if (LazyValue != DefaultValue) {
            FunctionalSegmentTree left = Left;
            Left = left.Add(value);
            Right = left != Right ? Right.Add(value) : Left;
            LazyValue = DefaultValue;
        }
    }

    FunctionalSegmentTree Add(STType value)
    {
        var node = (FunctionalSegmentTree)MemberwiseClone();
        node.LazyValue = Combine(node.LazyValue, value);
        node.Sum = Combine(node.Sum, CombineLength(value, node.Length));
        node.AddMinMax(value);
        return node;
    }

    FunctionalSegmentTree Cover(STType value) =>
        Length != 1 || value != Sum
            ? new FunctionalSegmentTree(Length, value)
            : this;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void UpdateNode()
    {
        Sum = Combine(Left.Sum, Right.Sum);
        UpdateMinMax();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    FunctionalSegmentTree UpdateNode(FunctionalSegmentTree left, FunctionalSegmentTree right)
    {
        if (left != Left || right != Right)
            return new FunctionalSegmentTree(left, right);
        return this;
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