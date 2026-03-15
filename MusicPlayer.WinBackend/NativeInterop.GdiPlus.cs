
using System;
using MP.Graphics;
using MP.Annotations;
using MP.NativeInterop.Windows;
using System.Runtime.InteropServices;

using unsafe NotificationUnhookProc = delegate* unmanaged[Stdcall]<System.UIntPtr, void>;
using unsafe DebugEventProc = delegate* unmanaged[Stdcall]<Interop.GdiPlus.DebugEventLevel, char*, void>;
using unsafe NotificationHookProc = delegate* unmanaged[Stdcall]<System.UIntPtr*, Interop.GdiPlus.GpStatus>;

partial class Interop
{
    public static unsafe class GdiPlus
    {
        public enum DebugEventLevel
        {
            DebugEventLevelFatal,
            DebugEventLevelWarning
        }

        public enum GpStatus
        {
            Ok = 0,
            GenericError = 1,
            InvalidParameter = 2,
            OutOfMemory = 3,
            ObjectBusy = 4,
            InsufficientBuffer = 5,
            NotImplemented = 6,
            Win32Error = 7,
            WrongState = 8,
            Aborted = 9,
            FileNotFound = 10,
            ValueOverflow = 11,
            AccessDenied = 12,
            UnknownImageFormat = 13,
            FontFamilyNotFound = 14,
            FontStyleNotFound = 15,
            NotTrueTypeFont = 16,
            UnsupportedGdiplusVersion = 17,
            GdiplusNotInitialized = 18,
            PropertyNotFound = 19,
            PropertyNotSupported = 20,
            ProfileNotFound = 21,
        }

        public enum PixelFormat : System.Int32
        {
            PixelFormatIndexed = 0x00010000, // Indexes into a palette
            PixelFormatGDI = 0x00020000, // Is a GDI-supported format
            PixelFormatAlpha = 0x00040000, // Has an alpha component
            PixelFormatPAlpha = 0x00080000, // Pre-multiplied alpha
            PixelFormatExtended = 0x00100000, // Extended color 16 bits/channel
            PixelFormatCanonical = 0x00200000,

            PixelFormat1bppIndexed = (1 | ( 1 << 8) | PixelFormatIndexed | PixelFormatGDI),
            PixelFormat4bppIndexed = (2 | ( 4 << 8) | PixelFormatIndexed | PixelFormatGDI),
            PixelFormat8bppIndexed = (3 | ( 8 << 8) | PixelFormatIndexed | PixelFormatGDI),
            PixelFormat16bppGrayScale  = (4 | (16 << 8) | PixelFormatExtended),
            PixelFormat16bppRGB555 = (5 | (16 << 8) | PixelFormatGDI),
            PixelFormat16bppRGB565 = (6 | (16 << 8) | PixelFormatGDI),
            PixelFormat16bppARGB1555 = (7 | (16 << 8) | PixelFormatAlpha | PixelFormatGDI),
            PixelFormat24bppRGB = (8 | (24 << 8) | PixelFormatGDI),
            PixelFormat32bppRGB = (9 | (32 << 8) | PixelFormatGDI),
            PixelFormat32bppARGB = (10 | (32 << 8) | PixelFormatAlpha | PixelFormatGDI | PixelFormatCanonical),
            PixelFormat32bppPARGB = (11 | (32 << 8) | PixelFormatAlpha | PixelFormatPAlpha | PixelFormatGDI),
            PixelFormat48bppRGB = (12 | (48 << 8) | PixelFormatExtended),
            PixelFormat64bppARGB = (13 | (64 << 8) | PixelFormatAlpha  | PixelFormatCanonical | PixelFormatExtended),
            PixelFormat64bppPARGB = (14 | (64 << 8) | PixelFormatAlpha  | PixelFormatPAlpha | PixelFormatExtended),
            PixelFormat32bppCMYK  = (15 | (32 << 8))
        }

        public enum DitherType
        {
            DitherTypeNone = 0,
            DitherTypeSolid = 1,
            DitherTypeOrdered4x4 = 2,
            DitherTypeOrdered8x8 = 3,
            DitherTypeOrdered16x16 = 4,
            DitherTypeSpiral4x4 = 5,
            DitherTypeSpiral8x8 = 6,
            DitherTypeDualSpiral4x4 = 7,
            DitherTypeDualSpiral8x8 = 8,
            DitherTypeErrorDiffusion = 9,
            DitherTypeMax = 10
        }

