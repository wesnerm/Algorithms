using Algorithms.RangeQueries;

namespace Algorithms.Strings;

public class SuffixArray
{
    #region Helpers

    public static void Swap<T>(ref T a, ref T b)
    {
        T tmp = a;
        a = b;
        b = tmp;
    }

    #endregion

    #region Variables

    int length;
    public CharSequence String;
    int[] _lcps;
    int[] _ranks;
    RangeMinimumQuery _rmq;

    #endregion

    #region Construction

    public SuffixArray(int[] suffixes, int[] ranks = null, Func<char, bool> isDelim = null)
    {
        Init(suffixes, ranks, isDelim);
    }

    void Init(int[] suffixes, int[] ranks, Func<char, bool> isDelim)
    {
        length = suffixes.Length;
        Suffixes = suffixes;
        _ranks = ranks ?? BuildRanks(suffixes);
        _lcps = BuildLongestCommonPrefixes(String, Suffixes, _ranks, isDelim);
    }

    #endregion

    #region Automaton Construction

    public SuffixArray(SuffixAutomaton sa, Func<char, bool> isDelim = null)
    {
        String = sa.Text as string;
        var ab = new AutomatonBuilder(sa);
        Init(ab.Suffixes, null, isDelim);
    }

    struct AutomatonBuilder
    {
        int idx;
        SuffixAutomaton sa;
        readonly int n;
        readonly SuffixAutomaton.SummarizedState[] summary;
        public readonly int[] Suffixes;

        public AutomatonBuilder(SuffixAutomaton sa) : this()
        {
            this.sa = sa;
            summary = sa.SummarizedAutomaton();
            n = sa.End.Len;
            Suffixes = new int[n];
            idx = 0;
            DfsSuffixArray(sa.Start, 0);
        }

        void DfsSuffixArray(SuffixAutomaton.Node node, int length)
        {
            SuffixAutomaton.SummarizedState c;

            while (node.IsTerminal) {
                Suffixes[idx++] = n - length;
                if (node.NextCount != 1) break;
                c = summary[node.Next[0].Index];
                node = c.Node;
                length += c.Length;
            }

            for (int index = 0; index < node.NextCount; index++) {
                c = summary[node.Next[index].Index];
                DfsSuffixArray(c.Node, length + c.Length);
            }
        }
    }

    #endregion

    #region Classic Construction

    public SuffixArray(CharSequence s, Func<char, bool> isDelim = null)
    {
        String = s;
        var builder = new ClassicBuilder(s);
        Init(builder.Suffixes, builder.Indices, isDelim);
    }

    public struct ClassicBuilder
    {
        static int length;
        public int[] Suffixes;
        public int[] Indices;
        Bucket[] _matrix2;
        Bucket[] _matrix;

        public static bool UseQuickSort;
        public static bool LateQuit;

        public ClassicBuilder(CharSequence s) : this()
        {
            length = s.Length;
            int[] ranks = new int[length];
            int[] ranksPrev = new int[length];
            for (int i = 0; i < length; i++)
                ranks[i] = s[i];

            _matrix = new Bucket[length];
            _matrix2 = new Bucket[length];

            for (int skip = 1; skip < length; skip *= 2) {
                int[] tmp = ranks;
                ranks = ranksPrev;
                ranksPrev = tmp;

                for (int i = 0; i < length; i++) {
                    _matrix[i].Item1 = ranksPrev[i] + 1;
                    _matrix[i].Item2 = i + skip < length ? ranksPrev[i + skip] + 1 : 0;
                    _matrix[i].Index = i;
                }

                if (!UseQuickSort)
                    RadixSort();
                else
                    Array.Sort(_matrix);

                int rank = 0;

                ranks[_matrix[0].Index] = 0;
                for (int i = 1; i < length; i++) {
                    if (_matrix[i].Item1 != _matrix[i - 1].Item1 || _matrix[i].Item2 != _matrix[i - 1].Item2)
                        rank++;
                    ranks[_matrix[i].Index] = rank;
                }

                if (rank >= length - 1 && !LateQuit)
                    break; // Important optimization
            }

            int[] array = new int[length];
            for (int i = 0; i < length; i++)
                array[i] = _matrix[i].Index;
            Suffixes = array;
            Indices = ranks;
        }

