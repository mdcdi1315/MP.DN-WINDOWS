
using System;
using MP.Annotations;
using System.Diagnostics;
using MP.ExceptionSystem;
using MP.NativeInterop.Windows;
using MP.NativeInterop.Windows.COM;

namespace MP.Graphics.Imaging
{
    public unsafe sealed class GDIPlusBitmapReader : IImage, ICloneable
    {
        private bool flipped;
        private byte* image_data;
        private Interop.GdiPlus.GpBitmap bitmap;

        private GDIPlusBitmapReader()
        {
            flipped = false;
            bitmap = default;
            image_data = null;
        }

        public GDIPlusBitmapReader(IImage image) : this()
        {
            ArgumentNullException.ThrowIfNull(image);

            Size i = image.Size;
            Interop.GdiPlus.GpBitmap bm;

            using (IImage img_temp = UnsafeMethods.IsLittleEndian ? image.TranslateToBGRA() : image.TranslateToARGB())
            {
                GDIPlusException.ThrowIfError(
                    Interop.GdiPlus.GdipCreateBitmapFromScan0(
                        i.Width,
                        i.Height,
                        i.Width * image.PixelFormat.GetByteSize(),
                        Interop.GdiPlus.PixelFormat.PixelFormat32bppARGB,
                        img_temp.NativePointer,
                        &bm
                    )
                );
            }

            GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipImageForceValidation(bitmap = bm));
        }

