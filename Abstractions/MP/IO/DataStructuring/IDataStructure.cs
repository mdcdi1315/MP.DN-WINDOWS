
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.IO.DataStructuring
{
    /// <summary>
    /// Represents a data structure, that is a structure containing fields that can be read from and written to a data stream.
    /// </summary>
    public interface IDataStructure
    {
        /// <summary>Loads the data structure from the specified stream.</summary>
        /// <param name="access">The data stream to load the structure from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="access"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        void Load(IDataStreamAccess access);

        /// <summary>Saves the data structure from the specified stream.</summary>
        /// <param name="access">The data stream to save the structure to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="access"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        void Save(IDataStreamAccess access);
    }
}
