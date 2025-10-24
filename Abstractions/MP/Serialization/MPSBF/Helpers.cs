
using System;

namespace MP.Serialization.MPSBF
{
    internal static class Helpers
    {
        public static ENTRY_TYPE GetStringEntryType(System.Text.Encoding encoding) => encoding.CodePage switch
        {
            65001 => ENTRY_TYPE.STRING_LE_UTF8,
            1200 => ENTRY_TYPE.STRING_LE,
            1201 => ENTRY_TYPE.STRING_BE,
            _ => throw new NotSupportedException($"Specified encoding is not supported: {encoding}"),
        };

        public static long EncodeStringValue(System.IO.Stream stream, String str, System.Text.Encoding encoding) => stream.WriteString(str, encoding);

        public static String DecodeStringValue(System.IO.Stream stream, ENTRY_TYPE type, UInt32 string_length_in_bytes)
        {
            System.Text.Encoding enc = type switch {
                ENTRY_TYPE.STRING_LE_UTF8 => System.Text.Encoding.UTF8,
                ENTRY_TYPE.STRING_LE => System.Text.Encoding.Unicode,
                ENTRY_TYPE.STRING_BE => System.Text.Encoding.BigEndianUnicode,
                _ => throw new NotSupportedException($"Entry type not supported: {type}"),
            };
            return stream.ReadString(enc, string_length_in_bytes);
        }

        public static long EncodeNumericValue(System.IO.Stream stream, System.Object obj, out ENTRY_TYPE type)
        {
            switch (obj)
            {
                case System.Byte b:
                    stream.WriteByte(b);
                    type = ENTRY_TYPE.UNSIGNED_CHAR;
                    return sizeof(System.Byte);
                case System.SByte sb:
                    stream.WriteSByte(sb);
                    type = ENTRY_TYPE.SIGNED_CHAR;
                    return sizeof(System.SByte);
                case System.Int16 i16:
                    stream.WriteInt16(i16);
                    type = ENTRY_TYPE.SHORT;
                    return sizeof(System.Int16);
                case System.UInt16 u16:
                    stream.WriteUInt16(u16);
                    type = ENTRY_TYPE.USHORT;
                    return sizeof(System.UInt16);
                case System.Int32 i32:
                    stream.WriteInt32(i32);
                    type = ENTRY_TYPE.INT;
                    return sizeof(System.Int32);
                case System.UInt32 u32:
                    stream.WriteUInt32(u32);
                    type = ENTRY_TYPE.UINT;
                    return sizeof(System.UInt32);
                case System.Int64 i64:
                    stream.WriteInt64(i64);
                    type = ENTRY_TYPE.LONG;
                    return sizeof(System.Int64);
                case System.UInt64 u64:
                    stream.WriteUInt64(u64);
                    type = ENTRY_TYPE.ULONG;
                    return sizeof(System.UInt64);
                case System.Single single:
                    stream.WriteSingle(single);
                    type = ENTRY_TYPE.FLOAT;
                    return sizeof(System.Single);
                case System.Double dbl:
                    stream.WriteDouble(dbl);
                    type = ENTRY_TYPE.DOUBLE;
                    return sizeof(System.Double);
                default:
                    throw new NotSupportedException();
            }
        }

        public static System.Object DecodeNumericValue(System.IO.Stream stream , ENTRY_TYPE type) => type switch
        {
            ENTRY_TYPE.UNSIGNED_CHAR => stream.ReadByte(),
            ENTRY_TYPE.SIGNED_CHAR => stream.ReadSByte(),
            ENTRY_TYPE.SHORT => stream.ReadInt16(),
            ENTRY_TYPE.USHORT => stream.ReadUInt16(),
            ENTRY_TYPE.INT => stream.ReadInt32(),
            ENTRY_TYPE.UINT => stream.ReadUInt32(),
            ENTRY_TYPE.LONG => stream.ReadInt64(),
            ENTRY_TYPE.ULONG => stream.ReadUInt64(),
            ENTRY_TYPE.FLOAT => stream.ReadSingle(),
            ENTRY_TYPE.DOUBLE => (object)stream.ReadDouble(),
            _ => throw new NotSupportedException(),
        };

