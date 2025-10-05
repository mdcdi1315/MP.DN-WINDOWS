
using MP;
using MP.Graphics.Imaging;

namespace System.Drawing
{
    /// <summary>
    /// Creates typed cursors from a specified <see cref="IImage"/> object,
    /// and a <see cref="Point"/> specifying the cursor's hotspot.
    /// </summary>
    public unsafe sealed class CursorImage : IDisposable , ICloneable
    {
        private sealed class CursorImgAsSafeHandle : Runtime.InteropServices.SafeHandle
        {
            public CursorImgAsSafeHandle(System.IntPtr dupethis) : base(0 , true)
            {
                handle = global::Interop.User32.CopyImage(dupethis, global::Interop.User32.ImageCopyType.IMAGE_CURSOR, 0, 0, 0);
                if (handle == IntPtr.Zero) {
                    throw new MP.ExceptionSystem.NativeWindowsException();
                }
            }

            public override System.Boolean IsInvalid => handle == IntPtr.Zero;

            protected override System.Boolean ReleaseHandle() => global::Interop.User32.DestroyCursor(handle) != global::Interop.BOOL.FALSE;
        }

        private Size imgsize;
        private Point hotspot;
        private System.IntPtr hcur;

        private CursorImage(System.IntPtr todupe)
        {
            hcur = global::Interop.User32.CopyImage(todupe, global::Interop.User32.ImageCopyType.IMAGE_CURSOR, 0, 0, 0);
            if (hcur == IntPtr.Zero) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
        }

        private static System.UInt32 ComputeImageSize(ImagePixelFormat pf , MP.Graphics.Size se , out System.Int32 align)
        {
            System.Int32 bc = pf switch {
                ImagePixelFormat.RGB => 24,
                ImagePixelFormat.RGBA => 32,
                ImagePixelFormat.ARGB => 32,
                _ => throw new NotSupportedException($"Unsupported image data layout: {pf}")
            };
            // In each scan line we may need a byte alignment as MSFT says, so do a + the alignment 8 the # of scan lines.
            // If no alignment is required it will just return the expected size.
            return ((se.Height * se.Width * (bc / 8)) + (se.Height * (align = UnsafeMethods.PadAlignment(4, se.Width * bc / 8)))).ToUInt32();
        }

        private static void FromARGB(System.Byte* psrc , System.Byte* pdest , MP.Graphics.Size size)
        {
            System.Byte* temp1, temp2;
            for (System.Int32 Y = 0; Y < size.Height; Y++)
            {
                for (System.Int32 X = 0; X < size.Width; X++)
                {
                    temp1 = psrc + (Y * size.Width + X) * 4;
                    temp2 = pdest + (Y * size.Width + X) * 4;
                    // ARGB to BGRA translation
                    temp2[0] = temp1[3]; // B
                    temp2[1] = temp1[2]; // G
                    temp2[2] = temp1[1]; // R
                    temp2[3] = temp1[0]; // A
                }
            }
        }

        private static void FromRGBA(System.Byte* psrc, System.Byte* pdest, MP.Graphics.Size size)
        {
            System.Byte* temp1, temp2;
            for (System.Int32 Y = 0; Y < size.Height; Y++)
            {
                for (System.Int32 X = 0; X < size.Width; X++)
                {
                    temp1 = psrc + (Y * size.Width + X) * 4;
                    temp2 = pdest + (Y * size.Width + X) * 4;
                    // RGBA to BGRA translation
                    temp2[0] = temp1[2]; // B
                    temp2[1] = temp1[1]; // G
                    temp2[2] = temp1[0]; // R
                    temp2[3] = temp1[3]; // A
                }
            }
        }

        private static void FromRGB(System.Byte* psrc, System.Byte* pdest, MP.Graphics.Size size , System.Int32 align)
        {
            System.Byte* temp1, temp2 = pdest;
            for (System.Int32 Y = 0; Y < size.Height; Y++)
            {
                for (System.Int32 X = 0; X < size.Width; X++)
                {
                    temp1 = psrc + (Y * size.Width + X) * 4;
                    // RGB to BGR translation
                    temp2[0] = temp1[2]; // B
                    temp2[1] = temp1[1]; // G
                    temp2[2] = temp1[0]; // R
                    temp2 += 3;
                }
                // Add alignment, if required.
                temp2 += align;
            }
        }

