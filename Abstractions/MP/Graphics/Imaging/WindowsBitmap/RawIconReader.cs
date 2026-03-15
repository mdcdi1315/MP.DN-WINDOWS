
using MP.IO;
using System;
using MP.Annotations;

namespace MP.Graphics.Imaging.WindowsBitmap
{
    /// <summary>
    /// Defines common information about a contained icon bitmap entry. <br />
    /// Returned by the <see cref="RawIconReader"/> class.
    /// </summary>
    public struct IconBitmapEntryData
    {
        /// <summary>
        /// Gets the width, in pixels, of the image.
        /// </summary>
        public System.Int32 Width;
        /// <summary>
        /// Gets the height, in pixels, of the image.
        /// </summary>
        public System.Int32 Height;
        /// <summary>
        /// Gets the number of bits that are used to store a single pixel in the image.
        /// </summary>
        public System.UInt16 BitsPerPixel;
        /// <summary>
        /// Gets the number of planes defined in the image. This will be usually 1.
        /// </summary>
        public System.UInt16 Planes;
        /// <summary>
        /// Gets the size, in bytes, of the specified icon bitmap entry.
        /// </summary>
        public System.UInt32 SizeInBytes;

        /// <summary>
        /// Gets critical information about this <see cref="IconBitmapEntryData"/> instance.
        /// </summary>
        public readonly override System.String ToString()
            => $"IconBitmapEntryData {{ Width = {Width} , Height = {Height} , BitsPerPixel = {BitsPerPixel} , SizeInBytes = {SizeInBytes} }}";
    }

    /// <summary>
    /// Reads bitmaps out from Windows icons. <br />
    /// A Windows icon is essentially a file that stores multiple images that describe the same thing, in different dimensions.
    /// </summary>
    [Preliminary]
    public sealed class RawIconReader : IDisposable
    {
        private PACKAGEHEADER header;
        private System.Boolean strmown;
        private PACKAGEENTRY[] entries;
        private DataStream basestream;
        private System.Int64 basestreamoffset;

        /// <summary>
        /// Creates a new instance of the <see cref="RawIconReader"/> class by reading the icon package specified in <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to read the package from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not readable and seekable.</exception>
        public RawIconReader(DataStream stream)
        {
            basestream = stream;
            if (basestream is null) { throw new ArgumentNullException(nameof(stream)); }
            if (basestream.CanRead == false) { throw new ArgumentException("Stream was unreadable." , nameof(stream)); }
            if (basestream.CanSeek == false) { throw new ArgumentException("Stream was unseekable.", nameof(stream)); }
            basestreamoffset = basestream.Seek(0L, SeekDisplacement.Begin);
            header = basestream.ReadStructure<PACKAGEHEADER>();
            strmown = false;
            if (header.DirEntryType == IconImageType.CURSOR) { throw new ArgumentException("This is not an icon package." , nameof(stream)); }
            if (header.DirEntryCount <= 0) { throw new ArgumentException("This icon does not contain any bitmaps." , nameof(stream)); }
            // Create and read all the entries.
            entries = new PACKAGEENTRY[header.DirEntryCount];
            for (System.Int32 I = 0; I < entries.Length; I++) {
                entries[I] = basestream.ReadStructure<PACKAGEENTRY>();
            }
        }

        /// <summary>
        /// Gets the number of bitmaps that this icon does contain.
        /// </summary>
        public System.Int32 Count => entries.Length;

        /// <summary>
        /// Gets a stream that represents the loaded bitmap , at <paramref name="ordinal"/> parameter.
        /// </summary>
        /// <param name="ordinal">The index of the bitmap to load.</param>
        /// <returns>The bitmap bytes.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The specified ordinal was invalid.</exception>
        public IDataStreamAccess GetBitmapStream(System.Int32 ordinal)
        {
            if (ordinal < 0 || ordinal >= entries.Length) { throw new ArgumentOutOfRangeException(nameof(ordinal) , "The specified image index does not exist , or is invalid."); }
            PACKAGEENTRY entry = entries[ordinal];
            basestream.Seek(basestreamoffset + entry.ImageOffset, SeekDisplacement.Begin);
            var retstream = new MemoryStream(entry.ImageSize);
            basestream.CopySpecificToStream(retstream, entry.ImageSize, 4096);
            retstream.Position = 0;
            return retstream;
        }

        /// <summary>
        /// Gets bitmap information before loading the image itself.
        /// </summary>
        /// <param name="ordinal">The index of the bitmap to query it's information.</param>
        /// <returns>The requested bitmap entry data.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The specified ordinal was invalid.</exception>
        public IconBitmapEntryData GetEntry(System.Int32 ordinal)
        {
            if (ordinal < 0 || ordinal >= entries.Length) { throw new ArgumentOutOfRangeException(nameof(ordinal), "The specified image index does not exist , or is invalid."); }
            var native = entries[ordinal];
            return new() { 
                Width = native.Width, 
                Height = native.Height , 
                BitsPerPixel = native.IconEntry.BitsPerPixel , 
                Planes = native.IconEntry.ColorPlanes , 
                SizeInBytes = native.ImageSize 
            };
        }

        /// <summary>
        /// Gets or sets a value whether the current class has control over the underlying stream , 
        /// when this class instance will be disposed.
        /// </summary>
        public System.Boolean IsStreamOwner
        {
            get => strmown;
            set => strmown = value;
        }

        /// <summary>
        /// Disposes the current instance of <see cref="RawIconReader"/> class.
        /// </summary>
        public void Dispose() 
        {
            entries = null;
            header = default;
            if (strmown && basestream is not null) {
                basestream.Dispose();
            }
            basestream = null;
            basestreamoffset = 0;
        }
    }
}