

using System.Collections.Generic;

namespace MP.TagReading.MP4
{
    public sealed class Box
    {
        private System.String type;
        private System.Byte[] data;
        private MP4BOXHEADER header;
        private System.Int32 sizeinternal;

        private Box() { data = null; type = null; }

        public Box(System.String type, System.Byte[] data)
        {
            this.type = type;
            this.data = data;
        }

        public static Box ReadFromStream(System.IO.Stream str)
        {
            Box box = new Box();
            (box.header , box.sizeinternal) = MP4BOXHEADER.ReadHeader(str);
            box.type = box.header.Type;
            box.data = str.ReadBytes(box.Length);
            return box;
        }

        public System.String Type => type;

        public System.Byte[] Data => data;

        public System.Int64 Length => (header.Size == 1 ? header.LongSize : header.Size) - sizeinternal;

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