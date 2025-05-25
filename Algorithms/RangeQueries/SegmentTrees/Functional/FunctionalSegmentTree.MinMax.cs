using System.Runtime.CompilerServices;
using static System.Math;
using STType = long;

namespace Algorithms.RangeQueries;

public partial class FunctionalSegmentTree
{
    #region Variables

    public const STType MaxValue = STType.MaxValue;
    public const STType MinValue = STType.MinValue;

    public STType Min = MaxValue;
    public STType Max = MinValue;

    #endregion

    #region Core Operations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    partial void AddMinMax(STType value)
    {
        Min = Combine(Min, value);
        Max = Combine(Max, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    partial void SetMinMax(STType value)
    {
        Min = value;
        Max = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    partial void UpdateMinMax()
    {
        FunctionalSegmentTree left = Left;
        FunctionalSegmentTree right = Right;
        Min = Min(left.Min, right.Min);
        Max = Max(left.Max, right.Max);
    }

    #endregion

    #region Other Operations

    public STType GetMin(int start, int end)
    {
        if (start <= 0 && end >= Length - 1) return Min;
        if (start >= Length || end < 0) return MaxValue;

        LazyPropagate();
        int mid = Left.Length;
        return Min(Left.GetMin(start, end),
            Right.GetMin(start - mid, end - mid));
    }

    public STType GetMax(int start, int end)
    {
        if (start <= 0 && end >= Length - 1) return Max;
        if (start >= Length || end < 0) return MinValue;

        LazyPropagate();
        int mid = Left.Length;
        return Max(Left.GetMax(start, end),
            Right.GetMax(start - mid, end - mid));
    }

    #endregion
}