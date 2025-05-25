namespace Algorithms.Collections;

public class DoubleEndedQueue<T> : IEnumerable<T>
{
    int _head;
    T[] _list;

    public DoubleEndedQueue() => _list = Array.Empty<T>();

    public DoubleEndedQueue(int capacity) => _list = new T[capacity];

    public int Count { get; private set; }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return _list[(_head + Count) % _list.Length];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void CheckCapacity()
    {
        if (Count == _list.Length) {
            int newCount = Math.Max(4, Count * 2);

            T[] oldList = _list;
            _list = new T[newCount];

            for (int i = 0; i < Count; i++)
                _list[i] = oldList[(_head + i) % oldList.Length];
            _head = 0;
        }
    }

    public void Add(T item)
    {
        PushLast(item);
    }

    public void PushLast(T item)
    {
        CheckCapacity();
        int newTail = (_head + Count) % _list.Length;
        Debug.Assert(_list[newTail] == null);
        _list[newTail] = item;
        Count++;
        Validate();
    }

    public T PopLast()
    {
        if (Count == 0) throw new InvalidOperationException();
        Count--;
        int tail = (_head + Count) % _list.Length;
        T item = _list[tail];
        _list[tail] = default;
        Validate();
        return item;
    }

    public T Last()
    {
        if (Count == 0) throw new InvalidOperationException();
        return _list[(_head + Count - 1) % _list.Length];
    }

    public void PushFirst(T item)
    {
        CheckCapacity();
        int newHead = (_head - 1 + _list.Length) % _list.Length;
        Debug.Assert(_list[newHead] == null);
        _list[newHead] = item;
        _head = newHead;
        Count++;
        Validate();
    }

    public T PopFirst()
    {
        if (Count == 0) throw new InvalidOperationException();
        T item = _list[_head];
        _list[_head] = default;
        _head = (_head + 1 + _list.Length) % _list.Length;
        Count--;
        Validate();
        return item;
    }

    public T First()
    {
        if (Count == 0) throw new InvalidOperationException();
        return _list[_head];
    }

    void Validate()
    {
        if (Count == 0) return;
        Debug.Assert(First() != null);
        Debug.Assert(Last() != null);
    }
}