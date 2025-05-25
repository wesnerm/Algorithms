namespace Algorithms.Collections;

/// <summary>
///     Summary description for PList
/// </summary>

// TODO: Check if serializable attribute and interface are necessary together
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
[Serializable]
public class PList<T> : Persistent<List<T>>, IList<T>
{
    #region Constructor

    [DebuggerStepThrough]
    public PList(IEnumerable<T> array)
        : base(new List<T>(array)) { }

    [DebuggerStepThrough]
    public PList()
        : base(new List<T>()) { }

    [DebuggerStepThrough]
    public PList(List<T> list, bool useOriginal = false)
        : base(useOriginal ? list : new List<T>(list)) { }

    [DebuggerStepThrough]
    PList(Change change, PList<T> array)
        : base(change, array) { }

    #endregion

    #region Properties

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Count => Data.Count;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsReadOnly => true;

    public T this[int index] {
        get => Data[index];
        set => throw new InvalidOperationException();
    }

    #endregion

    #region List Methods

    void ICollection<T>.Add(T item)
    {
        throw new InvalidOperationException();
    }

    void ICollection<T>.Clear()
    {
        throw new InvalidOperationException();
    }

    [DebuggerStepThrough]
    public bool Contains(T item) => Data.Contains(item);

    [DebuggerStepThrough]
    public void CopyTo(T[] array, int arrayIndex)
    {
        Data.CopyTo(array, arrayIndex);
    }

    bool ICollection<T>.Remove(T item) => throw new InvalidOperationException();

    [DebuggerStepThrough]
    public int IndexOf(T item) => Data.IndexOf(item);

    void IList<T>.Insert(int index, T item)
    {
        throw new InvalidOperationException();
    }

    void IList<T>.RemoveAt(int index)
    {
        throw new InvalidOperationException();
    }

    public PList<T> Set(int index, T element)
    {
        List<T> arr = Data;
        T old = arr[index];
        if (old != null
                ? EqualityComparer<T>.Default.Equals(old, element)
                : element == null)
            return this;
        return new PList<T>(new Diff(index, element), this);
    }

    [DebuggerStepThrough]
    public PList<T> AddAndCopy(T element) => InsertAndCopy(Count - 1, element);

    [DebuggerStepThrough]
    public PList<T> AppendRangeAndCopy(T[] range) => InsertRangeAndCopy(Count - 1, range);

    [DebuggerStepThrough]
    public PList<T> InsertAndCopy(int index, T element) => new(new Insert1Diff(index, element), this);

    [DebuggerStepThrough]
    public PList<T> InsertRangeAndCopy(int index, T[] range)
    {
        if (range.Length == 0)
            return this;
        return new PList<T>(new InsertDiff(index, range), this);
    }

    [DebuggerStepThrough]
    public PList<T> RemoveRangeAndCopy(int index, int count)
    {
        if (count == 0)
            return this;
        return new PList<T>(new InsertDiff(index, count), this);
    }

    [DebuggerStepThrough]
    public PList<T> RemoveAtAndCopy(int index) => new(new Insert1Diff(~index, this[index]), this);

    [DebuggerStepThrough]
    public PList<T> RemoveAndCopy(T element)
    {
        int index = Data.IndexOf(element);
        if (index < 0) return this;
        return RemoveAtAndCopy(index);
    }

    #endregion

    #region Enumeration

    public virtual IEnumerator<T> GetEnumerator()
    {
        int length = Count;
        for (int i = 0; i < length; i++)
            yield return Data[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Helper Classes

    #region Nested type: Diff

    class Diff : Change
    {
        readonly int Index;
        T Value;

        public Diff(int index, T value)
        {
            Index = index;
            Value = value;
        }

        public override Change Apply(List<T> a)
        {
            T vp = a[Index];
            a[Index] = Value;
            Value = vp;
            return this;
        }
    }

    #endregion

    #region Nested type: Insert1Diff

    class Insert1Diff : Change
    {
        int Index;
        T Value;

        public Insert1Diff(int index, T value)
        {
            Index = index;
            Value = value;
        }

        public override Change Apply(List<T> a)
        {
            if (Index >= 0) {
                a.Insert(Index, Value);
                Value = default;
            } else {
                Value = a[Index];
                a.RemoveAt(Index);
            }

            Index = ~Index;
            return this;
        }
    }

    #endregion

    #region Nested type: InsertDiff

    class InsertDiff : Change
    {
        readonly int Count;
        readonly int Index;
        T[] Range;

        public InsertDiff(int index, T[] values)
        {
            Index = index;
            Count = values.Length;
            Range = values;
        }

        public InsertDiff(int index, int count)
        {
            Index = index;
            Count = count;
            Range = null;
        }

        public override Change Apply(List<T> a)
        {
            T[]? range = Range;
            if (range == null) {
                Range = a.CopyRange(Index, Count);
                a.RemoveRange(Index, Count);
            } else {
                a.InsertRange(Index, range);
                Range = null;
            }

            return this;
        }
    }

    #endregion

    #endregion
}