
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace MP.PlaylistManagement
{
    /// <summary>
    /// Holds a playlist metadata value, along with it's descriptor.
    /// </summary>
    public sealed class PlaylistMetadataValue
    {
        private System.Object value;
        private METADATADESC state;

        /// <summary>
        /// Constructs a new and empty playlist metadata value instance.
        /// </summary>
        public PlaylistMetadataValue() {
            value = null;
            state = 0;
        }

        /// <summary>
        /// Constructs a new playlist metadata value instance from the specified raw value.
        /// </summary>
        /// <param name="newvalue">The value to initialize the returned instance to.</param>
        public PlaylistMetadataValue(System.Object newvalue) {
            value = newvalue;
            state = 0;
        }

        /// <summary>
        /// Constructs a new playlist metadata value instance from the specified raw valuem as well as the properties to apply to the value.
        /// </summary>
        /// <param name="newvalue">The value to initialize the returned instance to.</param>
        /// <param name="statistic">Is this metadata value a statistic?</param>
        /// <param name="read_only">Is this metadata value read-only?</param>
        /// <param name="preference">Is this metadata value a preference?</param>
        public PlaylistMetadataValue(System.Object newvalue , System.Boolean statistic , System.Boolean read_only , System.Boolean preference) {
            value = newvalue;
            state = 0;
            if (statistic) {
                if (preference) { throw new ArgumentException("Both statistic and preference cannot be set to the metadata value, they are mutually exclusive."); }
                state |= METADATADESC.IS_STATISTIC;
            } 
            if (read_only) {
                state |= METADATADESC.IS_READONLY;
            }
            if (preference) {
                if (statistic) { throw new ArgumentException("Both statistic and preference cannot be set to the metadata value, they are mutually exclusive."); }
                state |= METADATADESC.IS_PREFERENCE;
            }
        }

        /// <summary>
        /// Writes the specified playlist metadata value to the specified stream.
        /// </summary>
        /// <param name="pmv">The playlist metadata value to write.</param>
        /// <param name="stream">The stream to write the serialized data to.</param>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unwriteable.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="pmv"/> and/or <paramref name="stream"/> were <see langword="null"/>.</exception>
        public static void WriteTo(PlaylistMetadataValue pmv , IO.DataStream stream)
        {
            ArgumentNullException.ThrowIfNull(pmv);
            ArgumentNullException.ThrowIfNull(stream);
            if (!stream.CanWrite) {
                throw new ArgumentException("Stream was unwriteable.");
            }
            WriteToInternal(pmv , stream);
        }

        private unsafe static void WriteToInternal(PlaylistMetadataValue pmv, IO.DataStream stream)
        {
            SERIALIZEDMETADATAVALUE header = new();
            header.Descriptor = pmv.state;
            System.Byte[] temp;
            switch (pmv.value)
            {
                case null:
                    header.LengthInBytes = 0;
                    header.Type = PlaylistMetadataValueTypeCode.Empty;
                    temp = Array.Empty<byte>();
                    break;
                case DBNull:
                    header.LengthInBytes = 0;
                    header.Type = PlaylistMetadataValueTypeCode.DBNull;
                    temp = Array.Empty<byte>();
                    break;
                case Byte b:
                    header.LengthInBytes = 1;
                    header.Type = PlaylistMetadataValueTypeCode.Byte;
                    temp = new[] { b };
                    break;
                case SByte sb:
                    header.LengthInBytes = 1;
                    header.Type = PlaylistMetadataValueTypeCode.SByte;
                    temp = new[] { sb.ToByte() };
                    break;
                case Int16 i16:
                    header.LengthInBytes = sizeof(Int16);
                    header.Type = PlaylistMetadataValueTypeCode.Int16;
                    temp = i16.GetBytes();
                    break;
                case UInt16 u16:
                    header.LengthInBytes = sizeof(UInt16);
                    header.Type = PlaylistMetadataValueTypeCode.UInt16;
                    temp = u16.GetBytes();
                    break;
                case Int32 i32:
                    header.LengthInBytes = sizeof(Int32);
                    header.Type = PlaylistMetadataValueTypeCode.Int32;
                    temp = i32.GetBytes();
                    break;
                case UInt32 u32:
                    header.LengthInBytes = sizeof(UInt32);
                    header.Type = PlaylistMetadataValueTypeCode.UInt32;
                    temp = u32.GetBytes();
                    break;
                case Int64 i64:
                    header.LengthInBytes = sizeof(Int64);
                    header.Type = PlaylistMetadataValueTypeCode.Int64;
                    temp = i64.GetBytes();
                    break;
                case UInt64 u64:
                    header.LengthInBytes = sizeof(UInt64);
                    header.Type = PlaylistMetadataValueTypeCode.UInt64;
                    temp = u64.GetBytes();
                    break;
                case Double d:
                    header.LengthInBytes = sizeof(Double);
                    header.Type = PlaylistMetadataValueTypeCode.Double;
                    temp = d.GetBytes();
                    break;
                case Single sp:
                    header.LengthInBytes = sizeof(Single);
                    header.Type = PlaylistMetadataValueTypeCode.Single;
                    temp = sp.GetBytes();
                    break;
                case Decimal dec:
                    header.LengthInBytes = sizeof(Decimal);
                    header.Type = PlaylistMetadataValueTypeCode.Decimal;
                    temp = dec.GetBytes();
                    break;
                case DateTime dt:
                    header.LengthInBytes = sizeof(Int64);
                    header.Type = PlaylistMetadataValueTypeCode.DateTime;
                    temp = dt.Ticks.GetBytes();
                    break;
                case TimeSpan ts:
                    header.LengthInBytes = sizeof(Int64);
                    header.Type = PlaylistMetadataValueTypeCode.TimeSpan;
                    temp = ts.Ticks.GetBytes();
                    break;
                case Boolean b:
                    header.LengthInBytes = 1;
                    header.Type = PlaylistMetadataValueTypeCode.Boolean;
                    temp = new[] { (b ? 1 : 0).ToByte() };
                    break;
                case Char c:
                    header.LengthInBytes = sizeof(Char);
                    header.Type = PlaylistMetadataValueTypeCode.Char;
                    temp = c.GetBytes();
                    break;
                case System.String s:
                    header.Type = PlaylistMetadataValueTypeCode.String;
                    header.LengthInBytes = (s.Length * sizeof(System.Char)).ToUInt32();
                    temp = new byte[header.LengthInBytes];
                    Unsafe.CopyBlockUnaligned(ref temp[0], ref Unsafe.As<System.Char, System.Byte>(ref Unsafe.AsRef(s.GetPinnableReference())), header.LengthInBytes);
                    break;
                default:
                    throw new ArgumentException($"Value of type {pmv.value.GetType().FullName ?? "null?"} cannot be serialized: {pmv.value}");
            }
            stream.WriteStructure(header);
            stream.Write(temp, 0, temp.Length);
        }

        /// <summary>
        /// Reads a metadata value from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to read the serialized data from.</param>
        /// <returns>A new instance containing the data of a previously serialized <see cref="PlaylistMetadataValue"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unreadable.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> were <see langword="null"/>.</exception>
        [return: NotNull]
        public static PlaylistMetadataValue ReadFrom(IO.DataStream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (!stream.CanRead) {
                throw new ArgumentException("Stream was unreadable.");
            }
            return ReadFromInternal(stream);
        }

        private unsafe static System.String DecodeString(System.Byte[] data)
        {
            fixed (System.Byte* p = data) {
                return new((System.Char*)p, 0, data.Length / sizeof(System.Char));
            }
        }

        private static PlaylistMetadataValue ReadFromInternal(IO.DataStream stream)
        {
            PlaylistMetadataValue pmv;
            SERIALIZEDMETADATAVALUE header = stream.ReadStructure<SERIALIZEDMETADATAVALUE>();
            if (header.Version > 1) {
                throw new ArgumentException($"Invalid version: {header.Version}");
            }
            pmv = new();
            pmv.state = header.Descriptor;
            System.Byte[] data = stream.ReadBytes(header.LengthInBytes);
            pmv.value = header.Type switch { 
                PlaylistMetadataValueTypeCode.Empty or PlaylistMetadataValueTypeCode.DBNull => null,
                PlaylistMetadataValueTypeCode.Byte => data[0],
                PlaylistMetadataValueTypeCode.SByte => data[0].ToSByte(),
                PlaylistMetadataValueTypeCode.Int16 => data.ToInt16(0),
                PlaylistMetadataValueTypeCode.UInt16 => data.ToUInt16(0),
                PlaylistMetadataValueTypeCode.Int32 => data.ToInt32(0),
                PlaylistMetadataValueTypeCode.UInt32 => data.ToUInt32(0),
                PlaylistMetadataValueTypeCode.Int64 => data.ToInt64(0),
                PlaylistMetadataValueTypeCode.UInt64 => data.ToUInt64(0),
                PlaylistMetadataValueTypeCode.Boolean => data[0] != 0,
                PlaylistMetadataValueTypeCode.Char => data.ToChar(0),
                PlaylistMetadataValueTypeCode.Single => data.ToSingle(0),
                PlaylistMetadataValueTypeCode.Double => data.ToDouble(0),
                PlaylistMetadataValueTypeCode.Decimal => data.ToDecimal(0),
                PlaylistMetadataValueTypeCode.DateTime => new DateTime(data.ToInt64(0)),
                PlaylistMetadataValueTypeCode.TimeSpan => new TimeSpan(data.ToInt64(0)),
                PlaylistMetadataValueTypeCode.String => DecodeString(data),
                _ => null
            };
            return pmv;
        }

        // Defines boolean values describing a playlist metadata value.
        [Flags]
        private enum METADATADESC : System.Byte
        {
            IS_STATISTIC = 1 << 0,
            IS_READONLY = 1 << 1,
            IS_PREFERENCE = 1 << 2,
        }

        [StructLayout(LayoutKind.Explicit , Pack = 1 , Size = 8)]
        private struct SERIALIZEDMETADATAVALUE
        {
            [FieldOffset(0)]
            public System.Byte Version;
            [FieldOffset(1)]
            public PlaylistMetadataValueTypeCode Type;
            [FieldOffset(2)]
            public METADATADESC Descriptor;
            [FieldOffset(3)]
            public System.Byte Reserved;
            [FieldOffset(4)]
            public System.UInt32 LengthInBytes;
            
            public SERIALIZEDMETADATAVALUE() {
                Version = 1;
                Reserved = 0;
            }
        }

        /// <summary>
        /// Gets or sets the actual value held by this metadata value.
        /// </summary>
        /// <exception cref="InvalidOperationException">Attempted to set the property and the metadata value is read-only.</exception>
        public System.Object Value
        {
            get => value;
            set {
                if ((state & METADATADESC.IS_READONLY) != 0) {
                    throw new InvalidOperationException("Cannot modify a read-only metadata value after the read-only flag was defined.");
                }
                this.value = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual value held by this metadata value, as an <see cref="System.Int64"/> value.
        /// </summary>
        /// <exception cref="InvalidOperationException">Attempted to set the property and the metadata value is read-only.</exception>
        public System.Int64 Int64Value
        {
            get => (value is System.Int64 i) ? i : 0;
            set => Value = value; // Perform validation through Value property
        }

        /// <summary>
        /// Gets or sets the actual value held by this metadata value, as an <see cref="System.UInt64"/> value.
        /// </summary>
        /// <exception cref="InvalidOperationException">Attempted to set the property and the metadata value is read-only.</exception>
        public System.UInt64 UInt64Value
        {
            get => (value is System.UInt64 u) ? u : 0;
            set => Value = value; // Perform validation through Value property
        }

        /// <summary>
        /// Gets or sets the actual value held by this metadata value, as an <see cref="System.Boolean"/> value.
        /// </summary>
        /// <exception cref="InvalidOperationException">Attempted to set the property and the metadata value is read-only.</exception>
        public System.Boolean BooleanValue
        {
            get => (value is System.Boolean b) && b;
            set => Value = value; // Perform validation through Value property
        }

        /// <summary>
        /// Gets or sets the actual value held by this metadata value, as an <see cref="System.String"/> value.
        /// </summary>
        /// <exception cref="InvalidOperationException">Attempted to set the property and the metadata value is read-only.</exception>
        public String StringValue
        {
            get => (value is String s) ? s : System.String.Empty;
            set => Value = value; // Perform validation through Value property
        }

        /// <summary>
        /// Gets or sets whether this metadata value represents a statistic.
        /// </summary>
        /// <exception cref="InvalidOperationException">Attempted to set the property and the metadata value is read-only.</exception>
        public System.Boolean IsStatistic
        {
            get => (state & METADATADESC.IS_STATISTIC) != 0;
            set {
                if ((state & METADATADESC.IS_READONLY) != 0) {
                    throw new InvalidOperationException("Cannot modify a read-only metadata value after the read-only flag was defined.");
                }
                _ = value ? (state |= METADATADESC.IS_STATISTIC) : (state &= ~METADATADESC.IS_STATISTIC);
            }
        }

        /// <summary>
        /// Gets or sets whether this metadata value is read-only.
        /// </summary>
        /// <exception cref="InvalidOperationException">Attempted to set the property and the metadata value is read-only.</exception>
        public System.Boolean IsReadOnly
        {
            get => (state & METADATADESC.IS_READONLY) != 0;
            set {
                if (value) {
                    state |= METADATADESC.IS_READONLY;
                } else if ((state & METADATADESC.IS_READONLY) != 0) {
                    throw new InvalidOperationException("Cannot modify a read-only metadata value after the read-only flag was defined.");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this metadata value represents a playlist preference.
        /// </summary>
        /// <exception cref="InvalidOperationException">Attempted to set the property and the metadata value is read-only.</exception>
        public System.Boolean IsPreference
        {
            get => (state & METADATADESC.IS_PREFERENCE) != 0;
            set {
                if ((state & METADATADESC.IS_READONLY) != 0) {
                    throw new InvalidOperationException("Cannot modify a read-only metadata value after the read-only flag was defined.");
                }
                _ = value ? (state |= METADATADESC.IS_PREFERENCE) : (state &= ~METADATADESC.IS_PREFERENCE);
            }
        }

        /// <summary>
        /// Returns the string representation of the <see cref="Value"/> property.
        /// </summary>
        /// <returns>The string representation of the <see cref="Value"/> property; If <see langword="null"/> it returns <see langword="null"/> however.</returns>
        public override string ToString() => value?.ToString();
    }
}