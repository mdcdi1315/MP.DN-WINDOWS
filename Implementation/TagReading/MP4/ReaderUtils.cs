
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MP.TagReading.MP4
{
    /// <summary>
    /// Common MP4 reader utilities.
    /// </summary>
    public static unsafe class ReaderUtils
    {
        public static System.UInt32 ReadUInt32BigEndian(ref readonly System.Byte pp)
        {
            System.UInt32 size = sizeof(System.UInt32); 
            System.Byte[] dt = new System.Byte[size];
            Unsafe.CopyBlockUnaligned(ref dt[0], in pp, size);
            if (System.BitConverter.IsLittleEndian) { dt.Reverse(); }
            return dt.ToUInt32(0);
        }

        public static System.UInt32 ReadUInt32BigEndian(System.Byte* p)
        {
            System.UInt32 size = sizeof(System.UInt32);
            System.Byte[] dt = new System.Byte[size];
            Unsafe.CopyBlockUnaligned(ref dt[0], in p[0], size);
            if (System.BitConverter.IsLittleEndian) { dt.Reverse(); }
            return dt.ToUInt32(0);
        }

        public static System.Int64 ReadInt64BigEndian(ref readonly System.Byte pp)
        {
            System.UInt32 size = sizeof(System.Int64);
            System.Byte[] dt = new System.Byte[size];
            Unsafe.CopyBlockUnaligned(ref dt[0], in pp, size);
            if (System.BitConverter.IsLittleEndian) { dt.Reverse(); }
            return dt.ToInt64(0);
        }

        public static System.UInt32 ReadBoxUInt32Normal(this System.IO.Stream stream)
        {
            System.Byte[] dt = stream.ReadBytes(sizeof(System.UInt32));
            return ReadUInt32BigEndian(ref dt[0]);
        }

        public static System.UInt32 ReadBoxUInt32(this System.IO.Stream stream) => ReadBoxUInt32Normal(stream) - sizeof(System.UInt32);
    
        public static System.String[] MP4HeaderReadCompatBrands(System.Byte[] headerdata)
        {
            if (headerdata is null) { return System.Array.Empty<System.String>(); }
            System.String[] ret = new System.String[headerdata.Length / 4];
            fixed (System.Byte* p = headerdata)
            {
                System.Byte* pi = p;
                System.Int32 I = 0;
                while (pi - p < headerdata.Length)
                {
                    ret[I++] = new((System.SByte*)pi, 0, 4);
                    pi += 4;
                }
            }
            return ret;
        }

        public static System.String ReadASCIIHeaderString(ref readonly System.Byte buffer)
        {
            fixed (System.Byte* p = &buffer)
            {
                return new((System.SByte*)p , 0 , 4);
            }
        }
    }
}