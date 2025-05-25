using System.Runtime.CompilerServices;

namespace Algorithms.RangeQueries.SegmentTrees;

public struct FlipType
{
    public int Value;
    public int FlipValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FlipType Add(FlipType value) =>
        new() { Value = Value + value.Value, FlipValue = FlipValue + value.FlipValue };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FlipType Subtract(FlipType value) =>
        new() { Value = Value - value.Value, FlipValue = FlipValue - value.FlipValue };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FlipType Scale(int value) => new() { Value = Value * value, FlipValue = FlipValue * value };

    public void Flip()
    {
        Swap(ref Value, ref FlipValue);
    }

    public override string ToString() => $"{Value}/{FlipValue}";
}

[DebuggerDisplay("Length = {Length}")]
public class FlipSegmentTree
{
    #region Variables

    public FlipType Sum;
    public FlipType LazyValue;
    public FlipSegmentTree Left;
    public FlipSegmentTree Right;
    public byte Covering;
    public int Length;

    #endregion

    #region Constructor

    public FlipSegmentTree(int length)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new FlipSegmentTree(half);
            Right = new FlipSegmentTree(length - half);
            UpdateNode();
        } else {
            InitSingleton(new FlipType());
        }
    }

    public FlipSegmentTree(FlipType[] array)
        : this(array, 0, array.Length) { }

    FlipSegmentTree(FlipType[] array, int start, int length)
    {
        Length = length;
        if (length >= 2) {
            int half = (length + 1) >> 1;
            Left = new FlipSegmentTree(array, start, half);
            Right = new FlipSegmentTree(array, start + half, length - half);
            UpdateNode();
        } else {
            InitSingleton(array[start]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void InitSingleton(FlipType v)
    {
        Sum = v;
    }

    #endregion

    #region Properties

    public FlipType this[int index] {
        get => GetSum(index, 1);
        set => Cover(index, 1, value);
    }

    public FlipType[] Table {
        get
        {
            var result = new FlipType[Length];
            FillTable(result);
            return result;
        }
    }

    #endregion

    #region Core Operations

    static FlipType Combine(FlipType left, FlipType right) => left.Add(right);

    static FlipType CombineLength(FlipType v, int length) => v.Scale(length);

    public FlipType GetSumInclusive(int start, int end) => GetSum(start, end - start + 1);

    public FlipType GetSum(int start, int count)
    {
        int end = start + count;

        if (start <= 0 && Length <= end)
            return Sum;
        if (start >= Length || end <= 0)
            return new FlipType();

        LazyPropagate();
        FlipType left = Left.GetSum(start, count);
        FlipType right = Right.GetSum(start - Left.Length, count);
        return Combine(left, right);
    }

    public void CoverInclusive(int start, int end, FlipType v)
    {
        Cover(start, end - start + 1, v);
    }

    public void Cover(int start, int count, FlipType value)
    {
        int end = start + count;
        if (start >= Length || end <= 0)
            return;

        LazyPropagate();
        if (start <= 0 && Length <= end) {
            Cover(value);
            return;
        }

        Left.Cover(start, count, value);
        Right.Cover(start - Left.Length, count, value);
        UpdateNode();
    }

    public void FlipInclusive(int start, int end)
    {
        Flip(start, end - start + 1);
    }

    public void Flip(int start, int count)
    {
        int end = start + count;
        if (start >= Length || end <= 0)
            return;

        LazyPropagate();

        if (start <= 0 && Length <= end) {
            Flip();
            return;
        }

        Left.Flip(start, count);
        Right.Flip(start - Left.Length, count);
        UpdateNode();
    }

    void Cover(FlipType value)
    {
        LazyValue = value;
        Sum = CombineLength(value, Length);
        Covering = 1;
    }

    void Flip()
    {
        // Doing LazyPropagate() here is too slow, results in TLEs.

        Sum.Flip();
        switch (Covering) {
            case 0:
            case 2:
                Covering ^= 2;
                break;
            case 1:
                LazyValue.Flip();
                break;
        }
    }

    void LazyPropagate()
    {
        if (Covering == 0 || Length <= 1)
            return;

        if (Covering == 2) {
            Left.Flip();
            Right.Flip();
        } else {
            FlipType value = LazyValue;
            Left.Cover(value);
            Right.Cover(value);
            LazyValue = new FlipType();
        }

        Covering = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void UpdateNode()
    {
        FlipSegmentTree left = Left;
        FlipSegmentTree right = Right;
        Sum = Combine(left.Sum, right.Sum);
    }

    #endregion

    #region Misc

    public override string ToString() => $"Sum={Sum} Length={Length}";

    public void FillTable(FlipType[] table, int start = 0)
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