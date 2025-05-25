#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Collections;

public class MinPriorityQueue
{
    readonly Entry[] entries;
    readonly int[] handles;

    public int Unused = int.MinValue;

    public MinPriorityQueue(int n)
    {
        handles = new int[n];
        entries = new Entry[n];
        Count = 0;
    }

    public bool IsEmpty => Count == 0;

    public int Count { get; private set; }

    public int this[int handle] {
        get
        {
            int index = GetIndex(handle);
            return index >= 0 ? entries[index].value : Unused;
        }
        set
        {
            int index = GetIndex(handle);
            if (index < 0)
                Enqueue(handle, value);
            else
                Requeue(handle, value);
        }
    }

    public void Clear()
    {
        while (Count > 0)
            handles[entries[Count--].handle] = 0;
    }

    int GetIndex(int handle) => handles[handle] - 1;

    void SetIndex(int handle, int index)
    {
        handles[handle] = index + 1;
    }

    public int Top() => Count > 0 ? entries[0].handle : -1;

    public bool Requeue(int handle, int value)
    {
        //            bool type = Remove(value);
        //            Enqueue(value);
        //            return type;

        int index = GetIndex(handle);
        if (index >= 0) {
            RequeueAt(index);
            return true;
        }

        Enqueue(handle, value);
        return false;
    }

    public void Enqueue(int handle, int value)
    {
        int i = Count;
        while (i > 0) {
            int parent = (i - 1) >> 1;
            if (value >= entries[parent].value)
                break;
            entries[i] = entries[parent];
            SetIndex(entries[i].handle, i);
            i = parent;
        }

        entries[i] = new Entry { handle = handle, value = value };
        SetIndex(handle, i);
    }

    public bool Remove(int handle)
    {
        int index = GetIndex(handle);
        if (index < 0) return false;

        int i = index;
        Entry value = entries[i];
        while (i > 0) {
            int parent = (i - 1) >> 1;
            entries[i] = entries[parent];
            SetIndex(entries[i].handle, i);
            i = parent;
        }

        Debug.Assert(i == 0);
        entries[i] = value;
        SetIndex(entries[i].handle, i);
        Dequeue();
        return true;
    }

    void RequeueAt(int index)
    {
        // Move Upward
        int i = index;
        Entry entry = entries[i];
        while (i > 0) {
            int parent = (i - 1) >> 1;
            Entry parentItem = entries[parent];
            if (parentItem.value <= entry.value)
                break;
            entries[i] = parentItem;
            SetIndex(parentItem.handle, i);
            i = parent;
        }

        if (i == index) {
            // Move Downward
            while (true) {
                int child = 2 * i + 1;
                if (child >= Count)
                    break;

                Entry childItem = entries[child];
                if (child + 1 < Count && childItem.value > entries[child + 1].value)
                    childItem = entries[++child];

                if (entry.value <= childItem.value)
                    break;
                entries[i] = childItem;
                SetIndex(childItem.handle, i);
                i = child;
            }

            if (i == index)
                return;
        }

        entries[i] = entry;
        SetIndex(entry.handle, i);
    }

    public int Dequeue()
    {
        if (Count <= 0)
            throw new InvalidOperationException();

        --Count;
        Entry result = entries[0];
        Entry last = entries[Count];
        if (Count > 0) {
            // Fix up tree
            int i = 0;
            while (true) {
                int child = 2 * i + 1;
                if (child >= Count)
                    break;

                if (child + 1 < Count && entries[child].value > entries[child + 1].value)
                    child++;

                if (last.value <= entries[child].value)
                    break;

                entries[i] = entries[child];
                SetIndex(entries[i].handle, i);
                i = child;
            }

            entries[i] = last;
            SetIndex(entries[i].handle, i);
        }

        SetIndex(result.handle, -1);
        return result.handle;
    }

    struct Entry
    {
        public int value;
        public int handle;
    }
}