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
    int[] handles;
    Entry[] entries;
    int count;

    struct Entry
    {
        public int value;
        public int handle;
    }

    public MinPriorityQueue(int n)
    {
        handles = new int[n];
        entries = new Entry[n];
        count = 0;
    }

    public bool IsEmpty => count == 0;

    public int Count => count;

    public int Unused = int.MinValue;

    public void Clear()
    {
        while (count > 0)
            handles[entries[count--].handle] = 0;
    }

    private int GetIndex(int handle) => handles[handle] - 1;
    private void SetIndex(int handle, int index) => handles[handle] = index + 1;

    public int this[int handle]
    {
        get
        {
            var index = GetIndex(handle);
            return index >= 0 ? entries[index].value : Unused;
        }
        set
        {
            var index = GetIndex(handle);
            if (index < 0)
                Enqueue(handle, value);
            else
                Requeue(handle, value);
        }
    }

    public int Top() => count > 0 ? entries[0].handle : -1;

    public bool Requeue(int handle, int value)
    {
        //            bool type = Remove(value);
        //            Enqueue(value);
        //            return type;

        var index = GetIndex(handle);
        if (index >= 0)
        {
            RequeueAt(index);
            return true;
        }

        Enqueue(handle, value);
        return false;
    }

    public void Enqueue(int handle, int value)
    {
        int i = count;
        while (i > 0)
        {
            var parent = (i - 1) >> 1;
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
        var index = GetIndex(handle);
        if (index < 0) return false;

        var i = index;
        var value = entries[i];
        while (i > 0)
        {
            var parent = (i - 1) >> 1;
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

    private void RequeueAt(int index)
    {
        // Move Upward
        var i = index;
        var entry = entries[i];
        while (i > 0)
        {
            var parent = (i - 1) >> 1;
            var parentItem = entries[parent];
            if (parentItem.value <= entry.value)
                break;
            entries[i] = parentItem;
            SetIndex(parentItem.handle, i);
            i = parent;
        }

        if (i == index)
        {
            // Move Downward
            while (true)
            {
                var child = 2 * i + 1;
                if (child >= count)
                    break;

                var childItem = entries[child];
                if (child + 1 < count && childItem.value > entries[child + 1].value)
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
        if (count <= 0)
            throw new InvalidOperationException();

        --count;
        var result = entries[0];
        var last = entries[count];
        if (count > 0)
        {
            // Fix up tree
            var i = 0;
            while (true)
            {
                var child = 2 * i + 1;
                if (child >= count)
                    break;

                if (child + 1 < count && entries[child].value > entries[child + 1].value)
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
}