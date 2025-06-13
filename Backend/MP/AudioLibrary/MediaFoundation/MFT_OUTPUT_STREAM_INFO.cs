


using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    [StructLayout(LayoutKind.Explicit , Size = 12)]
    public struct MFT_OUTPUT_STREAM_INFO
    {
        [FieldOffset(0)]
        public MFT_INPUT_STREAM_INFO_FLAGS Flags;            // MFT_INPUT_STREAM_INFO_FLAGS
        [FieldOffset(4)]
        public System.UInt32 Size;             // size of each sample's buffer
        [FieldOffset(8)]
        public System.UInt32 Alignment;        // buffer alignment requirement
    }
}