
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Provides an enumerator implementation for returning array elements in the reverse order <br />
    /// of how they are saved in without needing to perform any copies of the underlying array. <br />
    /// If is needed, however, the arrays to be copied to avoid thread-safety issues, it is also possible. <br /> <br />
    /// 
    /// This class cannot be inherited, albeit the fact that is abstract.
    /// </summary>
    /// <typeparam name="T">The type of the elements to reverse.</typeparam>
    public abstract class ReversedArrayEnumerator<T> : BaseEnumerator<T>
    {
        private ReversedArrayEnumerator() : base() {}

        private sealed class SimpleReversedArrayEnumerator : ReversedArrayEnumerator<T>
        {
            private long index;
            private T[] array;

            public SimpleReversedArrayEnumerator(T[] array) : base() => index = (this.array = array).LongLength;

            public override T Current => array[index];

            protected override bool MoveNextImpl() => --index > -1;

            protected override void ResetImpl() => index = array.LongLength;

            public override void Dispose()
            {
                base.Dispose();
                array = null;
            }
        }

        private sealed class SimpleReversedArrayEnumeratorThreadSafe : ReversedArrayEnumerator<T>, ISyncronized
        {
            private T[] array;
            private long index;

            public SimpleReversedArrayEnumeratorThreadSafe(T[] array) : base()
            {
                this.array = new T[array.LongLength];
                Array.Copy(array , this.array, array.LongLength);
                index = this.array.LongLength;
            }

            public override T Current => array[index];

            protected override bool MoveNextImpl() => --index > -1;

            protected override void ResetImpl() => index = array.LongLength;

            public override void Dispose()
            {
                base.Dispose();
                array = null;
            }
        }

        private sealed class BoundedReversedArrayEnumerator : ReversedArrayEnumerator<T>
        {
            private T[] array;
            private long current_index;
            private readonly long index, count;

            public BoundedReversedArrayEnumerator(T[] array, long index, long count) : base()
            {
                this.array = array;
                current_index = (this.index = index) + (this.count = count);
            }

            public override T Current => array[current_index];

            protected override bool MoveNextImpl() => --current_index >= index;

            protected override void ResetImpl() => current_index = index + count;

            public override void Dispose()
            {
                base.Dispose();
                array = null;
            }
        }

        private sealed class BoundedReversedArrayEnumeratorThreadSafe : ReversedArrayEnumerator<T>, ISyncronized
        {
            private T[] array;
            private long index;

            public BoundedReversedArrayEnumeratorThreadSafe(T[] array, long index, long count) : base()
            {
                Array.Copy(array, index, this.array = new T[count], 0, count);
                this.index = this.array.LongLength;
            }

            public override T Current => array[index];

            protected override bool MoveNextImpl() => --index > -1;

            protected override void ResetImpl() => index = array.LongLength;

            public override void Dispose()
            {
                base.Dispose();
                array = null;
            }
        }

        /// <summary>
        /// Creates a new reversed array enumerator that will return all the elements in the <paramref name="array"/> parameter reversed.
        /// </summary>
        /// <param name="array">The array to create a reversed array enumerator from.</param>
        /// <returns>An enumerator implementation that will return the elements of <paramref name="array"/> in reverse manner.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static ReversedArrayEnumerator<T> Of(T[] array)
        {
            ArgumentNullException.ThrowIfNull(array);
            return new SimpleReversedArrayEnumerator(array);
        }

        /// <summary>
        /// Creates a new reversed array enumerator that will return all the elements in the <paramref name="array"/> parameter reversed.
        /// </summary>
        /// <param name="array">The array to create a reversed array enumerator from.</param>
        /// <returns>An enumerator implementation that will return the elements of <paramref name="array"/> in reverse manner.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static ReversedArrayEnumerator<T> OfCopied(T[] array)
        {
            ArgumentNullException.ThrowIfNull(array);
            return new SimpleReversedArrayEnumeratorThreadSafe(array);
        }

        /// <summary>
        /// Creates a new reversed array enumerator that will return all the elements in the <paramref name="array"/> parameter reversed.
        /// </summary>
        /// <param name="array">The array to create a reversed array enumerator from.</param>
        /// <param name="index">The index where to start reversing elements.</param>
        /// <param name="count">The number of elements to select and reverse.</param>
        /// <returns>An enumerator implementation that will return the elements of <paramref name="array"/> by the specified bounds in reverse manner.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> value is greater than the array's bounds.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static ReversedArrayEnumerator<T> ByBounds(T[] array, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            } else if ((index + count) > array.Length) {
                throw new ArgumentException("Specified index and count parameters are out of the given array bounds.");
            } else {
                return new BoundedReversedArrayEnumerator(array, index, count);
            }
        }

        /// <summary>
        /// Creates a new reversed array enumerator that will return all the elements in the <paramref name="array"/> parameter reversed.
        /// </summary>
        /// <param name="array">The array to create a reversed array enumerator from.</param>
        /// <param name="index">The index where to start reversing elements.</param>
        /// <param name="count">The number of elements to select and reverse.</param>
        /// <returns>An enumerator implementation that will return the elements of <paramref name="array"/> by the specified bounds in reverse manner.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> value is greater than the array's bounds.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static ReversedArrayEnumerator<T> ByBounds(T[] array, long index, long count)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (index < 0L) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (count < 0L) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            } else if ((index + count) > array.LongLength) {
                throw new ArgumentException("Specified index and count parameters are out of the given array bounds.");
            } else {
                return new BoundedReversedArrayEnumerator(array, index, count);
            }
        }

        /// <summary>
        /// Creates a new reversed array enumerator that will return all the elements in the <paramref name="array"/> parameter reversed.
        /// </summary>
        /// <param name="array">The array to create a reversed array enumerator from.</param>
        /// <param name="index">The index where to start reversing elements.</param>
        /// <param name="count">The number of elements to select and reverse.</param>
        /// <returns>An enumerator implementation that will return the elements of <paramref name="array"/> by the specified bounds in reverse manner.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> value is greater than the array's bounds.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static ReversedArrayEnumerator<T> ByBoundsCopied(T[] array, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            } else if ((index + count) > array.Length) {
                throw new ArgumentException("Specified index and count parameters are out of the given array bounds.");
            } else {
                return new BoundedReversedArrayEnumeratorThreadSafe(array, index, count);
            }
        }

        /// <summary>
        /// Creates a new reversed array enumerator that will return all the elements in the <paramref name="array"/> parameter reversed.
        /// </summary>
        /// <param name="array">The array to create a reversed array enumerator from.</param>
        /// <param name="index">The index where to start reversing elements.</param>
        /// <param name="count">The number of elements to select and reverse.</param>
        /// <returns>An enumerator implementation that will return the elements of <paramref name="array"/> by the specified bounds in reverse manner.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> value is greater than the array's bounds.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static ReversedArrayEnumerator<T> ByBoundsCopied(T[] array, long index, long count)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (index < 0L) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (count < 0L) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            } else if ((index + count) > array.LongLength) {
                throw new ArgumentException("Specified index and count parameters are out of the given array bounds.");
            } else {
                return new BoundedReversedArrayEnumeratorThreadSafe(array, index, count);
            }
        }
    }
}