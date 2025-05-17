#region

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.Collections;

#endregion

namespace Algorithms.Collections;

public partial class Rope<T>
{
    #region Enumerator

    public struct Enumerator : IEnumerator<T>
    {
        #region Variable

        private readonly Rope<T> _top;
        private int _position;
        private Rope<T> _node;
        private int _nodeStart;

        #endregion

        #region Construction

        public Enumerator(Rope<T> list)
            : this()
        {
            _top = list;
            _top.Seal();
            _position = -1;
        }

        public void Reset()
        {
            Position = -1;
        }

        public Enumerator Clone()
        {
            return this;
        }

        #endregion

        #region Navigation

        public bool MoveNext()
        {
            var newPosition = _position + 1;

            if (_node != null)
            {
                var index = newPosition - _nodeStart;
                if (unchecked((uint) index < (uint) _node.Count))
                {
                    _position = newPosition;
                    return true;
                }
            }

            Position = newPosition;
            return _node != null;
        }

        public bool MovePrevious()
        {
            var newPosition = _position - 1;

            if (_node != null)
            {
                var index = newPosition - _nodeStart;
                if (unchecked((uint) index < (uint) _node.Count))
                {
                    _position = newPosition;
                    return true;
                }
            }

            Position = newPosition;
            return _node != null;
        }

        public bool MoveToPreviousRun()
        {
            if (_node is RunRope)
                _position = _nodeStart;
            return MovePrevious();
        }

        public bool MoveToNextRun()
        {
            if (_node is RunRope)
                _position = _nodeStart + _node.Count - 1;
            return MoveNext();
        }

        #endregion

        #region Properties

        public bool InRun
        {
            get { return _top is RunRope; }
        }

        public int Position
        {
            get { return _position; }
            set
            {
                if (_position == value)
                    return;

                if (unchecked((uint) value >= (uint) _top.Count))
                {
                    _node = null;
                    _position = value < 0 ? -1 : _top.Count;
                    return;
                }

                _top.FindNode(value, out _node, out _nodeStart);
                _position = value;
            }
        }

        #region  Members

        object IEnumerator.Current
        {
            get { return Current; }
        }

        #endregion

        #region IEnumerator<T> Members

        public T Current
        {
            get { return _node[_position - _nodeStart]; }
        }

        [DebuggerStepThrough]
        public void Dispose()
        {
        }

        #endregion

        #endregion
    }

    #endregion
}