
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MP.Collections
{
    /// <summary>
    /// Defines an enumerator that does not return any elements. <br />
    /// This class is used by the <see cref="EmptyEnumerable{T}"/> class, but it can be also used and on it's own foundation for other classes as well.
    /// </summary>
    /// <typeparam name="T">The type of the elements that would be enumerated if this implementation was returning any elements.</typeparam>
    public sealed class EmptyEnumerator<T> :
        IEquatable<EmptyEnumerator<T>>,
        IEnumerator<T>,
        ICloneable
    {
        /// <inheritdoc />
        public T Current => default;

        object IEnumerator.Current => Current;

        /// <summary>
        /// Disposes this <see cref="EmptyEnumerator{T}"/> instance.
        /// </summary>
        public void Dispose() { }

        /// <inheritdoc />
        public bool MoveNext() => false;

        /// <inheritdoc />
        public void Reset() { }

        /// <inheritdoc />
        /// <remarks>
        /// This method is defined for the interface implementation. The only case that this does not return <see langword="true"/> is if <paramref name="other"/> is <see langword="null"/>.
        /// </remarks>
        public bool Equals(EmptyEnumerator<T> other) => other is not null; // Whatever the other instance is, they are equal, except for null.

        /// <summary>
        /// Checks whether the object specified is of type <see cref="EmptyEnumerator{T}"/>.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>A value whether <paramref name="obj"/> is of type <see cref="EmptyEnumerator{T}"/>.</returns>
        public override bool Equals(object obj) => obj is EmptyEnumerator<T>;

        System.Object ICloneable.Clone() => this; // Returning the object itself is perfectly valid.

        /// <inheritdoc />
        public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
    }
}
