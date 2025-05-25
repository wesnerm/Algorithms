using System.Runtime.CompilerServices;
using STType = long;

namespace Algorithms.RangeQueries.SegmentTrees;

[DebuggerDisplay("Length={Length}")]
public partial class PersistentSegmentTreeRQ
{
    #region Variables

    public STType Sum;
    internal PersistentSegmentTreeRQ Left, Right;
    public int Length;
    public bool ReadOnly;

    #endregion

    #region Constructor

    public PersistentSegmentTreeRQ(STType[] array, int start, int length)
    {
        Length = length;
        ReadOnly = true;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new PersistentSegmentTreeRQ(array, start, half);
            Right = new PersistentSegmentTreeRQ(array, start + half, length - half);
            UpdateNode();
        } else {
            InitSingleton(array[start]);
        }
    }

    public PersistentSegmentTreeRQ(STType[] array)
        : this(array, 0, array.Length) { }

    public PersistentSegmentTreeRQ(int length, STType defaultValue = DefaultValue)
    {
        Length = length;
        ReadOnly = true;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new PersistentSegmentTreeRQ(half, defaultValue);
            Right = (length & 1) == 0 ? Left : new PersistentSegmentTreeRQ(length - half, defaultValue);
            UpdateNode();
        } else {
            InitSingleton(defaultValue);
        }
    }

    PersistentSegmentTreeRQ(PersistentSegmentTreeRQ left, PersistentSegmentTreeRQ right)
    {
        Left = left;
        Right = right;
        Length = left.Length + right.Length;
        UpdateNode();
    }

    public PersistentSegmentTreeRQ Clone()
    {
        var clone = (PersistentSegmentTreeRQ)MemberwiseClone();
        clone.Left.Freeze();
        clone.Right.Freeze();
        clone.IncrementId();
        return clone;
    }

    PersistentSegmentTreeRQ Freeze()
    {
        if (!ReadOnly)
            MakeReadOnly();
        return this;
    }

    void MakeReadOnly()
    {
        if (ReadOnly) return;
        ReadOnly = true;
        Left?.MakeReadOnly();
        Right?.MakeReadOnly();
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PersistentSegmentTreeRQ MutableNode(STType value)
    {
        if (!ReadOnly) {
            Sum = value;
            return this;
        }

        var node = (PersistentSegmentTreeRQ)MemberwiseClone();
        node.ReadOnly = false;
        node.IncrementId();
        node.Sum = value;
        return node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    PersistentSegmentTreeRQ UpdateNode(PersistentSegmentTreeRQ left, PersistentSegmentTreeRQ right)
    {
        if (ReadOnly) return new PersistentSegmentTreeRQ(left, right);
        Left = left;
        Right = right;
        UpdateNode();
        return this;
    }

    partial void IncrementId();

    #endregion

    #region Core Operations

    public STType Query(int start, int end)
    {
        if (start <= 0 && end >= Length - 1) return Sum;
        if (start >= Length || end < 0)
            return DefaultValue;

        int mid = Left.Length;
        STType left = Left.Query(start, end);
        STType right = Right.Query(start - mid, end - mid);
        return Combine(left, right);
    }

    public PersistentSegmentTreeRQ Add(int start, STType value)
    {
        if (start >= Length || start < 0) return this;
        if (start != 0 || Length != 1) {
            PersistentSegmentTreeRQ left = Left, right = Right;
            int mid = left.Length;
            if (start < mid)
                left = left.Add(start, value);
            else
                right = right.Add(start - mid, value);
            return UpdateNode(left, right);
        }

        return MutableNode(Combine(Sum, value));
    }

    public PersistentSegmentTreeRQ Cover(int start, STType value)
    {
        if (start >= Length || start < 0) return this;
        if (start == 0 && Length == 1)
            return MutableNode(value);
        PersistentSegmentTreeRQ left = Left, right = Right;
        int mid = left.Length;
        if (start < mid)
            left = left.Cover(start, value);
        else
            right = right.Cover(start - mid, value);
        return UpdateNode(left, right);
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