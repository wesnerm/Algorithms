namespace Algorithms.Collections;

public class PArray<T> : Persistent<T[]>, IEnumerable<T>
    where T : IEquatable<T>
{
    #region Helper Classes

    class Diff : Change
    {
        readonly int Index;
        T Value;

        public Diff(int index, T value)
        {
            Index = index;
            Value = value;
        }

        public override Change Apply(T[] a)
        {
            T vp = a[Index];
            a[Index] = Value;
            Value = vp;
            return this;
        }
    }

    #endregion

    #region Constructor

    public PArray()
        : this(Array.Empty<T>()) { }

    public PArray(T[] array)
        : base(array) { }

    PArray(Change change, PArray<T> array)
        : base(change, array) { }

    #endregion

    #region Properties

    public int Length => Data.Length;

    public T this[int index] => Data[index];

    public void Resize(int count)
    {
        T[] arr = Data;
        if (count >= arr.Length)
            Array.Resize(ref arr, count);
    }

    public void Ensure(int n)
    {
        int length = Length;
        if (n < length) return;
        int capacity = length * 2;
        if (capacity < n) capacity = n;
        Resize(capacity + 4);
    }

    public PArray<T> Set(int index, T element)
    {
        T[] arr = Data;
        T? old = arr[index];
        if (old != null ? old.Equals(element) : element == null)
            return this;

        return new PArray<T>(new Diff(index, element), this);
    }

    #endregion

    #region Methods

    public virtual IEnumerator<T> GetEnumerator()
    {
        int length = Length;
        for (int i = 0; i < length; i++)
            yield return Data[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}