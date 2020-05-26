using System;
using UnityEngine;
using System.Collections;

namespace Util
{
    /// <summary>
    /// Start a coroutine that holds the last returned value.
    /// </summary>
    /// <remarks>Usage:
    /// <code>var cd = new CoroutineData(this, WaitForFrames(30));
    /// yield return cd.Coroutine;
    /// Debug.Log("Result: " + cd.Result);</code>
    /// where WaitForFrames(...) a function is with signature
    /// <code>IEnumerator WaitForFrames(/*...*/) { /*...*/ }</code>
    /// </remarks>
    public class CoroutineData
    {
        #region Fields
        private object _result;
        private readonly Coroutine _coroutine;

        protected readonly IEnumerator _target;
        protected bool _isDone = false;
        #endregion

        #region Properties
        /// <summary>
        /// The coroutine. Should be returned in the yield statement
        /// </summary>
        public Coroutine Coroutine
        {
            get { return _coroutine; }
        }

        /// <summary>
        /// The last returned object is saved here
        /// </summary>
        public object Result
        {
            get { return _result; }
            //set { _result = value; }
        }

        /// <summary>
        /// Whether the coroutine is done
        /// </summary>
        public bool IsDone
        {
            get { return _isDone; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a coroutine that saves the last returned object
        /// </summary>
        /// <param name="owner">To which object this coroutine is attached to</param>
        /// <param name="target">The coroutine</param>
        public CoroutineData(MonoBehaviour owner, IEnumerator target)
        {
            this._target = target;
            this._coroutine = owner.StartCoroutine(Run());
        }
        #endregion

        #region Methods
        /// <summary>
        /// Actions to do at the start of the run
        /// </summary>
        protected virtual void RunStart()
        {
            _isDone = false;
        }

        /// <summary>
        /// Actions to do every step (=frame)
        /// </summary>
        protected virtual void RunMiddle()
        { }

        /// <summary>
        /// Actions to do when the coroutine has ended
        /// </summary>
        protected virtual void RunEnd()
        { }

        /// <summary>
        /// How the coroutine is run
        /// </summary>
        /// <returns></returns>
        private IEnumerator Run()
        {
            RunStart();
            while (_target.MoveNext())
            {
                RunMiddle();
                yield return _target.Current;
            }
            _isDone = true;
            _result = _target.Current;
            RunEnd();
        }
        #endregion
    }

    public class CoroutineData<T> : CoroutineData
    {
        #region Fields
        private T _result;
        #endregion

        #region Properties
        /// <summary>
        /// Hides <see cref="CoroutineData.Result"/> to return an object of type <see cref="T"/> instead of an object.
        /// </summary>
        public new T Result
        {
            get { return _result; }
            set { _result = value; }
        }
        #endregion

        #region Constructor
        public CoroutineData(MonoBehaviour owner, IEnumerator target) : base(owner, target)
        { }
        #endregion

        #region Methods
        /// <summary>
        /// Overridden to cast the object to <see cref="T"/>. Sets result to default value of T if not castable instead of throwing an exception.
        /// </summary>
        protected override void RunEnd()
        {
            try
            {
                _result = (T)base.Result;
            }
            catch (InvalidCastException)
            {
                _result = default(T);
            }
        }
        #endregion
    }
}