        public static long EncodeBooleanValue(System.IO.Stream stream, System.Object obj, out ENTRY_TYPE type)
        {
            switch (obj)
            {
                case System.Boolean b:
                    stream.WriteBoolean(b);
                    type = ENTRY_TYPE.BOOLEAN;
                    return 1;
                case BOOL b:
                    stream.WriteInt32((int)b);
                    type = ENTRY_TYPE.BOOL;
                    return sizeof(int);
                default:
                    throw new NotSupportedException();
            }
        }

        public static System.Object DecodeBooleanValue(System.IO.Stream stream, ENTRY_TYPE type) => type switch {
            ENTRY_TYPE.BOOL => (BOOL)stream.ReadInt32(),
            ENTRY_TYPE.BOOLEAN => stream.ReadBoolean(),
            _ => throw new NotSupportedException(),
        };

        public static long EncodeStringArrayValue(System.IO.Stream stream, String[] values , System.Text.Encoding encoding)
        {
            long data_length = 0, temp_length;
            using (IO.MemoryStream ms = new(values.LongLength * encoding.GetMaxByteCount(1) * 20 , true))
            {
                ms.Position = 0;
                stream.WriteInt64(values.LongLength);
                data_length += sizeof(long);
                foreach (var value in values)
                {
                    stream.WriteInt64(temp_length = ms.WriteString(value, encoding));
                    ms.DirectCopyToStream(stream);
                    ms.SetLength(0); // Automatically the position/length are set to zero.
                    data_length += temp_length;
                }
            }
            return data_length;
        }

        public static System.String[] DecodeStringArrayValue(System.IO.Stream stream, ENTRY_TYPE type)
        {
            System.Text.Encoding enc = type switch {
                ENTRY_TYPE.STRING_LE_UTF8 => System.Text.Encoding.UTF8,
                ENTRY_TYPE.STRING_LE => System.Text.Encoding.Unicode,
                ENTRY_TYPE.STRING_BE => System.Text.Encoding.BigEndianUnicode,
                _ => throw new NotSupportedException($"Entry type not supported: {type}"),
            };
            long string_values = stream.ReadInt64();
            System.String[] strings = new System.String[string_values];

            for (long str_index = 0; str_index < string_values; str_index++)
            {
                strings[str_index] = stream.ReadString(enc, stream.ReadInt64());
            }

            return strings;
        }

        public static long EncodeNumericArrayValue(System.IO.Stream stream, System.Object obj, out ENTRY_TYPE type)
        {
            switch (obj)
            {
                case System.Byte[] b:
                    stream.WriteInt32(b.Length);
                    stream.WriteBytes(b);
                    type = ENTRY_TYPE.UNSIGNED_CHAR | ENTRY_TYPE.ARRAY;
                    return b.LongLength + sizeof(int);
                case System.SByte[] sb:
                    type = ENTRY_TYPE.SIGNED_CHAR | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.SByte>(sb) + sizeof(int);
                case System.Int16[] i16:
                    type = ENTRY_TYPE.SHORT | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.Int16>(i16) + sizeof(int);
                case System.UInt16[] u16:
                    type = ENTRY_TYPE.USHORT | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.UInt16>(u16) + sizeof(int);
                case System.Int32[] i32:
                    type = ENTRY_TYPE.INT | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.Int32>(i32) + sizeof(int);
                case System.UInt32[] u32:
                    type = ENTRY_TYPE.UINT | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.UInt32>(u32) + sizeof(int);
                case System.Int64[] i64:
                    type = ENTRY_TYPE.LONG | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.Int64>(i64) + sizeof(int);
                case System.UInt64[] u64:
                    type = ENTRY_TYPE.ULONG | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.UInt64>(u64) + sizeof(int);
                case System.Single[] s:
                    type = ENTRY_TYPE.FLOAT | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.Single>(s) + sizeof(int);
                case System.Double[] d:
                    type = ENTRY_TYPE.DOUBLE | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.Double>(d) + sizeof(int);
                default:
                    throw new NotSupportedException();
            }
        }

        public static System.Object DecodeNumericArrayValue(System.IO.Stream stream, ENTRY_TYPE type)
        {
            int length = stream.ReadInt32();
            return type switch {
                ENTRY_TYPE.UNSIGNED_CHAR => stream.ReadBytes(length),
                ENTRY_TYPE.SIGNED_CHAR => stream.ReadTypedArray<System.SByte>(length),
                ENTRY_TYPE.SHORT => stream.ReadTypedArray<System.Int16>(length),
                ENTRY_TYPE.USHORT => stream.ReadTypedArray<System.UInt16>(length),
                ENTRY_TYPE.INT => stream.ReadTypedArray<System.Int32>(length),
                ENTRY_TYPE.UINT => stream.ReadTypedArray<System.UInt32>(length),
                ENTRY_TYPE.LONG => stream.ReadTypedArray<System.Int64>(length),
                ENTRY_TYPE.ULONG => stream.ReadTypedArray<System.UInt64>(length),
                ENTRY_TYPE.FLOAT => stream.ReadTypedArray<System.Single>(length),
                ENTRY_TYPE.DOUBLE => stream.ReadTypedArray<System.Double>(length),
                _ => throw new NotSupportedException(),
            };
        }

