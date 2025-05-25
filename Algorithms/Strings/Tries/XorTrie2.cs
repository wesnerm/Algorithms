using System.Runtime.CompilerServices;

namespace Algorithms.Strings.Tries.Alt;

public class XorTrieTop
{
    readonly XorTrie[] branches;
    public List<XorTrie> list = new();
    public XorTrie root;

    public XorTrieTop(int n)
    {
        root = new XorTrie(n);
        branches = XorTrie.CreateQuickArray(root);
    }

    public XorTrie FindMaxXor(string s)
    {
        XorTrie? trie = root;
        while (trie != null) {
            int i = s.Length - trie.Length;
            if (i == s.Length)
                return trie;

            bool zero = i < 0 || s[i] == '0';
            XorTrie next;
            if (zero) {
                next = trie.right;
                if (next == null || next.Count == 0)
                    next = trie.left;
            } else {
                next = trie.left;
                if (next == null || next.Count == 0)
                    next = trie.right;
            }

            trie = next;
        }

        return null;
    }

    public void Compress()
    {
        root = XorTrie.Compress(root);
    }

    void PushDown(XorTrie trie)
    {
        if (trie.Length == 0) return;

        string info = trie.info;
        string s = info;
        int i = s.Length - trie.Length;
        XorTrie next = i < 0 || s[i] == '0' ? trie.left : trie.right;
        next.Count = trie.Count;
        next.info = info;
        trie.info = null;
    }

    public XorTrie FindMaxXorFast(string s)
    {
        XorTrie? trie = root;
        while (trie != null) {
            if (trie.info != null)
                return trie;

            int i = s.Length - trie.Length;
            if (i == s.Length)
                return trie;

            bool zero = i < 0 || s[i] == '0';
            XorTrie next;
            if (zero) {
                next = trie.right;
                if (next == null || next.Count == 0)
                    next = trie.left;
            } else {
                next = trie.left;
                if (next == null || next.Count == 0)
                    next = trie.right;
            }

            trie = next;
        }

        return null;
    }

    public XorTrie InsertFast(string info, int count = 1)
    {
        string s = info;
        XorTrie parent = null;
        XorTrie trie = root;
        while (true) {
            int prevCount = trie.Count;

            if (trie.info != null && trie.info != info)
                PushDown(trie);

            int newCount = prevCount + count;
            trie.Count = newCount;

            int length = trie.Length;
            if (length <= 0) return trie;

            if (prevCount == 0) trie.info = info;

            if (trie.info != null)
                return trie;

            int i = s.Length - length;
            parent = trie;
            trie = i < 0 || s[i] == '0' ? trie.left : trie.right;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XorTrie RemoveFast(string info) => InsertFast(info, -1);

    public XorTrie ReserveSpace(string info) => branches[info.Length].Insert(info, 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XorTrie Insert(string si) => root.Insert(si);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XorTrie Remove(string si) => root.Delete(si);
}

public class XorTrie
{
    public int Count;
    public int Id;
    public string info;
    public XorTrie left;
    public int Length;
    public XorTrie right;

    public XorTrie(int length) => Length = length;

    static int DfsNumber(XorTrie trie, ref int number)
    {
        if (trie == null) return 0;
        trie.Id = number++;
        return 1 + DfsNumber(trie.left, ref number) + DfsNumber(trie.right, ref number);
    }

    static void FillIds(XorTrie trie, XorTrie[] trace)
    {
        while (trie != null) {
            trace[trie.Id] = trie;
            if (trie.right == null) {
                trie = trie.left;
            } else {
                FillIds(trie.left, trace);
                trie = trie.right;
            }
        }
    }

    public static XorTrie[] BuildTrace(XorTrie trie)
    {
        int number = 0;
        int n = DfsNumber(trie, ref number);
        var trace = new XorTrie[n];
        FillIds(trie, trace);
        return trace;
    }

    public static List<int>[] BuildGraph(XorTrie[] trace)
    {
        int n = trace.Length;
        var g = new List<int>[n];
        for (int i = 0; i < n; i++) {
            XorTrie t = trace[i];
            List<int> list = g[i] = new List<int>(2);
            if (t.left != null) list.Add(t.left.Id);
            if (t.right != null) list.Add(t.right.Id);
        }

        return g;
    }

    public static XorTrie Compress(XorTrie trie)
    {
        if (trie == null) return null;
        while ((trie.left == null) ^ (trie.right == null))
            trie = trie.left ?? trie.right;
        trie.left = Compress(trie.left);
        trie.right = Compress(trie.right);
        return trie;
    }

    public static XorTrie[] CreateQuickArray(XorTrie root)
    {
        int n = root.Length;
        var result = new XorTrie[n + 1];

        result[n] = root;
        for (XorTrie trie = root; trie.Length > 0; trie = trie.left)
            result[trie.Length - 1] = trie.left ?? (trie.left = new XorTrie(trie.Length - 1));
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XorTrie Delete(string value) => Insert(value, -1);

    public XorTrie Insert(string s, int count = 1)
    {
        XorTrie root = this;
        while (true) {
            int length = root.Length;
            root.Count += count;

            if (length <= 0)
                return root;

            int i = s.Length - length;
            if (i < 0 || s[i] == '0') {
                if (root.left == null) root.left = new XorTrie(length - 1);
                root = root.left;
            } else {
                if (root.right == null) root.right = new XorTrie(length - 1);
                root = root.right;
            }
        }
    }
}