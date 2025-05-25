using static System.Math;

namespace Algorithms.RangeQueries.SegmentTrees;

public struct MaxSegmentArray
{
    #region Constructor

    readonly int[] values;

    public MaxSegmentArray(int size) => values = new int[size * 2];

    public MaxSegmentArray(int[] array)
        : this(array.Length)
    {
        int size = array.Length;
        for (int i = 0; i < size; i++)
            values[i + size] = array[i];

        Array.Copy(array, 0, values, size, array.Length);
        for (int i = size - 1; i > 0; i--)
            values[i] = Max(values[i << 1], values[(i << 1) | 1]);
    }

    #endregion

    #region Properties

    public int this[int index] {
        get => values[index + (values.Length >> 1)];
        set
        {
            // For O(1) updates for min, max queries by short-circuiting
            int i = index + (values.Length >> 1);
            values[i] = value;
            while (i > 1) {
                int v = Max(values[i], values[i ^ 1]);
                i >>= 1;
                if (v == values[i]) break;
                values[i] = v;
            }

            //for (int i = index + (maxs.Length >> 1); maxs[i] != value; i >>= 1)
            //{
            //    maxs[i] = value;
            //    if (i <= 1) break;
            //    value = Max(value, maxs[i ^ 1]);
            //}
        }
    }

    #endregion

    #region Methods

    public int GetMaxInclusive(int left, int right)
    {
        int size = values.Length >> 1;
        left += size;
        right += size;

        int max = int.MinValue;
        for (; left <= right; left >>= 1, right >>= 1) {
            if ((left & 1) == 1) max = Max(max, values[left++]);
            if ((right & 1) == 0) max = Max(max, values[right--]);
        }

        return max;
    }

    public override string ToString()
    {
        int[]? values = this.values;
        return string.Join(" ",
            Enumerable.Range(values.Length >> 1, values.Length >> 1).Select(x => values[x + (values.Length >> 1)]));
    }

    #endregion
}