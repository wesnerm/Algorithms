namespace Algorithms;

class ArrayProduct
{
    static long count(int[] numbers, int k)
    {
        long[] prefix = new long[numbers.Length + 1];
        prefix[0] = 1;
        for (int i = 1; i < prefix.Length; i++)
            prefix[i] = prefix[i - 1] * numbers[i - 1];

        long[] buffer = new long[prefix.Length];
        return countDAC(prefix, buffer, k, 1, prefix.Length - 1);
    }

    // nlg n with divide and conquer
    static long countDAC(long[] prefix, long[] buffer, int k, int left, int right)
    {
        if (left > right) return 0;
        if (left == right) return prefix[left] < k ? 1 : 0;

        int mid = left + (right - left) / 2; // avoid overflow
        long leftCount = countDAC(prefix, buffer, k, left, mid);
        long rightCount = countDAC(prefix, buffer, k, mid + 1, right);

        // Count all the prefixes
        int midCount = 0;
        for (int i = left, r = mid + 1; i <= mid; i++) {
            // make it work for negatives
            while (r <= right && prefix[r] / prefix[i] < k) r++;
            midCount += r - mid + 1;
        }

        // Merge the prefix ranges together
        int j = mid + 1;
        for (int i = left, t = left; t <= right; t++)
            if (i <= mid && (j > right || prefix[i] < prefix[j]))
                buffer[t] = prefix[i++];
            else
                buffer[t] = prefix[j++];

        Array.Copy(buffer, left, prefix, left, right - left + 1);
        return leftCount + rightCount + midCount;
    }
}