        const int shift = 8;
        const int buckets = 1 << shift;

        void RadixSort()
        {
            RadixSort(b => b.Item2);
            if (_matrix.Length >= 1 << shift) {
                RadixSort(b => b.Item2 >> shift);
                if (_matrix.Length >= 1 << (shift * 2)) {
                    RadixSort(b => b.Item2 >> (shift * 2));
                    if (_matrix.Length >= 1 << (shift * 3))
                        RadixSort(b => b.Item2 >> (shift * 3));
                }
            }

            RadixSort(b => b.Item1);
            if (_matrix.Length >= 1 << shift) {
                RadixSort(b => b.Item1 >> shift);
                if (_matrix.Length >= 1 << (shift * 2)) {
                    RadixSort(b => b.Item1 >> (shift * 2));
                    if (_matrix.Length >= 1 << (shift * 3))
                        RadixSort(b => b.Item1 >> (shift * 3));
                }
            }
        }

        unsafe void RadixSort(Func<Bucket, int> func)
        {
            int* offsets = stackalloc int[buckets + 1];

            for (int i = 0; i < buckets; i++)
                offsets[i] = 0;

            foreach (Bucket b in _matrix)
                offsets[func(b) & (buckets - 1)]++;

            int sum = 0;
            for (int i = 0; i < buckets; i++) {
                int newSum = sum + offsets[i];
                offsets[i] = sum;
                sum = newSum;
            }

            foreach (Bucket b in _matrix)
                _matrix2[offsets[func(b) & (buckets - 1)]++] = b;

            Swap(ref _matrix, ref _matrix2);
        }

        struct Bucket : IComparable<Bucket>
        {
            public int Item1;
            public int Item2;
            public int Index;

            public Bucket(int item1, int item2, int index)
            {
                Item1 = item1;
                Item2 = item2;
                Index = index;
            }

            public int CompareTo(Bucket b)
            {
                int cmp = Item1.CompareTo(b.Item1);
                if (cmp != 0) return cmp;
                return Item2.CompareTo(b.Item2);
            }
        }
    }

    #endregion

    #region Properties

    public int[] Ranks => _ranks ?? (_ranks = BuildRanks(Suffixes));

    public int[] Suffixes { get; private set; }

    public int[] LcpArray => _lcps ?? (_lcps = BuildLongestCommonPrefixes(String, Suffixes, Ranks));

    #endregion

    #region Methods

    public static int[] BuildRanks(int[] suffixArray)
    {
        int[] ranks = new int[suffixArray.Length];
        for (int i = 0; i < suffixArray.Length; i++)
            ranks[suffixArray[i]] = i;
        return ranks;
    }

    /// <summary>
    ///     Builds the longest common prefixes using Kasai's algorithm.
    /// </summary>
    /// <param name="txt">The text.</param>
    /// <param name="suffixArray">The suffix array.</param>
    /// <param name="ranks">The indices.</param>
    /// <param name="isDelim"></param>
    /// <returns></returns>
    public static int[] BuildLongestCommonPrefixes(CharSequence txt,
        int[] suffixArray,
        int[] ranks,
        Func<char, bool> isDelim = null)
    {
        int n = suffixArray.Length;
        if (ranks == null)
            ranks = BuildRanks(suffixArray);

        int[] lcp = new int[n];

        for (int i = 0, k = 0; i < n; i++) {
            if (ranks[i] == n - 1) {
                k = 0;
                continue;
            }

            int j = suffixArray[ranks[i] + 1];
            while (i + k < n && j + k < n && txt[i + k] == txt[j + k] && (isDelim == null || !isDelim(txt[i + k])))
                k++;

            lcp[ranks[i] + 1] = k;
            if (k > 0) k--;
        }

        return lcp;
    }

    public int LongestCommonPrefixOfString(int suffix1, int suffix2) =>
        LongestCommonPrefixOfLcpArray(_ranks[suffix1], _ranks[suffix2]);

    public int LongestCommonPrefixOfLcpArray(int rank1, int rank2)
    {
        if (rank1 > rank2) Swap(ref rank1, ref rank2);
        if (rank1 == rank2) return length - Suffixes[rank1];
        return _rmq.GetMin(rank1 + 1, rank2);
    }

