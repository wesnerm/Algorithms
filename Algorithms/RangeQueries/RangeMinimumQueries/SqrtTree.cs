using System.Numerics;
using System.Runtime.CompilerServices;

namespace Algorithms.RangeQueries.RangeMinimumQueries;

using SqrtTreeItem = int;

public class SqrtTree
{
    readonly SqrtTreeItem[][] between;
    readonly int[] clz;
    readonly int indexSz;
    readonly int lg;
    readonly int n;
    readonly int[] onLayer;
    readonly SqrtTreeItem[][] pref;
    readonly SqrtTreeItem[][] suf;
    readonly SqrtTreeItem[] v;
    public List<int> layers = new();

    public SqrtTree(SqrtTreeItem[] a)
    {
        n = a.Length;
        lg = Log2(n - 1) << 1;
        v = a;
        clz = new SqrtTreeItem[1 << lg];
        onLayer = new SqrtTreeItem[lg + 1];
        clz[0] = 0;
        for (int i = 1; i < clz.Length; i++)
            clz[i] = clz[i >> 1] + 1;

        int tlg = lg;
        int pos = 0;
        while (tlg > 1) {
            onLayer[tlg] = layers.Count;
            layers.Add(tlg);
            tlg = (tlg + 1) >> 1;
        }

        for (int i = lg - 1; i >= 0; i--)
            onLayer[i] = Math.Max(onLayer[i], onLayer[i + 1]);

        int betweenLayers = Math.Max(0, layers.Count - 1);
        int bSzLog = (lg + 1) >> 1;
        int bSz = 1 << bSzLog;
        indexSz = (n + bSz - 1) >> bSzLog;
        Array.Resize(ref v, n + indexSz);

        pref = new SqrtTreeItem[layers.Count][];
        suf = new SqrtTreeItem[layers.Count][];
        between = new SqrtTreeItem[betweenLayers][];

        for (int i = 0; i < layers.Count; i++) {
            pref[i] = new SqrtTreeItem[n + indexSz];
            suf[i] = new SqrtTreeItem[n + indexSz];
        }

        for (int i = 0; i < betweenLayers; i++)
            between[i] = new SqrtTreeItem[(1 << lg) + bSz];
        build(0, 0, n, 0);
    }

    void buildBlock(int layer, int l, int r)
    {
        pref[layer][l] = v[l];
        for (int i = l + 1; i < r; i++)
            pref[layer][i] = op(pref[layer][i - 1], v[i]);

        suf[layer][r - 1] = v[r - 1];
        for (int i = r - 2; i >= l; i--)
            suf[layer][i] = op(v[i], suf[layer][i + 1]);
    }

    void buildBetween(int layer, int lBound, int rBound, int betweenOffs)
    {
        int bSzLog = (layers[layer] + 1) >> 1;
        int bCntLog = layers[layer] >> 1;
        int bSz = 1 << bSzLog;
        int bCnt = (rBound - lBound + bSz - 1) >> bSzLog;
        for (int i = 0; i < bCnt; i++) {
            int ans = new();
            for (int j = i; j < bCnt; j++) {
                SqrtTreeItem add = suf[layer][lBound + (j << bSzLog)];
                ans = i == j ? add : op(ans, add);
                between[layer - 1][betweenOffs + lBound + (i << bCntLog) + j] = ans;
            }
        }
    }

    void buildBetweenZero()
    {
        int bSzLog = (lg + 1) >> 1;
        for (int i = 0; i < indexSz; i++)
            v[n + i] = suf[0][i << bSzLog];
        build(1, n, n + indexSz, (1 << lg) - n);
    }

    void updateBetweenZero(int bid)
    {
        int bSzLog = (lg + 1) >> 1;
        v[n + bid] = suf[0][bid << bSzLog];
        update(1, n, n + indexSz, (1 << lg) - n, n + bid);
    }

    void build(int layer, int lBound, int rBound, int betweenOffs)
    {
        if (layer >= layers.Count)
            return;

        int bSz = 1 << ((layers[layer] + 1) >> 1);
        for (int l = lBound; l < rBound; l += bSz) {
            int r = Math.Min(l + bSz, rBound);
            buildBlock(layer, l, r);
            build(layer + 1, l, r, betweenOffs);
        }

        if (layer == 0)
            buildBetweenZero();
        else
            buildBetween(layer, lBound, rBound, betweenOffs);
    }

    void update(int layer, int lBound, int rBound, int betweenOffs, int x)
    {
        if (layer >= layers.Count)
            return;

        int bSzLog = (layers[layer] + 1) >> 1;
        int bSz = 1 << bSzLog;
        int blockIdx = (x - lBound) >> bSzLog;
        int l = lBound + (blockIdx << bSzLog);
        int r = Math.Min(l + bSz, rBound);
        buildBlock(layer, l, r);
        if (layer == 0)
            updateBetweenZero(blockIdx);
        else
            buildBetween(layer, lBound, rBound, betweenOffs);
        update(layer + 1, l, r, betweenOffs, x);
    }

    SqrtTreeItem query(int l, int r, int betweenOffs, int @base)
    {
        if (l == r) return v[l];
        if (l + 1 == r) return op(v[l], v[r]);
        int layer = onLayer[clz[(l - @base) ^ (r - @base)]];
        int bSzLog = (layers[layer] + 1) >> 1;
        int bCntLog = layers[layer] >> 1;
        int lBound = (((l - @base) >> layers[layer]) << layers[layer]) + @base;
        int lBlock = ((l - lBound) >> bSzLog) + 1;
        int rBlock = ((r - lBound) >> bSzLog) - 1;
        SqrtTreeItem ans = suf[layer][l];
        if (lBlock <= rBlock) {
            SqrtTreeItem add = layer == 0
                ? query(n + lBlock, n + rBlock, (1 << lg) - n, n)
                : between[layer - 1][betweenOffs + lBound + (lBlock << bCntLog) + rBlock];
            ans = op(ans, add);
        }

        ans = op(ans, pref[layer][r]);
        return ans;
    }

    public SqrtTreeItem query(int l, int r) => query(l, r, 0, 0);

    public void update(int x, SqrtTreeItem item)
    {
        v[x] = item;
        update(0, 0, n, 0, x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    SqrtTreeItem op(SqrtTreeItem a, SqrtTreeItem b) => a + b;

    private static int Log2(long size) => size > 0 ? BitOperations.Log2((ulong)size) : -1;

}