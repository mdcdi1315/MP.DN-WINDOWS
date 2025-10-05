


using System.Runtime.CompilerServices;

namespace MP
{
    public static unsafe partial class UnsafeMethods
    {
        /// <summary>
        /// Creates a new <see cref="System.String"/> from a null-terminated UTF-8 memory pointer.
        /// </summary>
        /// <param name="p">The memory pointer to read.</param>
        /// <returns>The decoded string.</returns>
        public static System.String CreateUtf8NullTerminated(System.Byte* p)
        {
            if (p is null) { return null; }
            int length = 0;
            byte* pcopy = p;
            while (*pcopy != 0)
            {
                length++;
                pcopy++;
            }
            return System.Text.Encoding.UTF8.GetString(p, length);
        }

        /// <summary>
        /// Creates a new <see cref="System.String"/> from a null-terminated ASCII memory pointer.
        /// </summary>
        /// <param name="p">The memory pointer to read.</param>
        /// <returns>The decoded string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.String CreateASCIINullTerminated(System.Byte* p) => p is null ? null : new((System.SByte*)p);

        /// <summary>
        /// Creates a new <see cref="System.String"/> from a null-terminated UTF-16 memory pointer.
        /// </summary>
        /// <param name="p">The memory pointer to read.</param>
        /// <returns>The decoded string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.String CreateUtf16NullTerminated(System.Char* p) => p is null ? null : new(p);

        /// <summary>Copies the specified UTF-8 string to another location.</summary>
        /// <param name="psrc">The source location to copy UTF-8 characters from</param>
        /// <param name="pdest">The destination location to write the copied UTF-8 characters to</param>
        /// <param name="bytecount">The number of bytes to copy from <paramref name="psrc"/> to <paramref name="pdest"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyUtf8String(System.Byte* psrc , System.Byte* pdest , System.UInt32 bytecount) => Unsafe.CopyBlockUnaligned(psrc, pdest, bytecount);

        /// <summary>Copies the specified UTF-8 string to another location.</summary>
        /// <param name="psrc">The source location to copy UTF-8 characters from</param>
        /// <param name="pdest">The destination location to write the copied UTF-8 characters to</param>
        /// <param name="charcount">The number of UTF-8 characters to copy from <paramref name="psrc"/> to <paramref name="pdest"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyUtf8StringC(System.Byte* psrc , System.Byte* pdest , System.UInt32 charcount) => CopyUtf8String(psrc , pdest , charcount);

        /// <summary>Copies the specified UTF-16 string to another location.</summary>
        /// <param name="psrc">The source location to copy UTF-16 characters from</param>
        /// <param name="pdest">The destination location to write the copied UTF-16 characters to</param>
        /// <param name="bytecount">The number of bytes to copy from <paramref name="psrc"/> to <paramref name="pdest"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyUtf16String(System.Char* psrc , System.Char* pdest , System.UInt32 bytecount) => Unsafe.CopyBlockUnaligned(psrc, pdest, bytecount);

        /// <summary>Copies the specified UTF-16 string to another location.</summary>
        /// <param name="psrc">The source location to copy UTF-16 characters from</param>
        /// <param name="pdest">The destination location to write the copied UTF-16 characters to</param>
        /// <param name="charcount">The number of UTF-16 characters to copy from <paramref name="psrc"/> to <paramref name="pdest"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyUtf16StringC(System.Char* psrc, System.Char* pdest, System.UInt32 charcount) => Unsafe.CopyBlockUnaligned(psrc, pdest, charcount * sizeof(System.Char));

        /// <summary>
        /// Copies the specified managed string and places it into a UTF-16 pointer. <br />
        /// The method does also append the NULL terminator at the end of the string.
        /// </summary>
        /// <param name="str">The managed string to read characters from</param>
        /// <param name="p">The memory location to copy the specified managed string to.</param>
        /// <param name="psize">The size in bytes of the memory location <paramref name="p"/>.</param>
        /// <param name="bytecount">The number of bytes copied to <paramref name="p"/>, or required so that copy to <paramref name="p"/> can succeed.</param>
        /// <returns>A value whether copy succeeded or not. <see langword="false"/> means that a pointer with larger size is required.</returns>
        public static System.Boolean CopyUtf16StringAddNullTermination(System.String str , System.Char* p , System.UInt32 psize , out System.UInt32 bytecount)
        {
            bytecount = (str.Length + 1).ToUInt32() * sizeof(System.Char);
            if (psize < bytecount) {
                return false;
            }
            fixed (System.Char* psrc = str)
            {
                Unsafe.CopyBlockUnaligned(p , psrc , bytecount);
            }
            return true;
        }

        /// <summary>
        /// Copies the specified managed string and places it into a UTF-16 pointer.
        /// </summary>
        /// <param name="str">The managed string to read characters from</param>
        /// <param name="p">The memory location to copy the specified managed string to.</param>
        /// <param name="psize">The size in bytes of the memory location <paramref name="p"/>.</param>
        /// <param name="bytecount">The number of bytes copied to <paramref name="p"/>, or required so that copy to <paramref name="p"/> can succeed.</param>
        /// <returns>A value whether copy succeeded or not. <see langword="false"/> means that a pointer with larger size is required.</returns>
        public static System.Boolean CopyUtf16String(System.String str, System.Char* p, System.UInt32 psize , out System.UInt32 bytecount)
        {
            bytecount = str.Length.ToUInt32() * sizeof(System.Char);
            if (psize < bytecount) {
                return false;
            }
            fixed (System.Char* psrc = str)
            {
                Unsafe.CopyBlockUnaligned(p, psrc, bytecount);
            }
            return true;
        }

