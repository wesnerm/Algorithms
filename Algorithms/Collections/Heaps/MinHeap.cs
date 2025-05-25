namespace Algorithms.Collections;

public class MinHeap<T>
{
    readonly Comparison<T> comparison;
    T[] list;

    public MinHeap(Comparison<T> comparison = null, int n = 8)
    {
        list = new T[n];
        this.comparison = comparison ?? Comparer<T>.Default.Compare;
    }

    public int Count { get; private set; }

    public void Clear(bool nullify = false)
    {
        if (nullify) Array.Clear(list, 0, Count);
        Count = 0;
    }

    public T FindMin() => list[0];

    public T Dequeue()
    {
        T pop = list[0];
        T elem = list[--Count];
        list[Count] = default;
        if (Count > 0) ReplaceTop(elem);
        return pop;
    }

    public void ReplaceTop(T elem, int index = 0)
    {
        while (true) {
            int child = 2 * index + 1;
            if (child >= Count) break;

            if (child + 1 < Count && comparison(list[child], list[child + 1]) > 0)
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
        int i = Count;
        Place(push);
        while (i > 0) {
            int parent = (i - 1) >> 1;
            T value = list[parent];
            if (comparison(value, push) <= 0) break;
            list[i] = value;
            i = parent;
        }

        list[i] = push;
    }

    public void Place(T push)
    {
        if (Count >= list.Length) Array.Resize(ref list, Math.Max(list.Length * 2, Count + 1));
        list[Count++] = push;
    }

    public void Heapify()
    {
        for (int i = (Count - 1) >> 1; i >= 0; i--)
            ReplaceTop(list[i], i);
    }
}