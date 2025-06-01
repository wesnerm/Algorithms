using Algorithms.Collections;

// ReSharper disable InconsistentNaming

namespace Algorithms.Misc;

public class RollingMedian
{
    readonly MinHeap<double> maxheap = new();
    readonly MinHeap<double> minheap = new();
    int counter;

    public void Add(double d)
    {
        maxheap.Enqueue(d);
        minheap.Enqueue(-maxheap.Dequeue());
        while (minheap.Count > maxheap.Count)
            maxheap.Enqueue(-minheap.Dequeue());
    }

    //public void Remove(double d)
    //{
    //    if (d <= GetMedian())
    //    {
    //        maxheap.Remove(d);
    //        while (maxheap.Count < minheap.Count)
    //            maxheap.Enqueue(-minheap.Dequeue());
    //    }
    //    else
    //    {
    //        minheap.Remove(-d);
    //        while (minheap.Count > maxheap.Count)
    //            maxheap.Enqueue(-minheap.Dequeue());
    //    }
    //}

    public double GetMedian()
    {
        if (maxheap.Count == 0)
            throw new InvalidOperationException("No elements in the collection");
            
        if (maxheap.Count > minheap.Count)
            return maxheap.FindMin();
            
        return (maxheap.FindMin() - minheap.FindMin()) / 2.0;
    }

    void Adjust() { }
}