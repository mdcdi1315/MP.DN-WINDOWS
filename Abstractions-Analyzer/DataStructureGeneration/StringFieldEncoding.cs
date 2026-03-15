using Microsoft.CodeAnalysis;

namespace MP.AbstractionsLib.Analyzer.DataStructureGeneration
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

    public static class StringFieldEncodingHelpers
    {
        public static System.String ToSystemTextEncodingInstance(this StringFieldEncoding sfe) => sfe switch {
            StringFieldEncoding.ASCII => "System.Text.Encoding.ASCII",
            StringFieldEncoding.UTF8 => "System.Text.Encoding.UTF8",
            StringFieldEncoding.UTF16LE => "System.Text.Encoding.Unicode",
            StringFieldEncoding.UTF16BE => "System.Text.Encoding.BigEndianUnicode",
            _ => null
        };

        public static StringFieldEncoding ParseFromTC(TypedConstant constant)
        {
            if (constant.IsNull) {
                return StringFieldEncoding.ASCII;
            } else {
                return (StringFieldEncoding)System.Enum.Parse(typeof(StringFieldEncoding), constant.Value.ToString());
            }
        }
    }
}