#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Collections;

partial class Rope<T>
{
    #region Helpers

    public List ToMutableList() => new(this);

    #endregion

    public class List : IList<T>
    {
        #region Construction

        public List() => Rope = Empty;

        public List(Rope<T> rope) => Rope = rope.Seal();

        /// <summary>
        ///     Constructor from an existing list
        /// </summary>
        /// <param name="list"></param>
        public List(IEnumerable<T> list) : this()
        {
            Debug.Assert(Rope != null);
            InsertRange(0, list);
            Debug.Assert(Rope != null);
        }

        public List<T> Clone() => new(Rope);

        #endregion

        #region Properties

        /// <summary>
        ///     Number of elements in list
        /// </summary>
        public int Count {
            get => Rope.Count;
            set
            {
                int count = Rope.Count;
                if (count > value)
                    RemoveRange(value, count - value);
                else
                    Insert(count, value, default);
            }
        }

        [NotNull] public Rope<T> Rope { get; private set; }

        #endregion

        #region Indexing

        /// <summary>
        ///     Goes to next run.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public int NextRun(int position) => Rope.NextRun(position);

        /// <summary>
        ///     Goes to previous run.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public int PreviousRun(int position) => Rope.PreviousRun(position);

        #endregion

        #region List Operations

        #region IList<T> Members

        /// <summary>
        ///     Gets or sets an element in the array for list
        /// </summary>
        [NonSerialized] Rope<T> _mruNode;

        [NonSerialized] int _mruStart;

        public T this[int index] {
            get
            {
                Rope<T>? n = _mruNode;
                if (n == null || unchecked((uint)(index - _mruStart) >= (uint)n.Count)) {
                    Rope.CheckIndex(index);
                    Rope.FindNode(index, out _mruNode, out _mruStart);
                    n = _mruNode;
                }

                return n[index - _mruStart];
            }

            set
            {
                Rope.CheckIndex(index);
                if (EqualityComparer<T>.Default.Equals(this[index], value))
                    return;

                RemoveRange(index, 1);
                Insert(index, value);
                _mruNode = null;
            }
        }

        /// <summary>
        ///     Returns the index of a value in the list
        /// </summary>
        /// <param name="value">value to search for</param>
        /// <returns>nonnegative, if not found </returns>
        public int IndexOf(T value) => IndexOf(value, 0);

        /// <summary>
        ///     Inserts a single value
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        //[DebuggerStepThrough]
        public void Insert(int index, T value)
        {
            Rope = Rope.InsertX(index, value);
        }

        public void RemoveAt(int index)
        {
            RemoveRange(index, 1);
        }

        public void Clear()
        {
            RemoveRange(0, Count);
        }

        /// <summary>
        ///     Removes an item from the list
        /// </summary>
        /// <param name="value"></param>
        public bool Remove(T value)
        {
            int index = IndexOf(value);
            if (index != -1) {
                RemoveRange(index, 1);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Determine if value is contained in the list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public bool Contains(T value) => Rope.Contains(value);

        /// <summary>
        ///     Adds a single value to the list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //[DebuggerStepThrough]
        public void Add(T value)
        {
            Rope = Rope.Insert(Rope.Count, value);
        }

        /// <summary>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(T[] array, int index)
        {
            Rope.CopyTo(array, index);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Enumerator GetEnumerator() => Rope.GetEnumerator();

        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<T> Range(int start, int count) => Rope.Range(start, count);

        #endregion

        /// <summary>
        ///     Returns the index of a value in the list, starting from start, up to count characters
        /// </summary>
        /// <param name="value">value to search for</param>
        /// <param name="start">starting location</param>
        /// <param name="count">up to count characters</param>
        /// <returns></returns>
        public int IndexOf(T value, int start, int count = int.MaxValue) => Rope.IndexOf(value, start, count);

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="value"></param>
        [DebuggerStepThrough]
        public void Insert(int index, int count, T value)
        {
            Insert(index, RepeatX(count, value));
        }

        /// <summary>
        ///     Inserts the list at point index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="list"></param>
        public void InsertRange(int index, IEnumerable<T> list)
        {
            var slist = list as List;
            Rope<T> vector = slist != null ? slist.Rope : FromX(list);
            Insert(index, vector);
        }

        /// <summary>
        /// </summary>
        /// <param name="array"></param>
        public void AddRange(params T[] array)
        {
            InsertRange(Count, array);
        }

        /// <summary>
        ///     Removes a range of characters from the list starting at start
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public void RemoveRange(int start, int count)
        {
            Debug.Assert(Rope != null);
            Rope = Rope.RemoveX(start, count);
            _mruNode = null;
            Rope.Seal();
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="array"></param>
        public void InsertRange(int index, params T[] array)
        {
            InsertRange(index, (IEnumerable<T>)array);
        }

        public void ReplaceRange(int index, int count, IEnumerable<T> array)
        {
            RemoveRange(index, count);
            InsertRange(index, array);
        }

        void Insert(int index, Rope<T> insertion)
        {
            Debug.Assert(Rope != null);
            Rope = Rope.InsertX(index, insertion);
            _mruNode = null;
            Rope.Seal();
        }

        #endregion

        #region Editing

        /// <summary>
        ///     Set the range given by start and count to the specified value T
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="count">count of characters</param>
        /// <param name="value"></param>
        public void SetRange(int start, int count, T value)
        {
            RemoveRange(start, count);
            Insert(start, count, value);
        }

        /// <summary>
        ///     Copy a slice of the list from start, up to count elements
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="count">number of elements</param>
        /// <returns></returns>
        public Rope<T> Copy(int start, int count) =>
            // TODO: Might want to make this return a mutable list
            Rope.Copy(start, count);

        public Rope<T> Cut(int start, int count)
        {
            Rope<T> cut = Copy(start, count);
            RemoveRange(start, count);
            return cut;
        }

        #endregion

        #region Object Overloads

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;

            var list = obj as List;
            if (list == null)
                return false;

            return Rope.Equals(list.Rope);
        }

        /// <summary>
        ///     Returns a hash code for the list
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Rope.GetHashCode();

        /// <summary>
        ///     Returns the string representation of the list
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Rope.ToString();

        public static implicit operator Rope<T>(List builder) => builder.ToVector();

        public Rope<T> ToVector() => Rope.Seal();

        #endregion
    }
}