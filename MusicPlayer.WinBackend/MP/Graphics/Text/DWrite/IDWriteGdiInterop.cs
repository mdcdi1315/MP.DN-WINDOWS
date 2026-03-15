
using MP.Annotations;
using MP.NativeInterop.Windows;
using MP.NativeInterop.Windows.GDI;

namespace MP.Graphics.Text.DWrite
{
    /// <summary>
    /// The GDI interop interface provides interoperability with GDI.
    /// </summary>
    [COMInterfaceGenerator("1edd9491-9853-4299-898f-6432983b6f3a")]
    public unsafe partial interface IDWriteGdiInterop
    {
        /// <summary>
        /// Creates a font object that matches the properties specified by the LOGFONT structure
        /// in the system font collection (GetSystemFontCollection).
        /// </summary>
        /// <param name="logFont">Structure containing a GDI-compatible font description.</param>
        /// <param name="font">Receives a newly created font object if successful, or NULL in case of error.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        HRESULT CreateFontFromLOGFONT(
            LOGFONTW* logFont,
            void** font // IDWriteFont
        );

        /// <summary>
        /// Initializes a LOGFONT structure based on the GDI-compatible properties of the specified font.
        /// </summary>
        /// <param name="font">Specifies a font.</param>
        /// <param name="logFont">Structure that receives a GDI-compatible font description.</param>
        /// <param name="isSystemFont">Contains TRUE if the specified font object is part of the system font collection
        /// or FALSE otherwise.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        HRESULT ConvertFontToLOGFONT(
            void* font, // IDWriteFont
            LOGFONTW* logFont,
            BOOL* isSystemFont
        );

        /// <summary>
        /// Initializes a LOGFONT structure based on the GDI-compatible properties of the specified font.
        /// </summary>
        /// <param name="font">Specifies a font face.</param>
        /// <param name="logFont">Structure that receives a GDI-compatible font description.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        HRESULT ConvertFontFaceToLOGFONT(
            void* font, // IDWriteFontFace
            LOGFONTW* logFont
        );

        /// <summary>
        /// Creates a font face object that corresponds to the currently selected HFONT.
        /// </summary>
        /// <param name="hdc">Handle to a device context into which a font has been selected. It is assumed that the client
        /// has already performed font mapping and that the font selected into the DC is the actual font that would be used 
        /// for rendering glyphs.</param>
        /// <param name="fontFace">Contains the newly created font face object, or NULL in case of failure.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        HRESULT CreateFontFaceFromHdc(
            System.IntPtr hdc,
            void** fontFace // IDWriteFontFace
        );

        /// <summary>
        /// Creates an object that encapsulates a bitmap and memory DC which can be used for rendering glyphs.
        /// </summary>
        /// <param name="hdc">Optional device context used to create a compatible memory DC.</param>
        /// <param name="width">Width of the bitmap.</param>
        /// <param name="height">Height of the bitmap.</param>
        /// <param name="renderTarget">Receives a pointer to the newly created render target.</param>
        HRESULT CreateBitmapRenderTarget(
            System.IntPtr hdc,
            System.UInt32 width,
            System.UInt32 height,
            [IsPointerToCOMInterfaceType(typeof(IDWriteBitmapRenderTarget))] void** renderTarget
        );
    }
}
