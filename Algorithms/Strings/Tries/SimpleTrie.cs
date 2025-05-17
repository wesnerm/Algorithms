using System.Text;

namespace Algorithms.Strings;
public class SimpleTrie
{
    public char Ch;
    public bool EndOfWord;
    public int Size;
    public int WordCount;

    public SimpleTrie NextSibling;
    public SimpleTrie FirstChild;

    public string word;

    public SimpleTrie Insert(string word)
    {
        var p = Find(word, true);
        p.word = word;

        var trie = this;
        foreach (var ch in word)
        {
            trie.Size++;
            trie = trie.MoveNext(ch, true);
        }

        trie.word = word;
        trie.WordCount++;
        trie.Size++;
        return trie;
    }

    public void Insert(IEnumerable<string> words)
    {
        foreach (var w in words)
            Insert(w);
    }

    public void Delete(string word)
    {
        if (!Contains(word))
            return;

        var trie = this;
        foreach (var ch in word)
        {
            trie.Size--;
            var child = trie.MoveNext(ch, false);
            if (child.Size == 1)
            {
                DeleteNext(ch);
                return;
            }
            trie = child;
        }

        if (--trie.WordCount == 0)
            trie.word = null;
        trie.Size--;
    }

    public bool StartsWith(string word)
    {
        return Find(word) != null;
    }

    public bool Contains(string word)
    {
        var trie = Find(word);
        return trie?.word != null;
    }

    public SimpleTrie Find(string word, bool create = false)
    {
        var p = this;
        foreach (var c in word)
        {
            p = p.MoveNext(c, create);
            if (p == null)
                break;
        }
        return p;
    }

    public SimpleTrie MoveNext(char ch, bool create = false)
    {
        SimpleTrie prev = null;
        SimpleTrie newChild = null;
        SimpleTrie child = FirstChild;
        for (; child != null; prev = child, child = child.NextSibling)
        {
            if (child.Ch > ch) break;
            if (child.Ch == ch) return child;
        }

        if (create)
        {
            newChild = new SimpleTrie
            {
                Ch = ch,
                NextSibling = child
            };
            if (prev == null)
                FirstChild = newChild;
            else
                prev.NextSibling = newChild;
        }
        return newChild;
    }

    SimpleTrie DeleteNext(char ch)
    {
        SimpleTrie prev = null;
        for (SimpleTrie child = FirstChild; child != null; prev=child, child=child.NextSibling)
        {
            if (child.Ch < ch) continue;
            if (child.Ch > ch) break;
            if (prev == null)
                FirstChild = child.NextSibling;
            else
                prev.NextSibling = child.NextSibling;
            child.NextSibling = null;
            return child;
        }
        return null;
    }

    public override string ToString()
    {
        var kids = new StringBuilder();
        kids.Append('[');
        foreach (var child in GetChildren())
        {
            var ch = child.Ch;

            int len = kids.Length;
            if (len>=2 && kids[len-1] + 1 == ch)
            {
                if (kids[len - 2] + 1 == kids[len - 1])
                    // Case 1: abcd
                    kids[len - 1] = '-';
                else if (kids[len-2] == '-')
                    // Case 2: a-cd
                    kids.Length--; 
            }
            kids.Append(ch);
        }
        kids.Append(']');
        return kids.ToString();
    }

    public IEnumerable<SimpleTrie> GetChildren()
    {
        for (var child = FirstChild; child != null; child = child.NextSibling)
            yield return child;
    }

}