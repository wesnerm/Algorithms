namespace Algorithms.Mathematics.Multiplication;

using T = long;

public class KaratsubaMultiplication
{
    // Karatsuba Multiplication
    // a = p[0,m)
    // b = q[0,m)
    // c = p[m,n)
    // d = q[m,n]

    // hi = c * d
    // lo = a * b

    // rem2 = (a+c)*(b+d)
    // (a+c)*(b+d) - ab - cd = ab + ad + bc + cd - ab - cd = ad + bc

    public static T[] Karatsuba(T[] a, T[] b, int size = 0, int mod = 1000000007)
    {
        int expSize = Math.Max(0, a.Length + b.Length - 1);
        if (size == 0 || expSize < size) size = expSize;
        T[] result = new T[size];
        KaratsubaCore(result, a, 0, Math.Min(size, a.Length), b, 0, Math.Min(size, b.Length), mod);
        return result;
    }

    static void KaratsubaCore(
        T[] result,
        T[] p, int ip, int plen,
        T[] q, int iq, int qlen, int mod)
    {
        int resultLen = result.Length;
        if (ip >= plen || iq >= qlen) return;

        int n = Math.Max(plen - ip, qlen - iq);
        int j;
        if (n <= 35) {
            if (plen < qlen) {
                Swap(ref p, ref q);
                int t = ip;
                ip = iq;
                iq = t;
                t = plen;
                plen = qlen;
                qlen = t;
            }

            for (int i = ip; i < plen; i++) {
                int ind = i - ip - iq;
                int end = Math.Min(qlen, resultLen - ind);
                for (j = iq; j < end; j++)
                    result[ind + j] = (result[ind + j] + p[i] * q[j]) % mod;
            }

            return;
        }

        int m = (n + 1) >> 1;

        KaratsubaCore(result, p, ip, Math.Min(ip + m, plen), q, iq, Math.Min(iq + m, qlen), mod);

        for (int i = Math.Min(resultLen - m, m * 2 - 1) - 1; i >= 0; i--)
            result[m + i] -= result[i];

        T[] tmp = new T[m * 2 - 1];
        KaratsubaCore(tmp, p, ip + m, plen, q, iq + m, qlen, mod);

        for (int i = Math.Min(resultLen - m, tmp.Length) - 1; i >= 0; i--)
            result[m + i] -= tmp[i];
        for (int i = Math.Min(resultLen - m * 2, tmp.Length) - 1; i >= 0; i--)
            result[m * 2 + i] += tmp[i];

        T[] f1 = new T[m];
        T[] f2 = new T[m];

        for (j = Math.Min(plen - ip, n) - 1; j >= m; j--)
            f1[j - m] += p[j + ip];
        for (; j >= 0; j--) f1[j] = (f1[j] + p[j + ip]) % mod;
        for (j = Math.Min(qlen - iq, n) - 1; j >= m; j--)
            f2[j - m] += q[j + iq];
        for (; j >= 0; j--) f2[j] = (f2[j] + q[j + iq]) % mod;

        Array.Clear(tmp, 0, tmp.Length);
        KaratsubaCore(tmp, f1, 0, m, f2, 0, m, mod);

        for (int i = Math.Min(tmp.Length, resultLen - m) - 1; i >= 0; i--)
            result[m + i] += tmp[i];

        for (int i = Math.Min(plen + qlen - 1, resultLen) - 1; i >= 0; i--) {
            result[i] %= mod;
            if (result[i] < 0) result[i] += mod;
        }
    }

    public long[] PolyPow(long[] x, long n, int limit = 0, int mod = 1000000007)
    {
        if (n <= 1) return n == 1 ? x : new long[1];
        long[] t = PolyPow(x, n >> 1, limit, mod);
        T[] sq = KaratsubaFast(t, t, limit, mod);
        return (n & 1) == 0 ? sq : KaratsubaFast(x, sq, limit, mod);
    }

    public static unsafe T[] KaratsubaFast(T[] a, T[] b, int size = 0, int mod = 1000000007)
    {
        int expSize = a.Length + b.Length - 1;
        if (size == 0 || expSize < size) size = expSize;
        T[] result = new T[size];
        fixed (T* presult = result)
        fixed (T* pa = a)
        fixed (T* pb = b) {
            KaratsubaFastCore(presult, result.Length, pa, Math.Min(size, a.Length), pb, Math.Min(size, b.Length), mod);
            for (int i = 0; i < result.Length; i++)
                if (result[i] < 0)
                    result[i] += mod;
        }

        return result;
    }