        public enum PaletteType
        {
            PaletteTypeCustom = 0,
            PaletteTypeOptimal = 1,
            PaletteTypeFixedBW = 2,
            PaletteTypeFixedHalftone8 = 3,
            PaletteTypeFixedHalftone27 = 4,
            PaletteTypeFixedHalftone64 = 5,
            PaletteTypeFixedHalftone125 = 6,
            PaletteTypeFixedHalftone216 = 7,
            PaletteTypeFixedHalftone252 = 8,
            PaletteTypeFixedHalftone256 = 9
        }

        [Flags]
        public enum PaletteFlags : System.UInt32
        {
            PaletteFlagsHasAlpha = 0x0001,
            PaletteFlagsGrayScale = 0x0002,
            PaletteFlagsHalftone = 0x0004
        }

        public enum ImageType
        {
            ImageTypeUnknown,
            ImageTypeBitmap,
            ImageTypeMetafile
        }

        public enum RotateFlipType
        {
            RotateNoneFlipNone = 0,
            Rotate90FlipNone = 1,
            Rotate180FlipNone = 2,
            Rotate270FlipNone = 3,
            RotateNoneFlipX = 4,
            Rotate90FlipX = 5,
            Rotate180FlipX = 6,
            Rotate270FlipX = 7,
            RotateNoneFlipY,
            Rotate90FlipY,
            Rotate180FlipY,
            Rotate270FlipY,
            RotateNoneFlipXY,
            Rotate90FlipXY,
            Rotate180FlipXY,
            Rotate270FlipXY
        }

