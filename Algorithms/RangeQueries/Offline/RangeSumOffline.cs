namespace Algorithms.RangeQueries;

[DebuggerStepThrough]
public class RangeSumOffline
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public readonly long[] A;

    int deriv; // 1=difference, 0=function, -1=summation

    public RangeSumOffline(int size, int deriv = int.MinValue)
    {
        A = new long[size];
        this.deriv = deriv;
    }

    public RangeSumOffline(long[] array, bool clone = true) => A = clone ? (long[])array.Clone() : array;

    public RangeSumOffline(int[] array) : this(array.Length, 0)
    {
        for (int i = 0; i < array.Length; i++)
            A[i] = array[i];
    }

    public long this[int i] {
        get
        {
            if (deriv != 0) {
                if (deriv == -1) return QueryInclusive(i, i);
                GoToDerivative(0);
            }

            ;
            return A[i];
        }
        set
        {
            if (deriv != 0) GoToDerivative(0);
            A[i] = value;
        }
    }

    public void GoToDerivative(int d)
    {
        if (deriv == int.MinValue) deriv = d;
        for (; d < deriv; deriv--) // Integrate
        for (int i = 1; i < A.Length; i++)
            A[i] += A[i - 1];
        for (; d > deriv; deriv++) // Differentiate
        for (int i = A.Length - 1; i >= 1; i--)
            A[i] -= A[i - 1];
    }

    public void AddInclusive(int x1, int x2, long v)
    {
        if (deriv != 1) GoToDerivative(1);
        A[x1] += v;
        if (x2 + 1 < A.Length) A[x2 + 1] -= v;
    }

    public long QueryInclusive(int x1, int x2)
    {
        if (deriv != -1) GoToDerivative(-1);
        return A[x2] - (x1 > 0 ? A[x1 - 1] : 0);
    }
}