using MP.IO.DataStructuring;
using MP.IO.DataStructuring.Generation;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Specifies a header of an ID3V2 frame.
    /// </summary>
    [DataStructureGenerator]
    public partial struct ID3V2FRAMEHEADER : IDataStructure
    {
        /// <summary>First byte of the frame's ID.</summary>
        public System.Byte FID0;

        /// <summary>Second byte of the frame's ID.</summary>
        public System.Byte FID1;

        /// <summary>Third byte of the frame's ID.</summary>
        public System.Byte FID2;

        /// <summary>Fourth byte of the frame's ID.</summary>
        public System.Byte FID3;

        /// <summary>
        /// Gets the length of the frame as a <see cref="SYNCHSAFEINT"/> structure.
        /// </summary>
        public SYNCHSAFEINT Length;

        /// <summary>
        /// Gets the writing status of the current frame.
        /// </summary>
        public ID3V2FrameStatusFlags Status;

        /// <summary>
        /// Gets the format which it was used to save the frame.
        /// </summary>
        public ID3V2FrameFormatFlags Format;

        /// <summary>
        /// Gets the frame's ID as a string.
        /// </summary>
        // Frame ID as a string.
        public readonly unsafe System.String FrameID
        {
            get {
                fixed (System.Byte* p = &FID0) { return new((System.SByte*)p, 0, 4); }
            }
        }

        /// <summary>
        /// Gets a value whether this frame header is invalid.
        /// </summary>
        public readonly System.Boolean IsInvalid => FID0 == 0 && FID1 == 0 && FID2 == 0 && FID3 == 0;

        // May more exist , under how flags are defined.

        /// <inheritdoc />
        public readonly override string ToString() => $"Frame Header {{ ID: {FrameID} Status: {Status} Format: {Format} }}";
    }

}