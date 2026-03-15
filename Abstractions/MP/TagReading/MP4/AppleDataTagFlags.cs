
using System;

namespace MP.TagReading.MP4
{
    /// <summary>
    /// Defines constants for different box flags.
    /// </summary>
    [Flags]
    public enum AppleDataTagFlags : System.Int32
    {
        /// <summary>
        /// Box contains binary data.
        /// </summary>
        Data = 0x000000,
        /// <summary>
        /// Box contains textual data.
        /// </summary>
        Text = 0x000001,
        /// <summary></summary>
        TMPO_CPIL = 0x000015,
        /// <summary>
        /// Box contains image data.
        /// </summary>
        ImageData = 0x00000D
    }

}