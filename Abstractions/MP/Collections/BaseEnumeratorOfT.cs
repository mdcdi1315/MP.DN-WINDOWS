
using System;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Provides the base layout and enumeration services for all the <see cref="IEnumerator{T}"/> implementations provided in this namespace.
    /// </summary>
    /// <typeparam name="T">The type of the elements to be enumerated.</typeparam>
    public abstract class BaseEnumerator<T> : IEnumerator<T>
    {
        private volatile bool not_disposed;

        /// <summary>
        /// Initializes an instance of the <see cref="BaseEnumerator{T}"/> class.
        /// </summary>
        protected BaseEnumerator() => not_disposed = true;

        void IEnumerator.Reset() => Reset();

        object IEnumerator.Current => Current;

        bool IEnumerator.MoveNext() => MoveNext();

        /// <inheritdoc />
        [MaybeNull]
        public abstract T Current { get; }

        /// <summary>
        /// Disposes this <see cref="BaseEnumerator{T}"/> instance. <br />
        /// Implementers overriding this MUST also call this method as well.
        /// </summary>
        [MustNotReportException]
        public virtual void Dispose()
        {
            if (not_disposed)
            {
                not_disposed = false;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the collection.</returns>
        /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        [Throws(typeof(InvalidOperationException))]
        public bool MoveNext() => not_disposed && MoveNextImpl();

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The current enumerator instance is now disposed.</exception>
        /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        [Throws(typeof(InvalidOperationException) , typeof(ObjectDisposedException))]
        public void Reset()
        {
            ObjectDisposedException.ThrowIf(!not_disposed , this);
            ResetImpl();
        }

        /// <summary>
        /// Defines the actual implementation of the <see cref="Reset"/> method.
        /// </summary>
        protected abstract void ResetImpl();

        /// <summary>
        /// Defines the actual implementation of the <see cref="MoveNext"/> method.
        /// </summary>
        /// <returns>A value whether the enumerator moved successfully to the next element.</returns>
        protected abstract bool MoveNextImpl();
    }
}