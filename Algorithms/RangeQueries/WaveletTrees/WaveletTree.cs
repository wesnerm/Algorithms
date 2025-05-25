namespace Algorithms.RangeQueries;

public class WaveletTree
{
    public readonly int[] f;
    internal readonly WaveletTree left, right;
    internal readonly int lo, hi;

    public WaveletTree(int[] array)
        : this((int[])array.Clone(), 0, array.Length - 1, array.Min(), array.Max()) { }

    // array indices are [start, end]; values in [low, high]
    WaveletTree(int[] array, int start, int end, int low, int high)
    {
        lo = low;
        hi = high;
        if (lo == hi || start > end)
            return;

        int len = end - start + 1;
        f = new int[len + 1];

        while (lo != hi) {
            int mid = (lo + hi) >> 1;
            int v = 0;
            for (int i = 0; i < len; i++) {
                if (array[start + i] <= mid) v++;
                f[i + 1] = v;
            }

            if (v == 0) {
                lo = mid + 1;
                continue;
            }

            if (v == len) {
                hi = mid;
                continue;
            }

            int pivot = Partition(array, start, end, mid);
            left = new WaveletTree(array, start, pivot - 1, lo, mid);
            right = new WaveletTree(array, pivot, end, mid + 1, hi);
            return;
        }
    }

    public static unsafe int Partition(int[] nums, int start, int end, int value)
    {
        while (start <= end && nums[start] <= value) start++;
        while (start <= end && nums[end] > value) end--;

        int* save = stackalloc int[end - start + 1];
        int len = 0, w = start;

        for (int r = start; r <= end; r++)
            if (nums[r] <= value)
                nums[w++] = nums[r];
            else
                save[len++] = nums[r];

        for (len = 0; w <= end; w++, len++)
            nums[w] = save[len];
        return end - len + 1;
    }

    // Kth smallest value in [l, r]
    public int Kth(int l, int r, int k)
    {
        if (l > r)
            return 0;

        if (lo == hi)
            return lo;

        int lf = f[l];
        int rf = f[r + 1];
        int count = rf - lf;
        return k <= count ? left.Kth(lf, rf - 1, k) : right.Kth(l - lf, r - rf, k - count);
    }

    // Count of numbers <= K in [l, r]
    public int CountLessEqual(int l, int r, int x)
    {
        if (l > r || x < lo)
            return 0;

        if (hi <= x)
            return r - l + 1;

        int lf = f[l];
        int rf = f[r + 1];
        return left.CountLessEqual(lf, rf - 1, x) + right.CountLessEqual(l - lf, r - rf, x);
    }

    // Count of K in [l, r]
    public int Count(int l, int r, int x)
    {
        if (l > r || x < lo || x > hi)
            return 0;

        if (lo == hi)
            return r - l + 1;

        int lf = f[l];
        int rf = f[r + 1];
        return x <= left.hi ? left.Count(lf, rf - 1, x) : right.Count(l - lf, r - rf, x);
    }
}