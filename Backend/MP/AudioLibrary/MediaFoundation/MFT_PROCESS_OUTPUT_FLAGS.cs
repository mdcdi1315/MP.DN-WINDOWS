


using System;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// ProcessOutput() dwFlags (signals from caller to MFT)
    /// </summary>
    [Flags]
    public enum MFT_PROCESS_OUTPUT_FLAGS : System.UInt32
    {
        //
        // Carried over from DMO (IMediaObject)
        //
        MFT_PROCESS_OUTPUT_DISCARD_WHEN_NO_BUFFER = 0x00000001, // discard this sample if pSample ptr is NULL.

        // New flags for MFTs
        MFT_PROCESS_OUTPUT_REGENERATE_LAST_OUTPUT = 0x00000002
    }
}