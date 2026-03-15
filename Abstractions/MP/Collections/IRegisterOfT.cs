
using System;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Provides a way to add once items and access them many times. <br />
    /// Items added to objects implementing this interface cannot be removed.
    /// </summary>
    /// <typeparam name="T">The type of the items to be held by this register object.</typeparam>
    public interface IRegister<T> : IEnumerable<T>
    {
        /// <summary>Registers an item to this instance.</summary>
        /// <param name="item">The item to be saved to this instance.</param>
        /// <exception cref="ArgumentException">The class implementing the interface is enforcing object singularity and <paramref name="item"/> already exists in the current register object.</exception>
        [Throws(typeof(ArgumentException))]
        public void Register([AllowNull] T item);

        /// <summary>Registers a multiple of items to this instance.</summary>
        /// <param name="items">The item(s) to be saved to this instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public void RegisterRange(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            foreach (T item in items) { Register(item); }
        }

        /// <summary>
        /// Copies the elements of the <see cref="IRegister{T}"/> to an <see cref="System.Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="IRegister{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="IRegister{T}"/> is greater than the available space from arrayIndex to the end of the destination array.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public void CopyTo(T[] array, int arrayIndex);
    }
}