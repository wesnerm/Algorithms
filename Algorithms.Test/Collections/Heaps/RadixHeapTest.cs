

namespace Algorithms.Collections;

[TestFixture]
public class RadixHeapTest
{
    [Test]
    public void EnqueueTest()
    {
        var heap = new RadixHeap(n: 4);
        Assert.AreEqual(0, heap.Count);

        heap.Enqueue(1);
        Assert.AreEqual(1, heap.FindMin());
        Assert.AreEqual(1, heap.Count);

        heap.Enqueue(2);
        Assert.AreEqual(1, heap.FindMin());
        Assert.AreEqual(2, heap.Count);

        heap.Enqueue(3);
        Assert.AreEqual(1, heap.FindMin());
        Assert.AreEqual(3, heap.Count);

        heap.Enqueue(5);
        Assert.AreEqual(1, heap.FindMin());
        Assert.AreEqual(4, heap.Count);

        var pop = heap.Dequeue();
        Assert.AreEqual(1, pop);
        Assert.AreEqual(2, heap.FindMin());
        Assert.AreEqual(3, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(2, pop);
        Assert.AreEqual(3, heap.FindMin());
        Assert.AreEqual(2, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(3, pop);
        Assert.AreEqual(5, heap.FindMin());
        Assert.AreEqual(1, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(5, pop);
        Assert.AreEqual(long.MaxValue, heap.FindMin());
        Assert.AreEqual(0, heap.Count);
    }

    [Test]
    public void DequeueTest()
    {
        var heap = new RadixHeap(n: 4);
        Assert.AreEqual(0, heap.Count);

        heap.Enqueue(1);
        heap.Enqueue(2);
        heap.Enqueue(3);
        heap.Enqueue(5);

        var pop = heap.Dequeue();
        Assert.AreEqual(1, pop);
        Assert.AreEqual(2, heap.FindMin());
        Assert.AreEqual(3, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(2, pop);
        Assert.AreEqual(3, heap.FindMin());
        Assert.AreEqual(2, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(3, pop);
        Assert.AreEqual(5, heap.FindMin());
        Assert.AreEqual(1, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(5, pop);
        Assert.AreEqual(long.MaxValue, heap.FindMin());
        Assert.AreEqual(0, heap.Count);
    }

    public void SameTest()
    {
        var heap = new RadixHeap(n: 4);
        Assert.AreEqual(0, heap.Count);

        heap.Enqueue(1);
        heap.Enqueue(2);
        heap.Enqueue(2);
        heap.Enqueue(2);

        var pop = heap.Dequeue();
        Assert.AreEqual(1, pop);
        Assert.AreEqual(2, heap.FindMin());
        Assert.AreEqual(3, heap.Count);

        heap.Enqueue(3);
        heap.Enqueue(3);
        heap.Enqueue(3);

        pop = heap.Dequeue();
        Assert.AreEqual(2, pop);
        Assert.AreEqual(2, heap.FindMin());
        Assert.AreEqual(5, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(2, pop);
        Assert.AreEqual(2, heap.FindMin());
        Assert.AreEqual(4, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(2, pop);
        Assert.AreEqual(3, heap.FindMin());
        Assert.AreEqual(3, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(3, pop);
        Assert.AreEqual(3, heap.FindMin());
        Assert.AreEqual(2, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(3, pop);
        Assert.AreEqual(3, heap.FindMin());
        Assert.AreEqual(1, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(3, pop);
        Assert.AreEqual(long.MaxValue, heap.FindMin());
        Assert.AreEqual(0, heap.Count);

    }

    [Test]
    public void TestEnqueueAndDequeueTest()
    {
        var heap = new RadixHeap(n: 4);
        Assert.AreEqual(0, heap.Count);

        heap.Enqueue(1);
        heap.Enqueue(2);
        heap.Enqueue(3);
        heap.Enqueue(5);

        var pop = heap.Dequeue();
        Assert.AreEqual(1, pop);
        Assert.AreEqual(2, heap.FindMin());
        Assert.AreEqual(3, heap.Count);

        heap.Enqueue(6);
        heap.Enqueue(2);
        heap.Enqueue(8);

        pop = heap.Dequeue();
        Assert.AreEqual(2, pop);
        Assert.AreEqual(2, heap.FindMin());
        Assert.AreEqual(5, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(2, pop);
        Assert.AreEqual(3, heap.FindMin());
        Assert.AreEqual(4, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(3, pop);
        Assert.AreEqual(5, heap.FindMin());
        Assert.AreEqual(3, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(5, pop);
        Assert.AreEqual(6, heap.FindMin());
        Assert.AreEqual(2, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(6, pop);
        Assert.AreEqual(8, heap.FindMin());
        Assert.AreEqual(1, heap.Count);

        pop = heap.Dequeue();
        Assert.AreEqual(8, pop);
        Assert.AreEqual(long.MaxValue, heap.FindMin());
        Assert.AreEqual(0, heap.Count);

    }

}