#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using Algorithms.Collections;

#endregion

namespace Algorithms.Collections;

/// <summary>
///     Summary description for Text.Position.
/// </summary>
public class Position<T> : IComparable<Position<T>>
{
    #region Variables

    private Edit _edit;
    private Direction _direction;
    private int _deletions;
    private int _index;

    #endregion

    #region Construction

    //public Position(EditableVector<T> list, int index, 
    //    Direction direction = Direction.Positive)
    public Position(Edit edit, int index, Direction direction = Direction.Positive)
    {
        _edit = edit;
        _index = index;
        _direction = direction;
    }

    public Position<T> Clone()
    {
        return (Position<T>) MemberwiseClone();
    }

    #endregion

    #region Object Overrides

    public override bool Equals(object obj)
    {
        return Equals(obj as Position<T>);
    }

    public bool Equals(Position<T> pos)
    {
        if (ReferenceEquals(pos, this))
            return true;

        return pos != null
               && _edit == pos._edit
               && Index == pos.Index
               && Direction == pos.Direction;
    }

    public override int GetHashCode()
    {
        var hash = HashCode.Combine(_index, _deletions, (int) _direction);
        return hash;
    }

    public override string ToString()
    {
        return string.Format("Index={0}", Index);
    }

    #endregion

    #region Properties

    public bool AtBeginning
    {
        get { return Index <= 0; }
    }

    public int Index
    {
        get { return _index; }
        set
        {
            EnsureWriteable();
            _index = value;
        }
    }

    public bool IsReadOnly { get; private set; }

    public Direction Direction
    {
        get { return _direction; }
        set
        {
            EnsureWriteable();
            _direction = value;
        }
    }

    #endregion

    #region Comparison

    public int CompareTo(Position<T> position)
    {
        var compare = Index - position.Index;
        if (compare != 0)
            return compare;

        compare = _deletions - position._deletions;
        if (compare != 0)
            return compare;

        compare = (int) _direction - (int) position._direction;
        return compare;
    }

    // ReSharper disable ConditionIsAlwaysTrueOrFalse
    // ReSharper disable HeuristicUnreachableCode
    [DebuggerStepThrough]
    public static bool operator ==(Position<T> a, Position<T> b)
    {
        if ((object) a == null)
            return (object) b == null;
        return a.Equals(b);
    }

    [DebuggerStepThrough]
    public static bool operator !=(Position<T> a, Position<T> b)
    {
        if ((object) a == null)
            return (object) b != null;
        return !a.Equals(b);
    }

    // ReSharper restore HeuristicUnreachableCode
    // ReSharper restore ConditionIsAlwaysTrueOrFalse

    public static bool operator >(Position<T> a, Position<T> b)
    {
        return a.CompareTo(b) > 0;
    }

    public static bool operator >=(Position<T> a, Position<T> b)
    {
        return a.CompareTo(b) >= 0;
    }

    public static bool operator <=(Position<T> a, Position<T> b)
    {
        return a.CompareTo(b) <= 0;
    }

    public static bool operator <(Position<T> a, Position<T> b)
    {
        return a.CompareTo(b) < 0;
    }

    #endregion

    #region Navigation

    #region Navigation

    public int MoveTo(int index)
    {
        var oldPosition = Index;
        Index = index;
        return index - oldPosition;
    }

    public bool MovePrevious()
    {
        if (_index >= 0)
            return true;

        Index--;
        return false;
    }

    #endregion

    #endregion

    #region Misc

    public void Freeze()
    {
        IsReadOnly = true;
    }

    public Position<T> GetReadOnlyCopy()
    {
        if (IsReadOnly)
            return this;
        var clone = Clone();
        clone.Freeze();
        return clone;
    }

    private void EnsureWriteable()
    {
        if (IsReadOnly)
            throw new InvalidOperationException();
    }

    //public int Move(int count = 1)
    //{
    //    int oldPosition = Index;
    //    int position = oldPosition + count;
    //    int length = _list.Count;
    //    if (unchecked((uint)position > (uint)length))
    //        position = length;
    //    Index = position;
    //    return position - oldPosition;
    //}

    //public bool MoveNext()
    //{
    //    if (++Index < _list.Count)
    //        return true;
    //    Index = _list.Count;
    //    return false;
    //}

    //public T Item
    //{
    //    get { return _list[Index]; }
    //}

    //public bool AtEnd
    //{
    //    get
    //    {
    //        var i = Index;
    //        return i >= _list.Count;
    //    }
    //}

    public int GetIndex(IEditable editable)
    {
        var index = Index;
        var deletions = _deletions;
        var direction = _direction;
        var edit = _edit;
        if (Edit.AdjustIndex(edit, editable.Edits,
            ref index, ref deletions, direction))
            return index;
        return -1;
    }

    public void Synchronize(IEditable editable)
    {
        var index = Index;
        if (Edit.AdjustIndex(_edit, editable.Edits,
            ref index, ref _deletions, _direction))
        {
            _index = index;
            _edit = editable.Edits;
        }
    }

    #endregion
}