        [Flags]
        public enum ImageLockMode
        {
            ImageLockModeRead = 0x0001,
            ImageLockModeWrite = 0x0002,
            ImageLockModeUserInputBuf = 0x0004
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GdiplusStartupInput
        {
            public System.UInt32 Version;

            public DebugEventProc DebugEventCallback;

            public BOOL SuppressBackgroundThread;

            public BOOL SuppressExternalCodecs;

            public GdiplusStartupInput()
            {
                Version = 1;
                DebugEventCallback = null;
                SuppressExternalCodecs = BOOL.FALSE;
                SuppressBackgroundThread = BOOL.FALSE;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GdiplusStartupOutput
        {
            public NotificationHookProc NotificationHook;
            public NotificationUnhookProc NotificationUnhook;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ColorPalette
        {
            public PaletteFlags Flags;
            public System.UInt32 Count;
            // ARGB Entries[1];
        }

        public readonly struct GpBitmap : MP.Utilities.INullable
        {
            public readonly void* Pointer;

            public GpBitmap(void* ptr) => Pointer = ptr;

            public bool IsNull => Pointer is null;

            public static implicit operator GpImage(GpBitmap b) => new(b.Pointer);
        }

        public readonly struct GpImage : MP.Utilities.INullable
        {
            public readonly void* Pointer;

            public GpImage(void* ptr) => Pointer = ptr;

            public bool IsNull => Pointer is null;
        }

        [StructLayout(LayoutKind.Explicit)]
        public readonly struct ARGB : IColor
        {
            [FieldOffset(0)]
            private readonly byte A;

            [FieldOffset(1)]
            private readonly byte R;

            [FieldOffset(2)]
            private readonly byte G;

            [FieldOffset(3)]
            private readonly byte B;

            public ARGB(IColor c)
            {
                A = c.A;
                R = c.R;
                G = c.G;
                B = c.B;
            }

            byte IColor.A => A;

            byte IColor.R => R;

            byte IColor.G => G;

            byte IColor.B => B;
        }

        public readonly struct PROPID
        {
            public readonly System.UInt32 Value;

            public PROPID(System.UInt32 v) => Value = v;
        }

        //---------------------------------------------------------------------------
        // Property Item
        //---------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential)]
        public struct PropertyItem
        {
            public PROPID id;                    // ID of this property
            public System.UInt32 length; // Length of the property value, in bytes
            public System.UInt16 type;    // Type of the value, as one of TAG_TYPE_XXX
                                                          // defined above
            public void* value;                 // property value
        }

        public struct GpRect
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        //---------------------------------------------------------------------------
        // Information about image pixel data
        //---------------------------------------------------------------------------
        public struct BitmapData
        {
            public System.UInt32 Width;
            public System.UInt32 Height;
            public System.Int32 Stride;
            public PixelFormat PixelFormat;
            public void* Scan0;
            public System.UIntPtr Reserved;
        }

        [DllImport(Libraries.GdiPlus, EntryPoint = "GdiplusStartup", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        private static extern GpStatus GdiplusStartup_Native(
                UIntPtr* token,
                GdiplusStartupInput* input,
                GdiplusStartupOutput* output
        );

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern void GdiplusShutdown(UIntPtr token);

        public static GpStatus GdiplusStartup(out UIntPtr token, GdiplusStartupInput startup_input, out GdiplusStartupOutput output)
        {
            UIntPtr t;
            GdiplusStartupOutput o;
            GpStatus ret = GdiplusStartup_Native(&t, &startup_input, &o);
            output = o;
            token = t;
            return ret;
        }

        // Bitmap functions.

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipCreateBitmapFromStream([IsPointerToCOMInterfaceType(typeof(MP.NativeInterop.Windows.COM.IStream))] void* stream, GpBitmap* bitmap);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipCreateBitmapFromStreamICM([IsPointerToCOMInterfaceType(typeof(MP.NativeInterop.Windows.COM.IStream))] void* stream, GpBitmap* bitmap);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipBitmapGetPixel(GpBitmap bitmap, int x, int y, ARGB* color);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipBitmapSetPixel(GpBitmap bitmap, int x, int y, ARGB color);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipCreateBitmapFromScan0(
            int width,
            int height,
            int stride,
            PixelFormat format,
            System.Byte* scan0, // "height * stride"
            GpBitmap* bitmap
        );

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipBitmapConvertFormat(
            GpBitmap pInputBitmap,
            PixelFormat format,
            DitherType dithertype,
            PaletteType palettetype,
            ColorPalette* palette,
            float alphaThresholdPercent
        );

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipBitmapSetResolution(GpBitmap bitmap, float xdpi, float ydpi);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipBitmapLockBits(
            GpBitmap bitmap,
            GpRect* rect,
            ImageLockMode flags,
            PixelFormat format,
            BitmapData* lockedBitmapData);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipBitmapUnlockBits(GpBitmap bitmap, BitmapData* lockedBitmapData);

        // Image functions.

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipDisposeImage(GpImage image);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipCloneImage(GpImage image, GpImage* cloneImage);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipGetImageType(GpImage image, ImageType* type);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipGetImageWidth(GpImage image, System.UInt32* width);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipGetImageHeight(GpImage image, System.UInt32* height);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipGetImageHorizontalResolution(GpImage image, float* resolution);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)] 
        public static extern GpStatus GdipGetImageVerticalResolution(GpImage image, float* resolution);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipGetImageFlags(GpImage image, System.UInt32* flags);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipGetImageRawFormat(GpImage image, GUID* format);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipGetImagePixelFormat(GpImage image, PixelFormat* format);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipImageForceValidation(GpImage image);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipImageRotateFlip(GpImage image, RotateFlipType rfType);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipGetPropertyCount(GpImage image, System.UInt32* numOfProperty);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipGetPropertyIdList(GpImage image, System.UInt32 numOfProperty, PROPID* list);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipGetPropertyItemSize(GpImage image, PROPID propId, System.UInt32* size);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipGetPropertyItem(GpImage image, PROPID propId, System.UInt32 propSize, PropertyItem* buffer);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipRemovePropertyItem(GpImage image, PROPID propId);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipSetPropertyItem(GpImage image, PropertyItem* item);

        [DllImport(Libraries.GdiPlus, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern GpStatus GdipSaveImageToStream(
            GpImage image, 
            [IsPointerToCOMInterfaceType(typeof(MP.NativeInterop.Windows.COM.IStream))] void* stream,
            GUID* clsidEncoder,
            /* GDIPCONST EncoderParameters */ void* encoderParams = null);
    }
}