using System;
using System.Runtime.CompilerServices;

namespace MP.IO.DataStructuring.Generation
{
    /// <summary>
    /// Class containing helpers for the MP Data Generator. <br />
    /// Not meant to be used by your code.
    /// </summary>
    public static unsafe class DataGeneratorHelpers
    {
        // Copied from the StreamMethods class, see there for the original declaration
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReverseByteReference(ref byte elements, int length)
        {
            if (length < 2) { return; }

            ref byte first = ref elements;
            ref byte last = ref Unsafe.Add(ref first, length - 1);
            do {
                byte temp = first;
                first = last;
                last = temp;
                first = ref Unsafe.Add(ref first, 1);
                last = ref Unsafe.Subtract(ref last, 1);
            } while (Unsafe.IsAddressLessThan(ref first, ref last));
        }

        /// <summary>
        /// Reads an enumeration value of type <typeparamref name="TE"/>.
        /// </summary>
        /// <typeparam name="TE">The type of the enumeration to read.</typeparam>
        /// <param name="stream">The data stream to read from.</param>
        /// <returns>The read enumeration value of type <typeparamref name="TE"/>.</returns>
        /// <exception cref="IOException">Could not read the specified number of bytes from the data stream.</exception>
        public static TE ReadEnumLE<TE>(IDataStreamAccess stream)
            where TE : unmanaged, Enum
        {
            int en_size = sizeof(TE);
            // Typically, enumerations won't be larger than 8 bytes - so we can perform a costless stack allocation instead.
            Span<System.Byte> b = stackalloc System.Byte[en_size];
            int read = stream.Read(b);
            if (read < en_size) {
                throw new IOException($"Could not read {en_size} bytes from the data stream.");
            } else {
                if (!UnsafeMethods.IsLittleEndian) { ReverseByteReference(ref b[0], en_size); }
                return Unsafe.ReadUnaligned<TE>(ref b[0]);
            }
        }

        /// <summary>
        /// Reads an enumeration value of type <typeparamref name="TE"/>.
        /// </summary>
        /// <typeparam name="TE">The type of the enumeration to read.</typeparam>
        /// <param name="stream">The data stream to read from.</param>
        /// <returns>The read enumeration value of type <typeparamref name="TE"/>.</returns>
        /// <exception cref="IOException">Could not read the specified number of bytes from the data stream.</exception>
        public static TE ReadEnumBE<TE>(IDataStreamAccess stream)
            where TE : unmanaged, Enum
        {
            int en_size = sizeof(TE);
            // Typically, enumerations won't be larger than 8 bytes - so we can perform a costless stack allocation instead.
            Span<System.Byte> b = stackalloc System.Byte[en_size];
            int read = stream.Read(b);
            if (read < en_size) {
                throw new IOException($"Could not read {en_size} bytes from the data stream.");
            } else {
                if (UnsafeMethods.IsLittleEndian) { ReverseByteReference(ref b[0], en_size); }
                return Unsafe.ReadUnaligned<TE>(ref b[0]);
            }
        }
        
        /// <summary>
        /// Writes to a <see cref="IDataStreamAccess"/> object an enumeration value of type <typeparamref name="TE"/>.
        /// </summary>
        /// <typeparam name="TE">The enumeration type to be stored to the stream.</typeparam>
        /// <param name="stream">The <see cref="IDataStreamAccess"/> object to store the value to.</param>
        /// <param name="value">The enumeration value to store.</param>
        public static void WriteEnumLE<TE>(IDataStreamAccess stream, TE value)
            where TE : unmanaged, Enum
        {
            int en_size = sizeof(TE);
            // Typically, enumerations won't be larger than 8 bytes - so we can perform a costless stack allocation instead.
            Span<System.Byte> b = stackalloc System.Byte[en_size];
            Unsafe.As<System.Byte, TE>(ref b[0]) = value;
            if (!UnsafeMethods.IsLittleEndian) { ReverseByteReference(ref b[0], en_size); }
            stream.Write(b);
        }

        /// <summary>
        /// Writes to a <see cref="IDataStreamAccess"/> object an enumeration value of type <typeparamref name="TE"/>.
        /// </summary>
        /// <typeparam name="TE">The enumeration type to be stored to the stream.</typeparam>
        /// <param name="stream">The <see cref="IDataStreamAccess"/> object to store the value to.</param>
        /// <param name="value">The enumeration value to store.</param>
        public static void WriteEnumBE<TE>(IDataStreamAccess stream, TE value)
            where TE : unmanaged, Enum
        {
            int en_size = sizeof(TE);
            // Typically, enumerations won't be larger than 8 bytes - so we can perform a costless stack allocation instead.
            Span<System.Byte> b = stackalloc System.Byte[en_size];
            Unsafe.As<System.Byte, TE>(ref b[0]) = value;
            if (UnsafeMethods.IsLittleEndian) { ReverseByteReference(ref b[0], en_size); }
            stream.Write(b);
        }

        /// <summary>
        /// Writes a fixed-length string to <paramref name="stream"/>. <br />
        /// If the given string is less than the required fixed string length, this method appends zeroes to the end of the string.
        /// </summary>
        /// <param name="stream">The <see cref="IDataStreamAccess"/> object to store the value to.</param>
        /// <param name="value">The string value to be stored as a fixed-length string.</param>
        /// <param name="enc">The <see cref="System.Text.Encoding"/> object to use for encoding the string to bytes.</param>
        /// <param name="req_length">The fixed length of the string as passed to the field's <see cref="FixedStringAttribute"/>.</param>
        public static void WriteFixedLengthString(IDataStreamAccess stream, string value, System.Text.Encoding enc, int req_length)
        {
            if (req_length > value.Length) {
                throw new ArgumentException("Fixed string length was larger than the allowed fixed string length!");
            } else {
                System.Text.StringBuilder s = new(value);
                s.Append('\0', value.Length % req_length);
                stream.WriteString(s.ToString(), enc);
            }
        }

