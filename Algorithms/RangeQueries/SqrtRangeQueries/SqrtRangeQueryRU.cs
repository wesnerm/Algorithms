using System.Runtime.CompilerServices;

namespace Algorithms.RangeQueries.SegmentTrees;

/// <summary>
///     Full and optimal sqrt range update and range queries
/// </summary>
public class SqrtRangeQueryRU
{
    readonly int[] array, lazy;
    readonly int shift, mask;

    public SqrtRangeQueryRU(int n)
    {
        shift = DecompositionSqrt(n);
        mask = (1 << shift) - 1;
        lazy = new int[(n >> shift) + 1];
        array = new int[n];
    }

    public SqrtRangeQueryRU(int[] array) : this(array.Length)
    {
        for (int i = 0; i < array.Length; i++)
            Add(i, array[i]);
    }

    public int this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[index] + lazy[index >> shift];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Add(index, value - this[index]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int i, int v)
    {
        array[i] += v;
    }

    public void AddInclusive(int start, int end, int value)
    {
        int startBorder = Math.Min(end + 1, (start + mask) & ~mask);
        int endBorder = Math.Max(startBorder, (end + 1) & ~mask);

        for (int i = start; i < startBorder; i++)
            array[i] += value;

        int block = startBorder >> shift;
        for (int i = startBorder; i < endBorder; i += mask + 1, block++)
            lazy[block] += value;

        for (int i = endBorder; i <= end; i++)
            array[i] += value;
    }

    static int DecompositionSqrt(int n)
    {
        int shift = 1;
        while (2 * n > 5 * shift * shift) shift <<= 1;
        return shift;
    }
}