namespace Algorithms.Collections;

/// <summary>
///     Summary description for Persistent
/// </summary>
[DebuggerStepThrough]
[Pure]
public class Persistent<T>
{
    #region Variables

    protected Change _change;
    protected Persistent<T> _previous;

    #endregion

    #region Constructor

    public Persistent(T value)
        : this(new DataNode { Value = value }, null) { }

    protected Persistent(Change change, Persistent<T> previous)
    {
        Debug.Assert(previous != null || change is DataNode);
        _change = change;
        _previous = previous;
    }

    #endregion

    #region Properties

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public T Data {
        get
        {
            var n = _change as DataNode;
            return n != null ? n.Value : Sync();
        }
    }

    public bool IsSynced() => _previous == null;

    public Change[] AllChanges {
        get
        {
            var changes = new List<Change>();
            for (Persistent<T>? current = this; current != null; current = current._previous)
                changes.Add(current._change);
            return changes.ToArray();
        }
    }

    #endregion

    #region Methods

    public static void EnsureActive(Persistent<T> version)
    {
        if (version._previous != null)
            version.Sync();
    }

    public static Persistent<T> GetActiveVersion(Persistent<T> version)
    {
        for (Persistent<T>? current = version; current != null; current = current._previous)
            if (current._previous == null)
                return current;
        return null;
    }

    public T Sync()
    {
        Persistent<T>? current = this;
        var previous = (Persistent<T>)null;

        do {
            Persistent<T> tmp = current._previous;
            current._previous = previous;
            previous = current;
            current = tmp;
        } while (current != null);

        var core = (DataNode)previous._change;
#if DEBUG
        Debug.Assert(!core.Reentrant);
        core.Reentrant = true;
#endif
        T value = core.Value;

        while (true) {
            current = previous;
            previous = current._previous;
            if (previous == null) break;
            current._change = previous._change.Apply(value);
        }

#if DEBUG
        core.Reentrant = false;
#endif
        _change = core;
        return value;
    }

    [DebuggerNonUserCode]
    public static IEnumerable<Change> GetChanges(Persistent<T> p1, Persistent<T> p2)
    {
#line hidden
        p1.Sync();
        Persistent<T>? current = p2;
        while (current != null) {
            yield return current._change;
            EnsureActive(p1);
            current = current._previous;
        }
#line default
    }

    public static bool IsConnected(Persistent<T> p1, Persistent<T> p2)
    {
        p1.Sync();
        Persistent<T>? current = p2;
        while (current != null) {
            if (current == p1)
                return true;
            current = current._previous;
        }

        return false;
    }

    #endregion

    #region Helper Classes

    public abstract class Change
    {
        public abstract Change Apply(T unknown);
    }

    protected class DataNode : Change
    {
#if DEBUG
        public bool Reentrant;
#endif

        public T Value;

        public override Change Apply(T unknown) => this;
    }

    #endregion
}