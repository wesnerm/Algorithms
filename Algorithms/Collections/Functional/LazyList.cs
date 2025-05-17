#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2005, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.Collections;

#endregion

namespace Algorithms.Collections;

public class LazyList<T> : IEnumerable<T>
{
    #region Properties

    public LazyList<T> Next
    {
        get
        {
            if (_enumerator != null)
            {
                _next = Create(_enumerator);
                _enumerator = null;
            }
            return _next;
        }
    }

    #endregion

    #region Variables

    public readonly T Value;
    private IEnumerator<T> _enumerator;
    private LazyList<T> _next;

    #endregion

    #region Constructor

    private LazyList(T value, IEnumerator<T> enumerator)
    {
        Value = value;
        _enumerator = enumerator;
    }

    public static LazyList<T> Create(IEnumerable<T> enumerable)
    {
        return Create(enumerable.GetEnumerator());
    }

    private static LazyList<T> Create(IEnumerator<T> enumerator)
    {
        if (enumerator.MoveNext())
            return new LazyList<T>(enumerator.Current, enumerator);
        return null;
    }

    #endregion

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator()
    {
        var current = this;
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}