        public GDIPlusBitmapReader(System.IO.Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (!stream.CanRead) {
                throw new ArgumentException("Stream must be readable.", nameof(stream));
            }

            void* p_stream = ComInterop.CreateCCW<IStream>(new SystemIOStreamWrappedStream(stream));
            try {
                Interop.GdiPlus.GpBitmap bitmap;
                GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipCreateBitmapFromStream(p_stream, &bitmap));
                GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipImageForceValidation(this.bitmap = bitmap));
            } finally {
                ComInterop.Release(p_stream);
            }
        }

        public byte* NativePointer
        {
            get {
                VerifyNotDisposed();
                if (image_data is null) {
                    long length = this.GetMemoryByteLength();
                    image_data = (byte*)SystemInfo.GetDefaultMemoryManager().Allocate(length.ToUInt64());
                    Interop.GdiPlus.GpRect rct = new();
                    rct.X = 0;
                    rct.Y = 0;
                    Size s = Size;
                    Interop.GdiPlus.BitmapData bd = new();
                    bd.Width = (rct.Width = s.Width).ToUInt32();
                    bd.Height = (rct.Height = s.Height).ToUInt32();
                    bd.Stride = (bd.Width * 4U).ToInt32();
                    bd.Scan0 = image_data;
                    try {
                        GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipBitmapLockBits(
                            bitmap,
                            &rct,
                            Interop.GdiPlus.ImageLockMode.ImageLockModeRead | Interop.GdiPlus.ImageLockMode.ImageLockModeUserInputBuf,
                            Interop.GdiPlus.PixelFormat.PixelFormat32bppARGB,
                            &bd
                        ));
                        GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipBitmapUnlockBits(bitmap, &bd));
                    } catch {
                        SystemInfo.GetDefaultMemoryManager().Free(image_data);
                        image_data = null;
                        throw;
                    }
                    if (UnsafeMethods.IsLittleEndian) {
                        BGRAColor c;
                        byte* p_current = image_data;
                        int bgra_color_size = sizeof(BGRAColor);
                        for (long I = 0; I < length; I += bgra_color_size, p_current += bgra_color_size)
                        {
                            c = *(BGRAColor*)p_current;
                            *(ARGBColor*)p_current = new(c.A, c.R, c.G, c.B);
                        }
                    }
                }
                return image_data;
            }
        }

        public bool IsFlippedVertically => flipped;

        public ImagePixelFormat PixelFormat => ImagePixelFormat.ARGB;

        public Size Size
        {
            get {
                uint w, h;
                VerifyNotDisposed();
                GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipGetImageHeight(bitmap, &h));
                GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipGetImageWidth(bitmap, &w));
                return new Size(w.ToInt32(), h.ToInt32());
            }
        }

        public SizeF Resolution
        {
            get {
                float h, v;
                VerifyNotDisposed();
                GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipGetImageHorizontalResolution(bitmap, &h));
                GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipGetImageVerticalResolution(bitmap, &v));
                return new(h, v);
            }
            set => GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipBitmapSetResolution(bitmap, value.Width, value.Height));
        }

        public GUID ImageFormat
        {
            get {
                VerifyNotDisposed();
                GUID p;
                GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipGetImageRawFormat(bitmap, &p));
                return p;
            }
        }

        public System.UInt32 Flags
        {
            get {
                VerifyNotDisposed();
                System.UInt32 f;
                GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipGetImageFlags(bitmap, &f));
                return f;
            }
        }

        public void FlipVertically()
        {
            VerifyNotDisposed();
            GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipImageRotateFlip(bitmap, Interop.GdiPlus.RotateFlipType.RotateNoneFlipXY));
            flipped = flipped == false;
        }

        public IColor GetPixel(int x, int y)
        {
            VerifyNotDisposed();
            Interop.GdiPlus.ARGB argb;
            GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipBitmapGetPixel(bitmap, x, y, &argb));
            if (UnsafeMethods.IsLittleEndian) {
                return argb.ReinterpretStructure<Interop.GdiPlus.ARGB, BGRAColor>();
            } else {
                return argb;
            }
        }

        public IColor GetPixel(Point pt) => GetPixel(pt.X, pt.Y);

        public void SetPixel(int x, int y, IColor color)
        {
            VerifyNotDisposed();
            ArgumentNullException.ThrowIfNull(color);
            if (x < 0) {
                throw new ArgumentOutOfRangeException(nameof(x), "X coordinate cannot be a negative value.");
            } else if (y < 0) {
                throw new ArgumentOutOfRangeException(nameof(x), "Y coordinate cannot be a negative value.");
            } else {
                Interop.GdiPlus.ARGB argb = new(color);
                if (UnsafeMethods.IsLittleEndian) { argb = argb.ReverseEndianess(); }
                GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipBitmapSetPixel(bitmap, x, y, argb));
            }
        }

        public void SetPixel(Point pt, IColor color) => SetPixel(pt.X, pt.Y, color);

        public void SaveToStream(System.IO.Stream stream, GUID encoder)
        {
            VerifyNotDisposed();
            ArgumentNullException.ThrowIfNull(stream);
            void* p_stream = ComInterop.CreateCCW<IStream>(new SystemIOStreamWrappedStream(stream));
            try {
                GDIPlusException.ThrowIfError(
                    Interop.GdiPlus.GdipSaveImageToStream(bitmap, p_stream, &encoder)
                );
            } finally {
                ComInterop.Release(p_stream);
            }
        }

        public GDIPlusBitmapReader Clone()
        {
            VerifyNotDisposed();
            GDIPlusBitmapReader r = new();
            Interop.GdiPlus.GpBitmap bm;
            GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipCloneImage(bitmap, (Interop.GdiPlus.GpImage*)&bm));
            r.bitmap = bm;
            r.flipped = flipped;
            return r;
        }

        [DebuggerHidden]
        [StackTraceHidden]
        private void VerifyNotDisposed() => ObjectDisposedException.ThrowIf(bitmap.IsNull, this);

        [RequiresNativeLayer]
        public void Dispose()
        {
            if (bitmap.IsNull) { return; }
            if (image_data is not null) {
                SystemInfo.GetDefaultMemoryManager().Free(image_data);
                image_data = null;
            }
            GDIPlusException.ThrowIfError(Interop.GdiPlus.GdipDisposeImage(bitmap));
            bitmap = default;
        }

        object ICloneable.Clone() => Clone();
    }
}