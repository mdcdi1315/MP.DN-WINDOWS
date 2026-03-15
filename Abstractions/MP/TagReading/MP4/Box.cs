

using System.Collections.Generic;

namespace MP.TagReading.MP4
{
    /// <summary>
    /// Defines a single data item in an MP4 stream. <br />
    /// These are called boxes or in older terms, atoms.
    /// </summary>
    public sealed class Box
    {
        private System.String type;
        private System.Byte[] data;
        private MP4BOXHEADER header;
        private System.Int32 sizeinternal;

        private Box() { data = null; type = null; }

        /// <summary>
        /// Creates a typed <see cref="Box"/> instance with the specified array that contains the data for the newly created box.
        /// </summary>
        /// <param name="type">The type of the new box.</param>
        /// <param name="data">The data that the new box contains.</param>
        public Box(System.String type, System.Byte[] data)
        {
            this.type = type;
            this.data = data;
        }

        /// <summary>
        /// Reads a <see cref="Box"/> instance from an MP4 stream.
        /// </summary>
        /// <param name="str">The <see cref="IO.DataStream"/> to read the new <see cref="Box"/>.</param>
        /// <returns>A newly created box.</returns>
        public static Box ReadFromStream(IO.DataStream str)
        {
            Box box = new Box();
            (box.header, box.sizeinternal) = MP4BOXHEADER.ReadHeader(str);
            box.type = box.header.Type;
            box.data = str.ReadBytes(box.Length);
            return box;
        }

        /// <summary>
        /// Gets the type of the box.
        /// </summary>
        public System.String Type => type;

        /// <summary>
        /// Gets the value of the box.
        /// </summary>
        public System.Byte[] Data => data;

        /// <summary>
        /// Gets the total length of this box.
        /// </summary>
        public System.Int64 Length => (header.Size == 1 ? header.LongSize : header.Size) - sizeinternal;

        /// <summary>
        /// Returns additional meta-boxes if they exist.
        /// </summary>
        /// <returns>The read meta-boxes.</returns>
        public MetaBox[] GetMetaBoxes()
        {
            List<MetaBox> boxes = new();
            MetaBox temp = null;
            System.Int32 I = 0;
            while (I < data.Length)
            {
                temp = MetaBox.ReadFromBox(this, I);
                I += temp.TotalLength;
                boxes.Add(temp);
            }
            return boxes.ToArray();
        }
    }
}