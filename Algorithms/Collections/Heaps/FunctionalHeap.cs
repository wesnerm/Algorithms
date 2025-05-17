namespace Algorithms.Collections.Functional;

// http://www.eecs.usma.edu/webs/people/okasaki/sigcse05.pdf

public class FunctionalHeap<T>
{
    #region Variables        
    private readonly FunctionalHeap<T> _left;
    private readonly FunctionalHeap<T> _right;
    private readonly T _value;
    private readonly int _size;

    public static readonly FunctionalHeap<T> Null = new FunctionalHeap<T>();
    #endregion

    #region Constructor
    private FunctionalHeap()
    {
        _left = this;
        _right = this;
    }

    private FunctionalHeap(T value, FunctionalHeap<T> left, FunctionalHeap<T> right)
    {
        _value = value;
        _left = left;
        _right = right;
        _size = left._size + right._size;
    } 

    private FunctionalHeap(T value)
    {
        _value = value;
        _size = 1;
        _left = Null;
        _right = Null;
    }
    #endregion

    #region Properties

    public int Size => _size;

    public bool IsEmpty => _size == 0;

    #endregion

    #region Methods
    public T FindMin()
    {
        if (_size == 0)
            throw new InvalidOperationException();
        return _value;
    }

    public FunctionalHeap<T> DeleteMin()
    {
        if (_size == 0)
            throw new InvalidOperationException();
        return Merge(_left, _right);
    }

    public FunctionalHeap<T> Insert(T value)
    {
        return Merge(new FunctionalHeap<T>(value), this);
    }

    public FunctionalHeap<T> Merge(FunctionalHeap<T> h)
    {
        return Merge(this, h);
    } 

    private static FunctionalHeap<T> Merge(FunctionalHeap<T> h1, FunctionalHeap<T> h2)
    {
        if (h1._size == 0)
            return h2;
        if (h2._size == 0)
            return h1;

        var comparer = Comparer<T>.Default;
        if (comparer.Compare(h2._value, h1._value) < 0)
            Swap(ref h1, ref h2);

        // calculate size of merged tree
        var a = h1._left;
        var b = h1._right;
        var c = h2;

        // force a to be the biggest of the 3 subtrees
        if (b._size > a._size)
            Swap(ref a, ref b);

        if (c._size > a._size)
            Swap(ref a, ref c);

        return new FunctionalHeap<T>(h1._value, a, Merge(b,c));
    }

    private static void Swap(ref FunctionalHeap<T> h1, ref FunctionalHeap<T> h2)
    {
        var tmp = h1;
        h1 = h2;
        h2 = tmp;
    }
    #endregion

}
