// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Algorithms.Mathematics;

/// <summary>
///     Two-phase simplex algorithm for solving linear programs of the form
///     maximize     c^T x
///     subject to   Ax LE b
///     x GE 0
///     INPUT: A -- an m x n matrix
///     b -- an m-dimensional List
///     c -- an n-dimensional List
///     x -- a List where the optimal solution will be stored
///     OUTPUT: value of the optimal solution (infinity if unbounded
///     above, -Infinity if infeasible)
///     To use this code, create an LPSolver object with A, b, and c as
///     arguments.  Then, call Solve().
/// </summary>
public class LpSolver
{
    const double Epsilon = 1e-9;
    readonly int[] _b;
    readonly double[,] _d;
    readonly int _m;
    readonly int _n;
    public readonly double Answer;
    readonly int[] N;
    public readonly double[] OptimalVariables;

    public LpSolver(double[,] a, double[] b, double[] c)
    {
        _m = b.Length;
        _n = c.Length;
        N = new int[_n + 1];
        _b = new int[_m];
        _d = new double[_m + 2, _n + 2];

        for (int i = 0; i < _m; i++)
        for (int j = 0; j < _n; j++)
            _d[i, j] = a[i, j];

        for (int i = 0; i < _m; i++) {
            _b[i] = _n + i;
            _d[i, _n] = -1;
            _d[i, _n + 1] = b[i];
        }

        for (int j = 0; j < _n; j++) {
            N[j] = j;
            _d[_m, j] = -c[j];
        }

        N[_n] = -1;
        _d[_m + 1, _n] = 1;

        OptimalVariables = new double[_n];
        Answer = Solve();
    }

    void Pivot(int r, int s)
    {
        for (int i = 0; i < _m + 2; i++)
            if (i != r)
                for (int j = 0; j < _n + 2; j++)
                    if (j != s)
                        _d[i, j] -= _d[r, j] * _d[i, s] / _d[r, s];

        for (int j = 0; j < _n + 2; j++)
            if (j != s)
                _d[r, j] /= _d[r, s];

        for (int i = 0; i < _m + 2; i++)
            if (i != r)
                _d[i, s] /= -_d[r, s];
        _d[r, s] = 1.0 / _d[r, s];

        Swap(ref _b[r], ref N[s]);
    }

    bool Simplex(int phase)
    {
        int x = phase == 1 ? _m + 1 : _m;
        while (true) {
            int s = -1;
            for (int j = 0; j <= _n; j++) {
                if (phase == 2 && N[j] == -1) continue;
                if (s == -1 || _d[x, j] < _d[x, s]
                            || (_d[x, j] == _d[x, s] && N[j] < N[s]))
                    s = j;
            }

            if (_d[x, s] > -Epsilon)
                return true;

            int r = -1;
            for (int i = 0; i < _m; i++) {
                if (_d[i, s] < Epsilon) continue;
                if (r == -1 || _d[i, _n + 1] / _d[i, s] < _d[r, _n + 1] / _d[r, s]
                            || (_d[i, _n + 1] / _d[i, s] == _d[r, _n + 1] / _d[r, s]
                                && _b[i] < _b[r])) r = i;
            }

            if (r == -1)
                return false;

            Pivot(r, s);
        }
    }

    double Solve()
    {
        int r = 0;
        for (int i = 1; i < _m; i++)
            if (_d[i, _n + 1] < _d[r, _n + 1])
                r = i;

        if (_d[r, _n + 1] < -Epsilon) {
            Pivot(r, _n);
            if (!Simplex(1) || _d[_m + 1, _n + 1] < -Epsilon)
                return double.NegativeInfinity;

            for (int i = 0; i < _m; i++)
                if (_b[i] == -1) {
                    int s = -1;
                    for (int j = 0; j <= _n; j++)
                        if (s == -1 || _d[i, j] < _d[i, s] || (_d[i, j] == _d[i, s] && N[j] < N[s]))
                            s = j;
                    Pivot(i, s);
                }
        }

        if (!Simplex(2))
            return double.PositiveInfinity;

        double[] x = OptimalVariables;
        for (int i = 0; i < _m; i++)
            if (_b[i] < _n)
                x[_b[i]] = _d[i, _n + 1];
        return _d[_m, _n + 1];
    }
}