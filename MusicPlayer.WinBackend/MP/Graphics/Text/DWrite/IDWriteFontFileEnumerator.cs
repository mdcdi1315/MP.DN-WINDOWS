
using MP.Annotations;
using MP.NativeInterop.Windows;

namespace MP.Graphics.Text.DWrite
{
    /// <summary>
    /// The font file enumerator interface encapsulates a collection of font files. The font system uses this interface
    /// to enumerate font files when building a font collection.
    /// </summary>
    [COMInterfaceGenerator("72755049-5ff7-435d-8348-4be97cfa6c7c")]
    public unsafe partial interface IDWriteFontFileEnumerator
    {
        /// <summary>
        /// Advances to the next font file in the collection. When it is first created, the enumerator is positioned
        /// before the first element of the collection and the first call to MoveNext advances to the first file.
        /// </summary>
        /// <param name="hasCurrentFile">Receives the value TRUE if the enumerator advances to a file, or FALSE if
        /// the enumerator advanced past the last file in the collection.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        HRESULT MoveNext(BOOL* hasCurrentFile);

        /// <summary>
        /// Gets a reference to the current font file.
        /// </summary>
        /// <param name="fontFile">Pointer to the newly created font file object.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        HRESULT GetCurrentFontFile([IsPointerToCOMInterfaceType(typeof(IDWriteFontFile))] void** fontFile);
    }
}
