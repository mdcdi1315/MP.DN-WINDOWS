
using System;
using MP.Annotations.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP
{
    unsafe partial class StreamMethods
    {
        /// <summary>
        /// Writes a signed short integer to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the signed integer.</param>
        /// <param name="value">The signed short integer to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteInt16(this MP.IO.IDataStreamAccess stream, System.Int16 value)
        {
            System.Byte[] dt = value.GetBytes();
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref dt[0], sizeof(System.Int16));
            }
            stream.Write(dt, 0, dt.Length);
        }

        /// <summary>
        /// Writes an unsigned short integer to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the unsigned integer.</param>
        /// <param name="value">The unsigned short integer to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteUInt16(this MP.IO.IDataStreamAccess stream, System.UInt16 value)
        {
            System.Byte[] dt = value.GetBytes();
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref dt[0], sizeof(System.UInt16));
            }
            stream.Write(dt, 0, dt.Length);
        }

        /// <summary>
        /// Writes an unsigned integer encoded as big-endian to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the unsigned integer.</param>
        /// <param name="value">The unsigned integer to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteUInt32(this MP.IO.IDataStreamAccess stream, System.UInt32 value)
        {
            System.Byte[] dt = value.GetBytes();
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref dt[0], sizeof(System.UInt32));
            }
            stream.Write(dt, 0, dt.Length);
        }

        /// <summary>
        /// Writes a signed integer to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the signed integer.</param>
        /// <param name="value">The signed integer to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteInt32(this MP.IO.IDataStreamAccess stream, System.Int32 value)
        {
            System.Byte[] dt = value.GetBytes();
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref dt[0], sizeof(System.Int32));
            }
            stream.Write(dt, 0, dt.Length);
            dt = null;
        }

        /// <summary>
        /// Writes a signed long integer to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the signed integer.</param>
        /// <param name="value">The signed integer to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteInt64(this MP.IO.IDataStreamAccess stream, System.Int64 value)
        {
            System.Byte[] dt = value.GetBytes();
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref dt[0], sizeof(System.Int64));
            }
            stream.Write(dt, 0, dt.Length);
        }

        /// <summary>
        /// Writes an unsigned long integer to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the unsigned integer.</param>
        /// <param name="value">The unsigned long integer to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteUInt64(this MP.IO.IDataStreamAccess stream, System.UInt64 value)
        {
            System.Byte[] dt = value.GetBytes();
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref dt[0], sizeof(System.UInt64));
            }
            stream.Write(dt, 0, dt.Length);
        }

        /// <summary>
        /// Writes a double-precision floating-point value to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the number to.</param>
        /// <param name="value">The double-precision floating-point value to write.</param>
        public static void WriteDouble(this MP.IO.IDataStreamAccess stream, System.Double value)
        {
            System.Byte[] dt = value.GetBytes();
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref dt[0], dt.Length);
            }
            stream.Write(dt, 0, dt.Length);
        }

        /// <summary>
        /// Writes a single-precision floating-point value to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the number to.</param>
        /// <param name="value">The single-precision floating-point value to write.</param>
        public static void WriteSingle(this MP.IO.IDataStreamAccess stream, System.Single value)
        {
            System.Byte[] dt = value.GetBytes();
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref dt[0], dt.Length);
            }
            stream.Write(dt, 0, dt.Length);
        }

        /// <summary>
        /// Reads a signed short integer from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read signed short integer.</returns>
        public static System.Int16 ReadInt16(this MP.IO.IDataStreamAccess stream)
        {
            int size = sizeof(System.Int16);
            System.Byte[] data = ReadBytes(stream, size);
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref data[0], size);
            }
            return Unsafe.ReadUnaligned<System.Int16>(ref data[0]);
        }

        /// <summary>
        /// Reads a signed integer from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read signed integer.</returns>
        public static System.Int32 ReadInt32(this MP.IO.IDataStreamAccess stream)
        {
            int size = sizeof(System.Int32);
            System.Byte[] data = ReadBytes(stream, size);
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref data[0], size);
            }
            return Unsafe.ReadUnaligned<System.Int32>(ref data[0]);
        }

        /// <summary>
        /// Reads a signed long integer from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read signed long integer.</returns>
        public static System.Int64 ReadInt64(this MP.IO.IDataStreamAccess stream)
        {
            int size = sizeof(System.Int64);
            System.Byte[] data = ReadBytes(stream, size);
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref data[0], size);
            }
            return Unsafe.ReadUnaligned<System.Int64>(ref data[0]);
        }

        /// <summary>
        /// Reads an unsigned short integer from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read unsigned short integer.</returns>
        public static System.UInt16 ReadUInt16(this MP.IO.IDataStreamAccess stream)
        {
            int size = sizeof(System.UInt16);
            System.Byte[] data = ReadBytes(stream, size);
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref data[0], size);
            }
            return Unsafe.ReadUnaligned<System.UInt16>(ref data[0]);
        }

        /// <summary>
        /// Reads an unsigned integer from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read unsigned integer.</returns>
        public static System.UInt32 ReadUInt32(this MP.IO.IDataStreamAccess stream)
        {
            int size = sizeof(System.UInt32);
            System.Byte[] data = ReadBytes(stream, size);
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref data[0], size);
            }
            return Unsafe.ReadUnaligned<System.UInt32>(ref data[0]);
        }

        /// <summary>
        /// Reads an unsigned integer from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read unsigned integer.</returns>
        public static System.UInt64 ReadUInt64(this MP.IO.IDataStreamAccess stream)
        {
            int size = sizeof(System.UInt64);
            System.Byte[] data = ReadBytes(stream, size);
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref data[0], size);
            }
            return Unsafe.ReadUnaligned<System.UInt64>(ref data[0]);
        }

        /// <summary>
        /// Reads a double-precision floating-point value from the stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>The read double-precision floating-point value.</returns>
        public static System.Double ReadDouble(this MP.IO.IDataStreamAccess stream)
        {
            int size = sizeof(System.Double);
            System.Byte[] data = ReadBytes(stream, size);
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref data[0], size);
            }
            return data.ToDouble(0);
        }

        /// <summary>
        /// Reads a single-precision floating-point value from the stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>The read single-precision floating-point value.</returns>
        public static System.Single ReadSingle(this MP.IO.IDataStreamAccess stream)
        {
            int size = sizeof(System.Single);
            System.Byte[] data = ReadBytes(stream, size);
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref data[0], size);
            }
            return data.ToSingle(0);
        }

        /// <summary>Writes a decimal to the stream.</summary>
        /// <param name="stream">The stream to write the decimal.</param>
        /// <param name="dec">The decimal to write.</param>
        public static void WriteDecimal(this MP.IO.IDataStreamAccess stream, System.Decimal dec)
        {
            System.Byte[] dt = dec.GetBytes();
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref dt[0], dt.Length);
            }
            stream.Write(dt, 0, dt.Length);
        }

        /// <summary>Reads a decimal from the stream.</summary>
        /// <param name="stream">The stream to read the decimal from.</param>
        /// <returns>The read decimal value.</returns>
        public static System.Decimal ReadDecimal(this MP.IO.IDataStreamAccess stream)
        {
            int size = sizeof(System.Decimal);
            System.Byte[] data = ReadBytes(stream, size);
            if (!UnsafeMethods.IsLittleEndian)
            {
                ReverseByteReference(ref data[0], size);
            }
            return Unsafe.ReadUnaligned<System.Decimal>(ref data[0]);
        }
    }
}