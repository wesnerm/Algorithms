namespace Algorithms.Collections.Mutable;

public class QPHashtable<K, V>
{
    readonly Entry[] entries;
    int count;
    int shift = -1;

    public QPHashtable() => entries = Array.Empty<Entry>();

    public QPHashtable(int size)
    {
        if (size == 0) {
            entries = Array.Empty<Entry>();
        } else {
            int capacity = 2 << Log2(((size * 21L) >> 4) + 1);
            entries = new Entry[capacity];
        }
    }

    public bool Add(K item, V value)
    {
        int count = this.count;
        ref V slot = ref GetSlot(item);
        slot = value;
        return count != this.count;
    }

    public bool Remove(K item)
    {
        var equatable = EqualityComparer<K>.Default;
        int index = FindSlot(item);
        if (index >= 0) {
            count--;
            entries[index] = default;
            entries[index].hash = -1;
            return true;
        }

        return false;
    }

    public bool Contains(K item) => FindSlot(item) != -1;

    public K GetKey(K item)
    {
        int index = FindSlot(item);
        return index >= 0 ? entries[index].key : default;
    }

    public ref V GetSlot(K item)
    {
        int index = FindSlot(item, true);
        return ref entries[index].value;
    }

    public int GetHashCode(K item)
    {
        var equatable = EqualityComparer<K>.Default;
        return equatable.GetHashCode(item);
    }

    int ToIndex(int hash)
    {
        hash ^= hash >> 16;
        hash ^= hash >> 8;
        hash ^= hash >> 4;
        hash ^= hash >> 2;
        hash ^= hash >> 1;
        return hash;
    }

    int FindSlot(K key, bool create = false)
    {
        int hash = GetHashCode(key);
        int n = entries.Length;
        int index = ToIndex(hash) & (n - 1);
        int deleted = -1;
        var equatable = EqualityComparer<K>.Default;
        for (int i = 1; i <= n; i++) {
            int h = entries[index].hash;
            if (h < 0) {
                if (h == -1) return index;
                if (h == -2 && deleted != -1) deleted = index;
            } else if (h == hash && equatable.Equals(entries[index].key, key)) {
                return index;
            }

            index = (index + i) & (n - 1);
        }

        index = deleted;
        if (create && index != -1) {
            // TODO: Need to expand table

            count++;
            entries[index].key = key;
            entries[index].hash = hash;
        }

        return index;
    }

    public struct Entry
    {
        public int hash;
        public K key;
        public V value;
    }
}