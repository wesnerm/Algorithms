using System.Runtime.CompilerServices;

namespace Algorithms.Collections.Arrays;

public struct PartiallyPersistentArray<T>
{
    public struct Bucket
    {
        public Entry[] Entries;
        public int Position;
    }

    public struct Entry
    {
        public int Time;
        public T Value;
    }

    public Bucket[] buckets;

    public int Length => buckets.Length;

    public PartiallyPersistentArray(int n)
    {
        buckets = new Bucket[n];

        var empty = new Entry[1];
        for (int i = 0; i < n; i++) {
            buckets[i].Entries = empty;
            buckets[i].Position = 1;
        }
    }

    public T this[int time, int pos] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            int p1 = buckets[pos].Position - 1;
            Entry[] e = buckets[pos].Entries;
            int ind = e[p1].Time <= time ? p1 : Bound(e, time + 1, 0, p1) - 1;
            return e[ind].Value;
        }
        set
        {
            int p = buckets[pos].Position;
            Entry[] e = buckets[pos].Entries;
            int lastTime = e[p - 1].Time;
            int ind = time >= lastTime
                ? p + (lastTime == time ? -1 : 0)
                : Bound(e, time, 0, p - 1);

            if (ind < p && e[ind].Time == time) {
                e[ind].Value = value;
                return;
            }

            if (e.Length == p) {
                int newSize = Math.Max(4, e.Length * 2);
                Array.Resize(ref e, newSize);
                buckets[pos].Entries = e;
            }

            if (ind < p)
                Array.Copy(e, ind, e, ind + 1, p - ind);

            e[ind].Time = time;
            e[ind].Value = value;
            buckets[pos].Position = p + 1;
        }
    }

    static int Bound(Entry[] array, int time, int left, int right)
    {
        while (left <= right) {
            int mid = left + ((right - left) >> 1);
            if (time > array[mid].Time)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return left;
    }
}