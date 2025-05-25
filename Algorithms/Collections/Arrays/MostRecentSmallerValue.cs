namespace Algorithms.Collections.Arrays;

public static class MostRecentSmallerValue
{
    public static int[] PreviousValue(int[] a, Func<int, int, bool> check)
    {
        int[] stack = new int[a.Length];
        int[] result = new int[a.Length];
        int pos = 0;
        for (int i = 0; i < a.Length; i++) {
            int v = a[i];
            while (pos > 0 && !check(a[stack[pos - 1]], v)) pos--;
            result[i] = pos > 0 ? stack[pos - 1] : -1;
            stack[pos++] = i;
        }

        return result;
    }

    public static int[] NextValue(int[] a, Func<int, int, bool> check,
        int defaultValue = -1)
    {
        int[] stack = new int[a.Length];
        int[] result = new int[a.Length];
        int pos = 0;
        for (int i = a.Length - 1; i >= 0; i--) {
            int v = a[i];
            while (pos > 0 && !check(a[stack[pos - 1]], v)) pos--;
            result[i] = pos > 0 ? stack[pos - 1] : defaultValue;
            stack[pos++] = i;
        }

        return result;
    }

    public static int[] PreviousSmallerValue(int[] a)
    {
        int[] stack = new int[a.Length];
        int[] result = new int[a.Length];
        int pos = 0;
        for (int i = 0; i < a.Length; i++) {
            int v = a[i];
            while (pos > 0 && a[stack[pos - 1]] >= v) pos--;
            result[i] = pos > 0 ? stack[pos - 1] : -1;
            stack[pos++] = i;
        }

        return result;
    }

    public static int[] NextSmallerValue(int[] a, int defaultValue = -1)
    {
        int[] stack = new int[a.Length];
        int[] result = new int[a.Length];
        int pos = 0;
        for (int i = a.Length - 1; i >= 0; i--) {
            int v = a[i];
            while (pos > 0 && a[stack[pos - 1]] >= v) pos--;
            result[i] = pos > 0 ? stack[pos - 1] : defaultValue;
            stack[pos++] = i;
        }

        return result;
    }

    public static int[] PreviousLargerValue(int[] a)
    {
        int[] stack = new int[a.Length];
        int[] result = new int[a.Length];
        int pos = 0;
        for (int i = 0; i < a.Length; i++) {
            int v = a[i];
            while (pos > 0 && a[stack[pos - 1]] <= v) pos--;
            result[i] = pos > 0 ? stack[pos - 1] : -1;
            stack[pos++] = i;
        }

        return result;
    }

    public static int[] NextLargerValue(int[] a, int defaultValue = -1)
    {
        int[] stack = new int[a.Length];
        int[] result = new int[a.Length];
        int pos = 0;
        for (int i = a.Length - 1; i >= 0; i--) {
            int v = a[i];
            while (pos > 0 && a[stack[pos - 1]] <= v) pos--;
            result[i] = pos > 0 ? stack[pos - 1] : defaultValue;
            stack[pos++] = i;
        }

        return result;
    }

    public static int[] NextSameValue(int[] array, int valueSize)
    {
        int n = array.Length;
        int[] found = new int[valueSize + 1];
        for (int i = 0; i < found.Length; i++)
            found[i] = n;

        int[] next = new int[array.Length];
        for (int i = n - 1; i >= 0; i--) {
            next[i] = found[array[i]];
            found[array[i]] = i;
        }

        return next;
    }

    public static int[] PreviousSameValue(int[] array, int valueSize)
    {
        int n = array.Length;
        int[] found = new int[valueSize + 1];
        for (int i = 0; i < found.Length; i++)
            found[i] = -1;

        int[] prev = new int[array.Length];
        for (int i = 0; i < n; i++) {
            prev[i] = found[array[i]];
            found[array[i]] = i;
        }

        return prev;
    }
}