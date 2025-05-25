namespace Algorithms.Strings;

[TestFixture]
public class SuffixTreeTest
{
    /// <summary>
    ///     Test for EdgeLength(Node n)
    /// </summary>
    [Ignore("EdgeLength is not yet implemented")]
    public void EdgeLengthTest()
    {
        // var obj = new SuffixTree();
        // var expected = obj.EdgeLength();
        // var actual = default(int);
        // Assert.AreEqual(expected, actual, "EdgeLength");
        Fail();
    }

    /// <summary>
    ///     Test for OutputSuffixTree(Node n, StringBuilder builder)
    /// </summary>
    [Ignore("OutputSuffixTree is not yet implemented")]
    public void OutputSuffixTreeTest()
    {
        // var obj = new SuffixTree();
        // var expected = obj.OutputSuffixTree();
        // var actual = default(void);
        // Assert.AreEqual(expected, actual, "OutputSuffixTree");
        Fail();
    }

    /// <summary>
    ///     Test for BuildSuffixArray()
    /// </summary>
    [Test]
    public void BuildSuffixArrayTest()
    {
        CheckSuffixArray("banana$");
        CheckSuffixArray("GEEKSFORGEEKS$");
        CheckSuffixArray("AAAAAAAAAA$");
        CheckSuffixArray("ABCDEFG$");
        CheckSuffixArray("ABABABA$");
        CheckSuffixArray("abcabxabcd$");
        CheckSuffixArray("CCAAACCCGATTA$");
    }

    [Test]
    public static void SuffixArray()
    {
        var tree = new SuffixTree("bobocel$");
        var sa = new SuffixArray(tree.GetSuffixArray());
        int[] v = sa.Ranks;

        AreEqual(7, v.Length);
        AreEqual(0, v[0]);
        AreEqual(5, v[1]);
        AreEqual(1, v[2]);
        AreEqual(6, v[3]);
        AreEqual(2, v[4]);
        AreEqual(3, v[5]);
        AreEqual(4, v[6]);
    }

    void CheckSuffixArray(string s)
    {
        var suffixTree = new SuffixTree(s + "$");
        int[] suffixArray = suffixTree.GetSuffixArray();
        CheckSuffixArray(s, suffixArray);
    }

    public static void CheckSuffixArray(string s, int[] suffixArray, int extra = 0)
    {
        AreEqual(s.Length + extra, suffixArray.Length);
        string prev = s.Substring(suffixArray[0]);
        for (int i = 1; i < suffixArray.Length; i++) {
            string cur = s.Substring(suffixArray[i]);
            IsTrue(string.CompareOrdinal(prev, cur) <= 0);
            prev = cur;
        }
    }

    /// <summary>
    ///     Test for BuildSuffixArray()
    /// </summary>
    [Test]
    public void ContainsSubstringTest()
    {
        var tree = new SuffixTree("THIS IS A TEST TEXT$");

        IsTrue(tree.ContainsSubstring("TEST"));
        IsTrue(tree.ContainsSubstring("A"));
        IsTrue(tree.ContainsSubstring(" "));
        IsTrue(tree.ContainsSubstring("IS A"));
        IsTrue(tree.ContainsSubstring(" IS A "));
        IsFalse(tree.ContainsSubstring("TEST1"));
        IsFalse(tree.ContainsSubstring("THIS IS GOOD"));
        IsTrue(tree.ContainsSubstring("TES"));
        IsFalse(tree.ContainsSubstring("TESA"));
        IsFalse(tree.ContainsSubstring("ISB"));
    }

    [Test]
    public void IndicesOfTest()
    {
        CheckIndices("GEEKSFORGEEKS$", "GEEKS", "GEEKS1", "FOR");
        CheckIndices("AABAACAADAABAAABAA$", "AABA", "AA", "AAE");
        CheckIndices("AAAAAAAAA$", "AAAA", "AA", "A", "AB");
    }

    void CheckIndices(string text, params string[] pattern)
    {
        var tree = new SuffixTree(text);
        foreach (string p in pattern) {
            List<int> list = tree.IndicesOf(p).ToList();
            List<int> list2 = StringTools.Instances(text, p).ToList();
            AreEqual(list2.Count, list.Count);
            foreach (int v in list2)
                IsTrue(list.Contains(v));
        }
    }

    [Test]
    public void LongestRepeatedSubstring()
    {
        TestLongestRepeatedSubstring("GEEKSFORGEEKS$");
        TestLongestRepeatedSubstring("AAAAAAAAAA$");
        TestLongestRepeatedSubstring("ABCDEFG$");
        TestLongestRepeatedSubstring("ABABABA$");
        TestLongestRepeatedSubstring("ATCGATCGA$");
        TestLongestRepeatedSubstring("banana$");
        TestLongestRepeatedSubstring("abcpqrabpqpq$");
        TestLongestRepeatedSubstring("pqrpqpqabab$");
    }

    void TestLongestRepeatedSubstring(string s)
    {
        var tree = new SuffixTree(s);

        tree.LongestRepeatedSubstring(out int start, out int length);

        int ci = StringTools.CountInstancesOf(s, s.Substring(start, length));
        IsTrue(length == 0 || ci >= 2);

        int tr = length + 1;
        for (int i = 0; i + tr <= s.Length; i++)
            AreEqual(1, StringTools.CountInstancesOf(s, s.Substring(i, tr)));
    }
    // private SuffixTree sample;
}