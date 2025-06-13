
using System;
using Microsoft.IO;
using System.Drawing;
using System.Collections.Generic;

namespace MP.Imaging
{
    public unsafe sealed class RawIconWriter : IDisposable
    {
        private PACKAGEHEADER pkgheader;
        private List<PACKAGEENTRY> entries;
        private List<MemoryStream> memstreams;

        public RawIconWriter()
        {
            pkgheader = new();
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

        public void AddImage(IImage img)
        {
            ObjectDisposedException.ThrowIf(entries is null, this);
            if (img is null) { throw new ArgumentNullException(nameof(img)); }
            VerifySizeValid(img.Size);
            CreateImage(img.GetPixels2DPlane(), img.IsFlippedVertically);
        }

        private void CreateImage(Color[,] colors , System.Boolean flipped)
        {
            BITMAPV4HEADER bv4 = new();
            bv4.CoreHeader.Width = colors.GetLength(0);
            System.Int32 y = colors.GetLength(1);
            bv4.CoreHeader.Height = flipped ? y : -y;
            y = bv4.CoreHeader.Height;
            bv4.CoreHeader.Compression = ImageType.BI_RGB;
            bv4.CoreHeader.Planes = 1;
            bv4.CoreHeader.BitCount = 32;
            bv4.CoreHeader.ColorIndices = 0;
            bv4.CoreHeader.ImportantIndices = 0;
            bv4.ColorSpace = BitmapColorSpace.SRGB;
            MemoryStream ms = new();
            bv4.CoreHeader.Height *= 2;
            ms.WriteStructure(bv4);
            Color cl;
            for (System.Int32 Y = 0; Y < y; Y++)
            {
                for (System.Int32 X = 0; X < bv4.CoreHeader.Width; X++)
                {
                    cl = colors[X, Y];
                    ms.Write([cl.B , cl.G , cl.R , cl.A]);
                }
            }
            // We need also to fill in the empty 1-bit monochrome bitmap.
            System.Byte[] justempty = new System.Byte[(((bv4.CoreHeader.Width + 15) >> 4) << 1) * y];
            ms.Write(justempty, 0, justempty.Length);
            // Create the entry, save the current memory stream.
            memstreams.Add(ms);
            entries.Add(new() { 
                Height = y.ToByte() , 
                Width = bv4.CoreHeader.Width.ToByte() , 
                IconEntry = new() {
                    BitsPerPixel = 32, 
                    ColorPlanes = 1 
                } , 
                ImageSize = ms.Length.ToUInt32() 
            });
            // Done!!
        }

        public void Save(System.IO.Stream stream)
        {
            ObjectDisposedException.ThrowIf(entries is null, this);
            if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
            if (stream.CanWrite == false) { throw new ArgumentException("Stream was unwriteable.", nameof(stream)); }
            System.UInt32 baseofs = (sizeof(PACKAGEHEADER) + entries.Count * sizeof(PACKAGEENTRY)).ToUInt32();
            pkgheader.DirEntryCount = entries.Count.ToUInt16();
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
                iconstream.Seek(0, System.IO.SeekOrigin.Begin);
                iconstream.CopyTo(stream);
            }
            // Done!
        }

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
        }

        public void Dispose()
        {
            if (memstreams is not null)
            {
                foreach (var ms in memstreams)
                {
                    ms.Dispose();
                }
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