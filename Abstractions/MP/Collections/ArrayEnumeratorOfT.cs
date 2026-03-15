
using System;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// A rather simple <see cref="IEnumerator{T}"/> implementation for arrays.
    /// </summary>
    /// <seealso cref="ReversedArrayEnumerator{T}"/>
    /// <typeparam name="T">The type of the array elements to enumerate.</typeparam>
    public abstract class ArrayEnumerator<T> : BaseEnumerator<T>
    {
        private ArrayEnumerator() : base() {}

        private sealed class SimpleArrayEnumerator : ArrayEnumerator<T>
        {
            private T[] array;
            private long index;

            public SimpleArrayEnumerator(T[] array) : base()
            {
                this.array = array;
                index = -1;
            }

            public override T Current => array[index];

            protected override void ResetImpl() => index = -1;

            protected override bool MoveNextImpl() => ++index < array.LongLength;

            public override void Dispose()
            {
                base.Dispose();
                array = null;
            }
        }

        private sealed class SimpleArrayEnumeratorThreadSafe : ArrayEnumerator<T>, ISyncronized
        {
            private T[] array;
            private long index;

            public SimpleArrayEnumeratorThreadSafe(T[] array) : base()
            {
                Array.Copy(array, 0, this.array = new T[array.LongLength], 0, array.LongLength);
                index = -1;
            }

            public override T Current => array[index];

            protected override void ResetImpl() => index = -1;

            protected override bool MoveNextImpl() => ++index < array.LongLength;

            public override void Dispose()
            {
                base.Dispose();
                array = null;
            }
        }

        private sealed class BoundedArrayEnumerator : ArrayEnumerator<T>
        {
            private T[] elements;
            private long current;
            private readonly long bound, index;
            
            public BoundedArrayEnumerator(T[] elements, long index, long count)
            {
                this.elements = elements;
                bound = (this.index = index) + count;
                current = this.index - 1;
            }

            public override T Current => elements[current];

            protected override void ResetImpl() => current = index - 1;

            protected override bool MoveNextImpl() => ++current < bound;

            public override void Dispose()
            {
                base.Dispose();
                elements = null;
            }
        }

        private sealed class BoundedArrayEnumeratorThreadSafe : ArrayEnumerator<T>, ISyncronized
        {
            private T[] array;
            private long index;

            public BoundedArrayEnumeratorThreadSafe(T[] array, long index, long count) : base()
            {
                Array.Copy(array, index, this.array = new T[count], 0, count);
                this.index = -1;
            }

            public override T Current => array[index];

            protected override void ResetImpl() => index = -1;

            protected override bool MoveNextImpl() => ++index < array.LongLength;

            public override void Dispose()
            {
                base.Dispose();
                array = null;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ArrayEnumerator{T}"/> class from the specified array. <br />
        /// All the elements of the array will be returned by the enumerator.
        /// </summary>
        /// <param name="array">The array to be enumerated.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static ArrayEnumerator<T> Of(T[] array)
        {
            ArgumentNullException.ThrowIfNull(array);
            return new SimpleArrayEnumerator(array);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ArrayEnumerator{T}"/> class from the specified array. <br />
        /// All the elements of the array will be returned by the enumerator.
        /// </summary>
        /// <param name="array">The array to be enumerated.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static ArrayEnumerator<T> OfCopied(T[] array)
        {
            ArgumentNullException.ThrowIfNull(array);
            return new SimpleArrayEnumeratorThreadSafe(array);
        }

        /// <summary>
        /// Creates a new instance of the array enumerator from the specified array. <br />
        /// The <paramref name="index"/> and <paramref name="count"/> parameters indicate the portion of the array to be actually enumerated.
        /// </summary>
        /// <param name="array">The array to be enumerated.</param>
        /// <param name="index">The index in <paramref name="array"/> to start enumerating from.</param>
        /// <param name="count">The number of items that the enumerator will return.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> value is greater than the array's bounds.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static ArrayEnumerator<T> ByBounds(T[] array, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            } else if ((index + count) > array.Length) {
                throw new ArgumentException("Specified index and count parameters are out of the given array bounds.");
            } else {
                return new BoundedArrayEnumerator(array, index, count);
            }
        }

        /// <summary>
        /// Creates a new instance of the array enumerator from the specified array. <br />
        /// The <paramref name="index"/> and <paramref name="count"/> parameters indicate the portion of the array to be actually enumerated.
        /// </summary>
        /// <param name="array">The array to be enumerated.</param>
        /// <param name="index">The index in <paramref name="array"/> to start enumerating from.</param>
        /// <param name="count">The number of items that the enumerator will return.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> value is greater than the array's bounds.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static ArrayEnumerator<T> ByBounds(T[] array, long index, long count)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (index < 0L) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (count < 0L) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            } else if ((index + count) > array.LongLength) {
                throw new ArgumentException("Specified index and count parameters are out of the given array bounds.");
            } else {
                return new BoundedArrayEnumerator(array, index, count);
            }
        }

        /// <summary>
        /// Creates a new instance of the array enumerator from the specified array. <br />
        /// The <paramref name="index"/> and <paramref name="count"/> parameters indicate the portion of the array to be actually enumerated.
        /// </summary>
        /// <param name="array">The array to be enumerated.</param>
        /// <param name="index">The index in <paramref name="array"/> to start enumerating from.</param>
        /// <param name="count">The number of items that the enumerator will return.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> value is greater than the array's bounds.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static ArrayEnumerator<T> ByBoundsCopied(T[] array, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            } else if ((index + count) > array.Length) {
                throw new ArgumentException("Specified index and count parameters are out of the given array bounds.");
            } else {
                return new BoundedArrayEnumeratorThreadSafe(array, index, count);
            }
        }

        /// <summary>
        /// Creates a new instance of the array enumerator from the specified array. <br />
        /// The <paramref name="index"/> and <paramref name="count"/> parameters indicate the portion of the array to be actually enumerated.
        /// </summary>
        /// <param name="array">The array to be enumerated.</param>
        /// <param name="index">The index in <paramref name="array"/> to start enumerating from.</param>
        /// <param name="count">The number of items that the enumerator will return.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> value is greater than the array's bounds.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static ArrayEnumerator<T> ByBoundsCopied(T[] array, long index, long count)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (index < 0L) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (count < 0L) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            } else if ((index + count) > array.LongLength) {
                throw new ArgumentException("Specified index and count parameters are out of the given array bounds.");
            } else {
                return new BoundedArrayEnumeratorThreadSafe(array, index, count);
            }
        }
    }
}