namespace Algorithms.Strings.Tries;

public class XorTrie
{
    public int Count;
    XorTrie left;
    XorTrie right;
    private int Bit;

    public XorTrie(int bits)
    {
        Bit = bits - 1;
    }

    protected XorTrie()
    {

    }

    public XorTrie Insert(long value)
    {
        var result = this;
        return Insert(ref result, value, Bit);
    }

    static XorTrie Insert(ref XorTrie top, long value, int bit)
    {
        if (top == null)
            top = new XorTrie { Count = 1, Bit = bit };
        else
            top.Count++;

        if (bit < 0)
            return top;

        var mask = 1L << bit;
        return (mask & value) == 0
            ? Insert(ref top.left, value, bit - 1)
            : Insert(ref top.right, value, bit - 1);
    }

    public XorTrie Find(long v)
    {
        var t = this;
        while (t.Bit >= 0)
        {
            t = (v & 1L << t.Bit) == 0 ? t.left : t.right;
            if (t == null)
                return null;
        }
        return t;
    }

    public bool Delete(long value)
    {
        return Delete(this, value, Bit);
    }

    static bool Delete(XorTrie top, long value, int bit)
    {
        if (top == null || top.Count == 0)
            return false;

        if (bit < 0)
        {
            top.Count--;
            return true;
        }

        var mask = 1L << bit;
        var deleted = (mask & value) == 0
            ? Delete(top.left, value, bit - 1)
            : Delete(top.right, value, bit - 1);
        if (deleted)
            top.Count--;
        return deleted;
    }

    public XorTrie Merge(XorTrie trie)
    {
        return Merge(this, trie);
    }

    public static XorTrie Merge(XorTrie a, XorTrie b)
    {
        if (a == null || b == null)
            return a ?? b;

        a.Count += b.Count;
        a.left = Merge(a.left, b.left);
        a.right = Merge(a.right, b.right);
        return a;
    }

    public int CountLess(long k, long xor = 0)
    {
        var t = this;
        int result = 0;
        while (t != null && t.Bit >= 0)
        {
            var mask = 1L << t.Bit;
            bool kOn = (k & mask) != 0;
            bool flip = (xor & mask) != 0;

            var left = flip ? t.right : t.left;
            var right = flip ? t.left : t.right;

            if (kOn)
            {
                result += left != null ? left.Count : 0;
                t = right;
            }
            else
            {
                t = left;
            }
        }
        return result;
    }

    public int Contains(long v)
    {
        var t = this;
        while (t.Bit >= 0)
        {
            t = (v & 1L << t.Bit) == 0 ? t.left : t.right;
            if (t == null)
                return 0;
        }
        return t.Count;
    }

    public long Next(long v)
    {
        return Next(this, v);
    }

    static long Next(XorTrie trie, long v)
    {
        if (trie == null || trie.Count == 0 || trie.Bit < 0)
            return -1;

        var bit = 1L << trie.Bit;
        long next;
        if ((v & bit) == 0)
        {
            next = Next(trie.left, v);
            if (next != -1) return next;
            next = trie.right.First();
        }
        else
        {
            next = Next(trie.right, v);
        }

        if (next != -1)
            next += bit;
        return next;
    }

    public long First()
    {
        if (this.Count == 0) return -1;

        var t = this;
        long result = 0;
        while (t != null && t.Bit > 0)
        {
            if (t.left != null && t.left.Count > 0)
                t = t.left;
            else
            {
                t = t.right;
                result += 1L << t.Bit;
            }
        }
        return result;
    }

    public long Last()
    {
        if (this.Count == 0) return -1;

        var t = this;
        long result = 0;
        while (t != null && t.Bit > 0)
        {
            if (t.right != null && t.right.Count > 0)
            {
                t = t.right;
                result += 1L << t.Bit;
            }
            else
            {
                t = t.left;
            }
        }
        return result;
    }

}