        /// <summary>
        /// Copies the specified managed string and places it into a UTF-16 pointer. <br />
        /// The method does also append the NULL terminator at the end of the string. <br />
        /// This method variant does instead accept UTF-16 character counts instead of byte counts.
        /// </summary>
        /// <param name="str">The managed string to read characters from</param>
        /// <param name="p">The memory location to copy the specified managed string to.</param>
        /// <param name="pcharsize">The size, in <see cref="System.Char"/> units, of the memory location <paramref name="p"/>.</param>
        /// <param name="charcount">The number of UTF-16 characters copied to <paramref name="p"/>, or required so that copy to <paramref name="p"/> can succeed.</param>
        /// <returns>A value whether copy succeeded or not. <see langword="false"/> means that a pointer with larger size is required.</returns>
        public static System.Boolean CopyUtf16StringCAddNullTermination(System.String str, System.Char* p, System.UInt32 pcharsize , out System.UInt32 charcount)
        {
            charcount = (str.Length + 1).ToUInt32();
            if (pcharsize < charcount) {
                return false;
            }
            fixed (System.Char* psrc = str) {
                Unsafe.CopyBlockUnaligned(p, psrc, charcount * sizeof(System.Char));
            }
            return true;
        }

        /// <summary>
        /// Copies the specified managed string and places it into a UTF-16 pointer. <br />
        /// This method variant does instead accept UTF-16 character counts instead of byte counts.
        /// </summary>
        /// <param name="str">The managed string to read characters from</param>
        /// <param name="p">The memory location to copy the specified managed string to.</param>
        /// <param name="pcharsize">The size, in <see cref="System.Char"/> units, of the memory location <paramref name="p"/>.</param>
        /// <param name="charcount">The number of UTF-16 characters copied to <paramref name="p"/>, or required so that copy to <paramref name="p"/> can succeed.</param>
        /// <returns>A value whether copy succeeded or not. <see langword="false"/> means that a pointer with larger size is required.</returns>
        public static System.Boolean CopyUtf16StringC(System.String str, System.Char* p, System.UInt32 pcharsize , out System.UInt32 charcount)
        {
            charcount = str.Length.ToUInt32();
            if (pcharsize < charcount) {
                return false;
            }
            fixed (System.Char* psrc = str) {
                Unsafe.CopyBlockUnaligned(p, psrc, charcount * sizeof(System.Char));
            }
            return true;
        }

        /// <summary>
        /// Copies the specified managed string and places it into a UTF-8 pointer.
        /// </summary>
        /// <param name="str">The managed string to read characters from</param>
        /// <param name="pdest">The memory location to copy the specified managed string to.</param>
        /// <param name="pbytesize">The size in bytes of the memory location <paramref name="pdest"/>.</param>
        /// <param name="bytecount">The number of bytes copied to <paramref name="pdest"/>, or required so that copy to <paramref name="pdest"/> can succeed.</param>
        /// <returns>A value whether copy succeeded or not. <see langword="false"/> means that a pointer with larger size is required.</returns>
        public static System.Boolean CopyUtf8String(System.String str , System.Byte* pdest , System.UInt32 pbytesize , out System.UInt32 bytecount)
        {
            System.Int32 charcount = str.Length;
            System.Int32 bc = charcount;
            if (pbytesize < bc) {
                bytecount = bc.ToUInt32();
                return false;
            }
            fixed (System.Char* p = str) {
                bytecount = System.Text.Encoding.UTF8.GetBytes(p, charcount, pdest, bc).ToUInt32();
            }
            return true;
        }

        /// <summary>
        /// Copies the specified managed string and places it into a UTF-8 pointer. <br />
        /// The method does also append the NULL terminator at the end of the string.
        /// </summary>
        /// <param name="str">The managed string to read characters from</param>
        /// <param name="pdest">The memory location to copy the specified managed string to.</param>
        /// <param name="pbytesize">The size in bytes of the memory location <paramref name="pdest"/>.</param>
        /// <param name="bytecount">The number of bytes copied to <paramref name="pdest"/>, or required so that copy to <paramref name="pdest"/> can succeed.</param>
        /// <returns>A value whether copy succeeded or not. <see langword="false"/> means that a pointer with larger size is required.</returns>
        public static System.Boolean CopyUtf8StringAddNullTermination(System.String str, System.Byte* pdest, System.UInt32 pbytesize, out System.UInt32 bytecount)
        {
            System.Int32 charcount = str.Length + 1;
            System.Int32 bc = charcount;
            if (pbytesize < bc) {
                bytecount = bc.ToUInt32();
                return false;
            }
            fixed (System.Char* p = str) {
                bytecount = System.Text.Encoding.UTF8.GetBytes(p, charcount, pdest, bc).ToUInt32();
            }
            return true;
        }
    }
}
