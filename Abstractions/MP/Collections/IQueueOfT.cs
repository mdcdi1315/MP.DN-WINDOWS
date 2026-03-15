
using System;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Defines the base interface for queue (LIFO) collections. <br />
    /// Note: Implementations must return the elements from the enumerators in the order in which they will be dequeued from the queue.
    /// </summary>
    /// <typeparam name="T">The type of the elements that this queue will hold.</typeparam>
    public interface IQueue<T> : IEnumerable<T>
    {
        /// <summary>
        /// Attempts to dequeue an item from the working queue. <br />
        /// The return value indicates whether an item was actually dequeued from the queue.
        /// </summary>
        /// <param name="item">The item that was dequeued from the current queue object.</param>
        /// <returns>A value whether the queued value at the head of the queue was successfully dequeued from the queue.</returns>
        public System.Boolean TryDequeue([MaybeNull] out T item);

        /// <summary>
        /// Attempts to peek the next value that will be dequeued from the queue.
        /// </summary>
        /// <param name="value">The peeked value.</param>
        /// <returns>A value whether a value was found in the queue and it was returned.</returns>
        public System.Boolean TryPeek([MaybeNull] out T value);

        /// <summary>
        /// Enqueues an item into the current queue.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        public void Enqueue([AllowNull] T item);

        /// <summary>
        /// Removes all the enqueued items from the queue.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Enqueues all the values provided by the specified enumerable, in the order they are read from the enumerable.
        /// </summary>
        /// <remarks>
        /// Implementations that have implemented better ways to enqueue all queue items in bulk should override this implementation.
        /// </remarks>
        /// <param name="items">The items to push to the stack.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public void EnqueueAll(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            foreach (T item in items) { Enqueue(item); }
        }
    }

}