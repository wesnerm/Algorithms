namespace Algorithms.Collections;

public class MinHeap4<T>
{
    const int Shift = 2;
    readonly Comparison<T> comparison;
    T[] list;

    public MinHeap4(Comparison<T> comparison = null, int n = 8)
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
            int bestIndex = index;
            T bestValue = elem;

            for (int i = (index << Shift) + 1, end = Math.Min(i + 4, Count); i < end; i++) {
                T value = list[i];
                if (comparison(value, bestValue) < 0) {
                    bestIndex = i;
                    bestValue = value;
                }
            }

            if (bestIndex == index)
                break;

            list[index] = bestValue;
            index = bestIndex;
        }

        list[index] = elem;
    }

    public void Enqueue(T push)
    {
        int i = Count;
        Place(push);
        while (i > 0) {
            int parent = (i - 1) >> Shift;
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
        for (int i = (Count - 1) >> Shift; i >= 0; i--)
            ReplaceTop(list[i], i);
    }
}