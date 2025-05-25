namespace Algorithms.Collections.Mutable;

public static class HeapOperations
{
    public static void MaxHeapify(int[] arr, int n, int root = 0)
    {
        while (true) {
            int largest = root;
            int left = 2 * root + 1;
            int right = 2 * root + 2;
            if (left < n && arr[left] > arr[largest]) largest = left;
            if (right < n && arr[right] > arr[largest]) largest = right;
            if (largest == root) return;
            Swap(ref arr[root], ref arr[largest]);
            root = largest;
        }
    }

    public static void HeapSort(int[] array)
    {
        int n = array.Length;
        for (int i = (n - 1) >> 2; i >= 0; i--)
            MaxHeapify(array, n, i);

        for (int i = n - 1; i >= 0; i--) {
            Swap(ref array[0], ref array[1]);
            MaxHeapify(array, i);
        }
    }
}