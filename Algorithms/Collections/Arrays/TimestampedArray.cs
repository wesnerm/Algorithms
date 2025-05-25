namespace Algorithms.Collections;

public class TimestampedArray<T>
{
    public T[] Array;
    public T DefaultValue;
    public int Time;
    public int[] TimeStamp;

    public TimestampedArray(int size, T defaultValue) : this(size)
    {
        DefaultValue = defaultValue;
        Time = 1;
    }

    public TimestampedArray(int size)
    {
        Array = new T[size];
        TimeStamp = new int[size];
    }

    public T this[int x] {
        get => TimeStamp[x] >= Time ? Array[x] : DefaultValue;
        set
        {
            Array[x] = value;
            TimeStamp[x] = Time;
        }
    }

    public bool ContainsKey(int x) => TimeStamp[x] >= Time;

    public void InitializeAll()
    {
        for (int i = 0; i < Array.Length; i++) {
            if (TimeStamp[i] > Time) continue;
            Array[i] = DefaultValue;
            TimeStamp[i] = Time;
        }
    }

    public void Clear()
    {
        Time++;
    }
}