        private void CreateCursor(IImage image)
        {
            BITMAPV4HEADER hdr = new();
            hdr.CoreHeader.Height = image.IsFlippedVertically ? image.Size.Height : -image.Size.Height;
            hdr.CoreHeader.ImageSize = ComputeImageSize(image.PixelFormat , image.Size , out System.Int32 align24bit);
            hdr.CoreHeader.Compression = ImageType.BI_RGB;
            hdr.ColorSpace = BitmapColorSpace.SRGB;
            hdr.CoreHeader.Width = image.Size.Width;
            hdr.CoreHeader.ImportantIndices = 0;
            hdr.CoreHeader.ColorIndices = 0;
            hdr.CoreHeader.BitCount = 32;
            hdr.CoreHeader.Planes = 1;

            System.IntPtr dc = global::Interop.User32.GetDC(System.IntPtr.Zero);

            if (dc == IntPtr.Zero) {
                throw new InvalidOperationException("Cannot acquire a handle to the screen's device context.");
            }

            System.Byte* pbits;
            System.IntPtr hmask = IntPtr.Zero, hbm = global::Interop.Gdi32.CreateDIBSection(dc , (BITMAPINFOHEADER*)&hdr , global::Interop.Gdi32.DIBSECTION_USAGE.DIB_RGB_COLORS , (void**)&pbits , System.IntPtr.Zero , 0);

            System.Int32 erc = global::Interop.Kernel32.GetLastError();

            global::Interop.User32.ReleaseDC(System.IntPtr.Zero, dc);

            if (hbm == IntPtr.Zero)
            {
                if (erc != 0) {
                    throw new MP.ExceptionSystem.NativeWindowsException(erc);
                }
                throw new InvalidOperationException("Cannot create the bitmap handle for a reason. This is unexpected.");
            }

            try {
                // Verify that we can create hmask too.
                hmask = global::Interop.Gdi32.CreateBitmap(hdr.CoreHeader.Width, hdr.CoreHeader.Height, 1, 1, null);
                if (hmask == IntPtr.Zero) {
                    throw new MP.ExceptionSystem.NativeWindowsException();
                }

                // Now copy the raw bitmap data
                switch (image.PixelFormat)
                {
                    case ImagePixelFormat.RGB:
                        FromRGB(image.NativePointer, pbits, image.Size, align24bit);
                        break;
                    case ImagePixelFormat.RGBA:
                        FromRGBA(image.NativePointer, pbits, image.Size);
                        break;
                    case ImagePixelFormat.ARGB:
                        FromARGB(image.NativePointer , pbits, image.Size);
                        break;
                }
                
                // Complete an ICONINFO structure, and return hcur.
                global::Interop.User32.ICONINFO ii = new();
                ii.IsIcon = global::Interop.BOOL.FALSE; // Data are specifying a cursor
                ii.XHotspot = hotspot.X.ToUInt32();
                ii.YHotspot = hotspot.Y.ToUInt32();
                ii.HMaskBitmap = hmask;
                ii.HColorBitmap = hbm;

                hcur = global::Interop.User32.CreateIconIndirect(ii);
                // record the error, keep it after we free hbm and hmask respectively
                erc = global::Interop.Kernel32.GetLastError();
            } finally {
                // We do not need to check for an error for both DeleteObject calls
                // since the DC used is released from a long time ago...
                if (hbm != IntPtr.Zero) {
                    global::Interop.Gdi32.DeleteObject(hbm);
                    hbm = IntPtr.Zero;
                }
                if (hmask != IntPtr.Zero) {
                    global::Interop.Gdi32.DeleteObject(hmask);
                    hmask = IntPtr.Zero;
                }
            }

            // This should run on failure
            if (hcur == IntPtr.Zero)
            {
                if (erc != 0) {
                    throw new MP.ExceptionSystem.NativeWindowsException(erc);
                }
                throw new InvalidOperationException("Cannot create the handle to the cursor for a reason.");
            }
        }

        System.Object ICloneable.Clone() => Clone();

        /// <summary>
        /// Creates a <see cref="CursorImage"/> class instance from the specified image object, and the hotspot to apply to the newly created cursor.
        /// </summary>
        /// <param name="image">The image to convert as a cursor.</param>
        /// <param name="hotspot">The hotspot to be used in the newly created cursor.</param>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> was <see langword="null"/>.</exception>
        public CursorImage(IImage image, Point hotspot)
        {
            ArgumentNullException.ThrowIfNull(image, nameof(image));
            imgsize = new(image.Size.Width , image.Size.Height);
            this.hotspot = hotspot;
            CreateCursor(image);
        }

        /// <summary>
        /// The actual handle to the cursor <br />
        /// Must be passed to windowing API's
        /// </summary>
        public System.IntPtr Handle => hcur;
        
        /// <summary>
        /// The hotspot (the point where is the tip of the cursor)
        /// </summary>
        public Point HotSpot => hotspot;

        /// <summary>
        /// Gets the image size of the cursor, in pixels.
        /// </summary>
        public Size ImageSize => imgsize;

        /// <summary>Creates a copy of the current instance.</summary>
        /// <returns>The newly created copy. Must be disposed seperately.</returns>
        /// <exception cref="ObjectDisposedException">The current instance has been disposed of.</exception>
        /// <exception cref="MP.ExceptionSystem.NativeWindowsException">The copy was not performed successfully.</exception>
        public CursorImage Clone()
        {
            ObjectDisposedException.ThrowIf(hcur == IntPtr.Zero, this);
            CursorImage ci = new(hcur);
            ci.imgsize = imgsize;
            ci.hotspot = hotspot;
            return ci;
        }

        /// <summary>
        /// Creates a copy of the current instance, and returns it as a safe handle. <br />
        /// Suitable to be used in WPF scenarios.
        /// </summary>
        /// <returns>
        /// A new <see cref="Runtime.InteropServices.SafeHandle"/> representing a copy of this <see cref="CursorImage"/> instance. <br />
        /// Must be sepearately disposed of.
        /// </returns>
        /// <exception cref="ObjectDisposedException">The current instance has been disposed of.</exception>
        /// <exception cref="MP.ExceptionSystem.NativeWindowsException">The copy was not performed successfully.</exception>
        public Runtime.InteropServices.SafeHandle GetSafeHandle()
        {
            ObjectDisposedException.ThrowIf(hcur == IntPtr.Zero, this);
            return new CursorImgAsSafeHandle(hcur);
        }

        /// <summary>
        /// Disposes this <see cref="CursorImage"/> , if everything goes well.
        /// </summary>
        /// <exception cref="MP.ExceptionSystem.NativeWindowsException">A native error occured while calling DestroyCursor.</exception>
        public void Dispose()
        {
            if (hcur != IntPtr.Zero) {
                global::Interop.BOOL bl = global::Interop.User32.DestroyCursor(hcur);
                if (bl == global::Interop.BOOL.FALSE) {
                    throw new MP.ExceptionSystem.NativeWindowsException();
                }
                hcur = IntPtr.Zero;
            }
        }
    }
}