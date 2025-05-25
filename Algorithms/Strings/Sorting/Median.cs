namespace Algorithms;

public class Median
{
    public double FindMedianSortedArrays2(int[] nums1, int[] nums2)
    {
        int len = nums1.Length + nums2.Length;
        int k = (len + 1) / 2;
        int v = FindKth(nums1, nums2, k);
        if (len % 2 == 1) return v;
        return (v + FindKth(nums1, nums2, k + 1)) * .5;
    }

    public int FindKth(int[] nums1, int[] nums2, int kth)
    {
        if (nums1.Length > nums2.Length)
            return FindKth(nums2, nums1, kth);

        int k = kth - 1;
        if (k < 0) return int.MinValue;
        if (k >= nums1.Length + nums2.Length) return int.MaxValue;

        int left = 0;
        int right = Math.Min(k, nums1.Length - 1);
        while (left <= right) {
            int mid1 = left + (right - left) / 2;
            int mid2 = k - mid1;
            int val2 = mid2 < nums2.Length ? nums2[mid2] : int.MaxValue;
            if (nums1[mid1] < val2)
                left = mid1 + 1;
            else
                right = mid1 - 1;
        }

        return Math.Max(right >= 0 ? nums1[right] : int.MinValue, k - left >= 0 ? nums2[k - left] : int.MinValue);
    }

    // Accepted: 325 ms 3%
    public int FindKth4(int[] a, int m, int[] b, int n, int k, int start1 = 0, int start2 = 0)
    {
        if (m < n) return FindKth4(b, n, a, m, k, start2, start1);
        if (n == 0) return a[start1 + k - 1];
        if (k == 1) return Math.Min(a[start1], b[start2]);

        int j = Math.Min(n, k / 2);
        int i = k - j;

        return a[start1 + i - 1] > b[start2 + j - 1]
            ? FindKth4(a, i, b, n - j, k - j, start1, start2 + j)
            : FindKth4(a, m - i, b, j, k - i, start1 + i, start2);
    }

    // Accepted: 244ms: 38.39%
    public int FindIterativeKth2(int[] nums1, int[] nums2, int k)
    {
        int len1 = nums1.Length;
        int len2 = nums2.Length;
        int start1 = 0;
        int start2 = 0;
        while (true) {
            if (start1 >= len1) return nums2[start2 + k - 1];
            if (start2 >= len2) return nums1[start1 + k - 1];
            if (k == 1)
                return Math.Min(nums1[start1], nums2[start2]);

            int h = k / 2;
            int index1 = start1 + h - 1;
            int index2 = start2 + h - 1;
            int mid1 = index1 < len1 ? nums1[index1] : int.MaxValue;
            int mid2 = index2 < len2 ? nums2[index2] : int.MaxValue;
            if (mid1 < mid2)
                start1 += h;
            else
                start2 += h;
            k -= h;
        }
    }

    // https://discuss.leetcode.com/topic/54889/solution-in-c-well-explained/2

    public double FindMedianSortedArrays(int[] nums1, int[] nums2)
    {
        // We want the smaller element on the left side
        // -- it reduces chances of out of bounds errors and also lowers running time
        if (nums1.Length > nums2.Length)
            return FindMedianSortedArrays(nums2, nums1);

        int k = (nums1.Length + nums2.Length - 1) / 2; // k is the index of the first median point
        int left = 0;
        int right = nums1.Length - 1;
        while (left <= right) {
            int mid1 = (left + right) / 2;
            int mid2 = k - mid1;
            if (nums1[mid1] < nums2[mid2])
                left = mid1 + 1;
            else
                right = mid1 - 1;
        }

        double median = Math.Max(right >= 0 ? nums1[right] : int.MinValue,
            k - left >= 0 ? nums2[k - left] : int.MinValue);
        if ((nums1.Length + nums2.Length) % 2 == 1) return median;

        double median2 = Math.Min(left < nums1.Length ? nums1[left] : int.MaxValue,
            k - right < nums2.Length ? nums2[k - right] : int.MaxValue);
        return (median + median2) / 2;
    }
}