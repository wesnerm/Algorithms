using System.Runtime.CompilerServices;
using STType = long;

namespace Algorithms.RangeQueries;

[DebuggerDisplay("Length={Length}")]
public class PersistentSegmentTreeRU
{
    #region Variables

    public STType Sum;
    public PersistentSegmentTreeRU Left, Right;
    public int Length;
    public bool ReadOnly;

    #endregion

    #region Constructor

    public PersistentSegmentTreeRU(STType[] array)
        : this(array, 0, array.Length) { }

    public PersistentSegmentTreeRU(STType[] array, int start, int length)
    {
        Length = length;
        ReadOnly = true;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new PersistentSegmentTreeRU(array, start, half);
            Right = new PersistentSegmentTreeRU(array, start + half, length - half);
            Sum = DefaultValue;
        } else {
            Sum = array[start];
        }
    }

    public PersistentSegmentTreeRU(int length)
    {
        Length = length;
        Sum = DefaultValue;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new PersistentSegmentTreeRU(half);
            Right = (length & 1) == 0 ? Left : new PersistentSegmentTreeRU(length - half);
        }
    }

    public PersistentSegmentTreeRU(int length, STType defaultValue)
        : this(length) =>
        Sum = defaultValue;

    PersistentSegmentTreeRU(STType sum, PersistentSegmentTreeRU left, PersistentSegmentTreeRU right)
    {
        Left = left;
        Right = right;
        Length = left.Length + right.Length;
        Sum = sum;
    }

    public PersistentSegmentTreeRU Clone()
    {
        var clone = (PersistentSegmentTreeRU)MemberwiseClone();
        clone.Left.Freeze();
        clone.Right.Freeze();
        return clone;
    }

    PersistentSegmentTreeRU Freeze()
    {
        if (!ReadOnly) MakeReadOnly();
        return this;
    }

    void MakeReadOnly()
    {
        if (ReadOnly) return;
        ReadOnly = true;
        Left?.MakeReadOnly();
        Right?.MakeReadOnly();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PersistentSegmentTreeRU MutableNode(STType value)
    {
        if (!ReadOnly) {
            Sum = value;
            return this;
        }

        var node = (PersistentSegmentTreeRU)MemberwiseClone();
        node.ReadOnly = false;
        node.Sum = value;
        return node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    PersistentSegmentTreeRU UpdateNode(PersistentSegmentTreeRU left, PersistentSegmentTreeRU right)
    {
        if (ReadOnly) return new PersistentSegmentTreeRU(Sum, left, right);
        Left = left;
        Right = right;
        return this;
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

    public PersistentSegmentTreeRU Add(int start, int end, STType value)
    {
        if (start >= Length || end < 0)
            return this;

        if (start <= 0 && end >= Length - 1)
            return MutableNode(Combine(Sum, value));

        int mid = Left.Length;
        return UpdateNode(
            Left.Add(start, end, value),
            Right.Add(start - mid, end - mid, value));
    }

    public PersistentSegmentTreeRU Cover(int start, int end, STType value)
    {
        if (start >= Length || start < 0) return this;

        if (start <= 0 && end >= Length - 1)
            return new PersistentSegmentTreeRU(Length, value);

        int mid = Left.Length;
        return UpdateNode(
            Left.Cover(start, end, value - Sum),
            Right.Cover(start - mid, end - mid, value - Sum));
    }

    // See Propagate in SegmentTreeRU

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