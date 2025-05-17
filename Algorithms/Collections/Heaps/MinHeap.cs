namespace Algorithms.Collections;

public class MinHeap<T>
{
    Comparison<T> comparison;
    T[] list;
    int count;

    public MinHeap(Comparison<T> comparison = null, int n = 8)
    {
        this.list = new T[n];
        this.comparison = comparison ?? Comparer<T>.Default.Compare;
    }

    public int Count => count;

    public void Clear(bool nullify = false)
    {
        if (nullify) Array.Clear(list, 0, count);
        count = 0;
    }

    public T FindMin() => list[0];

    public T Dequeue()
    {
        var pop = list[0];
        var elem = list[--count];
        list[count] = default(T);
        if (count > 0) ReplaceTop(elem);
        return pop;
    }

    public void ReplaceTop(T elem, int index = 0)
    {
        while (true)
        {
            int child = 2 * index + 1;
            if (child >= count) break;

            if (child + 1 < count && comparison(list[child], list[child + 1]) > 0)
                child++;

            if (comparison(list[child], elem) >= 0)
                break;

            list[index] = list[child];
            index = child;
        }
        list[index] = elem;
    }

    public void Enqueue(T push)
    {
        int i = count;
        Place(push);
        while (i > 0)
        {
            int parent = (i - 1) >> 1;
            var value = list[parent];
            if (comparison(value, push) <= 0) break;
            list[i] = value;
            i = parent;
        }
        list[i] = push;
    }

    public void Place(T push)
    {
        if (count >= list.Length) Array.Resize(ref list, Math.Max(list.Length*2, count+1)); 
        list[count++] = push;
    }

    public void Heapify()
    {
        for (int i = count - 1 >> 1; i >= 0; i--)
            ReplaceTop(list[i], i);
    }
}
