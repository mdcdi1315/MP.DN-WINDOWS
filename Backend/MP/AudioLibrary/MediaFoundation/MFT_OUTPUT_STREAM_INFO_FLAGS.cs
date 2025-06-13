

using System;

namespace MP.AudioLibrary.MediaFoundation
{
    // Flags returned by GetOutputStreamInfo()
    [Flags]
    public enum MFT_OUTPUT_STREAM_INFO_FLAGS
    {
        //
        // Carried over from DMO (IMediaObject)
        //
        MFT_OUTPUT_STREAM_WHOLE_SAMPLES = 0x00000001,
        MFT_OUTPUT_STREAM_SINGLE_SAMPLE_PER_BUFFER = 0x00000002,
        MFT_OUTPUT_STREAM_FIXED_SAMPLE_SIZE = 0x00000004,
        MFT_OUTPUT_STREAM_DISCARDABLE = 0x00000008,
        MFT_OUTPUT_STREAM_OPTIONAL = 0x00000010,

        //
        // New for MFT
        //
        MFT_OUTPUT_STREAM_PROVIDES_SAMPLES = 0x00000100,
        MFT_OUTPUT_STREAM_CAN_PROVIDE_SAMPLES = 0x00000200,
        MFT_OUTPUT_STREAM_LAZY_READ = 0x00000400,
        MFT_OUTPUT_STREAM_REMOVABLE = 0x00000800
    }
}