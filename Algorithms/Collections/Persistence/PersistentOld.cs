namespace Softperson.Collections;

public class Persistent<T>
{
    #region Properties

    public T Data {
        get
        {
            var n = _change as DataNode;
            return n != null ? n.Value : Sync();
        }
    }

    #endregion

    #region Methods

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
        T value = core.Value;

        while (true) {
            current = previous;
            previous = current._previous;
            if (previous == null) break;
            current._change = previous._change.Apply(value);
        }

        _change = core;
        return value;
    }

    #endregion

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

    #region Helper Classes

    public abstract class Change
    {
        public abstract Change Apply(T unknown);
    }

    class DataNode : Change
    {
        public T Value;

        public override Change Apply(T unknown) => this;
    }

    #endregion
}