namespace Algorithms.Collections;

public class LFUCache
{
    readonly int _capacity;
    readonly Dictionary<int, LinkedList<Entry>> _freqMap = new();
    readonly Dictionary<int, LinkedListNode<Entry>> _map = new();
    int _minFreq = int.MaxValue;

    public LFUCache(int capacity) => _capacity = capacity;

    public int Get(int key)
    {
        LinkedListNode<Entry> node;
        if (!_map.TryGetValue(key, out node))
            return -1;
        Entry entry = node.Value;
        RemoveNode(node);
        AttachNode(node);
        return entry.Value;
    }

    public void Put(int key, int value)
    {
        LinkedListNode<Entry> node;
        if (_map.TryGetValue(key, out node)) {
            node.Value.Value = value;
            RemoveNode(node);
            AttachNode(node);
            return;
        }

        if (_map.Count == _capacity && _map.Count > 0) {
            LinkedListNode<Entry>? kill = _freqMap[_minFreq].First;
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
        Entry entry = node.Value;
        int freq = ++entry.Frequency;
        if (!_freqMap.ContainsKey(freq))
            _freqMap[freq] = new LinkedList<Entry>();
        _freqMap[freq].AddLast(node);
    }

    void RemoveNode(LinkedListNode<Entry> node)
    {
        LinkedList<Entry>? list = node.List;
        if (list == null) return;
        int freq = node.Value.Frequency;
        list.Remove(node);
        if (list.Count == 0) {
            _freqMap.Remove(freq);
            if (_minFreq == freq) _minFreq++;
        }
    }

    class Entry
    {
        public int Frequency;
        public int Key;
        public int Value;
    }
}