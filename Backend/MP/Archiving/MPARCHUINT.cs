
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MP.Archiving
{
    /// <summary>
    /// Defines a six-byte unsigned integer for use in Music Player archives. <br />
    /// The value is stored in the little-endian format. <br />
    /// For arithmetic ease , you can convert this value to/and from a <see cref="System.Int64"/>.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Pack = 1)]
    public readonly struct MPARCHUINT
    {
        /// <summary>
        /// Defines the maximum value that an <see cref="MPARCHUINT"/> value can get.
        /// </summary>
        public const System.Int64 MaxValue = 281474976710655;

        /// <summary>
        /// Defines the minimum value that an <see cref="MPARCHUINT"/> value can get.
        /// </summary>
        public const System.Int64 MinValue = 0;

        [FieldOffset(0)]
        public readonly System.Byte UI0;

        [FieldOffset(1)]
        public readonly System.Byte UI1;

        [FieldOffset(2)]
        public readonly System.Byte UI2;

        [FieldOffset(3)]
        public readonly System.Byte UI3;

        [FieldOffset(4)]
        public readonly System.Byte UI4;

        [FieldOffset(5)]
        public readonly System.Byte UI5;

        public MPARCHUINT()
        {
            UI0 = 0;
            UI1 = 0;
            UI2 = 0;
            UI3 = 0;
            UI4 = 0;
            UI5 = 0;
        }

        public static MPARCHUINT Zero => new();

        public static MPARCHUINT ToUnsignedSixByteInteger(System.Int64 integertoconvert)
        {
            if (integertoconvert < MinValue)
            {
                throw new OverflowException("Value is too small to be saved to an MPARCHUINT.");
            }
            if (integertoconvert > MaxValue)
            {
                throw new OverflowException("Value is too large to be saved to an MPARCHUINT.");
            }
            System.Byte[] data = integertoconvert.GetBytes();
            if (System.BitConverter.IsLittleEndian == false) { data.Reverse(); }
            return data.ReadStructure<MPARCHUINT>(0);
        }

        public readonly System.Int64 ToInt64()
        {
            System.Byte[] data = new System.Byte[Unsafe.SizeOf<System.Int64>()];
            data.WriteStructure(0, this);
            if (System.BitConverter.IsLittleEndian == false) { data.Reverse(); }
            return data.ToInt64(0);
        }
    }
}