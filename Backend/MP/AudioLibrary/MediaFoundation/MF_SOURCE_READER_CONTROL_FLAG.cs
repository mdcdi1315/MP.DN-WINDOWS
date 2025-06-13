


using System;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    ///     The enumeration type defines the various flags that can be passed
    ///     to the Source Reader's ReadSample method.
    /// </summary>
    [Flags]
    public enum MF_SOURCE_READER_CONTROL_FLAG
    {
        /// <summary>
        ///     Specifies that ReadSample should only drain samples from
        ///     the stream, and not request more samples from the media source.
        ///     This can be used to ensure all samples are returned for the 
        ///     stream before flushing, changing position, or changing the
        ///     output media type for the stream.
        /// </summary>
        MF_SOURCE_READER_CONTROLF_DRAIN = 0x00000001,
    }
}