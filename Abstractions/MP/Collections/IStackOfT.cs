

using System;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Defines the base interface for stack (FIFO) collections. <br />
    /// Note: Implementations must return the elements from the enumerators in the order in which they will be popped from the stack.
    /// </summary>
    /// <typeparam name="T">The type of the elements that this stack will hold.</typeparam>
    public interface IStack<T> : IEnumerable<T>
    {
        /// <summary>
        /// Attempts to pop the last pushed value from the stack. <br />
        /// The return value indicates whether an element was actually popped from the stack.
        /// </summary>
        /// <param name="value">The popped value.</param>
        /// <returns>A value whether the last pushed value was successfully popped from the stack.</returns>
        public System.Boolean TryPop([MaybeNull] out T value);

        /// <summary>
        /// Attempts to peek the last pushed value from the stack. (That is, getting the last pushed element without popping it) <br />
        /// It is equivalent as popping the element, then pushing it again.
        /// </summary>
        /// <param name="value">The peeked value.</param>
        /// <returns>A value whether a value was found in the stack and it was returned.</returns>
        public System.Boolean TryPeek([MaybeNull] out T value);

        /// <summary>
        /// Pushes a value to the stack.
        /// </summary>
        /// <param name="value">The value to push to the stack.</param>
        public void Push([AllowNull] T value);

        /// <summary>
        /// Removes all the pushed items from the stack.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Pushes all the values provided by the specified enumerable, in the order they are read from the enumerable.
        /// </summary>
        /// <remarks>
        /// Implementations that have implemented better ways to push all stack items in bulk should override this implementation.
        /// </remarks>
        /// <param name="items">The items to push to the stack.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public void PushAll(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            foreach (T item in items) { Push(item); }
        }
    }
}
