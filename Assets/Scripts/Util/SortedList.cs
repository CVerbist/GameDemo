using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Util
{
    /// <summary>
    /// A list ensuring it is always sorted according to the comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortedList<T> : IEnumerable<T>
    {
        #region Fields
        private readonly LinkedList<T> _list = new LinkedList<T>();
        private readonly Comparer<T> _comparer;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the amount of elements in this list
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor, uses default comparer of <see cref="T"/>
        /// </summary>
        public SortedList() : this(Comparer<T>.Default) {}

        /// <summary>
        /// Constructor where you supply the comparer (see <see cref="ComparerUtil{T}"/>)
        /// </summary>
        /// <param name="comparer"></param>
        public SortedList(Comparer<T> comparer)
        {
            _comparer = comparer;
        }

        /// <summary>
        /// Constructor that uses the default comparer of <see cref="T"/> and copies the list contents (shallow copy)
        /// </summary>
        /// <param name="list"></param>
        public SortedList(IEnumerable<T> list) : this(Comparer<T>.Default, list) {}

        /// <summary>
        /// Constructor where you supply the comparer and copies the list contents (shallow copy)
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="list"></param>
        public SortedList(Comparer<T> comparer, IEnumerable<T> list)
        {
            _comparer = Comparer<T>.Default;
            AddRange(list);
        }
        #endregion

        #region Methods
        /// <summary>
        /// See <see cref="IEnumerable{T}.GetEnumerator"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// See <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds the item to the list.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var e = _list.First;
            while (true)
            {
                if (e == null)
                {
                    _list.AddLast(item);
                    break;
                }
                if (_comparer.Compare(item, e.Value) < 0)
                {
                    _list.AddBefore(e, item);
                    break;
                }
                e = e.Next;
            }
        }

        /// <summary>
        /// Add multiple items to the list
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        /// <summary>
        /// Remove the matching item from the list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        /// <summary>
        /// Clears the whole list
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }
        #endregion
    }
}