namespace MP.Graphics.Text.DWrite
{
    /// <summary>
    /// The file format of a complete font face.
    /// Font formats that consist of multiple files, e.g. Type 1 .PFM and .PFB, have
    /// a single enum entry.
    /// </summary>
    public enum DWRITE_FONT_FACE_TYPE
    {
        // The prefix DWRITE_FONT_FACE_TYPE is ommitted for brevity.

        /// <summary>
        /// OpenType font face with CFF outlines.
        /// </summary>
        CFF,

        /// <summary>
        /// OpenType font face with TrueType outlines.
        /// </summary>
        TRUETYPE,

        /// <summary>
        /// OpenType font face that is a part of a TrueType or CFF collection.
        /// </summary>
        OPENTYPE_COLLECTION,

        /// <summary>
        /// A Type 1 font face.
        /// </summary>
        TYPE1,

        /// <summary>
        /// A vector .FON format font face.
        /// </summary>
        VECTOR,

        /// <summary>
        /// A bitmap .FON format font face.
        /// </summary>
        BITMAP,

        /// <summary>
        /// Font face type is not recognized by the DirectWrite font system.
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// The font data includes only the CFF table from an OpenType CFF font.
        /// This font face type can be used only for embedded fonts (i.e., custom
        /// font file loaders) and the resulting font face object supports only the
        /// minimum functionality necessary to render glyphs.
        /// </summary>
        RAW_CFF,

        // The following name is obsolete, but kept as an alias to avoid breaking existing code.
        TRUETYPE_COLLECTION = OPENTYPE_COLLECTION,
    }
}