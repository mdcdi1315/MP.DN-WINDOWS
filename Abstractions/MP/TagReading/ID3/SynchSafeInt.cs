
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Syncrosation-safe integer.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]
    public struct SYNCHSAFEINT
    {
        /// <summary>The first byte of the syncrosation-safe integer.</summary>
        [FieldOffset(0)]
        public System.Byte b0;

        /// <summary>The second byte of the syncrosation-safe integer.</summary>
        [FieldOffset(1)]
        public System.Byte b1;

        /// <summary>The third byte of the syncrosation-safe integer.</summary>
        [FieldOffset(2)]
        public System.Byte b2;

        /// <summary>The fourth byte of the syncrosation-safe integer.</summary>
        [FieldOffset(3)]
        public System.Byte b3;

        /// <summary>
        /// Converts this syncrosation-safe integer to an 32-bit integer.
        /// </summary>
        /// <returns>The 32-bit integer corresponding to the value of this instance.</returns>
        public readonly System.Int32 ToInt32()
        {
            System.Int32 result = b3;
            result |= (b2 << 7);
            result |= (b1 << 14);
            return result | (b0 << 21);
        }

        /// <summary>
        /// Converts this syncrosation-safe integer to an 32-bit integer. <br />
        /// This is intended to be used by older tags that were using the old fallback mechanism.
        /// </summary>
        /// <returns>The 32-bit integer corresponding to the value of this instance.</returns>
        // For ID3V2 tags with major version < 4.
        public readonly System.Int32 ToInt32Old()
        {
            System.Byte[] data = new System.Byte[] { b0, b1, b2, b3 };
            if (System.BitConverter.IsLittleEndian) { data.Reverse(); }
            return data.ToInt32(0);
        }

        // Below code borrowed from https://github.com/larsbs/id3v2lib/blob/dev/src/modules/utils.c#L111
        // See syncint_encode function.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Encode(int value)
        {
            int outg = 0, mask = 0x7F;

            while ((mask ^ 0x7FFFFFFF) != 0)
            {
                outg = value & ~mask;
                outg <<= 1;
                outg |= value & mask;
                mask = ((mask + 1) << 8) - 1;
                value = outg;
            }

            return outg;
        }

        /// <summary>
        /// Creates a new syncrosation-safe integer from the specified 32-bit integer.
        /// </summary>
        /// <param name="num">The 32-bit integer to create a new syncrosation-safe integer from.</param>
        /// <returns>The equivalent <see cref="SYNCHSAFEINT"/> of <paramref name="num"/>.</returns>
        public static SYNCHSAFEINT FromInt32(System.Int32 num)
        {
            SYNCHSAFEINT ret = new();
            System.Byte[] data = Encode(num).GetBytes();
            if (System.BitConverter.IsLittleEndian) { data.Reverse(); }
            ret.b0 = data[0];
            ret.b1 = data[1];
            ret.b2 = data[2];
            ret.b3 = data[3];
            return ret;
        }
    }
}