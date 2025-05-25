namespace Algorithms.Mathematics;

// http://www.cecm.sfu.ca/CAG/theses/justine.pdf
// Fast Multipoint Evaluation on n Arbitrary Points

public class MultipointEvaluation
{
    readonly long[][] _tree;
    long[] _result;
    long[] _xs;
    public Func<long[], long[], long[]> Multiply;

    public MultipointEvaluation(int n) => _tree = new long[4 * HighestOneBit(n - 1)][];

    public long[] Evaluate(long[] coefs, long[] xs)
    {
        _xs = xs;
        _result = new long[xs.Length];
        BuildTree(0, xs.Length);
        ChineseRemainderTheorem(coefs, 0, xs.Length);
        return _result;
    }

    public static long Evaluate(long[] poly, long x)
    {
        if (poly.Length == 0) return 0;

        long result = poly[poly.Length - 1];
        for (int i = poly.Length - 2; i >= 0; i--)
            result = (x * result + poly[i]) % MOD;
        return result;
    }

    long[] BuildTree(int s, int e, int node = 0)
    {
        int mid = (s + e) >> 1;
        return _tree[node] = s + 1 == e
            ? new[] { MOD - _xs[s], 1 }
            : Multiply(
                BuildTree(s, mid, 2 * node + 1),
                BuildTree(mid, e, 2 * node + 2));
    }

    void ChineseRemainderTheorem(long[] p, int s, int e, int node = 0)
    {
        if (e <= 8 + s) {
            while (s < e) {
                _result[s] = Evaluate(p, _xs[s]);
                s++;
            }

            return;
        }

        int mid = (s + e) >> 1;
        ChineseRemainderTheorem(PolynomialDivision.ModPolynomial(p, _tree[2 * node + 1]), s, mid, 2 * node + 1);
        ChineseRemainderTheorem(PolynomialDivision.ModPolynomial(p, _tree[2 * node + 2]), mid, e, 2 * node + 2);
    }
}