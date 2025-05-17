#region Copyright
/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
//
// Copyright (C) 2005-2016, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////
#endregion

namespace Algorithms.Strings;

[TestFixture]
public class SuffixAutomatonTest
{
    #region Variables
    // private SuffixAutomaton sample;
    #endregion

    #region Tests
    

    [Test]
    public void ContainsTest()
    {
        var tree = new SuffixAutomaton("THIS IS A TEST TEXT$");

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
    public void NumberOfDistinctSubstrings()
    {

        long NumSubstrings(string w)
        {
            var tree = new SuffixAutomaton(w);
            return tree.NumberOfDistinctSubstrings();
        }

        AreEqual(0, NumSubstrings(""));
        AreEqual(1, NumSubstrings("a"));
        AreEqual(2, NumSubstrings("aa"));
        AreEqual(3, NumSubstrings("ab"));
        AreEqual(5, NumSubstrings("aba"));
        AreEqual(6, NumSubstrings("abc"));
        AreEqual(9, NumSubstrings("abca"));
        // a ab abc abca b bc bca c ca
    }

    [Test]
    public void NodesTest()
    {
        var tree = new SuffixAutomaton("bobocel");
        foreach (var node in tree.GetNodes())
        {
            foreach (var c in node.Children)
                IsTrue(c.Index > node.Index);
        }

    }

    [Test]
    public static void KthDistinctSubstring()
    {
        var sa = new SuffixAutomaton("abca");

        // 0 - a ab abc abca 
        // 4 - b bc bca
        // 7 - c ca

        var dp = sa.PreprocessKthDistinctSubstring();
        AreEqual("a", sa.KthDistinctSubstring(0, dp));
        AreEqual("ab", sa.KthDistinctSubstring(1, dp));
        AreEqual("abc", sa.KthDistinctSubstring(2, dp));
        AreEqual("abca", sa.KthDistinctSubstring(3, dp));
        AreEqual("b", sa.KthDistinctSubstring(4, dp));
        AreEqual("bc", sa.KthDistinctSubstring(5, dp));
        AreEqual("bca", sa.KthDistinctSubstring(6, dp));
        AreEqual("c", sa.KthDistinctSubstring(7, dp));
        AreEqual("ca", sa.KthDistinctSubstring(8, dp));
    }

    /// <summary>
    /// Longest Common Substring
    /// </summary>
    [Test]
    public void LongestCommonSubstringTest()
    {
        string Lcs(string a, string b)
        {
            var sa = new SuffixAutomaton(a);
            return sa.LongestCommonSubstring(b);
        }

        AreEqual("Site:Geeks", Lcs("OldSite:GeeksforGeeks.org", "NewSite:GeeksQuiz.com"));
        AreEqual("", Lcs("abcd", "efgh"));
        AreEqual("", Lcs("", ""));
        AreEqual("a", Lcs("cat", "bag"));
        AreEqual("alphabet", Lcs("alphabet", "alphabet"));
    }

    [Test]
    public void LongestRepeatedSubstring()
    {
        TestLongestRepeatedSubstring("GEEKSFORGEEKS");
        TestLongestRepeatedSubstring("AAAAAAAAAA");
        TestLongestRepeatedSubstring("ABCDEFG");
        TestLongestRepeatedSubstring("ABABABA");
        TestLongestRepeatedSubstring("ATCGATCGA");
        TestLongestRepeatedSubstring("BANANA");
        TestLongestRepeatedSubstring("banana");
        TestLongestRepeatedSubstring("abcpqrabpqpq");
        TestLongestRepeatedSubstring("pqrpqpqabab");
    }

    void TestLongestRepeatedSubstring(string s)
    {
        var tree = new SuffixAutomaton(s);

        tree.LongestRepeatedSubstring(out int start, out int length);
        var lrs = s.Substring(start, length);

        var ci = StringTools.CountInstancesOf(s, lrs);
        IsTrue(length == 0 || ci >= 2);

        int tr = length + 1;
        for (int i = 0; i + tr <= s.Length; i++)
        {
            var sub = s.Substring(i, tr)                ;
            AreEqual(1, StringTools.CountInstancesOf(s, sub));
        }
    }

    [Test]
    public void AllOccurrencesTest()
    {
        var s = "How much wood would a woodchuck chuck if a woodchuck could chuck wood";
        var sa = new SuffixAutomaton(s);
        Console.WriteLine(s);

        var w = "woodchuck";
        var occurrences = sa.Occurrences();
        for (int i = 0; i < w.Length; i++)
            for (int j = i; j < w.Length; j++)
            {
                var ww = w.Substring(i, j - i + 1);
                var occ1 = sa.AllOccurrences(ww);
                var occ2 = new HashSet<int>(StringTools.Instances(s, ww));
                occ1.Sort();
                AreEqual(occ2.Count, occ1.Count);
                AreEqual(occ2.Count, occ2.Intersect(occ1).Count());
                Console.WriteLine($"{ww}) {string.Join(" ",occ1)}");
            }

        AreEqual(0, sa.Occurrences(occurrences, "bee"));
    }

    [Test]
    public void AllOccurrencesTest2()
    {
        CheckIndices("GEEKSFORGEEKS", "GEEKS", "GEEKS1", "FOR");
        CheckIndices("AABAACAADAABAAABAA", "AABA", "AA", "AAE");
        CheckIndices("AAAAAAAAA", "AAAA", "AA", "A", "AB");
    }

    void CheckIndices(string text, params string[] pattern)
    {
        var tree = new SuffixAutomaton(text);
        foreach (var p in pattern)
        {
            var list = tree.AllOccurrences(p).ToList();
            var list2 = StringTools.Instances(text, p).ToList();
            AreEqual(list2.Count, list.Count);
            foreach (var v in list2)
                IsTrue(list.Contains(v));
        }
    }

    [Test]
    public void InverseSuffixLinksTest()
    {
        var s = "How much wood would a woodchuck chuck if a woodchuck could chuck wood";
        var sa = new SuffixAutomaton(s);

        var ilinks = sa.InverseSuffixLinks();
        var nodes = sa.GetNodes();
        foreach (var v in nodes)
        {
            if (v.Link == null) continue;
            IsTrue(ilinks[v.Link.Index].Contains(v.Index));
        }

        foreach (var v in nodes)
        {
            foreach (var v2 in ilinks[v.Index])
                IsTrue(nodes[v2].Link == v);
        }
    }

    [Test]
    public void OccurrencesTest()
    {
        var s = "How much wood would a woodchuck chuck if a woodchuck could chuck wood";
        var sa = new SuffixAutomaton(s);
        Console.WriteLine(s);

        var w = "woodchuck";
        var occurrences = sa.Occurrences();
        for (int i = 0; i < w.Length; i++)
            for (int j = i; j < w.Length; j++)
            {
                var ww = w.Substring(i, j - i + 1);
                var count = sa.Occurrences(occurrences, w, i, j - i + 1);
                var occ = StringTools.Instances(s, ww).ToList();
                AreEqual(occ.Count, count);
                Console.WriteLine($"{ww}) {count}");
            }

        AreEqual(0, sa.Occurrences(occurrences, "bee"));
    }

    /// <summary>
    /// Test for Occurrences(string pattern, int index, int count)
    /// </summary>
    [Test]
    public void OccurrencesTest2()
    {
        var s = "How much wood would a woodchuck chuck if a woodchuck could chuck wood";
        var sa = new SuffixAutomaton(s);
        Console.WriteLine(s);

        var w = "woodchuck";
        var occurrences = sa.Occurrences2();
        for (int i = 0; i < w.Length; i++)
            for (int j = i; j < w.Length; j++)
            {
                var ww = w.Substring(i, j - i + 1);
                var count = sa.Occurrences(occurrences, w, i, j - i + 1);
                var occ = StringTools.Instances(s, ww).ToList();
                AreEqual(occ.Count, count);
                Console.WriteLine($"{ww}) {count}");
            }

        AreEqual(0, sa.Occurrences(occurrences, "bee"));
    }

    [Test]
    public void FirstOccurrencesTest()
    {
        var s = "How much wood would a woodchuck chuck if a woodchuck could chuck wood - a wouldchuck cooda wooda shooda";
        var sa = new SuffixAutomaton(s);
        Console.WriteLine(s);

        var w = "woodchuck";
        for (int i = 0; i < w.Length; i++)
            for (int j = i; j < w.Length; j++)
            {
                var ww = w.Substring(i, j - i + 1);
                var occ = s.IndexOf(ww);
                var occ2 = sa.FirstOccurrence(ww);
                AreEqual(occ, occ2);
                Console.WriteLine($"{ww}) {occ2}");
            }
    }

    [Test]
    public void FirstOccurrencesTest2()
    {
        var s = "a wouldchuck cooda wooda shooda - How much wood would a woodchuck chuck if a woodchuck could chuck wood";
        var sa = new SuffixAutomaton(s);
        Console.WriteLine(s);

        var w = "woodchuck";
        for (int i = 0; i < w.Length; i++)
            for (int j = i; j < w.Length; j++)
            {
                var ww = w.Substring(i, j - i + 1);
                var occ = s.IndexOf(ww);
                var occ2 = sa.FirstOccurrence(ww);
                AreEqual(occ, occ2);
                Console.WriteLine($"{ww}) {occ2}");
            }
    }

    [Test]
    public void FirstOccurrencesTestB()
    {
        var s = "How much wood would a woodchuck chuck if a woodchuck could chuck wood - a wouldchuck cooda wooda shooda";
        var sa = new SuffixAutomaton(s);
        var fo = sa.FirstOccurrences2();
        var w = "woodchuck";
        for (int i = 0; i < w.Length; i++)
            for (int j = i; j < w.Length; j++)
            {
                var ww = w.Substring(i, j - i + 1);
                var occ = s.IndexOf(ww);
                var occ2 = sa.FirstOccurrence2(ww, fo);
                AreEqual(occ, occ2);
                Console.WriteLine($"{ww}) {occ2}");
            }
    }

    [Test]
    public void MinimalCyclicShift()
    {
        AreEqual(0, SuffixAutomaton.MinimalCyclicShift(""));
        AreEqual(0, SuffixAutomaton.MinimalCyclicShift("abc"));
        AreEqual(1, SuffixAutomaton.MinimalCyclicShift("cab"));
        AreEqual(2, SuffixAutomaton.MinimalCyclicShift("bca"));
        AreEqual(3, SuffixAutomaton.MinimalCyclicShift("abca"));
        AreEqual(0, SuffixAutomaton.MinimalCyclicShift("aaaa"));
    }

    #endregion
}