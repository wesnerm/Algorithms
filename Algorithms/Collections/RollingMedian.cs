
using Algorithms.Collections;
// ReSharper disable InconsistentNaming

namespace Algorithms.Misc;

public class RollingMedian
{
    private MinHeap<double> maxheap = new MinHeap<double>();
    private MinHeap<double> minheap = new MinHeap<double>();
    private int counter;

    public void Add(double d)
    {
        maxheap.Enqueue(d);
        minheap.Enqueue(-maxheap.Dequeue());
        while (minheap.Count > maxheap.Count)
            maxheap.Enqueue(-minheap.Dequeue());
    }

    public void Remove(double d)
    {
        
    }

    void Adjust()
    {
        
    }

}