        public static long EncodeBooleanArrayValue(System.IO.Stream stream, System.Object obj, out ENTRY_TYPE type)
        {
            switch (obj)
            {
                case BOOL[] bls:
                    type = ENTRY_TYPE.BOOL | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<BOOL>(bls) + sizeof(int);
                case Boolean[] bools:
                    type = ENTRY_TYPE.BOOLEAN | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<Boolean>(bools) + sizeof(int);
                default:
                    throw new NotSupportedException();
            }
        }

        public static System.Object DecodeBooleanArrayValue(System.IO.Stream stream, ENTRY_TYPE type)
        {
            int length = stream.ReadInt32();
            return type switch {
                ENTRY_TYPE.BOOL => stream.ReadTypedArray<BOOL>(length),
                ENTRY_TYPE.BOOLEAN => stream.ReadTypedArray<Boolean>(length),
                _ => throw new NotSupportedException(),
            };
        }

        public static unsafe long EncodeField(System.IO.Stream stream, SerializedField field)
        {
            int namelen = field.Name.Length;
            if (namelen > System.UInt16.MaxValue) {
                throw new NotSupportedException("Cannot encode the field because it's name is too large.");
            }
            long total_data_len = sizeof(SBF_ENTRY_HEADER);
            SBF_ENTRY_HEADER entry = new();
            entry.NameLengthInChars = namelen.ToUInt16();
            using (IO.MemoryStream ms = new())
            {
                long length;
                if (field.Value.GetType().IsArray) {
                    switch (field.Value)
                    {
                        case null:
                            entry.Type = ENTRY_TYPE.NULL;
                            length = 0;
                            break;
                        case System.String[] strs:
                            length = EncodeStringArrayValue(ms, strs, System.Text.Encoding.Unicode);
                            entry.Type = ENTRY_TYPE.STRING_LE | ENTRY_TYPE.ARRAY;
                            break;
                        case BOOL[]:
                        case Boolean[]:
                            length = EncodeBooleanArrayValue(ms, field.Value, out entry.Type);
                            break;
                        case Record[] records:
                            length = EncodeRecordArray(stream, records);
                            entry.Type = ENTRY_TYPE.RECORD | ENTRY_TYPE.ARRAY;
                            break;
                        case UninitializedRecord[] urecords:
                            length = EncodeRecordArray(stream, urecords);
                            entry.Type = ENTRY_TYPE.RECORD | ENTRY_TYPE.ARRAY;
                            break;
                        default:
                            length = EncodeNumericArrayValue(ms, field.Value, out entry.Type);
                            break;
                    }
                } else {
                    switch (field.Value)
                    {
                        case null:
                            entry.Type = ENTRY_TYPE.NULL;
                            length = 0;
                            break;
                        case System.String s:
                            length = EncodeStringValue(ms, s, System.Text.Encoding.Unicode);
                            entry.Type = ENTRY_TYPE.STRING_LE;
                            break;
                        case BOOL:
                        case Boolean:
                            length = EncodeBooleanValue(ms, field.Value, out entry.Type);
                            break;
                        case Record r:
                            length = EncodeRecord(stream, r);
                            entry.Type = ENTRY_TYPE.RECORD;
                            break;
                        case UninitializedRecord ur:
                            length = EncodeRecord(stream, ur.Value);
                            entry.Type = ENTRY_TYPE.RECORD;
                            break;
                        default:
                            length = EncodeNumericValue(ms, field.Value, out entry.Type);
                            break;
                    }
                }
                if (length > System.UInt32.MaxValue) {
                    throw new NotSupportedException($"Value is too large to be saved on the data stream: Was {length} bytes while up to {System.UInt32.MaxValue} bytes are supported.");
                }
                total_data_len += length;
                entry.ValueLengthInBytes = length.ToUInt32();
                stream.WriteStructure(entry);
                System.Byte[] t;
                foreach (System.Char c in field.Name)
                {
                    t = c.GetBytes();
                    stream.Write(t , 0 , t.Length);
                    total_data_len += t.LongLength;
                }
                stream.WriteByte(0); // Sync byte - verifies correct bitstream
                total_data_len++;
                ms.Position = 0;
                ms.DirectCopyToStream(stream);
            }
            return total_data_len;
        }
    
