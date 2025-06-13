

using System;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Flags returned by GetInputStatusFlags()
    /// </summary>
    [Flags]
    public enum MFT_INPUT_STATUS_FLAGS
    {
        //
        // Carried over from DMO (IMediaObject)
        //

        //
        // ACCEPT_DATA indicates that the input stream is ready to accept
        // new data via ProcessInput().
        //
        MFT_INPUT_STATUS_ACCEPT_DATA = 0x00000001
    }
}