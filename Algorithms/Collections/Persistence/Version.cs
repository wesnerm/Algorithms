#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2006 - 2008, Wesner Moise.
/////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Collections;

[Pure]
public class Version<T>
{
    #region Properties

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public T Data {
        get
        {
            var n = _change as Core;
            return n != null ? n.Value : Sync();
        }
    }

    #endregion

    #region Helper Classes

    public abstract class Change
    {
        public abstract Change Apply(T unknown);
    }

    #endregion

    #region Nested type: Core

    [DebuggerStepThrough]
    class Core : Change
    {
        public readonly T Value;
#if DEBUG
        public bool Reentrant;
#endif

        [DebuggerStepThrough]
        public Core(T value) => Value = value;

        public override Change Apply(T unknown) => this;
    }

    #endregion

    #region Variables

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    protected Change _change;

    Version<T> _previous;

    #endregion

    #region Constructor

    [DebuggerStepThrough]
    public Version(T value)
        : this(new Core(value), null) { }

    [DebuggerStepThrough]
    Version(Change change, Version<T> previous)
    {
#if DEBUG
        if (previous != null)
            previous.Validate();
        else
            Debug.Assert(change is Core);
#endif

        _change = change;
        _previous = previous;
        Validate();
    }

    [Conditional("DEBUG")]
    public void Validate()
    {
#if DEBUG
        Change n = _change;
        Version<T>? p = _previous;
        while (p != null) {
            n = p._change;
            p = p._previous;
        }

        Debug.Assert(n is Core);
#endif
    }

    [DebuggerStepThrough]
    public static Version<T> operator +(Version<T> version, Change change) => new(change, version);

    #endregion

    #region Methods

    public T Sync()
    {
        Version<T> current;
        Version<T> previous;

        // Quick success
        var core = _change as Core;
        if (core != null)
            return core.Value;

        current = this;
        previous = null;
#if DEBUG
        int count1 = 0;
        int count2 = 0;
#endif

        do {
#if DEBUG
            count1++;
#endif
            Version<T> tmp = current._previous;
            current._previous = previous;
            previous = current;
            current = tmp;
        } while (current != null);

        Version<T> last = previous;
        core = (Core)previous._change;
        T s = core.Value;

#if DEBUG
        if (core.Reentrant)
            throw new InvalidOperationException();
        core.Reentrant = true;
#endif

        while (true) {
#if DEBUG
            count2++;
#endif
            current = previous;
            previous = previous._previous;
            if (previous == null)
                break;
            current._change = previous._change.Apply(s);
        }

        _change = core;
#if DEBUG
        core.Reentrant = false;
        Debug.Assert(current == this);
        Debug.Assert(_previous == null);
        Debug.Assert(count1 == count2);
        current.Validate();
        last.Validate();
#endif
        return s;
    }

    [DebuggerStepThrough]
    public static IEnumerable<Change> GetChanges(Version<T> p1, Version<T> p2) => GetChanges(p1, p2, null);

    [DebuggerStepThrough]
    public static IEnumerable<Change> GetChanges(
        Version<T> branch1, Version<T> branch2, Version<T> old)
    {
        branch1.Sync();

        for (Version<T>? current = branch2;
             current != null && current != old;
             current = current._previous)
            yield return current._change;
    }

    [DebuggerStepThrough]
    public static void ForEachhange(Version<T> p1, Version<T> p2,
        Action<Change> action)
    {
        ForEachChange(p1, p2, null, action);
    }

    [DebuggerStepThrough]
    public static void ForEachChange(
        Version<T> branch1, Version<T> branch2, Version<T> old,
        Action<Change> action)
    {
        branch1.Sync();

        for (Version<T>? current = branch2;
             current != null && current != old;
             current = current._previous)
            action(current._change);
    }

    [DebuggerStepThrough]
    public override string ToString() => _change.ToString();

    #endregion
}