        /// <summary>Prepares for writing a given string.</summary>
        /// <param name="string_">The string to be prepared for writing.</param>
        /// <param name="encoding">The encoding under which the string will be written as.</param>
        /// <param name="string_data">The string data as a data stream object that will be later unreferenced.</param>
        /// <returns>Number of bytes written to <paramref name="string_data"/>.</returns>
        public static long PrepareWriteString_VariableLength_Long(String string_, System.Text.Encoding encoding, out IDataStreamAccess string_data)
        {
            MemoryStream mem = new();
            long dr = mem.WriteString(string_, encoding);
            mem.Position = 0L;
            string_data = mem;
            return dr;
        }

        /// <summary>Prepares for writing a given string.</summary>
        /// <param name="string_">The string to be prepared for writing.</param>
        /// <param name="encoding">The encoding under which the string will be written as.</param>
        /// <param name="string_data">The string data as a data stream object that will be later unreferenced.</param>
        /// <returns>Number of bytes written to <paramref name="string_data"/>.</returns>
        public static ulong PrepareWriteString_VariableLength_ULong(String string_, System.Text.Encoding encoding, out IDataStreamAccess string_data) => PrepareWriteString_VariableLength_Long(string_, encoding, out string_data).ToUInt64();

        /// <summary>Prepares for writing a given string.</summary>
        /// <param name="string_">The string to be prepared for writing.</param>
        /// <param name="encoding">The encoding under which the string will be written as.</param>
        /// <param name="string_data">The string data as a data stream object that will be later unreferenced.</param>
        /// <returns>Number of bytes written to <paramref name="string_data"/>.</returns>
        public static uint PrepareWriteString_VariableLength_UInt(String string_, System.Text.Encoding encoding, out IDataStreamAccess string_data)
        {
            long d = PrepareWriteString_VariableLength_Long(string_, encoding, out string_data);
            if (d > System.UInt32.MaxValue) {
                throw new OverflowException($"String cannot be stored; it is larger than {System.UInt32.MaxValue}.");
            } else {
                return d.ToUInt32();
            }
        }

        /// <summary>Prepares for writing a given string.</summary>
        /// <param name="string_">The string to be prepared for writing.</param>
        /// <param name="encoding">The encoding under which the string will be written as.</param>
        /// <param name="string_data">The string data as a data stream object that will be later unreferenced.</param>
        /// <returns>Number of bytes written to <paramref name="string_data"/>.</returns>
        public static int PrepareWriteString_VariableLength_Int(String string_, System.Text.Encoding encoding, out IDataStreamAccess string_data)
        {
            long d = PrepareWriteString_VariableLength_Long(string_, encoding, out string_data);
            if (d > System.Int32.MaxValue) {
                throw new OverflowException($"String cannot be stored; it is larger than {System.Int32.MaxValue}.");
            } else {
                return d.ToInt32();
            }
        }

        /// <summary>Prepares for writing a given string.</summary>
        /// <param name="string_">The string to be prepared for writing.</param>
        /// <param name="encoding">The encoding under which the string will be written as.</param>
        /// <param name="string_data">The string data as a data stream object that will be later unreferenced.</param>
        /// <returns>Number of bytes written to <paramref name="string_data"/>.</returns>
        public static ushort PrepareWriteString_VariableLength_UShort(String string_, System.Text.Encoding encoding, out IDataStreamAccess string_data)
        {
            long d = PrepareWriteString_VariableLength_Long(string_, encoding, out string_data);
            if (d > System.UInt16.MaxValue) {
                throw new OverflowException($"String cannot be stored; it is larger than {System.UInt16.MaxValue}.");
            } else {
                return d.ToUInt16();
            }
        }

        /// <summary>Prepares for writing a given string.</summary>
        /// <param name="string_">The string to be prepared for writing.</param>
        /// <param name="encoding">The encoding under which the string will be written as.</param>
        /// <param name="string_data">The string data as a data stream object that will be later unreferenced.</param>
        /// <returns>Number of bytes written to <paramref name="string_data"/>.</returns>
        public static short PrepareWriteString_VariableLength_Short(String string_, System.Text.Encoding encoding, out IDataStreamAccess string_data)
        {
            long d = PrepareWriteString_VariableLength_Long(string_, encoding, out string_data);
            if (d > System.Int16.MaxValue) {
                throw new OverflowException($"String cannot be stored; it is larger than {System.Int16.MaxValue}.");
            } else {
                return d.ToInt16();
            }
        }

        /// <summary>Prepares for writing a given string.</summary>
        /// <param name="string_">The string to be prepared for writing.</param>
        /// <param name="encoding">The encoding under which the string will be written as.</param>
        /// <param name="string_data">The string data as a data stream object that will be later unreferenced.</param>
        /// <returns>Number of bytes written to <paramref name="string_data"/>.</returns>
        public static byte PrepareWriteString_VariableLength_Byte(String string_, System.Text.Encoding encoding, out IDataStreamAccess string_data)
        {
            long d = PrepareWriteString_VariableLength_Long(string_, encoding, out string_data);
            if (d > System.Byte.MaxValue) {
                throw new OverflowException($"String cannot be stored; it is larger than {System.Byte.MaxValue}.");
            } else {
                return d.ToByte();
            }
        }
    }
}