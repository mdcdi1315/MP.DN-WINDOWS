
using System;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;

namespace MP.Resources
{
    /// <summary>
    /// Defines the base reader for reading resource files, <br />
    /// which are files containing data and their access to them is optimized.
    /// </summary>
    public interface IResourceReader : IEnumerable<ResourceEntry>, IDisposable
    {
        private sealed class DefaultEnumerator : IEnumerator<ResourceEntry>
        {
            private ResourceEntry entry;
            private IResourceReader reader;
            private IEnumerator<System.String> keysenumerator;

            public DefaultEnumerator(IResourceReader r) => keysenumerator = (reader = r).Keys.GetEnumerator();

            public ResourceEntry Current
            {
                get {
                    ObjectDisposedException.ThrowIf(reader is null, this);
                    return entry;
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                reader = null;
                keysenumerator = null;
            }

            public bool MoveNext()
            {
                bool value = keysenumerator.MoveNext();
                if (value) {
                    System.String s = keysenumerator.Current;
                    entry = new(s, reader.GetValue(s));
                }
                return value;
            }

            public void Reset() {
                ObjectDisposedException.ThrowIf(reader is null, this);
                keysenumerator.Reset();
            }
        }

        /// <summary>
        /// Gets all the resource keys contained in the current resource reader.
        /// </summary>
        public ICollection<System.String> Keys {
            [return: MaybeReturnEmptyCollectionButNeverNull]
            get; 
        }

        /// <summary>
        /// Gets a resource value by the specified key.
        /// </summary>
        /// <param name="key">The key that the resource you want is associated with.</param>
        /// <returns>The associated value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> was <see langword="null"/>.</exception>
        public Object GetValue(String key);

        /// <summary>
        /// Gets an enumerator, enumerating through all the resource entries defined by the current resource reader.
        /// </summary>
        /// <returns>An enumerator implementation for traversing through the held resource entries.</returns>
        public new IEnumerator<ResourceEntry> GetEnumerator() => new DefaultEnumerator(this);
    }
}