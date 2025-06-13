


using System;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// ProcessOutput() pdwStatus (output from MFT to caller)
    /// </summary>
    [Flags]
    public enum MFT_PROCESS_OUTPUT_STATUS
    {
        //
        // new for MFT
        //
        MFT_PROCESS_OUTPUT_STATUS_NEW_STREAMS = 0x00000100 // Output flag
    }
}