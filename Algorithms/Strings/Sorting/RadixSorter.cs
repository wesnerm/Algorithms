namespace Algorithms;

public class RadixSorter
{
    public static void RadixSort(int[] list)
    {
        const long adjust = int.MaxValue + 1L;
        RadixSort(list,
            x => x + adjust,
            null,
            list.Max() + adjust);
    }

    public static void RadixSort<T>(T[] list,
        Func<T, long> func,
        T[] buffer = null,
        long maxValue = int.MaxValue)
    {
        const int shift0 = 8;
        const int buckets = 1 << shift0;
        const int mask = buckets - 1;
        int[] offsets = new int[buckets];

        if (buffer == null || buffer.Length < list.Length)
            buffer = new T[list.Length];
        T[] main = list;

        int shifts = 0;
        while (maxValue >> (shift0 * shifts) > 0)
            shifts++;

        for (int shift = (shifts - 1) * shift0; shift >= 0; shift -= shift0) {
            Array.Clear(offsets, 0, buckets + 1);

            for (int i = 0; i < main.Length; i++) {
                long radix = (func(list[i]) >> shift) & mask;
                offsets[radix]++;
            }

            int sum = 0;
            for (int i = 0; i < buckets; i++) {
                int newSum = sum + offsets[i];
                offsets[i] = sum;
                sum = newSum;
            }

            for (int i = 0; i < main.Length; i++) {
                T e = main[i];
                long radix = (func(e) >> shift) & mask;
                buffer[offsets[radix]++] = e;
            }

            T[] tmp = main;
            main = buffer;
            buffer = tmp;
        }

        if (main != list)
            Array.Copy(main, 0, list, 0, list.Length);
    }

    public void RadixSort(string[] array, int start, int count)
    {
        var srs = new StringRadixSort
        {
            _array = array,
            _buffer = new string[array.Length],
        };

        srs.Sort(start, start + count - 1, 0);
    }

    struct StringRadixSort
    {
        public string[] _array;
        public string[] _buffer;

        public unsafe void Sort(int left, int right, int index)
        {
            if (left >= right || index >= _array[left].Length)
                return;

            int buckets = 256;
            int* offsets = stackalloc int[buckets];
            int* pos = stackalloc int[buckets];

            while (left < right && index < _array[left].Length) {
                for (int i = 0; i < 10; i++)
                    offsets[i] = 0;

                for (int i = left; i <= right; i++)
                    offsets[_array[i][index] & 0xff]++;

                int sum = left;
                int maxrange = 0;
                int maxradix = 0;
                for (int i = 0; i < 10; i++) {
                    int range = offsets[i];
                    if (range >= maxrange) {
                        maxrange = range;
                        maxradix = i;
                    }

                    offsets[i] = sum;
                    pos[i] = sum;
                    sum += range;
                }

                for (int i = left; i <= right; i++) {
                    int radix = _array[i][index] - '0';
                    _buffer[pos[radix]++] = _array[i];
                }

                for (int i = left; i <= right; i++)
                    _array[i] = _buffer[i];

                index++;
                for (int i = 0; i < 10; i++)
                    if (i != maxradix)
                        Sort(offsets[i], pos[i] - 1, index);

                // This reduces recursion since index can go up to 10^6
                left = offsets[maxradix];
                right = pos[maxradix] - 1;
            }
        }
    }
}