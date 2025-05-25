namespace Algorithms.Collections.Functional;

// http://www.eecs.usma.edu/webs/people/okasaki/sigcse05.pdf

public class FunctionalHeap<T>
{
    #region Variables

    readonly FunctionalHeap<T> _left;
    readonly FunctionalHeap<T> _right;
    readonly T _value;

    public static readonly FunctionalHeap<T> Null = new();

    #endregion

    #region Constructor

    FunctionalHeap()
    {
        _left = this;
        _right = this;
    }

    FunctionalHeap(T value, FunctionalHeap<T> left, FunctionalHeap<T> right)
    {
        _value = value;
        _left = left;
        _right = right;
        Size = left.Size + right.Size;
    }

    FunctionalHeap(T value)
    {
        _value = value;
        Size = 1;
        _left = Null;
        _right = Null;
    }

    #endregion

    #region Properties

    public int Size { get; }

    public bool IsEmpty => Size == 0;

    #endregion

    #region Methods

    public T FindMin()
    {
        if (Size == 0)
            throw new InvalidOperationException();
        return _value;
    }

    public FunctionalHeap<T> DeleteMin()
    {
        if (Size == 0)
            throw new InvalidOperationException();
        return Merge(_left, _right);
    }

    public FunctionalHeap<T> Insert(T value) => Merge(new FunctionalHeap<T>(value), this);

    public FunctionalHeap<T> Merge(FunctionalHeap<T> h) => Merge(this, h);

    static FunctionalHeap<T> Merge(FunctionalHeap<T> h1, FunctionalHeap<T> h2)
    {
        if (h1.Size == 0)
            return h2;
        if (h2.Size == 0)
            return h1;

        var comparer = Comparer<T>.Default;
        if (comparer.Compare(h2._value, h1._value) < 0)
            Swap(ref h1, ref h2);

        // calculate size of merged tree
        FunctionalHeap<T> a = h1._left;
        FunctionalHeap<T> b = h1._right;
        FunctionalHeap<T> c = h2;

        // force a to be the biggest of the 3 subtrees
        if (b.Size > a.Size)
            Swap(ref a, ref b);

        if (c.Size > a.Size)
            Swap(ref a, ref c);

        return new FunctionalHeap<T>(h1._value, a, Merge(b, c));
    }

    static void Swap(ref FunctionalHeap<T> h1, ref FunctionalHeap<T> h2)
    {
        FunctionalHeap<T> tmp = h1;
        h1 = h2;
        h2 = tmp;
    }

    #endregion
}