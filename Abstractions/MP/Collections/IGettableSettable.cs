

namespace MP.Collections
{
    /// <summary>
    /// Defines the indexer property to those classes that their implementing interfaces do not enforce it. <br />
    /// Multiple interface implementations may coexist in a class indicating that different indexers are supported.
    /// </summary>
    /// <typeparam name="TIndex">The type of the index to use.</typeparam>
    /// <typeparam name="T">The type set or retrieved through the indexer property.</typeparam>
    public interface IGettableSettable<in TIndex, T>
    {
        /// <summary>Gets or sets a value.</summary>
        /// <param name="index">The index value to get or set the specified value to.</param>
        /// <returns>When retrieveing a value, it returns the value obtained through <paramref name="index"/>.</returns>
        /// <exception cref="System.NotSupportedException">Thrown when a getter or setter or both are not supported by the collection class.</exception>
        public T this[TIndex index] { get; set; }
    }
}