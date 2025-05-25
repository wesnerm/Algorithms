namespace Algorithms.RangeQueries;

public struct RangeQueryLine
{
    readonly double[] x0, x1, x2;

    public RangeQueryLine(int n)
    {
        x0 = new double[n + 1];
        x1 = new double[n + 1];
        x2 = new double[n + 1];
    }

    public int Length => x1.Length;

    public double this[int index] => QueryInclusive(index, index);

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public double[] Table {
        get
        {
            double[] table = new double[x1.Length - 1];
            for (int i = 0; i < table.Length; i++) table[i] = this[i];
            return table;
        }
    }

    public void AddInclusive(int left, int right, Func<double, double> line)
    {
        double b = line(0), m = line(1) - b;
        AddInclusive(left, right, m, b);
    }

    public void AddInclusive(int left, int right, double m, double b)
    {
        // Add f2*x(x+1) = f2*x2 + f2*x 
        double f2 = m * 0.5;
        double f1 = f2 + b;
        Add(x2, left, f2);
        Add(x2, right + 1, -f2);
        Add(x1, left, f1);
        Add(x1, right + 1, -f1);
        Add(x0, left, -(left - 1) * (f1 + f2 * (left - 1)));
        Add(x0, right + 1, right * (f1 + f2 * right));
    }

    public double SumInclusive(int x) => (SumInclusive(x2, x) * x + SumInclusive(x1, x)) * x + SumInclusive(x0, x);

    public double QueryInclusive(int i, int j) => SumInclusive(j) - SumInclusive(i - 1);

    void Add(double[] A, int i, double val)
    {
        if (val != 0)
            for (i++; i < A.Length; i += i & -i)
                A[i] += val;
    }

    double SumInclusive(double[] A, int i)
    {
        double sum = 0;
        for (i++; i > 0; i -= i & -i) sum += A[i];
        return sum;
    }
}