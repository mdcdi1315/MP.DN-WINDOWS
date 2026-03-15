

namespace MP.IO.DataStructuring.Generation
{
    /// <summary>Provides constants for common string field encodings.</summary>
    public enum StringFieldEncoding : System.Byte
    {
        /// <summary>Provides the ASCII string encoding.</summary>
        ASCII,
        /// <summary>Provides the UTF-8 string encoding.</summary>
        UTF8,
        /// <summary>Provides the UTF-16 Little Endian string encoding.</summary>
        UTF16LE,
        /// <summary>Provides the UTF-16 Big Endian string encoding.</summary>
        UTF16BE
    }
}