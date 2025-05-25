using System.Runtime.CompilerServices;

namespace Algorithms.Collections;

public class MinMaxHeap<T>
{
    readonly Comparison<T> comparison;
    T[] list;

    public MinMaxHeap(Comparison<T> comparison = null, int n = 8)
    {
        list = new T[n];
        this.comparison = comparison ?? Comparer<T>.Default.Compare;
    }

    public int Count { get; private set; }

    public T Min => Count > 0 ? list[0] : default;

    public T Max => list[GetMaxIndex()];

    public void Clear(bool nullify = false)
    {
        if (nullify) Array.Clear(list, 0, Count);
        Count = 0;
    }

    int GetMaxIndex()
    {
        switch (Count) {
            default:
                return comparison(list[1], list[2]) >= 0 ? 1 : 2;
            case 0:
            case 1:
                return 0;
            case 2:
                return 1;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T DequeueMin() => Dequeue(0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T DequeueMax() => Dequeue(GetMaxIndex());

    T Dequeue(int index)
    {
        T elem = list[Count - 1];
        T pop = list[index];
        list[--Count] = default;
        if (Count > 0) ReplaceTop(elem, index);
        return pop;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int SignOfIndex(int index) => ((index + 1) & 0x55555555) >= ((index + 1) & 0xaaaaaaaa) ? 1 : -1;

    public void ReplaceTop(T elem, int index = 0)
    {
        int sign = SignOfIndex(index);
        int currentIndex = index;
        int bestIndex = currentIndex;
        T bestValue = elem;
        T value;

        // Examine grandchildren
        while (true) {
            int i = 4 * currentIndex + 3;
            int end = i + 4;

            if (end > Count) {
                if (i >= Count) {
                    i = 2 * currentIndex + 1;
                    end = i + 2;
                } else if (Count - i <= 2 && comparison(list[2 * currentIndex + 2], bestValue) * sign < 0) {
                    bestIndex = 2 * currentIndex + 2;
                    bestValue = list[2 * currentIndex + 2];
                }

                end = Math.Min(end, Count);
            }

            for (; i < end; i++) {
                value = list[i];
                if (comparison(value, bestValue) * sign < 0) {
                    bestIndex = i;
                    bestValue = value;
                }
            }

            if (bestIndex == currentIndex)
                break;

            list[currentIndex] = bestValue;
            currentIndex = bestIndex;
        }

        // Check parent
        int parent = (currentIndex - 1) >> 1;
        if (parent > index) {
            value = list[parent];
            if (comparison(value, elem) * sign > 0) {
                list[currentIndex] = value;
                currentIndex = parent;
            }
        }

        list[currentIndex] = elem;
    }

    public void Enqueue(T elem)
    {
        int sign = SignOfIndex(Count);
        Place(elem);

        int i = Count;
        if (i == 0) return;

        int parent = (i - 1) >> 1;
        T value = list[parent];
        if (comparison(value, elem) * sign < 0) {
            sign = -sign;
            list[i] = value;
            i = parent;
        }

        while (i >= 3) {
            parent = (i - 3) >> 2;
            value = list[parent];
            if (comparison(value, elem) * sign <= 0) break;
            list[i] = value;
            i = parent;
        }

        list[i] = elem;
    }

    public void Place(T push)
    {
        if (Count >= list.Length) Array.Resize(ref list, Math.Max(list.Length * 2, Count + 1));
        list[Count++] = push;
    }

    public void Heapify()
    {
        for (int i = (Count - 1) >> 2; i >= 0; i--)
            ReplaceTop(list[i], i);
    }
}