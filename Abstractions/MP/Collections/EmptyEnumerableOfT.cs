
using System.Collections;
using System.Collections.Generic;

namespace MP.Collections
{
    /// <summary>
    /// Defines an enumerable that has no elements (i.e. is empty). <br />
    /// Useful when working with <see cref="IEnumerable{T}"/> instances. <br />
    /// Additionally, this class is implemented as a record, which it means that you can check for equality with another enumerable instance.
    /// </summary>
    /// <typeparam name="T">The type of the elements that would be returned if this class was not used.</typeparam>
    public sealed record class EmptyEnumerable<T> : IEnumerable<T>
    {
        /// <summary>
        /// Creates a new <see cref="EmptyEnumerable{T}"/> class instance.
        /// </summary>
        public EmptyEnumerable() { }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => new EmptyEnumerator<T>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
