

using System;
using System.Collections;
using System.Collections.Generic;

namespace MP.Collections
{
    /// <summary>
    /// A wrapping implementation around the <see cref="IEnumerator{T}"/> interface to find out while running in a loop whether a next element does exist from the underlying enumerator.
    /// </summary>
    /// <typeparam name="T">The type of the elements to be enumerated.</typeparam>
    public class NextNextEnumerator<T> : IEnumerator<T>
    {
        private T current, next;
        private bool hasnextnext;
        private readonly IEnumerator<T> enumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="NextNextEnumerator{T}"/> class by wrapping the specified <see cref="IEnumerator{T}"/> implementation.
        /// </summary>
        /// <param name="enumerator">The enumerator implementation to be wrapped.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> is <see langword="null"/>.</exception>
        public NextNextEnumerator(IEnumerator<T> enumerator)
        {
            ArgumentNullException.ThrowIfNull(enumerator);
            this.enumerator = enumerator;
        }

        /// <inheritdoc />
        public T Current => current;

        object IEnumerator.Current => Current;

        /// <summary>
        /// Gets a value whether the enumerator has a next element in the collection. <br />
        /// If this returns <see langword="true"/>, and then calling the <see cref="MoveNext"/> method will make that found element the current one.
        /// </summary>
        public bool HasNextNextElement => hasnextnext;

        /// <summary>
        /// Disposes this <see cref="NextNextEnumerator{T}"/> instance. 
        /// </summary>
        public virtual void Dispose()
        {
            try {
                enumerator.Dispose();
            } finally {
                GC.SuppressFinalize(this);
            }
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (hasnextnext) {
                current = next;
                next = (hasnextnext = enumerator.MoveNext()) ? enumerator.Current : default;
                return true;
            } else if (enumerator.MoveNext()) {
                current = enumerator.Current;
                next = (hasnextnext = enumerator.MoveNext()) ? enumerator.Current : default;
                return true;
            } else {
                return false;
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            enumerator.Reset();
            hasnextnext = false;
            current = next = default;
        }
    }
}