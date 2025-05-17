#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Collections;

public partial class IndexedMinHeap<T> 
{
    #region Construction

    public IndexedMinHeap()
    {
        _comparer = Comparer<T>.Default;
    }

    #endregion

    #region Variables

    private readonly IComparer<T> _comparer;
    private readonly List<T> _list = new List<T>();

    #endregion

    #region Properties

    public bool ReverseSort { get; set; }

    public int Limit { get; set; } = int.MaxValue;

    public bool IsEmpty => _list.Count == 0;

    public int Count => _list.Count;

    #endregion

    #region Methods

    public void Clear()
    {
        foreach (var value in _list)
        {
            OnClear(value);
        }
        _list.Clear();
    }

    public bool Remove(T value)
    {
        var index = GetIndex(value);
        if (index < 0)
            return false;
        RemoveAt(index);
#if CHECK
        Debug.Assert(GetIndex(value) == -1);
#endif
        return true;
    }

    public T Top()
    {
        if (_list.Count == 0)
            throw new InvalidOperationException();
        return _list[0];
    }

    public void AddRange(IEnumerable<T> range)
    {
        foreach (var obj in range)
            Enqueue(obj);
    }

    public void AddRange(params T[] range)
    {
        AddRange((IEnumerable<T>) range);
    }

    public bool Requeue(T value)
    {
//            bool type = Remove(value);
//            Enqueue(value);
//            return type;

        var index = GetIndex(value);
        if (index >= 0)
        {
            RequeueAt(index);
            return true;
        }

        Enqueue(value);
        return false;
    }

    private int GetIndex(T obj)
    {
        var index = OnGetIndex(obj);
        Debug.Assert(index == -1 || _list[index].Equals(obj));
        return index;
    }

    protected virtual int OnGetIndex(T obj)
    {
        // The default implementation is for unit-testing
        // It's O(n). Override this with an O(1) implementation
        return _list.IndexOf(obj);
    }

    public virtual void Enqueue(T value)
    {
#if CHECK
        Debug.Assert(!Contains(value));
#endif

        if (value == null)
            throw new ArgumentNullException();

        _list.Add(default(T));
        var i = _list.Count - 1;
        while (i > 0)
        {
            var parent = (i - 1) >> 1;
            if (Compare(value, _list[parent]) >= 0)
                break;
            _list[i] = _list[parent];
            OnSetIndex(_list[i], i);
            i = parent;
        }
        _list[i] = value;
        OnSetIndex(_list[i], i);

        if (_list.Count > Limit)
            Dequeue();
    }

    private void RemoveAt(int index)
    {
        var i = index;
        var value = _list[i];
        while (i > 0)
        {
            var parent = (i - 1) >> 1;
            _list[i] = _list[parent];
            OnSetIndex(_list[i], i);
            i = parent;
        }
        Debug.Assert(i == 0);
        _list[i] = value;
        OnSetIndex(_list[i], i);
        Dequeue();
    }

    private void RequeueAt(int index)
    {
        // Move Upward
        var i = index;
        var value = _list[i];
        while (i > 0)
        {
            var parent = (i - 1) >> 1;
            var parentItem = _list[parent];
            if (Compare(parentItem, value) <= 0)
                break;
            _list[i] = parentItem;
            OnSetIndex(parentItem, i);
            i = parent;
        }

        if (i == index)
        {
            // Move Downward
            var count = _list.Count;
            while (true)
            {
                var child = 2*i + 1;
                if (child >= count)
                    break;

                var childItem = _list[child];
                if (child + 1 < count && Compare(childItem, _list[child + 1]) > 0)
                    childItem = _list[++child];

                if (Compare(value, childItem) <= 0)
                    break;
                _list[i] = childItem;
                OnSetIndex(childItem, i);
                i = child;
            }

            if (i == index)
                return;
        }

        _list[i] = value;
        OnSetIndex(value, i);
    }

    public virtual T Dequeue()
    {
        if (_list.Count <= 0)
            throw new InvalidOperationException();

        var count = _list.Count - 1;

        var result = _list[0];
        var last = _list[count];
        _list.RemoveAt(count);
        if (count > 0)
        {
            // Fix up tree
            var i = 0;
            while (true)
            {
                var child = 2*i + 1;
                if (child >= count)
                    break;

                if (child + 1 < count && Compare(_list[child], _list[child + 1]) > 0)
                    child++;

                if (Compare(last, _list[child]) <= 0)
                    break;

                _list[i] = _list[child];
                OnSetIndex(_list[i], i);
                i = child;
            }
            _list[i] = last;
            OnSetIndex(_list[i], i);
        }

        OnSetIndex(result, -1);
#if CHECK
        Debug.Assert(IndexOf(result) == -1);
#endif
        return result;
    }

    protected virtual void OnClear(T value)
    {
        OnSetIndex(value, -1);
    }

    private int Compare(T obj1, T obj2)
    {
        var cmp = OnCompare(obj1, obj2);
        return ReverseSort ? -cmp : cmp;
    }

    protected virtual int OnCompare(T obj1, T obj2)
    {
        return _comparer.Compare(obj1, obj2);
    }

    protected virtual void OnSetIndex(T obj, int index)
    {
        // Override this method
        Debug.Assert(obj != null);
    }
    #endregion
}