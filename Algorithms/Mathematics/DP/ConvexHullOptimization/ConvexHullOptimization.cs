using T = long;

namespace Algorithms.Mathematics.DP;

public class ConvexHullOptimization
{
    readonly int sign;
    T[] A, B;
    int index, length;

    public ConvexHullOptimization(bool max, int size = 4)
    {
        A = new T[size];
        B = new T[size];
        sign = max ? -1 : 1;
    }

    public void Clear()
    {
        index = length = 0;
    }

    // a descends
    public void AddLine(T a, T b)
    {
        a *= sign;
        b *= sign;

        // intersection of (A[len-2],B[len-2]) with (A[len-1],B[len-1]) must lie to the left of intersection of (A[len-1],B[len-1]) with (a,b)
        while (length >= 2 &&
               (B[length - 2] - B[length - 1]) * (a - A[length - 1])
               >= (B[length - 1] - b) * (A[length - 1] - A[length - 2]))
            --length;

        if (length >= A.Length) {
            Array.Resize(ref A, 2 * length);
            Array.Resize(ref B, 2 * length);
        }

        A[length] = a;
        B[length] = b;
        ++length;
    }

    public void AddLine(Func<T, T> fx)
    {
        T y1 = fx(0);
        AddLine(fx(1) - y1, y1);
    }

    // x ascends
    public T GetMinimum(T x)
    {
        index = Math.Min(index, length - 1);
        while (index + 1 < length && A[index + 1] * x + B[index + 1] <= A[index] * x + B[index]) ++index;
        return (A[index] * x + B[index]) * sign;
    }
}