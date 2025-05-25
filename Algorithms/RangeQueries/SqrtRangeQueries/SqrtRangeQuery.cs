using System.Runtime.CompilerServices;

namespace Algorithms.RangeQueries.SegmentTrees;

/// <summary>
///     Full and optimal sqrt range update and range queries
/// </summary>
public class SqrtRangeQuery
{
    readonly int[] array, blocks, lazy;
    readonly int shift, mask;

    public SqrtRangeQuery(int n)
    {
        shift = DecompositionSqrt(n);
        mask = (1 << shift) - 1;
        blocks = new int[(n >> shift) + 1];
        lazy = new int[(n >> shift) + 1];
        array = new int[n];
    }

    public SqrtRangeQuery(int[] array) : this(array.Length)
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

    public static int DecompositionSqrt(int n)
    {
        int shift = 1;
        // while (n - shift * shift > (2 * shift)*(2 * shift) - n)
        while (2 * n > 5 * shift * shift) shift <<= 1;
        return shift;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int i, int v)
    {
        blocks[i & mask] += v;
        array[i] += v;
    }

    public void AddInclusive(int start, int end, int value)
    {
        int startBorder = Math.Min(end + 1, (start + mask) & ~mask);
        int endBorder = Math.Max(startBorder, (end + 1) & ~mask);

        for (int i = start; i < startBorder; i++)
            Add(i, value);

        int block = startBorder >> shift;
        for (int i = startBorder; i < endBorder; i += mask + 1, block++) {
            blocks[block] += value * (mask + 1);
            lazy[block] += value;
        }

        for (int i = endBorder; i <= end; i++)
            Add(i, value);
    }

    public long SumInclusive(int start, int end)
    {
        int startBorder = Math.Min(end + 1, (start + mask) & ~mask);
        int endBorder = Math.Max(startBorder, (end + 1) & ~mask);

        long sum = 0;
        for (int i = start; i < startBorder; i++)
            sum += this[i];

        int block = startBorder >> shift;
        for (int i = startBorder; i < endBorder; i += mask + 1, block++)
            sum += blocks[block];

        for (int i = endBorder; i <= end; i++)
            sum += this[i];

        return sum;
    }

    public int FindGreater(int sum)
    {
        long accum = 0;
        int block = 0;
        for (; block < blocks.Length; block++) {
            int s = blocks[block];
            if (accum + s > sum) break;
            accum += s;
        }

        int index = Math.Min(array.Length, block << shift);
        for (; index < array.Length; index++) {
            accum += this[index];
            if (accum > sum) break;
        }

        return index;
    }
}