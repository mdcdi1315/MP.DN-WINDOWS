namespace MP.Graphics.Text.DWrite
{
    /// <summary>
    /// The type of a font represented by a single font file.
    /// Font formats that consist of multiple files, e.g. Type 1 .PFM and .PFB, have
    /// separate enum values for each of the file type.
    /// </summary>
    public enum DWRITE_FONT_FILE_TYPE
    {
        // The prefix DWRITE_FONT_FILE_TYPE is ommitted for brevity.

        /// <summary>
        /// Font type is not recognized by the DirectWrite font system.
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// OpenType font with CFF outlines.
        /// </summary>
        CFF,

        /// <summary>
        /// OpenType font with TrueType outlines.
        /// </summary>
        TRUETYPE,

        /// <summary>
        /// OpenType font that contains a TrueType collection.
        /// </summary>
        OPENTYPE_COLLECTION,

        /// <summary>
        /// Type 1 PFM font.
        /// </summary>
        TYPE1_PFM,

        /// <summary>
        /// Type 1 PFB font.
        /// </summary>
        TYPE1_PFB,

        /// <summary>
        /// Vector .FON font.
        /// </summary>
        VECTOR,

        /// <summary>
        /// Bitmap .FON font.
        /// </summary>
        BITMAP,

        // The following name is obsolete, but kept as an alias to avoid breaking existing code.
        TRUETYPE_COLLECTION = OPENTYPE_COLLECTION,
    }
}