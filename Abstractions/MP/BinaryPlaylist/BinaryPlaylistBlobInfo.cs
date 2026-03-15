
using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.BinaryPlaylist
{
    /// <summary>
    /// Provides information about a playlist's blob.
    /// </summary>
    public sealed class BinaryPlaylistBlobInfo
    {
        /// <summary>
        /// Provides a way to build <see cref="BinaryPlaylistBlobInfo"/> instances efficiently.
        /// </summary>
        public sealed class Builder : IObjectBuilder<BinaryPlaylistBlobInfo>
        {
            private string id;
            private BlobFlags flags;
            private System.Int64 length;
            private System.UInt32 count;
            private System.UInt16 version;

            /// <summary>
            /// Constructs a new and empty instance of the <see cref="Builder"/> class.
            /// </summary>
            public Builder()
            {
                id = System.String.Empty;
                flags = BlobFlags.Normal;
                length = 0L;
                count = 0U;
                version = 0;
            }

            /// <inheritdoc />
            [return: NotNull]
            public BinaryPlaylistBlobInfo Build() => new(id, flags, version, count, length);

            /// <summary>Gets/sets the ID of the under construction blob.</summary>
            public string ID
            {
                [return: NotNull]
                [MustNotReportException]
                get => id;
                [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
                set {
                    ArgumentNullException.ThrowIfNull(value);
                    if (value.Length > 4) {
                        throw new ArgumentException("Blob ID's must be no longer than 4 characters.");
                    } else {
                        id = value;
                    }
                }
            }

            /// <summary>
            /// Gets/sets the additional flags of the under construction blob.
            /// </summary>
            public BlobFlags Flags
            {
                [MustNotReportException]
                get => flags;
                [MustNotReportException]
                set => flags = value;
            }

            /// <summary>
            /// Gets/sets the version of the under construction blob.
            /// </summary>
            public System.UInt16 Version
            {
                [MustNotReportException]
                get => version;
                [MustNotReportException]
                set => version = value;
            }

            /// <summary>
            /// Gets/sets the length of the under construction blob.
            /// </summary>
            public System.Int64 Length
            {
                [MustNotReportException]
                get => length;
                [MustNotReportException]
                set => length = value;
            }

            /// <summary>
            /// Gets/sets the number of elements that the under construction blob will have.
            /// </summary>
            public System.UInt32 Count
            {
                [MustNotReportException]
                get => count;
                [MustNotReportException]
                set => count = value;
            }
        }

        private readonly string id;
        private readonly BlobFlags flags;
        private System.Int64 length;
        private System.UInt32 count;
        private readonly System.UInt16 version;

        /// <summary>
        /// Constructs a new instance of the <see cref="BinaryPlaylistBlobInfo"/> class by specifying all the properties at once.
        /// </summary>
        /// <param name="id">The blob's ID.</param>
        /// <param name="flags">Any flags that do specially describe the blob.</param>
        /// <param name="version">Versioning of the blob itself.</param>
        /// <param name="count">Number of elements that are contained in the blob.</param>
        /// <param name="length">The length, in bytes, of the blob.</param>
        /// <exception cref="ArgumentException"><paramref name="id"/> is longer than 4 characters.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        public BinaryPlaylistBlobInfo(string id, BlobFlags flags, System.UInt16 version, System.UInt32 count = 0U, System.Int64 length = 0L)
        {
            ArgumentNullException.ThrowIfNull(id);
            if (id.Length > 4) {
                throw new ArgumentException("Blob ID's must be no longer than 4 characters.");
            } else {
                this.id = id;
                this.flags = flags;
                this.version = version;
                this.count = count;
                this.length = length;
            }
        }

        /// <summary>Gets the ID of this blob.</summary>
        public string ID => id;

        /// <summary>
        /// Gets additional flags for the blob.
        /// </summary>
        public BlobFlags Flags => flags;

        /// <summary>
        /// Gets/sets the length, in bytes, of the blob. <br />
        /// The length can modified when writing new blobs to a stream, <br />
        /// DO not use the setter for any other purpose.
        /// </summary>
        public System.Int64 Length
        {
            get => length;
            set => length = value;
        }

        /// <summary>
        /// Gets/sets the number of elements contained in the blob. <br />
        /// Implementation note: It does not mean that a blob will contain only structures ,  <br />
        /// it can also contain data of arbitrary length. <br /> The field can have the value zero , 
        /// and it is used to assist the programmer in random memory access cases. <br />
        /// Use the <see cref="Length"/> property to learn the exact size of this blob. <br />
        /// </summary>
        public System.UInt32 Count
        {
            get => count;
            set => count = value;
        }

        /// <summary>
        /// Gets the version of the blob. <br />
        /// Implementation note: This is a blob-specific value tied to it. As such, developers are free to choose the value of this for 
        /// documenting their data format version.
        /// </summary>
        public System.UInt16 Version => version;
    }
}
