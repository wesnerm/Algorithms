namespace Algorithms.Strings;

public partial class SuffixAutomaton
{
    #region Diagnostics

    /// <summary>
    ///     Extracts a string representation of node
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public string Word(Node node)
    {
        if (node == null) return null;

        int len = 0;
        for (Node tmp = node; !tmp.IsTerminal; tmp = tmp.Next[0])
            len++;

        int length = node.Len;
        int start = End.Len - len - length;
        return GetString(start, length);
    }

    public string GetString(int start, int count)
    {
        if (Text is string)
            return ((string)Text).Substring(start, count);
        if (Text is char[])
            return new string((char[])Text, start, count);
        throw new InvalidOperationException();
    }

    public string Label(Node n) => n.Index + (n.IsCloned ? "C" : "");

    #endregion

    #region Sample Problems

    public int Occurrences(int[] occurrences, string pattern, int index, int count)
    {
        Node? node = FindNode(pattern, index, count);
        if (node == null) return 0;
        return occurrences[node.Index];
    }

    public int Occurrences(int[] occurrences, string pattern) => Occurrences(occurrences, pattern, 0, pattern.Length);

    public int[] Occurrences()
    {
        int[] counts = new int[NodeCount];
        foreach (Node n in NodesBottomUp()) {
            int count = 0;
            for (int i = 0; i < n.NextCount; i++) {
                Node child = n.Next[i];
                count += counts[child.Index];
            }

            if (n.IsTerminal)
                count++;
            counts[n.Index] = count;
        }

        return counts;
    }

    public int FirstOccurrence(string w)
    {
        Node? node = FindNode(w);
        return node != null ? node.FirstOccurrence - w.Length : -1;
    }

    public List<int> AllOccurrences(string w)
    {
        SummarizedAutomaton();
        var list = new List<int>();
        Node? node = FindNode(w);
        if (node != null)
            DfsAllOccurrences(node, w.Length, list);
        return list;
    }

    void DfsAllOccurrences(Node n, int length, List<int> list)
    {
        if (n.IsTerminal)
            list.Add(End.Len - length);

        foreach (Node c in n.Children)
            DfsAllOccurrences(
                _summary[c.Index].Node,
                length + _summary[c.Index].Length,
                list);
    }

    public long NumberOfDistinctSubstrings()
    {
        Node[] nodes = GetNodes();
        long[] dp = new long[NodeCount];
        dp[0] = 1;
        long ret = 0;
        for (int i = 0; i < NodeCount; i++) {
            Node n = nodes[i];
            ret += dp[i];
            for (int j = 0; j < n.NextCount; j++)
                dp[n.Next[j].Index] += dp[i];
        }

        return ret - 1;
    }

    public long[] PreprocessKthDistinctSubstring()
    {
        long[] counts = new long[NodeCount];
        foreach (Node node in NodesBottomUp()) {
            long count = 1;
            for (int j = 0; j < node.NextCount; j++)
                count += counts[node.Next[j].Index];
            counts[node.Index] = count;
        }

        return counts;
    }

    public string KthDistinctSubstring(long k, long[] counts)
    {
        if (k >= counts[0] - 1)
            return null;

        Node cur = Start;
        var sb = new StringBuilder();
        for (; k >= 0; k--)
        for (int j = 0; j < cur.NextCount; j++) {
            Node next = cur.Next[j];
            int index = next.Index;
            if (k - counts[index] < 0) {
                sb.Append(next.Key);
                cur = next;
                break;
            }

            k -= counts[index];
        }

        return sb.ToString();
    }

    public static int MinimalCyclicShift(string s)
    {
        var sa = new SuffixAutomaton(s + s);
        Node node = sa.Start;
        int len = 0;
        while (node.NextCount > 0) {
            node = node.Next[0];
            len++;
        }

        return sa.End.Len - len;
    }

    public void LongestRepeatedSubstring(out int start, out int maxLength)
    {
        start = 0;
        maxLength = 0;
        foreach (Node n in GetNodes()) {
            int count = n.NextCount + (n.IsTerminal ? 1 : 0);
            if (count < 2) continue;
            int length = n.Len;
            if (maxLength < length) {
                maxLength = length;
                start = (n.Original ?? n).Len - length;
            }
        }
    }

    /// <summary>
    ///     Returns the longest common substring of the automaton text and a given string
    /// </summary>
    /// <param name="pattern">pattern to search for</param>
    /// <param name="start">index of substring of pattern</param>
    /// <param name="length">length of substring of pattern</param>
    public void LongestCommonSubstring(string pattern, out int start, out int length)
    {
        start = 0;
        length = 0;
        if (pattern.Length == 0) return;
        Node v = Start;
        int len = 0;
        for (int i = 0; i < pattern.Length; i++) {
            while (v != Start && v[pattern[i]] == null) {
                v = v.Link;
                len = v.Len;
            }

            if (v[pattern[i]] != null) {
                v = v[pattern[i]];
                len++;
            }

            if (len > length) {
                length = len;
                start = i;
            }
        }

        start += 1 - length;
    }

    /// <summary>
    ///     Returns longest common substring
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public string LongestCommonSubstring(string pattern)
    {
        int start;
        int length;
        LongestCommonSubstring(pattern, out start, out length);
        return pattern.Substring(start, length);
    }

    #endregion

    #region Alternative Approaches

