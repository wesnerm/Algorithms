using System.Runtime.CompilerServices;
using STType = long;

namespace Algorithms.RangeQueries;

[DebuggerDisplay("Length={Length}")]
public partial class PersistentSegmentTree
{
    #region Variables

    public STType Sum, LazyValue = DefaultValue; // IMPORTANT!
    public PersistentSegmentTree Left, Right;
    public byte Covering;
    public bool ReadOnly;
    public int Length;

    #endregion

    #region Constructor

    public PersistentSegmentTree(int length, STType defaultValue = DefaultValue)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new PersistentSegmentTree(half);
            Right = (length & 1) == 0 ? Left.Freeze() : new PersistentSegmentTree(length - half);
            UpdateNode();
        } else {
            InitSingleton(defaultValue);
        }
    }

    public PersistentSegmentTree(STType[] array)
        : this(array, 0, array.Length) { }

    public PersistentSegmentTree(STType[] array, int start, int length)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new PersistentSegmentTree(array, start, half);
            Right = new PersistentSegmentTree(array, start + half, length - half);
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

    PersistentSegmentTree(PersistentSegmentTree left, PersistentSegmentTree right)
    {
        Length = left.Length + right.Length;
        Left = left;
        Right = right;
        UpdateNode();
    }

    public PersistentSegmentTree Clone()
    {
        var clone = (PersistentSegmentTree)MemberwiseClone();
        clone.ReadOnly = false;
        clone.Covering &= unchecked((byte)~2);
        if (clone.Length != 1) {
            clone.Left.Freeze();
            clone.Right.Freeze();
        }

        clone.IncrementId();
        return clone;
    }

    PersistentSegmentTree Freeze()
    {
        if (!ReadOnly) {
            LazyPropagate(); // Propagate before setting readonly for O(1)
            ReadOnly = true;
            Covering |= 2;
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    PersistentSegmentTree MutableNode() => !ReadOnly ? this : Clone();

    #endregion

    #region Properties

    public STType this[int index] {
        get => GetSum(index, 1);
        set => Cover(index, 1, value);
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
    partial void IncrementId();

    #endregion

    #region Core Operations

    const int DefaultValue = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static STType Combine(STType left, STType right) => left + right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static STType CombineLength(STType v, STType length) => v * length;

    public STType GetSum(int start, int end)
    {
        if (start <= 0 && Length - 1 <= end)
            return Sum;
        if (start >= Length || end < 0)
            return DefaultValue;

        LazyPropagate();
        int mid = Left.Length;
        STType left = Left.GetSum(start, end);
        STType right = Right.GetSum(start - mid, end - mid);
        return Combine(left, right);
    }

    public PersistentSegmentTree Add(int start, int end, STType value)
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

    public PersistentSegmentTree Cover(int start, int end, STType value)
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
        if (Covering != 0) {
            if ((Covering & 1) != 0) {
                Left = Left.Cover(value);
                Right = !ReadOnly || Left.Length != Right.Length
                    ? Right = Right.Cover(value)
                    : Left.Freeze();
            }

            LazyValue = DefaultValue;
            Covering = 0;
            if (ReadOnly) {
                Left.Freeze();
                Right.Freeze();
            }
        } else if (LazyValue != DefaultValue) {
            PersistentSegmentTree left = Left;
            Left = left.Add(value);
            Right = left != Right ? Right.Add(value) : Left.Freeze();
            LazyValue = DefaultValue;
        }
    }

    PersistentSegmentTree Add(STType value)
    {
        PersistentSegmentTree node = MutableNode();
        node.LazyValue = Combine(node.LazyValue, value);
        node.Sum = Combine(node.Sum, CombineLength(value, node.Length));
        node.AddMinMax(value);
        return node;
    }

    PersistentSegmentTree Cover(STType value)
    {
        PersistentSegmentTree node = this;

        if (ReadOnly) {
            if ((Covering & 1) != 0 && LazyValue == value) return this;

            node = MutableNode();
        }

        node.LazyValue = value;
        node.Sum = CombineLength(value, Length);
        node.Covering |= 1;
        node.SetMinMax(value);
        return node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void UpdateNode()
    {
        Sum = Combine(Left.Sum, Right.Sum);
        UpdateMinMax();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    PersistentSegmentTree UpdateNode(PersistentSegmentTree left, PersistentSegmentTree right)
    {
        if (ReadOnly)
            return new PersistentSegmentTree(left, right);
        Left = left;
        Right = right;
        UpdateNode();
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