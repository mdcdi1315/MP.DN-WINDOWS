
using MP.IO;
using System;
using MP.Annotations;
using System.Collections.Generic;
using MP.NativeInterop.Windows.GDI;

namespace MP.Graphics.Imaging.WindowsBitmap
{
    /// <summary>
    /// Produces a Windows Icon image from a list of images. <br />
    /// NOTE: Still work on progress, this does not work well yet. 
    /// Do not use it from your code.
    /// </summary>
    [Preliminary]
    public unsafe sealed class RawIconWriter : IDisposable
    {
        private PACKAGEHEADER pkgheader;
        private List<PACKAGEENTRY> entries;
        private List<MemoryStream> memstreams;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawIconWriter"/> class.
        /// </summary>
        public RawIconWriter()
        {
            pkgheader = new();
            pkgheader.DirEntryCount = 0;
            pkgheader.DirEntryType = IconImageType.ICON;
            entries = new(10);
            memstreams = new();
        }

        private void VerifySizeValid(Size size)
        {
            if (size.Width > 256 || size.Height > 256)
            {
                throw new ArgumentException("Image sizes larger than 256 pixels cannot be defined.");
            }
            foreach (var entry in entries) 
            {
                if (entry.Height == size.Height && entry.Width == size.Width)
                {
                    throw new InvalidOperationException("Attempted to add an image entry with the same image size.");
                }
            }
        }

        /// <summary>Adds a new image to the icon.</summary>
        /// <param name="img">The image to write.</param>
        /// <exception cref="ArgumentNullException"><paramref name="img"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="img"/> was larger than 256 pixels.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="img"/> was already added to this icon writer.</exception>
        /// <exception cref="ObjectDisposedException">This writer has been disposed.</exception>
        public void AddImage(IImage img)
        {
            ObjectDisposedException.ThrowIf(entries is null, this);
            if (img is null) { throw new ArgumentNullException(nameof(img)); }
            VerifySizeValid(img.Size);
            CreateImage(img.GetPixels2DPlane(), img.IsFlippedVertically);
            pkgheader.DirEntryCount++;
        }

        // NOTE: I want to provide better alternatives rather than allocating a second array.
        private void CreateImage(IColor[,] colors , System.Boolean flipped)
        {
            BITMAPINFOHEADER bh = new();
            bh.Width = colors.GetLength(0);
            System.Int32 y = colors.GetLength(1);
            bh.Height = flipped ? y : -y;
            bh.Compression = BitmapImageType.BI_RGB;
            bh.Planes = 1;
            bh.BitCount = 32;
            bh.ColorIndices = 0;
            bh.ImportantIndices = 0;
            MemoryStream ms = new(bh.Size + (colors.GetLongLength(0) * colors.GetLongLength(1) * 4));
            bh.Height *= 2; // Icons do require this to define the 1-bit monochrome bitmap after them. For such entries this is empty, see below.
            ms.WriteStructure(bh);
            IColor cl;
            Span<byte> temp = new System.Byte[4];
            for (System.Int32 Y = 0; Y < y; Y++)
            {
                for (System.Int32 X = 0; X < bh.Width; X++)
                {
                    cl = colors[X, Y];
                    temp[0] = cl.B;
                    temp[1] = cl.G;
                    temp[2] = cl.R;
                    temp[3] = cl.A;
                    ms.Write(temp);
                }
            }
            // We need also to fill in the empty 1-bit monochrome bitmap.
            ms.Write(new System.Byte[(((bh.Width + 15) >> 4) << 1) * y]);
            // Create the entry, save the current memory stream.
            memstreams.Add(ms);
            entries.Add(new() { 
                Height = y.ToByte() , 
                Width = bh.Width.ToByte() , 
                IconEntry = new() {
                    BitsPerPixel = 32, 
                    ColorPlanes = 1 
                } , 
                ImageSize = ms.Length.ToUInt32() 
            });
            // Done!!
        }

        /// <summary>
        /// Saves the current icon to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to save the constructed icon data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unwriteable.</exception>
        /// <exception cref="ObjectDisposedException">This writer has been disposed.</exception>
        public void Save(IDataStreamAccess stream)
        {
            ObjectDisposedException.ThrowIf(entries is null, this);
            ArgumentNullException.ThrowIfNull(stream);
            if (stream.CanWrite == false) { throw new ArgumentException("Stream was unwriteable.", nameof(stream)); }
            System.UInt32 baseofs = (sizeof(PACKAGEHEADER) + pkgheader.DirEntryCount * sizeof(PACKAGEENTRY)).ToUInt32();
            stream.WriteStructure(pkgheader);
            foreach (PACKAGEENTRY entry in entries)
            {
                PACKAGEENTRY petemp = entry;
                petemp.ImageOffset = baseofs;
                baseofs += petemp.ImageSize;
                stream.WriteStructure(petemp);
            }
            foreach (var iconstream in memstreams)
            {
                iconstream.Seek(0, SeekDisplacement.Begin);
                iconstream.DirectCopyToStream(stream);
            }
        }

        /*
        public void Save(System.String filepath)
        {
            ObjectDisposedException.ThrowIf(entries is null, this);
            if (System.String.IsNullOrEmpty(filepath)) { throw new ArgumentNullException(nameof(filepath)); }
            FileStream fsm = null;
            try {
                fsm = new(filepath , FileMode.Create);
                Save(fsm);
            } finally {
                fsm?.Dispose();
                fsm = null;
            }
        }*/

        /// <summary>
        /// Disposes this <see cref="RawIconWriter"/> class instance.
        /// </summary>
        public void Dispose()
        {
            if (memstreams is not null)
            {
                foreach (var ms in memstreams) { ms.Dispose(); }
                memstreams.Clear();
                memstreams = null;
            }
            if (entries is not null)
            {
                entries.Clear();
                entries = null;
            }
        }
    }
}