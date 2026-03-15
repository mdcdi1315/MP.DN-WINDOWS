
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Provides an array enumerator implementation that excludes 
    /// enumerating the specified array elements selected during enumeration time. <br />
    /// Below is provided some code scaffolding on how to use this special enumerator: <br />
    /// <code>
    /// 
    /// using MP.Collections;
    /// 
    /// // In your class,
    /// 
    /// void AMethod()
    /// {
    ///     ArrayExclusiveEnumerator&lt;String&gt; E; // Let's assume that this is properly initialized.
    ///     while (E.MoveNext()) {
    ///         if (APredicate(E.Current)) {
    ///             // Some predicate that matches your criteria for this element to be removed
    ///             E.Exclude();
    ///         }
    ///     }
    /// }
    /// 
    /// </code>
    /// </summary>
    /// <remarks>
    /// Calling <see cref="Exclude"/> before flushing the intention (with <see cref="BaseEnumerator{T}.MoveNext"/>) will effectively DISCARD the exclusion you specified before.
    /// </remarks>
    /// <typeparam name="T">The type of the elements to be enumerated.</typeparam>
    public abstract class ArrayExclusiveEnumerator<T> : BaseEnumerator<T>, ISyncronized
    {
        private sealed class Node
        {
            public readonly T Value;

            public Node Next;
            public Node Prev;

            public Node(T value)
            {
                Value = value;
                Next = Prev = null;
            }
        }

        private Node root, current;
        private bool exclude_current, stop;

        private static Node CreateList(T[] array, long index, long count)
        {
            if (count == 0L) {
                return null;
            } else {
                Node root = new(array[0L]);
                Node g = root;
                long bound = index + count;
                for (long I = index + 1L; I < bound; I++)
                {
                    g = g.Next = new(array[I]) { Prev = g };
                }
                return root;
            }
        }

        private sealed class Simple : ArrayExclusiveEnumerator<T>
        {
            public Simple(T[] array) : base(CreateList(array, 0L, array.LongLength)) { }
        }

        private sealed class Bounds : ArrayExclusiveEnumerator<T>
        {
            public Bounds(T[] array, int index, int count) : base(CreateList(array, index, count)) { }

            public Bounds(T[] array, long index, long count) : base(CreateList(array, index, count)) { }
        }

        private ArrayExclusiveEnumerator(Node t_root) : base()
        {
            stop = false;
            root = t_root;
            current = null;
            exclude_current = false;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ArrayExclusiveEnumerator{T}"/> class from the specified array. <br />
        /// All the elements of the array will be returned by the enumerator.
        /// </summary>
        /// <param name="array">The array to be enumerated.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static ArrayExclusiveEnumerator<T> Of(T[] array)
        {
            ArgumentNullException.ThrowIfNull(array);
            return new Simple(array);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ArrayExclusiveEnumerator{T}"/> class from the specified array. <br />
        /// The <paramref name="index"/> and <paramref name="count"/> parameters indicate the portion of the array to be actually enumerated.
        /// </summary>
        /// <param name="array">The array to be enumerated.</param>
        /// <param name="index">The index in <paramref name="array"/> to start enumerating from.</param>
        /// <param name="count">The number of items that the enumerator will return.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> value is greater than the array's bounds.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static ArrayExclusiveEnumerator<T> ByBounds(T[] array, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            } else if ((index + count) > array.Length) {
                throw new ArgumentException("Specified index and count parameters are out of the given array bounds.");
            } else {
                return new Bounds(array, index, count);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ArrayExclusiveEnumerator{T}"/> class from the specified array. <br />
        /// The <paramref name="index"/> and <paramref name="count"/> parameters indicate the portion of the array to be actually enumerated.
        /// </summary>
        /// <param name="array">The array to be enumerated.</param>
        /// <param name="index">The index in <paramref name="array"/> to start enumerating from.</param>
        /// <param name="count">The number of items that the enumerator will return.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> value is greater than the array's bounds.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static ArrayExclusiveEnumerator<T> ByBounds(T[] array, long index, long count)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (index < 0L) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (count < 0L) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            } else if ((index + count) > array.LongLength) {
                throw new ArgumentException("Specified index and count parameters are out of the given array bounds.");
            } else {
                return new Bounds(array, index, count);
            }
        }

        /// <inheritdoc />
        public sealed override T Current => current.Value;

        /// <inheritdoc />
        protected sealed override bool MoveNextImpl()
        {
            if (stop) {
                return false;
            } else {
                if (current is null) {
                    current = root;
                } else if (exclude_current) {
                    Node n = current.Next;
                    if (current.Prev is null)
                    {
                        root = n; // Root element was excluded
                    }
                    else
                    {
                        current.Prev.Next = n;
                    }
                    current = n;
                    // When being on the last element of the list, the next element will be NULL,
                    // and as a result we will have directly moved to the end of it, where false should be returned by the method.
                    // However, our element will have been successfully excluded.
                } else {
                    current = current.Next;
                }
                return (stop = current is null) == false;
            }
        }

        /// <inheritdoc />
        protected sealed override void ResetImpl()
        {
            stop = false;
            current = null;
            exclude_current = false; // Clean any invalid exclusion state
        }

        /// <summary>
        /// Excludes the current element to be enumerated in the next enumeration pass.
        /// </summary>
        public void Exclude() => exclude_current = true;

        /// <summary>
        /// Disposes this <see cref="ArrayExclusiveEnumerator{T}"/> instance.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            root = null;
            current = null;
        }
    }
}