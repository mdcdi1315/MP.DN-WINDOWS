

using System.Collections.Generic;

namespace MP.Collections
{
    /// <summary>
    /// Defines a way how an object implementing the <see cref="IAttributeable"/> interface can be done as a collection.
    /// </summary>
    public interface ICollectableAttributeable : IEnumerable<AttributeKeyValuePair>, IAttributeable
    {
        /// <summary>
        /// Gets the number of attributes contained in the current object.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets all the keys of the attributes currently defined in the current object.
        /// </summary>
        public IEnumerable<System.String> Keys { get; }
    }
}