namespace Algorithms.Collections;

[TestFixture]
public class RadixHeapTest
{
    [Test]
    public void EnqueueTest()
    {
        var heap = new RadixHeap(n: 4);
        AreEqual(0, heap.Count);

        heap.Enqueue(1);
        AreEqual(1, heap.FindMin());
        AreEqual(1, heap.Count);

        heap.Enqueue(2);
        AreEqual(1, heap.FindMin());
        AreEqual(2, heap.Count);

        heap.Enqueue(3);
        AreEqual(1, heap.FindMin());
        AreEqual(3, heap.Count);

        heap.Enqueue(5);
        AreEqual(1, heap.FindMin());
        AreEqual(4, heap.Count);

        long pop = heap.Dequeue();
        AreEqual(1, pop);
        AreEqual(2, heap.FindMin());
        AreEqual(3, heap.Count);

        pop = heap.Dequeue();
        AreEqual(2, pop);
        AreEqual(3, heap.FindMin());
        AreEqual(2, heap.Count);

        pop = heap.Dequeue();
        AreEqual(3, pop);
        AreEqual(5, heap.FindMin());
        AreEqual(1, heap.Count);

        pop = heap.Dequeue();
        AreEqual(5, pop);
        AreEqual(long.MaxValue, heap.FindMin());
        AreEqual(0, heap.Count);
    }

    [Test]
    public void DequeueTest()
    {
        var heap = new RadixHeap(n: 4);
        AreEqual(0, heap.Count);

        heap.Enqueue(1);
        heap.Enqueue(2);
        heap.Enqueue(3);
        heap.Enqueue(5);

        long pop = heap.Dequeue();
        AreEqual(1, pop);
        AreEqual(2, heap.FindMin());
        AreEqual(3, heap.Count);

        pop = heap.Dequeue();
        AreEqual(2, pop);
        AreEqual(3, heap.FindMin());
        AreEqual(2, heap.Count);

        pop = heap.Dequeue();
        AreEqual(3, pop);
        AreEqual(5, heap.FindMin());
        AreEqual(1, heap.Count);

        pop = heap.Dequeue();
        AreEqual(5, pop);
        AreEqual(long.MaxValue, heap.FindMin());
        AreEqual(0, heap.Count);
    }

    public void SameTest()
    {
        var heap = new RadixHeap(n: 4);
        AreEqual(0, heap.Count);

        heap.Enqueue(1);
        heap.Enqueue(2);
        heap.Enqueue(2);
        heap.Enqueue(2);

        long pop = heap.Dequeue();
        AreEqual(1, pop);
        AreEqual(2, heap.FindMin());
        AreEqual(3, heap.Count);

        heap.Enqueue(3);
        heap.Enqueue(3);
        heap.Enqueue(3);

        pop = heap.Dequeue();
        AreEqual(2, pop);
        AreEqual(2, heap.FindMin());
        AreEqual(5, heap.Count);

        pop = heap.Dequeue();
        AreEqual(2, pop);
        AreEqual(2, heap.FindMin());
        AreEqual(4, heap.Count);

        pop = heap.Dequeue();
        AreEqual(2, pop);
        AreEqual(3, heap.FindMin());
        AreEqual(3, heap.Count);

        pop = heap.Dequeue();
        AreEqual(3, pop);
        AreEqual(3, heap.FindMin());
        AreEqual(2, heap.Count);

        pop = heap.Dequeue();
        AreEqual(3, pop);
        AreEqual(3, heap.FindMin());
        AreEqual(1, heap.Count);

        pop = heap.Dequeue();
        AreEqual(3, pop);
        AreEqual(long.MaxValue, heap.FindMin());
        AreEqual(0, heap.Count);
    }

    [Test]
    public void TestEnqueueAndDequeueTest()
    {
        var heap = new RadixHeap(n: 4);
        AreEqual(0, heap.Count);

        heap.Enqueue(1);
        heap.Enqueue(2);
        heap.Enqueue(3);
        heap.Enqueue(5);

        long pop = heap.Dequeue();
        AreEqual(1, pop);
        AreEqual(2, heap.FindMin());
        AreEqual(3, heap.Count);

        heap.Enqueue(6);
        heap.Enqueue(2);
        heap.Enqueue(8);

        pop = heap.Dequeue();
        AreEqual(2, pop);
        AreEqual(2, heap.FindMin());
        AreEqual(5, heap.Count);

        pop = heap.Dequeue();
        AreEqual(2, pop);
        AreEqual(3, heap.FindMin());
        AreEqual(4, heap.Count);

        pop = heap.Dequeue();
        AreEqual(3, pop);
        AreEqual(5, heap.FindMin());
        AreEqual(3, heap.Count);

        pop = heap.Dequeue();
        AreEqual(5, pop);
        AreEqual(6, heap.FindMin());
        AreEqual(2, heap.Count);

        pop = heap.Dequeue();
        AreEqual(6, pop);
        AreEqual(8, heap.FindMin());
        AreEqual(1, heap.Count);

        pop = heap.Dequeue();
        AreEqual(8, pop);
        AreEqual(long.MaxValue, heap.FindMin());
        AreEqual(0, heap.Count);
    }
}