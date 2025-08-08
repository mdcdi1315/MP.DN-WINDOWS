using System.Runtime.InteropServices;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Specifies a header of an ID3V2 frame.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ID3V2FRAMEHEADER
    {
        /// <summary>First byte of the frame's ID.</summary>
        [FieldOffset(0)]
        public System.Byte FID0;

        /// <summary>Second byte of the frame's ID.</summary>
        [FieldOffset(1)]
        public System.Byte FID1;

        /// <summary>Third byte of the frame's ID.</summary>
        [FieldOffset(2)]
        public System.Byte FID2;

        /// <summary>Fourth byte of the frame's ID.</summary>
        [FieldOffset(3)]
        public System.Byte FID3;

        /// <summary>
        /// Gets the length of the frame as a <see cref="SYNCHSAFEINT"/> structure.
        /// </summary>
        [FieldOffset(4)]
        public SYNCHSAFEINT Length;

        /// <summary>
        /// Gets the writing status of the current frame.
        /// </summary>
        [FieldOffset(8)]
        public ID3V2FrameStatusFlags Status;

        /// <summary>
        /// Gets the format which it was used to save the frame.
        /// </summary>
        [FieldOffset(9)]
        public ID3V2FrameFormatFlags Format;

        /// <summary>
        /// Gets the frame's ID as a string.
        /// </summary>
        // Frame ID as a string.
        public readonly System.String FrameID => $"{FID0.ToChar()}{FID1.ToChar()}{FID2.ToChar()}{FID3.ToChar()}";

        /// <summary>
        /// Gets a value whether this frame header is invalid.
        /// </summary>
        public readonly System.Boolean IsInvalid => FID0 == 0 && FID1 == 0 && FID2 == 0 && FID3 == 0;

        // May more exist , under how flags are defined.
    }

}