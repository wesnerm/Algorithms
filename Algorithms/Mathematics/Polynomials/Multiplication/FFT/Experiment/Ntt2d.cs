namespace Algorithms.Mathematics.Multiplication.NTT;

// https://www.codechef.com/viewsolution/27204812

public class Ntt2d
{
    readonly NttBase ntt;

    public Ntt2d(NttBase ntt) => this.ntt = ntt;

    void Ntt(long[][] a, bool inverse, int mod, int g)
    {
        int n = a.GetLength(0);
        int m = a.GetLength(1);
        long[] ha = new long[n];

        // pre(m);
        for (int i = 0; i < n; i++)
            ntt.Ntt(a[i], a[i], inverse, mod, g);

        // pre(n);
        for (int j = 0; j < m; j++) {
            for (int i = 0; i < n; i++)
                ha[i] = a[i][j];

            ntt.Ntt(ha, ha, inverse, mod, g);
            for (int i = 0; i < n; i++)
                a[i][j] = ha[i];
        }
    }
}