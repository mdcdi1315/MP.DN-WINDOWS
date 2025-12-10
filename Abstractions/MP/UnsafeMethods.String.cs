


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
    }
}
