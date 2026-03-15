
using System;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Provides a default implementation of the <see cref="ITraversableRegister{T}"/> interface. <br />
    /// Note down that this register implementation cannot enforce object singularity.
    /// </summary>
    /// <typeparam name="T">The type of items that this register will retain.</typeparam>
    public class ArrayBasedRegister<T> : ITraversableRegister<T>, IArrayBasedCollection, ISyncronized
    {
        private int count;
        private T[] elements;
        private readonly IEqualityComparer<T> comparer;

        /// <summary>
        /// Initializes a new and empty instance of the <see cref="ArrayBasedRegister{T}"/> class.
        /// </summary>
        [Throws]
        public ArrayBasedRegister()
        {
            comparer = EqualityComparer<T>.Default;
            elements = Array.Empty<T>();
            count = 0;
        }

        /// <summary>
        /// Initializes a new and empty instance of the <see cref="ArrayBasedRegister{T}"/> class with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity of the register.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        [Throws(typeof(ArgumentOutOfRangeException))]
        public ArrayBasedRegister(int capacity)
        {
            if (capacity < 0) {
                throw new ArgumentOutOfRangeException(nameof(capacity) , "Initial collection capacity cannot be negative.");
            }
            comparer = EqualityComparer<T>.Default;
            elements = new T[capacity];
            count = 0;
        }

        /// <summary>
        /// Initializes a new and empty instance of the <see cref="ArrayBasedRegister{T}"/> class, by using the specified equality comparer for comparing registered elements.
        /// </summary>
        /// <param name="comparer">The equality comparer to be used. Can also be <see langword="null"/>.</param>
        [Throws]
        public ArrayBasedRegister([AllowNull] IEqualityComparer<T> comparer)
        {
            this.comparer = comparer ?? EqualityComparer<T>.Default;
            elements = Array.Empty<T>();
            count = 0;
        }

        /// <summary>
        /// Initializes a new and empty instance of the <see cref="ArrayBasedRegister{T}"/> class, by using the specified equality comparer for comparing registered elements and with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity of the register.</param>
        /// <param name="comparer">The equality comparer to be used. Can also be <see langword="null"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        [Throws(typeof(ArgumentOutOfRangeException))]
        public ArrayBasedRegister(int capacity, IEqualityComparer<T> comparer)
        {
            if (capacity < 0) {
                throw new ArgumentOutOfRangeException(nameof(capacity) , "Initial collection capacity cannot be negative.");
            }
            this.comparer = comparer ?? EqualityComparer<T>.Default;
            elements = new T[capacity];
            count = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayBasedRegister{T}"/> class containing the specified items from the specified enumerable of items.
        /// </summary>
        /// <param name="items">The items to be added to this register instance.</param>
        [Throws(typeof(ArgumentNullException))]
        public ArrayBasedRegister(IEnumerable<T> items) : this() => RegisterRange(items);

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayBasedRegister{T}"/> class containing the specified items from the specified enumerable of items.
        /// </summary>
        /// <param name="items">The items to be added to this register instance.</param>
        /// <param name="comparer">The equality comparer to be used. Can also be <see langword="null"/>.</param>
        [Throws(typeof(ArgumentNullException))]
        public ArrayBasedRegister(IEnumerable<T> items, [AllowNull] IEqualityComparer<T> comparer) : this(comparer) => RegisterRange(items);

        private void Grow(int by)
        {
            int new_count = unchecked(count + by);
            if (new_count < 0) {
                // Overflow detected, throw
                throw new OverflowException("The register has reached it's maximum capacity.");
            } else if (new_count > elements.Length) {
                Array.Resize(ref elements, new_count);
            }
        }

        /// <inheritdoc />
        public virtual T this[int index] => ((uint)index >= count) ? throw new ArgumentOutOfRangeException(nameof(index), "Index was out of the register's bounds.") : elements[index];

        /// <inheritdoc />
        public virtual int Count => count;

        /// <inheritdoc />
        public virtual void CopyTo(T[] array, int arrayIndex) => Array.Copy(elements, 0, array, arrayIndex, count);

        /// <inheritdoc />
        public virtual int IndexOf(T item) => item is null ? -1 : ArrayHelpers.FindIndex(elements, 0, count, item , comparer);

        /// <inheritdoc />
        public virtual void Register([AllowNull] T item)
        {
            Grow(1);
            RegisterUnchecked(item);
        }

        private void RegisterUnchecked([AllowNull] T item) => elements[count++] = item;

        /// <inheritdoc />
        public virtual void RegisterRange(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            if (items is T[] array) {
                RegisterRange_Array(array);
            } else if (items is IList<T> list) {
                RegisterRange_List(list);
            } else if (items is ArrayBasedRegister<T> reg) {
                RegisterRange_Register(reg);
            } else {
                bool has_fast_path = true;
                if (items is ICollection<T> c) { 
                    Grow(c.Count); 
                } else if (items is ITraversableCollection<T> t) { 
                    Grow(t.Count);
                } else {
                    has_fast_path = false;
                }
                IEnumerator<T> enumerator = items.GetEnumerator();
                try {
                    if (has_fast_path) {
                        while (enumerator.MoveNext()) { RegisterUnchecked(enumerator.Current); }
                    } else {
                        while (enumerator.MoveNext()) { Register(enumerator.Current); }
                    }
                } finally {
                    enumerator.Dispose();
                }
            }
        }

        private void RegisterRange_Array(T[] data)
        {
            int c = data.Length;
            Grow(c);
            Array.Copy(data, 0, elements, count, c);
            count += c;
        }

        private void RegisterRange_List(IList<T> list)
        {
            int c = list.Count;
            Grow(c);
            for (int I = 0; I < c; I++) { elements[count + I] = list[I]; }
            count += c;
        }

        private void RegisterRange_Register(ArrayBasedRegister<T> reg)
        {
            int c = reg.count;
            Grow(c);
            Array.Copy(reg.elements, 0, elements, count, c);
            count += c;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public virtual IEnumerator<T> GetEnumerator() => ArrayEnumerator<T>.ByBounds(elements, 0, count);

        /// <summary>
        /// Removes all the elements from the current register that cannot be accessed by the public methods of this class. <br />
        /// This does optimize the memory usage of the current object.
        /// </summary>
        public virtual void TrimExcess()
        {
            if (count == 0) {
                elements = Array.Empty<T>();
            } else if (count != elements.Length) {
                Array.Resize(ref elements, count);
            }
        }

        /// <inheritdoc />
        public virtual void EnsureCapacity(int n_elements)
        {
            if (n_elements < 0) {
                throw new ArgumentOutOfRangeException(nameof(n_elements), "Number of elements to be ensured of should not be negative.");
            } else {
                Grow(n_elements);
            }
        }
    }
}