    public int[] Occurrences2()
    {
        int[] occurrences = new int[NodeCount];
        SortTopologically(); // necessary
        foreach (Node node in NodesBottomUp()) {
            if (node == Start) break;
            if (!node.IsCloned) occurrences[node.Index]++;
            occurrences[node.Link.Index] += occurrences[node.Index];
        }

        return occurrences;
    }

    /// <summary>
    ///     This is used if the existing first occurrences code is incorrect!
    /// </summary>
    /// <returns></returns>
    public int FirstOccurrence2(string w, int[] fo)
    {
        Node? node = FindNode(w);
        return node != null ? fo[node.Index] - w.Length : -1;
    }

    public int[] FirstOccurrences2()
    {
        Node[] nodes = SortTopologically();
        int[] fo = new int[NodeCount];

        for (int i = 0; i < fo.Length; i++)
            fo[i] = int.MaxValue;

        for (int i = NodeCount - 1; i >= 0; i--) {
            Node node = nodes[i];
            if (!node.IsCloned)
                fo[i] = node.Len;

            if (i > 0 && node.Link.IsCloned && fo[i] < fo[node.Link.Index])
                fo[node.Link.Index] = fo[i];
        }

        return fo;
    }

    public List<int>[] InverseSuffixLinks()
    {
        Node[] nodes = GetNodes();
        var lists = new List<int>[NodeCount];
        for (int i = 0; i < NodeCount; i++)
            lists[i] = new List<int>();
        foreach (Node n in nodes)
            if (n.Link != null)
                lists[n.Link.Index].Add(n.Index);
        return lists;
    }

    /// <summary>
    ///     More carefully sorted list of nodes that orders suffix links properly
    /// </summary>
    /// <returns></returns>
    public Node[] SortTopologically()
    {
        int[] indeg = new int[NodeCount];
        Node[] nodes = GetNodes();
        for (int i = 0; i < NodeCount; i++) {
            Node cur = nodes[i];
            for (int j = 0; j < cur.NextCount; j++)
                indeg[cur.Next[j].Index]++;
        }

        var sorted = new Node[NodeCount];
        sorted[0] = Start;
        int p = 1;
        for (int i = 0; i < NodeCount; i++) {
            Node cur = sorted[i];
            for (int j = 0; j < cur.NextCount; j++)
                if (--indeg[cur.Next[j].Index] == 0)
                    sorted[p++] = cur.Next[j];
        }

        _nodes = sorted;
        UpdateNodeIndices();
        return sorted;
    }

    public static void SetVirtualNode<T>(T[][] store, Node node, int len, T value)
    {
        T[]? array = store[node.Index];
        if (array == null)
            store[node.Index] = array = new T[node.Len - node.Link.Len];
        array[len - node.Link.Len] = value;
    }

    public static T GetVirtualNode<T>(T[][] store, Node node, int len)
    {
        T[]? array = store[node.Index];
        return array != null ? array[len - node.Link.Len] : default;
    }

    public static void SetVirtualNode<T>(Dictionary<long, T> store, Node node, int len, T value)
    {
        store[Code(node, len)] = value;
    }

    public static T GetVirtualNode<T>(Dictionary<long, T> store, Node node, int len)
    {
        T result;
        store.TryGetValue(Code(node, len), out result);
        return result;
    }

    public static bool GetVirtualNode<T>(Dictionary<long, T> store, Node node, int len, out T result) =>
        store.TryGetValue(Code(node, len), out result);

    static long Code(Node node, int len) => (long)len << (21 + node.Index);

    public static bool GetVirtualNode(BitArray[] store, Node node, int len)
    {
        BitArray? array = store[node.Index];
        return array != null && array[len - node.Link.Len];
    }

    public static void SetVirtualNode(BitArray[] store, Node node, int len, bool value)
    {
        BitArray? array = store[node.Index];
        if (array == null) store[node.Index] = array = new BitArray(node.Len - node.Link.Len);
        array[len - node.Link.Len] = value;
    }

    public class FastSubstring
    {
        readonly Node[] nodes;
        readonly int[,] table;
        readonly int[] table2;
        SuffixAutomaton sa;

        public FastSubstring(SuffixAutomaton sa, char[] str = null)
        {
            this.sa = sa;
            nodes = sa.GetNodes();
            int nodeCount = sa.NodeCount;
            int lg = BitTools.Log2(nodeCount) + 1;
            table = new int[nodeCount, lg];
            for (int i = 0; i < nodeCount; i++)
                table[i, 0] = nodes[i].Link.Index;
            for (int j = 1; j < lg; ++j)
            for (int i = 0; i < nodeCount; ++i)
                table[i, j] = table[table[i, j - 1], j - 1];

            if (str != null) {
                table2 = new int[nodeCount + 1];
                Node state = sa.Start;
                for (int i = 0; i < nodeCount; i++) {
                    state = state[str[i]];
                    table2[i + 1] = state.Index;
                }
            }
        }

        public Node GetNthSuffixLink(Node node, int nth)
        {
            int nodeIndex = node.Index;
            int originalLength = node.Len;
            for (int i = table.GetLength(1) - 1; i >= 0; --i) {
                int t = table[nodeIndex, i];
                if (t != 0 && originalLength - nodes[t].Link.Len < nth)
                    nodeIndex = t;
            }

            if (originalLength - nodes[nodeIndex].Link.Len < nth)
                nodeIndex = nodes[nodeIndex].Link.Index;

            return nodes[nodeIndex];
        }

        public Node GetSubstringExclusive(int start, int end) => GetNthSuffixLink(nodes[table2[end]], start);
    }

    #endregion
}