namespace Algorithms.Strings.Sorting;

public class Inversions
{
    public static int SortAndCountInversions(int[] arr, int array_size)
    {
        int[] temp = new int[array_size];
        return SaciMerge(arr, temp, 0, array_size - 1);
    }

    static int SaciMerge(int[] arr, int[] temp, int left, int right)
    {
        if (right <= left) return 0;

        int mid = (right + left) >> 1;
        int invCount =
            SaciMerge(arr, temp, left, mid)
            + SaciMerge(arr, temp, mid + 1, right);

        int i = left, j = mid + 1, k = left;
        while (i <= mid && j <= right)
            if (arr[i] <= arr[j]) {
                temp[k++] = arr[i++];
            } else {
                temp[k++] = arr[j++];
                invCount += mid - i + 1;
            }

        while (i <= mid) temp[k++] = arr[i++];
        while (j <= right) temp[k++] = arr[j++];
        Array.Copy(temp, left, arr, left, right - left + 1);
        return invCount;
    }

    // array from [0..n)
    static long ComputeInversions(int[] array, int[] buffer)
    {
        int N = array.Length;
        Array.Clear(buffer, 0, N + 1);

        long result = 0;
        for (int n = 0; n < N; n++) {
            for (int i = array[n] + 1; i <= N; i += i & -i)
                result += buffer[i];
            for (int i = array[n] + 1; i > 0; i &= i - 1)
                buffer[i] += 1;
        }

        return result;
    }
}