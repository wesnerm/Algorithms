namespace Algorithms.Collections;

public struct ListMap<K, V> where K : IComparable<K>
{
    Node head;

    public class Node
    {
        public K Key;
        public Node Next;
        public V Value;
    }

    public ListMap<K, V> Clone()
    {
        Node list = null;

        for (Node? cur = head; cur != null; cur = cur.Next) {
            var node = new Node { Next = list, Key = cur.Key, Value = cur.Value };
            list = node;
        }

        for (Node? cur = list; cur != null;) {
            Node node = cur;
            cur = cur.Next;
            node.Next = head;
            head = node;
        }

        return new ListMap<K, V> { head = list };
    }

    public V this[K key] {
        get
        {
            for (Node? cur = head; cur != null; cur = cur.Next) {
                int cmp = key.CompareTo(cur.Key);
                if (cmp > 0) break;
                if (cmp == 0) return cur.Value;
            }

            return default;
        }

        set
        {
            Node prev = null;
            Node cur;
            for (cur = head; cur != null; prev = cur, cur = cur.Next) {
                int cmp = key.CompareTo(cur.Key);
                if (cmp > 0) break;
                if (cmp == 0) {
                    cur.Value = value;
                    return;
                }
            }

            var node = new Node { Key = key, Value = value, Next = cur };
            if (prev == null)
                head = node;
            else
                prev.Next = node;
        }
    }

    public bool Remove(K key)
    {
        Node prev = null;
        Node cur;
        for (cur = head; cur != null; prev = cur, cur = cur.Next) {
            int cmp = key.CompareTo(cur.Key);
            if (cmp > 0) break;
            if (cmp == 0) {
                if (prev == null) head = cur.Next;
                else prev.Next = cur.Next;
                return true;
            }
        }

        return false;
    }

    public IEnumerable<K> Keys {
        get
        {
            for (Node? cur = head; cur != null; cur = cur.Next)
                yield return cur.Key;
        }
    }

    public IEnumerable<V> Values {
        get
        {
            for (Node? cur = head; cur != null; cur = cur.Next)
                yield return cur.Value;
        }
    }

    public IEnumerable<KeyValuePair<K, V>> GetEnumerator()
    {
        for (Node? cur = head; cur != null; cur = cur.Next)
            yield return new KeyValuePair<K, V>(cur.Key, cur.Value);
    }
}