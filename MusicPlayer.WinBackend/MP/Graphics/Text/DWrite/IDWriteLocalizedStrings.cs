using MP.Annotations;
using MP.NativeInterop.Windows;
using System.Runtime.InteropServices;

namespace MP.Graphics.Text.DWrite
{
    /// <summary>
    /// Represents a collection of strings indexed by locale name.
    /// </summary>
    [COMInterfaceGenerator("08256209-099a-4b34-b86d-c22b110e7771")]
    public unsafe partial interface IDWriteLocalizedStrings
    {
        /// <summary>
        /// Gets the number of language/string pairs.
        /// </summary>
        uint GetCount();

        /// <summary>
        /// Gets the index of the item with the specified locale name.
        /// </summary>
        /// <param name="localeName">Locale name to look for.</param>
        /// <param name="index">Receives the zero-based index of the locale name/string pair.</param>
        /// <param name="exists">Receives TRUE if the locale name exists or FALSE if not.</param>
        /// <returns>
        /// Standard HRESULT error code. If the specified locale name does not exist, the return value is S_OK, 
        /// but *index is UINT_MAX and *exists is FALSE.
        /// </returns>
        HRESULT FindLocaleName(
            [In] char* localeName,
            [Out] uint* index,
            [Out] BOOL* exists
        );

        /// <summary>
        /// Gets the length in characters (not including the null terminator) of the locale name with the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the locale name.</param>
        /// <param name="length">Receives the length in characters, not including the null terminator.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
         HRESULT GetLocaleNameLength(uint index, uint* length);

        /// <summary>
        /// Copies the locale name with the specified index to the specified array.
        /// </summary>
        /// <param name="index">Zero-based index of the locale name.</param>
        /// <param name="localeName">Character array that receives the locale name.</param>
        /// <param name="size">Size of the array in characters. The size must include space for the terminating
        /// null character.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        HRESULT GetLocaleName(
            uint index,
            char* localeName,
            uint size
        );

        /// <summary>
        /// Gets the length in characters (not including the null terminator) of the string with the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the string.</param>
        /// <param name="length">Receives the length in characters, not including the null terminator.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        HRESULT GetStringLength(
            uint index,
            [Out] uint* length
        );

        /// <summary>
        /// Copies the string with the specified index to the specified array.
        /// </summary>
        /// <param name="index">Zero-based index of the string.</param>
        /// <param name="stringBuffer">Character array that receives the string.</param>
        /// <param name="size">Size of the array in characters. The size must include space for the terminating
        /// null character.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
         HRESULT GetString(
            uint index,
            [Out] char* stringBuffer,
            uint size
         );
    }
}
