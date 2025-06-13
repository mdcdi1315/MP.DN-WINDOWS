

using System;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// SetType flags
    /// </summary>
    [Flags]
    public enum MFT_SET_TYPE_FLAGS : System.UInt32
    {
        // No additional behavior defined.
        None = 0,
        //
        // Carried over from DMO (IMediaObject)
        //
        MFT_SET_TYPE_TEST_ONLY = 0x00000001,// check but don't set

        //
        // not carried over from DMO - use NULL type to unset.
        //
        //MFT_SET_TYPE_CLEAR       = 0x00000002 // unset
    }
}