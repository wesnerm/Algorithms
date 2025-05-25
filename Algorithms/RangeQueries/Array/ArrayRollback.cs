using System.Runtime.CompilerServices;

namespace Algorithms.Collections;

public class ArrayRollback<T>
{
    #region Construction

    public ArrayRollback(int n, int bufferSize = -1)
    {
        if (bufferSize == -1) bufferSize = n;
        _values = new T[bufferSize];
        _indices = new int[bufferSize];
        Array = new T[n];
        Length = n;
        _time = 0;
    }

    #endregion

    #region Variables

    T[] _values;
    int[] _indices;
    int _time;
    public readonly T[] Array;
    public readonly int Length;

    #endregion

    #region Properties

    public T this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Array[index];
        set
        {
            int time = _time;
            if (time >= 0) {
                if (time >= _indices.Length) {
                    int newSize = Math.Max(4, time * 2);
                    System.Array.Resize(ref _values, newSize);
                    System.Array.Resize(ref _indices, newSize);
                }

                _indices[time] = index;
                _values[time] = Array[index];
                _time++;
            }

            Array[index] = value;
        }
    }

    public int Time {
        get => _time;
        set
        {
            if (value < 0) {
                _time = -1;
                return;
            }

            while (value < _time) {
                _time--;
                Array[_indices[_time]] = _values[_time];
                _values[_time] = default;
            }

            if (_time < 0)
                _time = 0;
        }
    }

    #endregion
}