
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MP
{
    partial class UnsafeMethods
    {
        /// <summary>
        /// Gets a value whether the CPU running this code is little-endian.
        /// </summary>
        public static readonly bool IsLittleEndian = DetermineEndianess();

        [StructLayout(LayoutKind.Explicit, Size = 4)]
        private struct UnpackedInt32
        {
            [FieldOffset(0)]
            public byte B0;

            [FieldOffset(1)]
            public byte B1;

            [FieldOffset(2)]
            public byte B2;

            [FieldOffset(3)]
            public byte B3;
        }

        private static bool DetermineEndianess() {
            System.Int32 i = 1;
            return Unsafe.As<System.Int32, UnpackedInt32>(ref i).B0 == 1;
        }
    }
}