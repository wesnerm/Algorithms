using System.Numerics;
using static System.Math;

// SOURCE: http://e-maxx.ru/algo/fft_multiply

namespace HackerRank.Experiment.FFT;

class FftEmaxx
{
    const int MAXN = 700000;
    static readonly int[] rev = new int[MAXN];
    static readonly Complex[] wlen_pw = new Complex[MAXN];

    public static unsafe void Fft(Complex[] array, int n, bool invert)
    {
        fixed (Complex* a = array) {
            for (int i = 0; i < n; ++i)
                if (i < rev[i]) {
                    Complex tmp = a[i];
                    a[i] = a[rev[i]];
                    a[rev[i]] = tmp;
                }

            for (int len = 2; len <= n; len <<= 1) {
                double ang = 2 * PI / len * (invert ? -1 : +1);
                int len2 = len >> 1;

                var wlen = new Complex(Cos(ang), Sin(ang));
                wlen_pw[0] = new Complex(1, 0);
                for (int i = 1; i < len2; ++i)
                    wlen_pw[i] = wlen_pw[i - 1] * wlen;

                for (int i = 0; i < n; i += len) {
                    Complex* pu = a + i;
                    Complex* pv = a + i + len2;
                    Complex* pu_end = a + i + len2;

                    fixed (Complex* pwStart = wlen_pw) {
                        Complex* pw = pwStart;
                        for (; pu != pu_end; ++pu, ++pv, ++pw) {
                            Complex t = *pv * *pw;
                            *pv = *pu - t;
                            *pu += t;
                        }
                    }
                }
            }

            if (invert)
                for (int i = 0; i < n; ++i)
                    a[i] /= n;
        }
    }

    void CalcRev(int n, int logN)
    {
        for (int i = 0; i < n; ++i) {
            rev[i] = 0;
            for (int j = 0; j < logN; ++j)
                if ((i & (1 << j)) != 0)
                    rev[i] |= 1 << (logN - 1 - j);
        }
    }
}