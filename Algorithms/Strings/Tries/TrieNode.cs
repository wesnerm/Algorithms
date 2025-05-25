namespace Algorithms.Strings;

public class TrieNode<T> where T : TrieNode<T>
{
    public bool IsTerminal;
    public char Key;
    int KeyMask;
    public T[] Next;
    public byte NextCount;

    public TrieNode() => Next = Array.Empty<T>();

    public T this[char ch] {
        get
        {
            if (KeyMask << ~ch < 0) {
                int left = 0;
                int right = NextCount - 1;
                while (left <= right) {
                    int mid = (left + right) >> 1;
                    T val = Next[mid];
                    int cmp = val.Key - ch;
                    if (cmp < 0)
                        left = mid + 1;
                    else if (cmp > 0)
                        right = mid - 1;
                    else
                        return val;
                }
            }

            return null;
        }
        set
        {
            int left = 0;
            int right = NextCount - 1;
            while (left <= right) {
                int mid = (left + right) >> 1;
                T val = Next[mid];
                int cmp = val.Key - ch;
                if (cmp < 0) {
                    left = mid + 1;
                } else if (cmp > 0) {
                    right = mid - 1;
                } else {
                    Next[mid] = value;
                    return;
                }
            }

            if (NextCount >= Next.Length)
                Array.Resize(ref Next, Math.Max(2, NextCount * 2));
            if (NextCount > left)
                Array.Copy(Next, left, Next, left + 1, NextCount - left);
            NextCount++;
            Next[left] = value;
            KeyMask |= 1 << ch;
        }
    }

    /// <summary>
    ///     Return child nodes
    /// </summary>
    public IEnumerable<T> Children {
        get
        {
            for (int i = 0; i < NextCount; i++)
                yield return Next[i];
        }
    }

    /// <summary>
    ///     Clones an node
    /// </summary>
    /// <returns></returns>
    public T Clone()
    {
        var node = (T)MemberwiseClone();
        node.Next = (T[])node.Next.Clone();
        return node;
    }
}