    public void InitializeRMQ()
    {
        if (_rmq != null) return;

        int[] lcps = LcpArray;
        _rmq = new RangeMinimumQuery(lcps);
    }

    public void FindExclusive(int pos, int len, out int left, out int right)
    {
        left = pos;
        right = pos + 1;
        FindExclusive(len, ref left, ref right);
    }

    public void FindExclusive(int len, ref int left, ref int right)
    {
        /*
        var leftSuffixOrig = leftSuffix;
        var rightSuffixOrig = rightSuffix;

        var leftSuffix2 = leftSuffix;
        var rightSuffix2 = rightSuffix;
        while (lcps[rightSuffix2] >= len)
            rightSuffix2++;
        while (lcps[leftSuffix2] >= len && leftSuffix2 >= 0)
            leftSuffix2--;
            */

        int point = right;
        if (_lcps[right] >= len) {
            int dist = 1;
            while (right + dist < _lcps.Length && _rmq.GetMin(point, right + dist) >= len) {
                right += dist;
                dist <<= 1;
            }

            for (; dist > 0; dist >>= 1)
                while (right + dist < _lcps.Length && _rmq.GetMin(point, right + dist) >= len)
                    right += dist;

            if (_lcps[right + 1] < len)
                right++;
        }

        point = left;
        if (_lcps[left] >= len) {
            int dist = 1;
            while (left - dist >= 0 && _rmq.GetMin(left - dist + 1, point) >= len) {
                left -= dist;
                dist <<= 1;
            }

            for (; dist > 0; dist >>= 1)
                while (left - dist >= 0 && _rmq.GetMin(left - dist + 1, point) >= len)
                    left -= dist;
        }

        /*
            Debug.Assert(leftSuffix == leftSuffix2);
            Debug.Assert(rightSuffix == rightSuffix2);
            */
    }

    /*
     * int get(int pp, int len){
            int R = pp;
            int l = pp + 1;
            int r = nn - 1;
            while(l <= r){
                int mid = (l + r)>>1;
                if(getlcp(pp, mid-1) >= len){
                    R = mid;
                    l = mid+1;
                }
                else r = mid-1;
            }
            int L = pp;
            l = 0;
            r = pp - 1;
            while(l <= r){
                int mid = (l + r)>>1;
                if(getlcp(mid, pp-1) >= len){
                    L = mid;
                    r = mid-1;
                }
                else l = mid+1;
            }
            return sum[R] - sum[L-1];
    }*/

    /*
     *
     *
     *
    int getLcpForSuffixes(int l, int r) {
        if (l == r) return n - l;
        l = ra[l]; r = ra[r];
        if (l > r) swap(l, r);
        --r;
        int j = lgt[r - l];
        return min(rmq[j][l], rmq[j][r - (1 << j) + 1]);
    }
    int getLcpForSuffixesIndexes(int l, int r) {
        if (l == r) return n - sa[l];
        if (l > r) swap(l, r);
        --r;
        int j = lgt[r - l];
        return min(rmq[j][l], rmq[j][r - (1 << j) + 1]);
    }

      pair<int, int> getIntervalForSuffixIndex(int sufIndex, int length) {
        int l, r;
        l = sufIndex;
        r = n;
        int L, R;
        while (l < r) {
            int m = (l + r) >> 1;
            if (getLcpForSuffixesIndexes(sufIndex, m) >= length) {
                l = m + 1;
            } else {
                r = m;
            }
        }
        R = r;
        l = 0, r = sufIndex;
        while (l < r) {
            int m = (l + r) >> 1;
            if (getLcpForSuffixesIndexes(sufIndex, m) >= length) {
                r = m;
            } else {
                l = m + 1;
            }
        }
        L = l;
        return make_pair(L, R);
    }

    */
    public int[] ComputeOccurrencesPresums(int stringLength)
    {
        int sum = 0;
        int[] presum = new int[length];
        for (int i = 0; i < presum.Length; i++) {
            presum[i] = sum;
            if (Suffixes[i] < stringLength) sum++;
        }

        return presum;
    }

    public static int Occurrences(int[] presum, int left, int right) => presum[right] - presum[left];

    #endregion
}