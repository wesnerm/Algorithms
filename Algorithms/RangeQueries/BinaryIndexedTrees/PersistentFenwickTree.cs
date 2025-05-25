#region Usings

using System.Runtime.CompilerServices;
using static System.Array;

#pragma warning disable CS0675

#endregion

public struct PersistentFenwickTree
{
    public PersistentFenwickTree(int n)
    {
        buckets = new Bucket[n];

        var empty = new Entry[1];
        for (int i = 0; i < n; i++) {
            buckets[i].Entries = empty;
            buckets[i].Position = 1;
        }
    }

    public Bucket[] buckets;

    public int Length => buckets.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int time, int i, int val)
    {
        if (val == 0) return;
        int len = buckets.Length;
        for (i++; i < len; i += i & -i) SetItem(time, i, val, true);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int SumInclusive(int time, int i)
    {
        int sum = 0;
        for (i++; i > 0; i -= i & -i) sum += GetItem(time, i);
        return sum;
    }

    public struct Bucket
    {
        public Entry[] Entries;
        public int Position;
    }

    public struct Entry
    {
        public int Time;
        public int Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetItem(int time, int pos)
    {
        int p1 = buckets[pos].Position - 1;
        Entry[] e = buckets[pos].Entries;
        return e[p1].Time <= time
            ? e[p1].Value
            : e[Bound(e, time + 1, 0, p1) - 1].Value;
    }

    public void SetItem(int time, int pos, int value, bool add = false)
    {
        int p = buckets[pos].Position;
        Entry[] e = buckets[pos].Entries;
        int lastTime = e[p - 1].Time;
        int ind = time >= lastTime
            ? p + (lastTime == time ? -1 : 0)
            : Bound(e, time, 0, p - 1);

        if (ind < p && e[ind].Time == time) {
            e[ind].Value = add ? value + e[ind].Value : value;
            return;
        }

        if (e.Length == p) {
            int newSize = Math.Max(4, e.Length * 2);
            Resize(ref e, newSize);
            buckets[pos].Entries = e;
        }

        if (ind < p)
            Copy(e, ind, e, ind + 1, p - ind);

        e[ind].Time = time;
        e[ind].Value = add ? value + e[ind - 1].Value : value;
        buckets[pos].Position = p + 1;
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