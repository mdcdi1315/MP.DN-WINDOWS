using System;
using MP.Utilities;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MP.NativeInterop.Windows
{
    /// <summary>
    /// GUID native marshalling type for Windows. <br />
    /// Provides also methods to convert from , and to , a <see cref="Guid"/> structure.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 16, Pack = 4)]
    public unsafe struct GUID : INullable
    {
        /// <summary></summary>
        [FieldOffset(0)]
        public uint Data1;
        /// <summary></summary>
        [FieldOffset(4)]
        public ushort Data2;
        /// <summary></summary>
        [FieldOffset(6)]
        public ushort Data3;
        /// <summary></summary>
        [FieldOffset(8)]
        public byte Data4_0;
        /// <summary></summary>
        [FieldOffset(9)]
        public byte Data4_1;
        /// <summary></summary>
        [FieldOffset(10)]
        public byte Data4_2;
        /// <summary></summary>
        [FieldOffset(11)]
        public byte Data4_3;
        /// <summary></summary>
        [FieldOffset(12)]
        public byte Data4_4;
        /// <summary></summary>
        [FieldOffset(13)]
        public byte Data4_5;
        /// <summary></summary>
        [FieldOffset(14)]
        public byte Data4_6;
        /// <summary></summary>
        [FieldOffset(15)]
        public byte Data4_7;

        /// <summary>
        /// Default constructor. <br />
        /// Someone could also say that the exact same result can be generated with the <see langword="default"/> keyword.
        /// </summary>
        public GUID() { }

        /// <summary>
        /// Gets back a <see cref="Guid"/> instance from this <see cref="GUID"/> instance.
        /// </summary>
        /// <returns>The original <see cref="Guid"/> instance.</returns>
        public readonly Guid GetGuid()
        {
            Guid ret = Guid.Empty;
            Unsafe.CopyBlockUnaligned(
                ref Unsafe.As<Guid, byte>(ref ret), 
                ref Unsafe.As<GUID, byte>(ref Unsafe.AsRef(in this)), 
                16U);
            return ret;
        }

        /// <summary>
        /// Gets a value whether this <see cref="GUID"/> is the <see cref="Empty"/> instance.
        /// </summary>
        public readonly bool IsNull 
            => Data1 == 0 && Data2 == 0 && Data3 == 0 && Data4_0 == 0 && Data4_1 == 0 && Data4_2 == 0 && Data4_3 == 0 && Data4_4 == 0 && Data4_5 == 0 && Data4_6 == 0 && Data4_7 == 0;

        /// <summary>
        /// Returns the fully constructed GUID.
        /// </summary>
        public override readonly string ToString() => GetGuid().ToString();

        /// <summary>
        /// Translates a <see cref="Guid"/> to a new COM <see cref="GUID"/> instance.
        /// </summary>
        /// <param name="guid">The globally unique identifier to translate.</param>
        /// <returns>The translated <see cref="GUID"/>.</returns>
        public static GUID FromGUID(Guid guid)
        {
            GUID result = new();
            Unsafe.CopyBlockUnaligned(
                ref Unsafe.As<GUID , byte>(ref result), 
                ref Unsafe.As<Guid , byte>(ref guid), 
                16U);
            return result;
        }

        /// <summary>
        /// Gets a <see cref="GUID"/> instance that is guaranteed to be filled with zeroes.
        /// </summary>
        public static GUID Empty
        {
            get {
                GUID ret = new();
                // Although .NET assures us that any structure fields will always be zero , 
                // ensure that the returned structure will be empty memory , just in case.
                Unsafe.InitBlockUnaligned(ref Unsafe.As<GUID, byte>(ref ret), 0, 16U);
                return ret;
            }
        }

        /// <summary>
        /// Retrieves a new COM <see cref="GUID"/> instance from the specified string.
        /// </summary>
        /// <param name="str">The string to translate.</param>
        /// <returns>The translated <see cref="GUID"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was <see langword="null"/>.</exception>
        /// <exception cref="FormatException"><paramref name="str"/> was not a valid guid.</exception>
        public static GUID FromString(string str) => FromGUID(new(str));
    }
}
