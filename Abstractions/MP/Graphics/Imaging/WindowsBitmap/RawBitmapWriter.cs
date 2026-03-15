
using MP.IO;
using System;
using MP.NativeInterop.Windows.GDI;

namespace MP.Graphics.Imaging.WindowsBitmap
{
    /// <summary>
    /// Creates .BMP bitmaps.
    /// </summary>
    public unsafe sealed class RawBitmapWriter : IDisposable
    {
        private MemoryStream stream;

        /// <summary>
        /// Creates a new bitmap writer by using the specified image.
        /// </summary>
        /// <param name="image">The image to write as a bitmap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> was null.</exception>
        /// <exception cref="ArgumentException"><paramref name="image"/> does not fall into a 256-pixel dimension boundary.</exception>
        public RawBitmapWriter(IImage image)
        {
            if (image is null) { throw new ArgumentNullException(nameof(image)); }
            stream = new();
            CreateImage(image);
        }

        private void CreateImage(IImage img)
        {
            BITMAPV4HEADER bmi = new();
            bmi.CoreHeader.Width = img.Size.Width;
            bmi.CoreHeader.Height = img.IsFlippedVertically ? img.Size.Height : -img.Size.Height;
            bmi.CoreHeader.Compression = BitmapImageType.BI_RGB;
            bmi.CoreHeader.Planes = 1;
            bmi.ColorSpace = BitmapColorSpace.SRGB;
            switch (img.PixelFormat)
            {
                case ImagePixelFormat.RGB:
                    bmi.CoreHeader.BitCount = 24;
                    Create24BitRGB(bmi, img.NativePointer);
                    break;
                case ImagePixelFormat.RGBA:
                    bmi.CoreHeader.BitCount = 32;
                    Create32BitRGBA(bmi, img.NativePointer);
                    break;
                case ImagePixelFormat.ARGB:
                    bmi.CoreHeader.BitCount = 32;
                    Create32BitARGB(bmi, img.NativePointer);
                    break;
            }
        }

        private void WriteHeaders(BITMAPV4HEADER hdr)
        {
            BITMAPFILEHEADER bfh = new();
            bfh.Type = BITMAPFILEHEADER.BMTYPE;
            bfh.Offset = (sizeof(BITMAPFILEHEADER) + hdr.CoreHeader.Size).ToUInt32();
            bfh.Size = (sizeof(BITMAPFILEHEADER) + hdr.CoreHeader.Size + hdr.CoreHeader.DataSize).ToUInt32();
            // We can specify the capacity of the memory stream, so specify it
            stream.Capacity = bfh.Size;
            stream.WriteStructure(bfh);
            stream.WriteStructure(hdr);
        }

        // Provided for reference only.
        private void WriteHeaders(BITMAPINFOHEADER hdr)
        {
            BITMAPFILEHEADER bfh = new();
            bfh.Type = BITMAPFILEHEADER.BMTYPE;
            bfh.Offset = (sizeof(BITMAPFILEHEADER) + hdr.Size).ToUInt32();
            bfh.Size = (sizeof(BITMAPFILEHEADER) + hdr.Size + hdr.DataSize).ToUInt32();
            // We can specify the capacity of the memory stream, so specify it
            stream.Capacity = bfh.Size;
            stream.WriteStructure(bfh);
            stream.WriteStructure(hdr);
        }

