using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Specialization of the <see cref="IRegister{T}"/> interface for collections that can be traversed by an index value.
    /// </summary>
    /// <typeparam name="T">The type of the items to be held by this register object.</typeparam>
    public interface ITraversableRegister<T> : IRegister<T>, ITraversableCollection<T>
    {
        /// <summary>
        /// Determines the index of a specific item in the <see cref="IRegister{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IRegister{T}"/>.</param>
        /// <returns> The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        public int IndexOf([AllowNull] T item);
    }
}