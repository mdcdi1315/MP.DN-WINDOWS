
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MP.TagReading.MP4
{
    /// <summary>
    /// Common MP4 reader utilities.
    /// </summary>
    public static unsafe class ReaderUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pp"></param>
        /// <returns></returns>
        public static System.UInt32 ReadUInt32BigEndian(ref System.Byte pp)
        {
            System.UInt32 size = sizeof(System.UInt32);
            System.Byte[] dt = new System.Byte[size];
            Unsafe.CopyBlockUnaligned(ref dt[0], ref pp, size);
            if (System.BitConverter.IsLittleEndian) { dt.Reverse(); }
            return dt.ToUInt32(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pp"></param>
        /// <returns></returns>
        public static System.Int64 ReadInt64BigEndian(ref System.Byte pp)
        {
            System.UInt32 size = sizeof(System.Int64);
            System.Byte[] dt = new System.Byte[size];
            Unsafe.CopyBlockUnaligned(ref dt[0], ref pp, size);
            if (System.BitConverter.IsLittleEndian) { dt.Reverse(); }
            return dt.ToInt64(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static System.UInt32 ReadBoxUInt32Normal(this IO.DataStream stream) => stream.ReadUInt32BE();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headerdata"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static System.String ReadASCIIHeaderString(ref System.Byte buffer)
        {
            fixed (System.Byte* p = &buffer)
            {
                return new((System.SByte*)p, 0, 4);
            }
        }
    }
}