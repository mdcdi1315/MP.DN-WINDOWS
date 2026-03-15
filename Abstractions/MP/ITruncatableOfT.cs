
using System;
using MP.Annotations.CodeAnalysis;

namespace MP
{
    /// <summary>
    /// Implemented by a value type or a class, indicating that an instance of type <typeparamref name="T"/> 
    /// can be created by the implemententing class inheriting the field values that the current instance has,
    /// but it might loose some information such as arithmetic precision.
    /// </summary>
    /// <typeparam name="T">The type that can be created from this class but it can lose some information.</typeparam>
    public interface ITruncatable<T>
    {
        /// <summary>Creates a truncated clone of this instance.</summary>
        /// <returns>An instance of type <typeparamref name="T"/> representing the cloned and truncated result.</returns>
        /// <exception cref="OutOfMemoryException">Truncating can be a memory-expensive operation, and as such, object creation may inherently fail.</exception>
        [Throws(typeof(OutOfMemoryException))]
        public T Truncate();
    }
}