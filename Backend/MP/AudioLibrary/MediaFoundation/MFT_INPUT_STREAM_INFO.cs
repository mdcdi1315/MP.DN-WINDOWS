


using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    [StructLayout(LayoutKind.Explicit , Size = 24)]
    public struct MFT_INPUT_STREAM_INFO
    {
        [FieldOffset(0)]
        public System.Int64 MaxLatency;      // maximum time latency in 100ns increments
        [FieldOffset(8)]
        public MFT_INPUT_STREAM_INFO_FLAGS Flags;            // MFT_INPUT_STREAM_INFO_FLAGS
        [FieldOffset(12)]
        public System.UInt32 Size;             // size of each sample's buffer
        [FieldOffset(16)]
        public System.UInt32 MaxLookahead;     // max total bytes held
        [FieldOffset(20)]
        public System.UInt32 Alignment;        // buffer alignment requirement
    }
}