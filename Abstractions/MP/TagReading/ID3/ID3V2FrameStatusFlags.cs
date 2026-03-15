
using System;

namespace MP.TagReading.ID3
{

    /// <summary>
    /// Defines different ID3V2 frame writing status flags.
    /// </summary>
    [Flags]
    public enum ID3V2FrameStatusFlags : System.Byte
    {
        /// <summary>No special behavior is defined.</summary>
        None = 0,
        /// <summary>The tag containing this frame should be discarded.</summary>
        DiscardTag = 2,
        /// <summary>The frame must not be read.</summary>
        DiscardFrame = 4,
        /// <summary>The frame must not be modified.</summary>
        ReadOnly = 8
    }

}