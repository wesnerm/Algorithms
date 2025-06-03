using System.Numerics;

namespace Algorithms.RangeQueries;

// http://codeforces.com/contest/494/submission/9121141

public class RmqLinearPreprocess
{
    readonly int[,] _block;
    readonly int[] _data, sblock, lookup;
    readonly int _lg;

    public RmqLinearPreprocess(int[] data)
    {
        _data = data;
        int n = _data.Length;
        _lg = BitOperations.Log2((uint)n) + 1;

        int blockLen = (n + _lg - 1) / _lg;
        _block = new int[blockLen, _lg];
        for (int i = 0; i < n; i++) _block[i / _lg, 0] = i % _lg != 0 ? Min(i, _block[i / _lg, 0]) : i;

        for (int j = 1; j < _lg; j++)
        for (int i = 0; i < blockLen; i++) {
            int ii = i + (1 << (j - 1));
            _block[i, j] = Min(_block[i, j - 1],
                _block[ii < blockLen ? ii : 0, j - 1]);
        }

        sblock = new int[n];

        var stack = new Stack<int>();
        for (int i = 0; i < blockLen; i++) {
            stack.Clear();
            for (int j = i * _lg; j < n && j < i * _lg + _lg; j++) {
                while (stack.Count > 0 && _data[j] < _data[stack.Peek()])
                    stack.Pop();
                sblock[j] = 1 << (i * _lg + _lg - j - 1);
                if (stack.Count > 0)
                    sblock[j] |= sblock[stack.Peek()];
                stack.Push(j);
            }
        }

        lookup = new int[1 << _lg];
        for (int i = 1, ans = _lg - 1; i < 1 << _lg; i++) {
            if (1 << (_lg - ans) <= i)
                ans--;
            lookup[i] = ans;
        }
    }

    public int GetArgMin(int l, int r)
    {
        if (r < l)
            Swap(ref l, ref r);
        int l1 = l / _lg + 1;
        int r1 = r / _lg - 1;
        int ans = l;
        int t;

        if (l1 <= r1) {
            t = _lg - lookup[r1 - l1 + 1] - 1;
            ans = Min(ans, Min(_block[l1, t], _block[r1 - (1 << t) + 1, t]));
        }

        t = l1 * _lg - 1 < r ? l1 * _lg - 1 : r;
        ans = Min(ans,
            lookup[sblock[t] & ~(((1 << (l - (l1 * _lg - _lg))) - 1) << (l1 * _lg - l))] + l1 * _lg - _lg);

        t = r;
        l = l > r1 * _lg + _lg ? l : r1 * _lg + _lg;
        ans = Min(ans,
            lookup[sblock[t]
                   & ~(((1 << (l - (r1 * _lg + _lg))) - 1) << (r1 * _lg + (_lg << 1) - l))]
            + r1 * _lg + _lg);
        return ans;
    }

    int Min(int a, int b) => _data[b] < _data[a] ? b : a;
}