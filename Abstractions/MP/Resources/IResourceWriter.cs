
using System;

namespace MP.Resources
{
    /// <summary>
    /// Defines the base writer for writing new resource files.
    /// </summary>
    public interface IResourceWriter : IDisposable
    {
        /// <summary>
        /// Adds a new resource entry to the list of the resources to be written.
        /// </summary>
        /// <param name="entry">The resource entry to add.</param>
        public void AddEntry(ResourceEntry entry);

        /// <summary>
        /// Ensures that all the entries written so far are written to the underlying data target.
        /// </summary>
        public void Flush();
    }
}