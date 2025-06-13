


using System;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Flags returned by GetOutputStatusFlags()
    /// </summary>
    [Flags]
    public enum MFT_OUTPUT_STATUS_FLAGS : System.UInt32
    {
        //
        // New for MFT
        //

        //
        // SAMPLE_READY indicates that a sample is available on at least one
        // of the output streams, and a call to ProcessOutput will
        // retrieve it.
        //
        MFT_OUTPUT_STATUS_SAMPLE_READY = 0x00000001
    }
}