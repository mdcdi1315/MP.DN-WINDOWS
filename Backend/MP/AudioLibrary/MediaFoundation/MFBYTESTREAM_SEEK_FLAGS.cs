

using System;

namespace MP.AudioLibrary.MediaFoundation
{
    [Flags]
    public enum MFBYTESTREAM_SEEK_FLAGS : System.UInt32
    {
        None = 0,
        CANCEL_PENDING_IO = 0x00000001
    }
}