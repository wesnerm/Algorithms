#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the express permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Collections;

/// <summary>
///     Summary description for FastArray.
/// </summary>
public struct StackArray<T>
{
    #region Constant

    public const int Length = 13;

    #endregion

    #region Variables

    public T Elem0;
    public T Elem1;
    public T Elem10;
    public T Elem11;
    public T Elem12;
    public T Elem2;
    public T Elem3;
    public T Elem4;
    public T Elem5;
    public T Elem6;
    public T Elem7;
    public T Elem8;
    public T Elem9;

    #endregion

    public T this[int index] {
        get
        {
            switch (index) {
                case 0:
                    return Elem0;
                case 1:
                    return Elem1;
                case 2:
                    return Elem2;
                case 3:
                    return Elem3;
                case 4:
                    return Elem4;
                case 5:
                    return Elem5;
                case 6:
                    return Elem6;
                case 7:
                    return Elem7;
                case 8:
                    return Elem8;
                case 9:
                    return Elem9;
                case 10:
                    return Elem10;
                case 11:
                    return Elem11;
                case 12:
                    return Elem12;
            }

            throw new IndexOutOfRangeException();
        }
        set
        {
            switch (index) {
                case 0:
                    Elem0 = value;
                    return;
                case 1:
                    Elem1 = value;
                    return;
                case 2:
                    Elem2 = value;
                    return;
                case 3:
                    Elem3 = value;
                    return;
                case 4:
                    Elem4 = value;
                    return;
                case 5:
                    Elem5 = value;
                    return;
                case 6:
                    Elem6 = value;
                    return;
                case 7:
                    Elem7 = value;
                    return;
                case 8:
                    Elem8 = value;
                    return;
                case 9:
                    Elem9 = value;
                    return;
                case 10:
                    Elem10 = value;
                    return;
                case 11:
                    Elem11 = value;
                    return;
                case 12:
                    Elem12 = value;
                    return;
            }

            throw new IndexOutOfRangeException();
        }
    }

    public T[] ToArray()
    {
        return new[]
        {
            Elem0, Elem1, Elem2,
            Elem3, Elem4, Elem5,
            Elem6, Elem7, Elem8,
            Elem9, Elem10, Elem11,
            Elem12,
        };
    }

    public List<T> ToList()
    {
        var list = new List<T>(Length);
        list.Add(Elem0);
        list.Add(Elem1);
        list.Add(Elem2);
        list.Add(Elem3);
        list.Add(Elem4);
        list.Add(Elem5);
        list.Add(Elem6);
        list.Add(Elem7);
        list.Add(Elem8);
        list.Add(Elem9);
        list.Add(Elem10);
        list.Add(Elem11);
        list.Add(Elem12);
        return list;
    }

    [DebuggerStepThrough]
    public void Clear()
    {
        this = default;
    }

    public IEnumerator<T> GetEnumerator()
    {
        IEnumerable<T> en = ToArray();
        return en.GetEnumerator();
    }
}