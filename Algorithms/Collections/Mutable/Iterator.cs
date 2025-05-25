#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Collections;

/// <summary>
///     Summary description for Iterator.
/// </summary>
public class Iterator<T> : IComparable<Iterator<T>>
    where T : class, IIndexable<T>
{
    #region Variables

    T[] expressions;
    int[] indices;
    int level;

    #endregion

    #region Constructors

    public Iterator(T node)
    {
        LookAtCurrent = true;
        const int DefaultCapacity = 6;
        indices = new int[DefaultCapacity];
        expressions = new T[DefaultCapacity];
        Push(node, 0);
    }

    public Iterator<T> Clone()
    {
        var clone = (Iterator<T>)MemberwiseClone();
        clone.indices = (int[])indices.Clone();
        clone.expressions = (T[])expressions.Clone();
        return clone;
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Sets whether the next move instruction will consider the current
    ///     iterator or not
    /// </summary>
    public bool LookAtCurrent { get; set; }

    /// <summary>
    ///     Gets the current level of the search
    /// </summary>
    public int Level {
        get => level;
        set
        {
            if (level < value)
                throw new ArgumentOutOfRangeException();
            Array.Clear(expressions, value, level - value);
            level = value;
        }
    }

    #endregion

    #region Comparison and Other Operators

    /// <summary>
    ///     Compares against another iterator
    /// </summary>
    /// <param name="iterator"></param>
    /// <returns></returns>
    public int CompareTo(Iterator<T> iterator)
    {
        int commonLevel;
        return CompareTo(iterator, out commonLevel);
    }

    public int CompareTo(Iterator<T> iterator, out int maxLevel)
    {
        int common = level > iterator.level ? iterator.level : level;

        for (int i = 0; i < common; i++) {
            int cmp = indices[i] - iterator.indices[i];
            if (cmp != 0) {
                maxLevel = i;
                return cmp;
            }
        }

        maxLevel = common;
        return level - iterator.Level;
    }

    public static bool operator ==(Iterator<T> context1, Iterator<T> context2) => context1.Equals(context2);

    public static bool operator !=(Iterator<T> context1, Iterator<T> context2) => !context1.Equals(context2);

    public static bool operator <=(Iterator<T> context1, Iterator<T> context2) => context1.CompareTo(context2) <= 0;

    public static bool operator >=(Iterator<T> context1, Iterator<T> context2) => context1.CompareTo(context2) >= 0;

    public static bool operator <(Iterator<T> context1, Iterator<T> context2) => context1.CompareTo(context2) < 0;

    public static bool operator >(Iterator<T> context1, Iterator<T> context2) => context1.CompareTo(context2) > 0;

    #endregion

    #region Chain Management

    void Push(T e, int pos)
    {
        if (level == indices.Length) {
            int newLength = level * 2;
            Array.Resize(ref indices, newLength);
            Array.Resize(ref expressions, newLength);
        }

        expressions[level] = e;
        indices[level] = pos;
        level++;
    }

    void Pop()
    {
        Debug.Assert(level > 0);
        if (level <= 0) return;
        expressions[--level] = null;
    }

    public void SkipChildren(bool backward)
    {
        Index = backward ? 0 : Current.Count;
    }

    #endregion

    #region Data Accessors

    /// <summary>
    ///     Gets the current item
    /// </summary>
    public T Current => expressions[level - 1];

    /// <summary>
    ///     Gets the current index
    /// </summary>
    public int Index {
        get
        {
            Debug.Assert(indices[level - 1] >= 0, "Index is < 0");
            return indices[level - 1];
        }
        set => indices[level - 1] = value;
    }

    /// <summary>
    ///     Gets the element at the specified level
    /// </summary>
    public T this[int i] => expressions[i];

    #endregion

    #region Movement

    public bool AtStart => level == 1 && indices[0] == 0;

    public bool AtEnd => level == 1 && indices[0] >= expressions[0].Count;

    public bool MoveUp()
    {
        if (level <= 1)
            return false;
        Pop();
        return true;
    }

    public bool MoveDown(int child)
    {
        T list = expressions[level - 1];
        int length = list.Count;
        if (child >= length)
            return false;
        indices[level - 1] = child;
        Push(list[child], 0);
        return true;
    }

    /// <summary>
    ///     Moves to previous element of the tree
    /// </summary>
    public bool MovePrevious()
    {
        if (LookAtCurrent) {
            LookAtCurrent = false;
            return level > 0;
        }

        while (level > 0) {
            T list = expressions[level - 1];
            int count = list.Count;
            int index = indices[level - 1];

            if (index > count)
                index = count;
            indices[level - 1] = --index;

            if (index >= 0 && index < count) {
                T child = list[index];
                Push(child, child.Count);
                return true;
            }

            // If there are no children, always go up
            // If we are past the end, always go up
            if (!MoveUp())
                return false;
        }

        return false;
    }

    public bool MoveNext()
    {
        if (LookAtCurrent) {
            LookAtCurrent = false;
            return level > 0;
        }

        while (level > 0) {
            T list = expressions[level - 1];
            int count = list.Count;
            int index = indices[level - 1];

            if (index >= 0 && index < count) {
                Push(list[index], 0);
                return true;
            }

            // If there are no children, always go up
            // If we are past the end, always go up
            if (!MoveUp())
                return false;

            indices[level - 1]++;
        }

        return false;
    }

    /// <summary>
    ///     Moves the iterator all the way to the beginning
    /// </summary>
    public void MoveToStart()
    {
        Level = 1;
        indices[level - 1] = 0;
        LookAtCurrent = true;
    }

    /// <summary>
    ///     Moves the iterator all the way to the end
    /// </summary>
    public void MoveToEnd()
    {
        Level = 1;
        indices[0] = Current.Count;
        LookAtCurrent = true;
    }

    /// <summary>
    ///     Sets the next move instruction to look at the current iterator
    ///     before preceeding
    /// </summary>
    public void ResetMove()
    {
        LookAtCurrent = true;
    }

    #endregion

    #region Object Routines

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < level; i++) {
            sb.Insert(0, indices[i]);
            if (i > 0) sb.Insert(0, ':');
        }

        return sb.ToString();
    }

    public override int GetHashCode()
    {
        int hash = level;
        for (int i = 0; i < level; i++)
            hash ^= indices[i];
        return hash;
    }

    public bool Equals(Iterator<T> iterator)
    {
        if (iterator.level != level)
            return false;

        for (int i = 0; i < level; i++)
            if (iterator.indices[i] != indices[i])
                return false;

        return true;
    }

    public override bool Equals(object o)
    {
        var iterator = o as Iterator<T>;
        if (iterator == null) return false;
        return Equals(iterator);
    }

    #endregion
}