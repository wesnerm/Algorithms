using System.Collections;

namespace Algorithms.Collections;

public class DoubleEndedQueue<T> : IEnumerable<T>
{
    int _head;
    int _count;
    T [] _list;

    public DoubleEndedQueue()
    {
        _list = Array.Empty<T>();
    }

    public DoubleEndedQueue(int capacity)
    {
        _list = new T[capacity];
    }

    public int Count => _count;

    void CheckCapacity()
    {
        if (_count == _list.Length)
        {
            int newCount = Math.Max(4, _count * 2);

            var oldList = _list;
            _list = new T[newCount];

            for (int i = 0; i < _count; i++)
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
        int newTail = (_head + _count) % _list.Length;
        Debug.Assert(_list[newTail] == null);
        _list[newTail] = item;
        _count++;
        Validate();
    }

    public T PopLast()
    {
        if (_count == 0) throw new InvalidOperationException();
        _count--;
        var tail = (_head + _count) % _list.Length;
        T item = _list[tail];
        _list[tail] = default(T);
        Validate();
        return item;
    }

    public T Last()
    {
        if (_count == 0) throw new InvalidOperationException();
        return  _list[(_head + _count - 1) % _list.Length];
    }

    public void PushFirst(T item)
    {
        CheckCapacity();
        int newHead = (_head - 1 + _list.Length) % _list.Length;
        Debug.Assert(_list[newHead] == null);
        _list[newHead] = item;
        _head = newHead;
        _count++;
        Validate();
    }

    public T PopFirst()
    {
        if (_count == 0) throw new InvalidOperationException();
        T item = _list[_head];
        _list[_head] = default(T);
        _head = (_head + 1 + _list.Length) % _list.Length;
        _count--;
        Validate();
        return item;
    }

    public T First()
    {
        if (_count == 0) throw new InvalidOperationException();
        return _list[_head];
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i=0; i<_count; i++)
            yield return _list[(_head + _count) % _list.Length];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    void Validate()
    {
        if (Count == 0) return;
        Debug.Assert(First() != null);
        Debug.Assert(Last() != null);
    }
}
