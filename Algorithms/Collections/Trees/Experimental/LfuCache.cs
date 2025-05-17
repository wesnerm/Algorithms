namespace Algorithms.Collections;

public class LFUCache
{
    Dictionary<int, LinkedListNode<Entry>> _map = new Dictionary<int, LinkedListNode<Entry>>();
    Dictionary<int, LinkedList<Entry>> _freqMap = new Dictionary<int, LinkedList<Entry>>();
    int _minFreq = int.MaxValue;
    int _capacity;

    public LFUCache(int capacity)
    {
        this._capacity = capacity;
    }

    public int Get(int key)
    {
        LinkedListNode<Entry> node;
        if (!_map.TryGetValue(key, out node))
            return -1;
        var entry = node.Value;
        RemoveNode(node);
        AttachNode(node);
        return entry.Value;
    }

    public void Put(int key, int value)
    {
        LinkedListNode<Entry> node;
        if (_map.TryGetValue(key, out node))
        {
            node.Value.Value = value;
            RemoveNode(node);
            AttachNode(node);
            return;
        }

        if (_map.Count == _capacity && _map.Count > 0)
        {
            var kill = _freqMap[_minFreq].First;
            RemoveNode(kill);
            _map.Remove(kill.Value.Key);
        }

        if (_capacity == 0)
            return;

        var entry = new Entry { Key = key, Value = value };
        _map[key] = node = new LinkedListNode<Entry>(entry);
        _minFreq = 1;

        AttachNode(node);
    }

    void AttachNode(LinkedListNode<Entry> node)
    {
        var entry = node.Value;
        var freq = ++entry.Frequency;
        if (!_freqMap.ContainsKey(freq))
            _freqMap[freq] = new LinkedList<Entry>();
        _freqMap[freq].AddLast(node);
    }

    void RemoveNode(LinkedListNode<Entry> node)
    {
        var list = node.List;
        if (list == null) return;
        var freq = node.Value.Frequency;
        list.Remove(node);
        if (list.Count == 0)
        {
            _freqMap.Remove(freq);
            if (_minFreq == freq) _minFreq++;
        }
    }

    class Entry
    {
        public int Key;
        public int Value;
        public int Frequency;
    }
}