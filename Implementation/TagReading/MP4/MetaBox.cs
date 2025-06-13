

using System.Runtime.CompilerServices;

namespace MP.TagReading.MP4
{
    public sealed class MetaBox
    {
        private MP4METABOXCOREHEADER header;
        private System.Byte[] data;

        private MetaBox() { data = null; }

        public static MetaBox ReadFromBox(Box box , System.Int32 index)
        {
            MetaBox bx = new();
            bx.header = box.Data.ReadStructure<MP4METABOXCOREHEADER>(index);
            System.Int32 idxf = index + Unsafe.SizeOf<MP4METABOXCOREHEADER>() , 
                dse = bx.header.DataSize.ToInt32();
            if (bx.header.IsDataHeader)
            {
                idxf += sizeof(System.UInt32);
                dse -= sizeof(System.UInt32);
            }
            bx.data = box.Data.GetBytes(idxf , dse);
            return bx;
        }

        public System.Int32 TotalLength => header.DataSize.ToInt32() + Unsafe.SizeOf<MP4METABOXCOREHEADER>();

        public System.Byte[] Data => data;

        public System.String ID => header.HeaderId;

        public AppleDataTagFlags Flags => header.Flags;

        public unsafe System.String TextData
        {
            get {
                if (header.Flags.HasFlag(AppleDataTagFlags.Text) == false)
                {
                    throw new System.InvalidOperationException("No text data found");
                }
                fixed (System.Byte* p = data) 
                { 
                    return new((System.SByte*)p , 0 , data.Length);
                }
            }
        }
    }
}