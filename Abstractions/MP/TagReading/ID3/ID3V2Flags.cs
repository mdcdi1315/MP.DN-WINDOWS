
using System;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Defines additional behavioral flags for the current ID3V2 tag.
    /// </summary>
    [Flags] // Reversed bit flags.
    public enum ID3V2Flags : System.Byte
    {
        /// <summary>
        /// No additional flags were specified.
        /// </summary>
        None = 0,
        /// <summary>
        /// All frames are unsyncronized
        /// </summary>
        Unsynchronisation = 1,
        /// <summary>
        /// The ID3V2 tag contains an extended header after this header
        /// </summary>
        ExtendedHeader = 2,
        /// <summary>
        /// The ID3V2 tag contains experimental data
        /// </summary>
        Experimental = 4,
        /// <summary>
        /// This ID3V2 tag contains a footer tag at the end of the file.
        /// </summary>
        Footer = 8
    }
}