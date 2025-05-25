using static System.Math;

namespace Algorithms.RangeQueries.SegmentTrees;

public struct MinSegmentArray
{
    #region Constructor

    readonly int[] values;

    public MinSegmentArray(int size) => values = new int[size * 2];

    public MinSegmentArray(int[] array)
        : this(array.Length)
    {
        int size = array.Length;
        for (int i = 0; i < size; i++)
            values[i + size] = array[i];

        Array.Copy(array, 0, values, size, array.Length);
        for (int i = size - 1; i > 0; i--)
            values[i] = Min(values[i << 1], values[(i << 1) | 1]);
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
                int v = Min(values[i], values[i ^ 1]);
                i >>= 1;
                if (v == values[i]) break;
                values[i] = v;
            }

            //for (int i = index + (mins.Length >> 1); mins[i] != value; i >>= 1)
            //{
            //    mins[i] = value;
            //    if (i <= 1) break;
            //    value = Min(value, mins[i ^ 1]);
            //}
        }
    }

    #endregion

    #region Methods

    public int GetMinInclusive(int left, int right)
    {
        int size = values.Length >> 1;
        left += size;
        right += size;

        int min = int.MaxValue;
        for (; left <= right; left >>= 1, right >>= 1) {
            if ((left & 1) == 1) min = Min(min, values[left++]);
            if ((right & 1) == 0) min = Min(min, values[right--]);
        }

        return min;
    }

    public override string ToString()
    {
        int[]? values = this.values;
        return string.Join(" ",
            Enumerable.Range(values.Length >> 1, values.Length >> 1).Select(x => values[x + (values.Length >> 1)]));
    }

    #endregion
}