        public static unsafe SerializedField DecodeField(System.IO.Stream stream)
        {
            SBF_ENTRY_HEADER entry = stream.ReadStructure<SBF_ENTRY_HEADER>();
            System.String name;
            {
                System.Text.StringBuilder sb = new(entry.NameLengthInChars);
                for (ushort I = 0; I < entry.NameLengthInChars; I++)
                {
                    sb.Append(stream.ReadUInt16().ToChar());
                }
                name = sb.ToString();
            }
            // Verify sync byte
            if (stream.ReadLiteralByte() != 0) {
                throw new SerializationException("Cannot decode the data stream because it is invalid.");
            }
            System.Object obj;
            if (entry.Type.HasFlag(ENTRY_TYPE.ARRAY)) {
                ENTRY_TYPE t;
                switch (t = (entry.Type & ~ENTRY_TYPE.ARRAY))
                {
                    case ENTRY_TYPE.NULL:
                        obj = null;
                        break;
                    case ENTRY_TYPE.RECORD:
                        obj = DecodeRecords(stream);
                        break;
                    case ENTRY_TYPE.BOOL:
                    case ENTRY_TYPE.BOOLEAN:
                        obj = DecodeBooleanArrayValue(stream, t);
                        break;
                    case ENTRY_TYPE.STRING_LE:
                    case ENTRY_TYPE.STRING_BE:
                    case ENTRY_TYPE.STRING_LE_UTF8:
                        obj = DecodeStringArrayValue(stream, t);
                        break;
                    default:
                        obj = DecodeNumericArrayValue(stream, t);
                        break;
                }
            } else {
                switch (entry.Type)
                {
                    case ENTRY_TYPE.NULL:
                        obj = null;
                        break;
                    case ENTRY_TYPE.RECORD:
                        obj = DecodeRecord(stream);
                        break;
                    case ENTRY_TYPE.BOOL:
                    case ENTRY_TYPE.BOOLEAN:
                        obj = DecodeBooleanValue(stream, entry.Type);
                        break;
                    case ENTRY_TYPE.STRING_LE:
                    case ENTRY_TYPE.STRING_BE:
                    case ENTRY_TYPE.STRING_LE_UTF8:
                        obj = DecodeStringValue(stream, entry.Type, entry.ValueLengthInBytes);
                        break;
                    default:
                        obj = DecodeNumericValue(stream , entry.Type);
                        break;
                }
            }
            return new SerializedField(name , obj);
        }

        public static long EncodeRecord(System.IO.Stream stream, Record rec)
        {
            long total_length = sizeof(int);
            stream.WriteInt32(rec.Count);
            foreach (SerializedField sf in rec)
            {
                total_length += EncodeField(stream, sf);
            }
            return total_length;
        }

        public static long EncodeRecordArray(System.IO.Stream stream, Record[] recs)
        {
            long total_length = sizeof(uint);
            stream.WriteUInt32(recs.LongLength.ToUInt32());
            foreach (Record rc in recs)
            {
                total_length += EncodeRecord(stream, rc);
            }
            return total_length;
        }

        public static long EncodeRecordArray(System.IO.Stream stream, UninitializedRecord[] recs)
        {
            long total_length = sizeof(uint);
            stream.WriteUInt32(recs.LongLength.ToUInt32());
            foreach (UninitializedRecord rc in recs)
            {
                total_length += EncodeRecord(stream, rc.Value);
            }
            return total_length;
        }

        public static Record DecodeRecord(System.IO.Stream stream)
        {
            int count = stream.ReadInt32();
            Record.Builder builder = new(count);
            for (int I = 0; I < count; I++) 
            {
                builder.Add(DecodeField(stream));
            }
            return builder.Build();
        }

        public static Record[] DecodeRecords(System.IO.Stream stream)
        {
            uint nrecs = stream.ReadUInt32();
            Record[] rds = new Record[nrecs];
            for (uint I = 0; I < nrecs; I++)
            {
                rds[I] = DecodeRecord(stream);
            }
            return rds;
        }
    }
}