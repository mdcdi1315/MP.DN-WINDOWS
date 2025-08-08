

using System.Runtime.CompilerServices;

namespace MP.TagReading.MP4
{
    /// <summary>
    /// Defines a metadata box in another MP4 box.
    /// </summary>
    public sealed class MetaBox
    {
        private MP4METABOXCOREHEADER header;
        private System.Byte[] data;

        private MetaBox() { data = null; }

        /// <summary>
        /// Reads a <see cref="MetaBox"/> from another <see cref="Box"/>.
        /// </summary>
        /// <param name="box">The box where the <see cref="MetaBox"/> to read is located to.</param>
        /// <param name="index">The index inside the byte array to start reading from.</param>
        /// <returns>A new <see cref="MetaBox"/> instance.</returns>
        public static MetaBox ReadFromBox(Box box, System.Int32 index)
        {
            MetaBox bx = new();
            bx.header = box.Data.ReadStructure<MP4METABOXCOREHEADER>(index);
            System.Int32 idxf = index + Unsafe.SizeOf<MP4METABOXCOREHEADER>(),
                dse = bx.header.DataSize.ToInt32();
            if (bx.header.IsDataHeader)
            {
                idxf += sizeof(System.UInt32);
                dse -= sizeof(System.UInt32);
            }
            bx.data = box.Data.GetBytes(idxf, dse);
            return bx;
        }

        /// <summary>
        /// The total length of the <see cref="MetaBox"/>, including the size of it's header.
        /// </summary>
        public System.Int32 TotalLength => header.DataSize.ToInt32() + Unsafe.SizeOf<MP4METABOXCOREHEADER>();

        /// <summary>
        /// The value referenced by the current <see cref="MetaBox"/>.
        /// </summary>
        public System.Byte[] Data => data;

        /// <summary>
        /// The ID of the current <see cref="MetaBox"/>.
        /// </summary>
        public System.String ID => header.HeaderId;

        /// <summary>
        /// Behavioral flags of the current <see cref="MetaBox"/>.
        /// </summary>
        public AppleDataTagFlags Flags => header.Flags;

        /// <summary>
        /// Gets the string value associated with the current <see cref="MetaBox"/>. <br />
        /// Requires the <see cref="AppleDataTagFlags.Text"/> flag to have been defined.
        /// </summary>
        public unsafe System.String TextData
        {
            get
            {
                if (header.Flags.HasFlag(AppleDataTagFlags.Text) == false)
                {
                    throw new System.InvalidOperationException("No text data found");
                }
                fixed (System.Byte* p = data)
                {
                    return new((System.SByte*)p, 0, data.Length);
                }
            }
        }
    }
}