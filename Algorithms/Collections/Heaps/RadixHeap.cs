using System.Runtime.CompilerServices;

namespace Algorithms.Collections;

public class RadixHeap
{
    Comparison<long> comparison;
    List<long> same;
    List<long>[] lists;
    long lastDeleted;
    long mask;
    int count;

    public RadixHeap(Comparison<long> comparison = null, int n = 32)
    {
        same = new List<long>();
        this.lists = new List<long>[n];
        for (int i = 0; i < n; i++)
            lists[i] = new List<long>();
        this.comparison = comparison ?? Comparer<long>.Default.Compare;
    }

    public int Count => count;

    public void Clear()
    {
        foreach (var v in lists)
            v.Clear();
        count = 0;
        lastDeleted = long.MinValue;
    }

    public long FindMin()
    {
        if (count < 1) return long.MaxValue;
        if (same.Count > 0)
            return same[0];
        int i = Log2(unchecked(mask & -mask));
        return lists[i][0];
    }

    public long Dequeue()
    {
        if (count == 0)
            return long.MaxValue;

        count--;

        if (same.Count > 0)
        {
            int i = same.Count - 1;
            lastDeleted = same[i];
            same.RemoveAt(i);
            return lastDeleted;
        }

        unchecked
        {
            int index = Log2(mask & -(long)mask);
            var list = lists[index];
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
        count++;
        Place(push);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Place(long value)
    {
        var log = Log2(value ^ lastDeleted);
        if (log >= 0)
        {
            var list = lists[log];
            if (list.Count > 0 && list[0] > value)
            {
                var tmp = list[0];
                list[0] = value;
                value = tmp;
            }
            lists[log].Add(value);
            mask |= 1L << log;
        }
        else
        {
            same.Add(value);
        }
    }
}