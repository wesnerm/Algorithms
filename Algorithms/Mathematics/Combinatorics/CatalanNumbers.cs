namespace Algorithms.Mathematics.Combinatorics;

public class CatalanNumbers
{
    const int MOD = 1000 * 1000 * 1000 + 7;

    // Catalan Numbers
    // Number of Dyck paths or balanced prefixes
    public static long Catalan(int n) => Div(Comb(2 * n, n), n + 1);

    public static long CatalanTriangle(int n, int k)
    {
        if (k < 0 || k > n) return 0;
        return Div(Mult(Comb(n + 1 + k, k), n + 1 - k), n + 1 + k);
    }

    // Number of combinations of steps
    // where in each step the # of P chosen >= # of Q chosen by C
    // or #Q is zero
    // #P - #Q >= C
    public static long CatalanTriangle(int P, int Q, int C)
    {
        // Hasn't been necessary
        // if (Q > 0 && P - C < Q) return 0; 
        // if (P == 0 || Q == 0) return 1;  
        long result = Comb(P + Q, Q) + MOD - Comb(P + Q, Q + C - 1);
        if (result >= MOD) result -= MOD;
        return result;
    }

    public static long NumberOfDyckPathPrefix(int n) => Comb(n, (n + 1) >> 1);
}