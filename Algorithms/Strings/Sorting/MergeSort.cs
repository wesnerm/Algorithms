namespace Algorithms;

public class MergeSort<T>
{
    readonly Comparison<T> compare;
    T[] array, buffer;

    public MergeSort(Comparison<T> compare = null) => this.compare = compare ?? Comparer<T>.Default.Compare;

    public void SortExclusive(T[] array, int start = 0, int end = -1)
    {
        this.array = array;
        if (buffer == null)
            buffer = new T[array.Length];
        else if (buffer.Length < array.Length)
            buffer = new T[Math.Max(array.Length, buffer.Length * 2)];
        if (end < 0) end += array.Length + 1;
        SortExclusive(start, end);
    }

    void SortExclusive(int start, int end)
    {
        if (end - start <= 1) return;
        int mid = (start + end) >> 1;
        SortExclusive(start, mid);
        SortExclusive(mid, end);
        int p = 0, i = start, j = mid;
        T[] a = array;
        while (i < mid && j < end)
            buffer[p++] = compare(a[i], a[j]) < 0 ? a[i++] : a[j++];
        while (i < mid) buffer[p++] = a[i++];
        while (j < end) buffer[p++] = a[j++];
        Array.Copy(buffer, 0, a, start, end - start);
    }
}