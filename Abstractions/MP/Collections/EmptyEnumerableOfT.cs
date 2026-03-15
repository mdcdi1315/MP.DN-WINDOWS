
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Defines an enumerable that has no elements (i.e. is empty). <br />
    /// Useful when working with <see cref="IEnumerable{T}"/> instances. <br />
    /// Additionally, this class is implemented as a record, which it means that you can check for equality with another enumerable instance.
    /// </summary>
    /// <typeparam name="T">The type of the elements that would be returned if this class was not used.</typeparam>
    public sealed class EmptyEnumerable<T> : IEnumerable<T>, IEquatable<EmptyEnumerable<T>>, ISyncronized
    {
        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => new EmptyEnumerator<T>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public override int GetHashCode() => 0; // We can perfectly assume a hash code of zero since we do not have any instance data.

        /// <inheritdoc />
        public bool Equals(EmptyEnumerable<T> other) => other is not null;

        /// <inheritdoc />
        public override bool Equals([AllowNull] object o) => o is EmptyEnumerable<T>;

        /// <summary>
        /// Gets a string that describes the current <see cref="EmptyEnumerable{T}"/> instance.
        /// </summary>
        /// <returns>A string that describes the current empty enumerable object.</returns>
        public override string ToString() => $"IEnumerable<{typeof(T).FullName}> {{ }}";
    }
}