    static unsafe void KaratsubaFastCore(
        T* result, int resultLen,
        T* p, int plen,
        T* q, int qlen, int mod)
    {
        if (plen < qlen) {
            T* tp = p;
            p = q;
            q = tp;
            int t = plen;
            plen = qlen;
            qlen = t;
        }

        int n = plen;
        int j;
        if (qlen <= 35 || resultLen <= 35) {
            if (qlen > 0 && resultLen > 0)
                for (int i = 0; i < plen; i++) {
                    T pi = p[i];
                    T* presult = &result[i];
                    int end = Math.Min(qlen, resultLen - i);
                    for (j = 0; j < end; j++)
                        presult[j] = (presult[j] + pi * q[j]) % mod;
                }

            return;
        }

        int m = (n + 1) >> 1;

        if (qlen <= m) {
            KaratsubaFastCore(result, resultLen, p, Math.Min(m, plen), q, Math.Min(m, qlen), mod);
            KaratsubaFastCore(result + m, resultLen - m, p + m, plen - m, q, qlen, mod);
            return;
        }

        int tmpLength = 2 * m - 1;
        T* tmp = stackalloc T[2 * m];

        // Step 1
        // NOTE: StackAlloc may be preinitialized if local-init is set true
        for (T* ptmp = tmp, pMac = tmp + tmpLength; ptmp < pMac; ptmp++) *ptmp = 0;
        KaratsubaFastCore(tmp, tmpLength, p, Math.Min(m, plen), q, Math.Min(m, qlen), mod);

        for (int i = Math.Min(resultLen, tmpLength) - 1; i >= 0; i--)
            result[i] += tmp[i];

        T* ptr = result + m;
        for (int i = Math.Min(resultLen - m, tmpLength) - 1; i >= 0; i--)
            ptr[i] -= tmp[i];

        // Step 2
        for (T* ptmp = tmp, pMac = tmp + tmpLength; ptmp < pMac; ptmp++) *ptmp = 0;
        KaratsubaFastCore(tmp, tmpLength, p + m, plen - m, q + m, qlen - m, mod);

        ptr = result + 2 * m;
        for (int i = Math.Min(resultLen - m * 2, tmpLength) - 1; i >= 0; i--) ptr[i] += tmp[i];
        ptr = result + m;
        for (int i = Math.Min(resultLen - m, tmpLength) - 1; i >= 0; i--) ptr[i] -= tmp[i];

        // Step 3
        for (T* ptmp = tmp, pMac = tmp + tmpLength; ptmp < pMac; ptmp++) *ptmp = 0;

        T* f1 = tmp;
        T* f2 = tmp + m;

        for (j = Math.Min(plen, n) - 1; j >= m; j--) f1[j - m] += p[j];
        for (; j >= 0; j--) f1[j] = (f1[j] + p[j]) % mod;
        for (j = Math.Min(qlen, n) - 1; j >= m; j--) f2[j - m] += q[j];
        for (; j >= 0; j--) f2[j] = (f2[j] + q[j]) % mod;

        KaratsubaFastCore(result + m, Math.Min(2 * m - 1, resultLen - m), f1, m, f2, m, mod);

        for (int i = Math.Min(plen + qlen - 1, resultLen) - 1; i >= 0; i--)
            result[i] %= mod;
    }

    /*
      Faster version

private static unsafe void KaratsubaFastCore(
    T* result, int resultLen,
    T* p, int plen,
    T* q, int qlen, int mod)
{
    if (plen < qlen)
    {
        var tp = p; p = q; q = tp;
        var t = plen; plen = qlen; qlen = t;
    }

    int n = plen;
    int j;
    if (qlen <= 35 || resultLen <= 35)
    {
        if (qlen > 0 && resultLen > 0)
            for (int i = 0; i < plen; i++)
            {
                var pi = p[i];
                var presult = &result[i];
                int end = Math.Min(qlen, resultLen - i);
                for (j = 0; j < end; j++)
                    presult[j] = (presult[j] + pi * q[j]) % mod;
            }
        return;
    }

    int m = (n + 1) >> 1;

    if (qlen <= m)
    {
        KaratsubaFastCore(result, resultLen, p, Math.Min(m, plen), q, Math.Min(m, qlen), mod);
        KaratsubaFastCore(result + m, resultLen - m, p + m, plen - m, q, qlen, mod);
        return;
    }

    var tmpLength = 2 * m - 1;
    var tmp = stackalloc T[2 * m];

    // Step 1
    // NOTE: StackAlloc may be preinitialized if local-init is set true
    for (T* ptmp = tmp, pMac = tmp + tmpLength; ptmp < pMac; ptmp++) *ptmp = 0;
    KaratsubaFastCore(tmp, tmpLength, p, Math.Min(m, plen), q, Math.Min(m, qlen), mod);

    for (int i = Math.Min(resultLen, tmpLength) - 1; i >= 0; i--)
        result[i] += tmp[i];

    var ptr = result + m;
    for (int i = Math.Min(resultLen - m, tmpLength) - 1; i >= 0; i--)
        ptr[i] -= tmp[i];

    if (resultLen > m)
    {
        int crop = resultLen - m;
        // Step 2
        for (T* ptmp = tmp, pMac = tmp + tmpLength; ptmp < pMac; ptmp++) *ptmp = 0;
        KaratsubaFastCore(tmp, Math.Min(crop,tmpLength), p + m, Math.Min(crop, plen - m), q + m, Math.Min(crop, qlen - m), mod);

        ptr = result + 2 * m;
        for (int i = Math.Min(resultLen - m * 2, tmpLength) - 1; i >= 0; i--)
            ptr[i] += tmp[i];
        ptr = result + m;
        for (int i = Math.Min(crop, tmpLength) - 1; i >= 0; i--)
            ptr[i] -= tmp[i];

        // Step 3
        for (T* ptmp = tmp, pMac = tmp + tmpLength; ptmp < pMac; ptmp++) *ptmp = 0;

        var f1 = tmp;
        var f2 = tmp + m;

        for (j = Math.Min(plen, n) - 1; j >= m; j--)
            f1[j - m] += p[j];
        for (; j >= 0; j--) f1[j] = (f1[j] + p[j]) % mod;
        for (j = Math.Min(qlen, n) - 1; j >= m; j--)
            f2[j - m] += q[j];
        for (; j >= 0; j--) f2[j] = (f2[j] + q[j]) % mod;

        KaratsubaFastCore(result + m, Math.Min(2 * m - 1, crop), f1, Math.Min(crop, m), f2, Math.Min(crop, m), mod);
    }

    for (int i = Math.Min(plen + qlen - 1, resultLen) - 1; i >= 0; i--)
        result[i] %= mod;
}

     * */
}