        private void Create24BitRGB(BITMAPV4HEADER hdr , System.Byte* pdata)
        {
            System.Byte* temp;
            System.Int32 alignment = UnsafeMethods.PadAlignment(4, (hdr.CoreHeader.Width * hdr.CoreHeader.BitCount) / 8);
            System.Byte[] tempmgd = new System.Byte[3];
            // 24-bit images are not exact multiples of DWORD's and thus when their widths are not multiples of 4 do need alignment,
            hdr.CoreHeader.ImageSize = ((hdr.CoreHeader.AbsoluteHeight * hdr.CoreHeader.Width * 3) + (hdr.CoreHeader.AbsoluteHeight * alignment)).ToUInt32();
            WriteHeaders(hdr);
            for (System.Int32 Y = 0; Y < hdr.CoreHeader.AbsoluteHeight; Y++)
            {
                for (System.Int32 X = 0; X < hdr.CoreHeader.Width; X++)
                {
                    temp = pdata + (Y * hdr.CoreHeader.Width + X) * 3;
                    tempmgd[0] = temp[2]; // B
                    tempmgd[1] = temp[1]; // G
                    tempmgd[2] = temp[0]; // R
                    stream.Write(tempmgd, 0, 3);
                }
                // 24-bit images may require an alignment thus we must add a pad to each scan line end.
                stream.WritePadString("IMGPAD", alignment);
            }
        }

        private void Create32BitARGB(BITMAPV4HEADER hdr, System.Byte* pdata)
        {
            hdr.CoreHeader.ImageSize = (hdr.CoreHeader.AbsoluteHeight * hdr.CoreHeader.Width * 4).ToUInt32();
            WriteHeaders(hdr);
            System.Byte* temp;
            System.Byte[] tempmgd = new System.Byte[4];
            for (System.Int32 Y = 0; Y < hdr.CoreHeader.AbsoluteHeight; Y++)
            {
                for (System.Int32 X = 0; X < hdr.CoreHeader.Width; X++)
                {
                    temp = pdata + (Y * hdr.CoreHeader.Width + X) * 4;
                    tempmgd[0] = temp[3]; // B
                    tempmgd[1] = temp[2]; // G
                    tempmgd[2] = temp[1]; // R
                    tempmgd[3] = temp[0]; // A
                    stream.Write(tempmgd, 0, 4);
                }
            }
        }

        private void Create32BitRGBA(BITMAPV4HEADER hdr, System.Byte* pdata)
        {
            hdr.CoreHeader.ImageSize = (hdr.CoreHeader.AbsoluteHeight * hdr.CoreHeader.Width * 4).ToUInt32();
            WriteHeaders(hdr);
            System.Byte* temp;
            System.Byte[] tempmgd = new System.Byte[4];
            for (System.Int32 Y = 0; Y < hdr.CoreHeader.AbsoluteHeight; Y++)
            {
                for (System.Int32 X = 0; X < hdr.CoreHeader.Width; X++)
                {
                    temp = pdata + (Y * hdr.CoreHeader.Width + X) * 4;
                    tempmgd[0] = temp[2]; // B
                    tempmgd[1] = temp[1]; // G
                    tempmgd[2] = temp[0]; // R
                    tempmgd[3] = temp[3]; // A
                    stream.Write(tempmgd, 0, 4);
                }
            }
        }

        /// <summary>
        /// Saves the created image to a stream.
        /// </summary>
        /// <param name="stream">The stream to save the image to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not writeable.</exception>
        public void Save(IDataStreamAccess stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));
            if (stream.CanWrite == false) { throw new ArgumentException("Stream was not writeable.", nameof(stream)); }
            ObjectDisposedException.ThrowIf(this.stream is null, this);
            this.stream.Position = 0;
            this.stream.DirectCopyToStream(stream);
        }

        /*
        /// <summary>
        /// Saves the created image into a file. If the file exists , it will be overwritten.
        /// </summary>
        /// <param name="file">The file to write the image to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> was null or represented the empty string.</exception>
        public void Save(System.String file)
        {
            if (stream is null) { throw new ObjectDisposedException(nameof(RawBitmapWriter)); }
            if (System.String.IsNullOrEmpty(file)) { throw new ArgumentNullException(nameof(file), "File name must not be empty."); }
            using (var fs = new FileStream(file , FileMode.Create , FileAccess.Write))
            {
                stream.Position = 0;
                stream.CopyTo(fs);
            }
        }*/

        /// <summary>
        /// Disposes all the resources that the <see cref="RawBitmapWriter"/> class has used.
        /// </summary>
        public void Dispose() 
        {
            stream?.Dispose();
            stream = null;
        }
    }
}