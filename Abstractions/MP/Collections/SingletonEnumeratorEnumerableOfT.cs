
using System;
using System.Collections;
using System.Collections.Generic;

namespace MP.Collections
{
    /// <summary>
    /// Provides an <see cref="IEnumerable{T}"/> implementation that does always return the same enumerator instance. <br />
    /// Typically this is to be used where there the <see langword="yield return"/> and <see langword="yield break"/> statements would be used.
    /// </summary>
    /// <typeparam name="T">The type of elements to be returned.</typeparam>
    public sealed class SingletonEnumeratorEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> enumerator;

        /// <summary>
        /// Constructs a new instance of the <see cref="SingletonEnumeratorEnumerable{T}"/> instance.
        /// </summary>
        /// <param name="enumerator">The <see cref="IEnumerator{T}"/> implementation to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> is <see langword="null"/>.</exception>
        public SingletonEnumeratorEnumerable(IEnumerator<T> enumerator) => ArgumentNullException.ThrowIfNull(this.enumerator = enumerator, nameof(enumerator));

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => enumerator;

        IEnumerator IEnumerable.GetEnumerator() => enumerator;
    }
}