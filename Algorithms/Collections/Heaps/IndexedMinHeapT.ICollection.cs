#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.Collections;

#endregion

namespace Algorithms.Collections;

[DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof (CollectionDebugView<>))]
[DebuggerStepThrough]
public partial class IndexedMinHeap<T> : ICollection<T>
{

    #region Object Overloads

    public override string ToString()
    {
        return _list.ToString();
    }

    #endregion

    #region Properties

    bool ICollection<T>.IsReadOnly => false;

    #endregion

    #region Enumeration

    public List<T>.Enumerator GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public List<T> ExtractSortedList()
    {
        var count = _list.Count;
        var result = new List<T>(count);
        while (count-- > 0)
            result.Add(Dequeue());
        return result;
    }

    #endregion

    #region Methods

    void ICollection<T>.Add(T value)
    {
        Enqueue(value);
    }

    public virtual bool Contains(T obj)
    {
        return GetIndex(obj) >= 0;
    }

    public void Check()
    {
        for (var i = 1; i < _list.Count; i++)
        {
            Debug.Assert(Compare(_list[(i - 1) >> 1], _list[i]) <= 0);
        }

        for (var i = 0; i < _list.Count; i++)
            Debug.Assert(GetIndex(_list[i]) == i);
    }

    #endregion

    #region ICollection<T> Members

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    #endregion
}