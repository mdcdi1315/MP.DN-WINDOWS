

using System;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>Per-buffer flags that apply to input buffers</summary>
    [Flags]
    public enum MFT_INPUT_DATA_BUFFER_FLAGS : System.UInt32
    {
        //
        // Not carried over from DMO (IMediaObject), but should be
        // reserved so no new MFT flag clashes with them:
        //
        // DMO_INPUT_DATA_BUFFER_SYNCPOINT       = 0x00000001,
        // DMO_INPUT_DATA_BUFFER_TIME            = 0x00000002,
        // DMO_INPUT_DATA_BUFFER_TIMELENGTH      = 0x00000004

        MFT_INPUT_DATA_BUFFER_PLACEHOLDER = 0xFFFFFFFF // right now there are no flags defined
    }
}