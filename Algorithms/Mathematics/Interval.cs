#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Mathematics;

/// <summary>
///     Summary description for Interval.
/// </summary>
public struct Interval
{
    #region Variables

    public int End;
    public int Start;

    #endregion

    #region Construction

    public static Interval<T> New<T>(T t1, T t2) where T : IComparable<T> => new(t1, t2);

    public Interval(int position) => Start = End = position;

    public Interval(int start, int end)
    {
        Start = start;
        End = end;
    }

    #endregion

    static readonly Random _rnd = new();

    public int Length {
        get => End - Start;
        set => End = Start + value;
    }

    public bool IsEmpty {
        get => End <= Start;
        set
        {
            if (value) End = Start;
            else IsInfinite = true;
        }
    }

    public bool IsInfinite {
        get => Start == int.MinValue && End == int.MaxValue;
        set
        {
            if (value) {
                Start = int.MinValue;
                End = int.MaxValue;
            } else {
                IsEmpty = true;
            }
        }
    }

    public bool Contains(int position) => position >= Start && position < End;

    public bool Contains(Interval interval) => interval.Start >= Start && interval.End <= End;

    public void Union(Interval interval)
    {
        Start = Math.Min(interval.Start, Start);
        End = Math.Max(interval.End, End);
    }

    public void Union(int position)
    {
        if (position < Start) Start = position;
        if (position > End) End = position;
    }

    public void Intersect(Interval interval)
    {
        Start = Math.Max(interval.Start, Start);
        End = Math.Min(interval.End, End);
    }

    public override string ToString() => $"({Start},{End})";

    public void Offset(int offset)
    {
        Start += offset;
        End += offset;
    }

    public void Inflate(int amount)
    {
        Start -= amount;
        End += amount;
    }

    public void Normalize()
    {
        if (Start > End) {
            int temp = Start;
            Start = End;
            End = Start;
        }
    }

    public int Random()
    {
        if (Start > End) return _rnd.Next(End, Start);
        return _rnd.Next(Start, End);
    }

    public static bool operator <(Interval i1, Interval i2) => i1.End <= i2.Start;

    public static bool operator >(Interval i1, Interval i2) => i2.End <= i1.Start;
}