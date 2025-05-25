using System.Runtime.CompilerServices;

namespace Algorithms.Collections;

public class RadixHeap
{
    readonly List<long>[] lists;
    readonly List<long> same;
    Comparison<long> comparison;
    long lastDeleted;
    long mask;

    public RadixHeap(Comparison<long> comparison = null, int n = 32)
    {
        same = new List<long>();
        lists = new List<long>[n];
        for (int i = 0; i < n; i++)
            lists[i] = new List<long>();
        this.comparison = comparison ?? Comparer<long>.Default.Compare;
    }

    public int Count { get; private set; }

    public void Clear()
    {
        foreach (List<long> v in lists)
            v.Clear();
        Count = 0;
        lastDeleted = long.MinValue;
    }

    public long FindMin()
    {
        if (Count < 1) return long.MaxValue;
        if (same.Count > 0)
            return same[0];
        int i = Log2(unchecked(mask & -mask));
        return lists[i][0];
    }

    public long Dequeue()
    {
        if (Count == 0)
            return long.MaxValue;

        Count--;

        if (same.Count > 0) {
            int i = same.Count - 1;
            lastDeleted = same[i];
            same.RemoveAt(i);
            return lastDeleted;
        }

        unchecked {
            int index = Log2(mask & -mask);
            List<long> list = lists[index];
            lastDeleted = list[0];
            for (int j = list.Count - 1; j >= 1; j--)
                Place(list[j]);
            list.Clear();
            mask &= ~(1L << index);
        }

        return lastDeleted;
    }

    static unsafe int Log2(long value)
    {
        double f = (ulong)value + .5; // +.5 -> -1 for zero
        return (((int*)&f)[1] >> 20) - 1023;
    }

    public void Enqueue(long push)
    {
        Count++;
        Place(push);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Place(long value)
    {
        int log = Log2(value ^ lastDeleted);
        if (log >= 0) {
            List<long> list = lists[log];
            if (list.Count > 0 && list[0] > value) {
                long tmp = list[0];
                list[0] = value;
                value = tmp;
            }

            lists[log].Add(value);
            mask |= 1L << log;
        } else {
            same.Add(value);
        }
    }
}