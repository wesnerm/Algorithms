#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.Runtime.InteropServices;

#endregion

namespace Algorithms.Collections;

/// <summary>
///     WeakCollection.
/// </summary>
public class WeakCollection<T> : ICloneable, IList, IList<T>
    where T : class
{
    #region Constants

    const int DefaultSize = 8;

    #endregion

    #region ICloneable Members

    object ICloneable.Clone() => Clone();

    #endregion

    public WeakListEnumerator GetEnumerator() => new(this);

    #region Nested type: WeakListEnumerator

    public struct WeakListEnumerator : IEnumerator<T>
    {
        #region Variables

        readonly WeakCollection<T> _list;
        int _index;
        int _version;

        #endregion

        #region Construction

        public WeakListEnumerator(WeakCollection<T> list)
        {
            _list = list;
            _index = list._count;
            _version = list._version;
        }

        #endregion

        #region IEnumerator<T> Members

        public void Reset()
        {
            _index = _list._count;
            _version = _list._version;
        }

        public T Current {
            get
            {
                if (_version != _list._version)
                    throw new InvalidOperationException();
                return _list[_index];
            }
            set => _list[_index] = value;
        }

        object IEnumerator.Current => Current;

        public void Dispose() { }

        public bool MoveNext()
        {
            _index--;
            return _index >= 0;
        }

        #endregion

        public void RemoveCurrent()
        {
            _list[_index] = null;
        }
    }

    #endregion

    #region Variables

    GCHandle[] _array;
    int _count;
    GCHandle _handle;
    int _version;

    #endregion

    #region Construction

    public WeakCollection() : this(DefaultSize) { }

    public WeakCollection(int capacity)
    {
        _array = new GCHandle[capacity];
        _handle = GCHandle.Alloc(_array, GCHandleType.Normal);
    }

    public WeakCollection<T> Clone()
    {
        var newarray = new WeakCollection<T>(ActualCount);
        int count = Count;
        for (int i = 0; i < count; i++) {
            T? o = this[i];
            if (o != null)
                newarray.Add(o);
        }

        return newarray;
    }

    ~WeakCollection()
    {
        Zero(0, _array.Length);
        _handle.Free();
    }

    #endregion

    #region Overrides

    public override bool Equals(object obj)
    {
        var col = obj as WeakCollection<T>;
        if (col == null)
            return false;
        return Equals(col);
    }

    public bool Equals(WeakCollection<T> col)
    {
        int count = ActualCount;
        if (count != col.ActualCount)
            return false;

        for (int i = 0; i < count; i++)
            if (!this[i].Equals(col[i]))
                return false;

        return true;
    }

    public override int GetHashCode()
    {
        int hash = ActualCount;
        foreach (object o in this)
            hash ^= o.GetHashCode();
        return hash;
    }

    public override string ToString()
    {
        Compress();
        var sb = new StringBuilder();
        sb.Append('{');
        int count = ActualCount;
        for (int i = 0; i < count; i++) {
            if (i > 0) sb.Append(", ");
            sb.Append(this[i]);
        }

        sb.Append('}');
        return sb.ToString();
    }

    #endregion

    #region Properties

    public int Capacity => _array.Length;

    public int ActualCount {
        get
        {
            Compress();
            return _count;
        }
    }

    public int Count => _count;

    #endregion

    #region Indexing

    public T this[int index] {
        get
        {
            CheckIndex(index);
            GCHandle h = _array[index];
            object? result = h.IsAllocated ? h.Target : null;
            if (result != null)
                return (T)result;
            return default;
        }
        set
        {
            CheckIndex(index);

            if (value != null) {
                if (_array[index].IsAllocated)
                    _array[index].Target = value;
                else
                    _array[index] = GCHandle.Alloc(value, GCHandleType.Weak);
            } else if (_array[index].IsAllocated) {
                _array[index].Free();
            }
        }
    }

    public void CheckIndex(int index)
    {
        if (index < 0 || index > _count)
            throw new IndexOutOfRangeException();
    }

    public void CheckIndex(int start, int count)
    {
        if (start < 0)
            throw new ArgumentOutOfRangeException();
        if (count < 0 || start + count > _count)
            throw new ArgumentOutOfRangeException();
    }

    #endregion

    #region Array Functions

    public void Insert(int index, T value)
    {
        CheckIndex(index);
        if (_count == _array.Length)
            Compress(ref index);

        ArrayTools.InsertRange(ref _array, ref _count, index, 1);
        _handle.Target = _array;
        this[index] = value;
        _version++;
    }

    public void SetNull(int start, int length)
    {
        CheckIndex(start, length);
        Zero(start, length);
    }

    void Zero(int start, int length)
    {
        for (int i = 0; i < length; i++) {
            if (_array[start].IsAllocated) {
                _array[start].Free();
                _array[start] = new GCHandle();
                Debug.Assert(!_array[start].IsAllocated);
            }

            start++;
        }
    }

    public void TrimToSize()
    {
        Compress();
        Array.Resize(ref _array, _count);
        _handle.Target = _array;
    }

    public void Compress()
    {
        int i = 0;
        Compress(ref i);
    }

    void Compress(ref int index)
    {
        var dead = new GCHandle();
        int write = 0;
        int oldIndex = index;
        for (int read = 0; read < _count; read++) {
            if (read == oldIndex)
                index = write;

            GCHandle handle = _array[read];

            if (!handle.IsAllocated)
                continue;

            _array[read] = dead;

            if (handle.Target == null)
                handle.Free();
            else
                _array[write++] = handle;
        }

        _count = write;
        if (index > _count)
            index = _count;
        _version++;
    }

    public void Push(T o)
    {
        Insert(_count, o);
    }

    public T Pop()
    {
        var dead = new GCHandle();
        while (_count > 0) {
            T? result = this[_count - 1];
            _count--;
            _array[_count] = dead;
            if (result != null)
                return result;
        }

        return null;
    }

    public void RemoveRange(int index, int count)
    {
        CheckIndex(index, count);
        Zero(index, count);
        ArrayTools.RemoveRange(_array, ref _count, index, count);
        _version++;
    }

    public void AddRange(ICollection<T> col)
    {
        InsertRange(_count, col);
    }

    public void InsertRange(int index, ICollection<T> col)
    {
        CheckIndex(index);
        int colCount = col.Count;
        if (_count + colCount > _array.Length)
            Compress();

        ArrayTools.InsertRange(ref _array, ref _count, index, colCount);
        _handle.Target = _array;

        foreach (T o in col)
            this[index++] = o;

        _version++;
    }

    public void Reverse()
    {
        Array.Reverse(_array, 0, _count);
    }

    public void Reverse(int start, int count)
    {
        CheckIndex(start, count);
        Array.Reverse(_array, start, count);
    }

    #endregion

    #region IList Members

    public bool IsReadOnly => false;

    public void RemoveAt(int index)
    {
        RemoveRange(index, 1);
    }

    public void Clear()
    {
        Zero(0, _count);
        _count = 0;
    }

    public bool IsFixedSize => false;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region IList<T> Members

    public bool Remove(T value)
    {
        int index = IndexOf(value);
        if (index != -1) {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public bool Contains(T value) => IndexOf(value) != -1;

    public int IndexOf(T value)
    {
        object o = value;
        for (int i = 0; i < _count; i++) {
            GCHandle handle = _array[i];
            object? target = handle.IsAllocated ? handle.Target : null;
            if (Equals(target, o))
                return i;
        }

        return -1;
    }

    public void Add(T value)
    {
        Insert(_count, value);
    }

    public void CopyTo(T[] array, int index)
    {
        foreach (T o in this)
            array[index++] = o;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    #endregion

    #region Nongeneric list

    int IList.Add(object value)
    {
        Add((T)value);
        return Count - 1;
    }

    bool IList.Contains(object value) => Contains((T)value);

    int IList.IndexOf(object value) => IndexOf((T)value);

    void IList.Insert(int index, object value)
    {
        Insert(index, (T)value);
    }

    void IList.Remove(object value)
    {
        Remove((T)value);
    }

    object IList.this[int index] {
        get => this[index];

        set => this[index] = (T)value;
    }

    void ICollection.CopyTo(Array array, int index)
    {
        CopyTo((T[])array, index);
    }

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => this;

    #endregion
}