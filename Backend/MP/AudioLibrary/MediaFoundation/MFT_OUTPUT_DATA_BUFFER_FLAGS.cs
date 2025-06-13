


using System;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Per-buffer flags that apply to output buffers.
    /// </summary>
    [Flags]
    public enum MFT_OUTPUT_DATA_BUFFER_FLAGS
    {
        //
        // Not carried over from DMO (IMediaObject), but should be
        // reserved so no new MFT flag clashes with them:
        //
        // DMO_OUTPUT_DATA_BUFFER_SYNCPOINT        = 0x00000001,
        // DMO_OUTPUT_DATA_BUFFER_TIME             = 0x00000002,
        // DMO_OUTPUT_DATA_BUFFER_TIMELENGTH       = 0x00000004,

        //
        // This flag means the object can produce more samples without any more input.
        //
        MFT_OUTPUT_DATA_BUFFER_INCOMPLETE = 0x01000000,

        //
        // New for MFT
        //
        MFT_OUTPUT_DATA_BUFFER_FORMAT_CHANGE = 0x00000100,
        MFT_OUTPUT_DATA_BUFFER_STREAM_END = 0x00000200,
        MFT_OUTPUT_DATA_BUFFER_NO_SAMPLE = 0x00000300
    }
}