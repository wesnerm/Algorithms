
namespace Algorithms.Collections;

public class LruCache
{
    struct Entry
    {
        public int Key, Value;
    }

    private readonly LinkedList<Entry> _list = new LinkedList<Entry>();
    private readonly Dictionary<int, LinkedListNode<Entry>> _hash = new Dictionary<int, LinkedListNode<Entry>>();
    public int Capacity;

    public LruCache(int capacity)
    {
        Capacity = capacity;
    }

    public int this[int key]
    {
        get
        {
            LinkedListNode<Entry> result;
            if (!_hash.TryGetValue(key, out result))
                return -1;
            _list.AddFirst(result);
            return result.Value.Value;
        }
        set
        {
            LinkedListNode<Entry> result;
            if (_hash.TryGetValue(key, out result))
                _list.Remove(result);

            _hash[key] = _list.AddFirst(new Entry { Key = key, Value = value });
            while (_hash.Count > Capacity)
            {
                var node = _list.Last;
                _list.Remove(node);
                _hash.Remove(node.Value.Key);
            }
        }
    }
}