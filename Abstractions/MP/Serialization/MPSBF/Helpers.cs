
using System;
using MP.Collections;
using System.Collections.Generic;

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

        public static ENTRY_TYPE ToEntryType(SerializedFieldType sft)
        {
            switch (sft)
            {
                case SerializedFieldType.Empty:
                    return ENTRY_TYPE.NULL;
                case SerializedFieldType.Object:
                    return ENTRY_TYPE.RECORD;
                case SerializedFieldType.Boolean:
                    return ENTRY_TYPE.BOOLEAN;
                case SerializedFieldType.String:
                    return ENTRY_TYPE.STRING_BE;
                case SerializedFieldType.Byte:
                    return ENTRY_TYPE.UNSIGNED_CHAR;
                case SerializedFieldType.SByte:
                    return ENTRY_TYPE.SIGNED_CHAR;
                case SerializedFieldType.Int16:
                    return ENTRY_TYPE.SHORT;
                case SerializedFieldType.UInt16:
                    return ENTRY_TYPE.USHORT;
                case SerializedFieldType.Int32:
                    return ENTRY_TYPE.INT;
                case SerializedFieldType.UInt32:
                    return ENTRY_TYPE.UINT;
                case SerializedFieldType.Int64:
                    return ENTRY_TYPE.LONG;
                case SerializedFieldType.UInt64:
                    return ENTRY_TYPE.ULONG;
                case SerializedFieldType.Single:
                    return ENTRY_TYPE.FLOAT;
                case SerializedFieldType.Double:
                    return ENTRY_TYPE.DOUBLE;
                default:
                    throw new NotSupportedException("Unmappable type " + sft + " detected!");
            }
        }

        public static SerializedFieldType ToSerializedFieldType(ENTRY_TYPE type)
        {
            switch (type)
            {
                case ENTRY_TYPE.NULL:
                    return SerializedFieldType.Empty;
                case ENTRY_TYPE.RECORD:
                    return SerializedFieldType.Object;
                case ENTRY_TYPE.BOOLEAN:
                    return SerializedFieldType.Boolean;
                case ENTRY_TYPE.STRING_BE:
                    return SerializedFieldType.String;
                case ENTRY_TYPE.UNSIGNED_CHAR:
                    return SerializedFieldType.Byte;
                case ENTRY_TYPE.SIGNED_CHAR:
                    return SerializedFieldType.SByte;
                case ENTRY_TYPE.SHORT:
                    return SerializedFieldType.Int16;
                case ENTRY_TYPE.USHORT:
                    return SerializedFieldType.UInt16;
                case ENTRY_TYPE.INT:
                    return SerializedFieldType.Int32;
                case ENTRY_TYPE.UINT:
                    return SerializedFieldType.UInt32;
                case ENTRY_TYPE.LONG:
                    return SerializedFieldType.Int64;
                case ENTRY_TYPE.ULONG:
                    return SerializedFieldType.UInt64;
                case ENTRY_TYPE.FLOAT:
                    return SerializedFieldType.Single;
                case ENTRY_TYPE.DOUBLE:
                    return SerializedFieldType.Double;
                default:
                    throw new NotSupportedException("Unmappable type " + type + " detected!");
            }
        }

        public static long EncodeStringValue(IO.DataStream stream, String str, System.Text.Encoding encoding) => stream.WriteString(str, encoding);

        public static String DecodeStringValue(IO.DataStream stream, ENTRY_TYPE type, UInt32 string_length_in_bytes)
        {
            System.Text.Encoding enc = type switch {
                ENTRY_TYPE.STRING_LE_UTF8 => System.Text.Encoding.UTF8,
                ENTRY_TYPE.STRING_LE => System.Text.Encoding.Unicode,
                ENTRY_TYPE.STRING_BE => System.Text.Encoding.BigEndianUnicode,
                _ => throw new NotSupportedException($"Entry type not supported: {type}"),
            };
            return stream.ReadString(enc, string_length_in_bytes);
        }

        public static long EncodeNumericValue(IO.DataStream stream, System.Object obj, out ENTRY_TYPE type)
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

        public static System.Object DecodeNumericValue(IO.DataStream stream , ENTRY_TYPE type) => type switch
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

        public static long EncodeBooleanValue(IO.DataStream stream, System.Object obj, out ENTRY_TYPE type)
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

        public static System.Object DecodeBooleanValue(IO.DataStream stream, ENTRY_TYPE type) => type switch {
            ENTRY_TYPE.BOOL => (BOOL)stream.ReadInt32(),
            ENTRY_TYPE.BOOLEAN => stream.ReadBoolean(),
            _ => throw new NotSupportedException(),
        };

        public static long EncodeStringArrayValue(IO.DataStream stream, String[] values , System.Text.Encoding encoding)
        {
            long data_length = 0, temp_length;
            using (IO.MemoryStream ms = new(encoding.GetMaxByteCount(1) * 20 , true))
            {
                stream.WriteInt64(values.LongLength);
                data_length += sizeof(long);
                foreach (var value in values)
                {
                    stream.WriteInt64(temp_length = ms.WriteString(value, encoding));
                    ms.Position = 0;
                    ms.DirectCopyToStream(stream);
                    ms.SetLength(0); // Automatically the position/length are set to zero.
                    data_length += temp_length;
                }
            }
            return data_length;
        }

        public static long EncodeStringListValue(IO.DataStream stream, IList<String> values, System.Text.Encoding encoding)
        {
            long data_length = 0, temp_length;
            using (IO.MemoryStream ms = new(encoding.GetMaxByteCount(1) * 20, true))
            {
                stream.WriteInt64(values.Count);
                data_length += sizeof(long);
                foreach (var value in values)
                {
                    stream.WriteInt64(temp_length = ms.WriteString(value, encoding));
                    ms.Position = 0;
                    ms.DirectCopyToStream(stream);
                    ms.SetLength(0); // Automatically the position/length are set to zero.
                    data_length += temp_length;
                }
            }
            return data_length;
        }

        public static System.String[] DecodeStringArrayValue(IO.DataStream stream, ENTRY_TYPE type)
        {
            System.Text.Encoding enc = type switch {
                ENTRY_TYPE.STRING_LE_UTF8 => System.Text.Encoding.UTF8,
                ENTRY_TYPE.STRING_LE => System.Text.Encoding.Unicode,
                ENTRY_TYPE.STRING_BE => System.Text.Encoding.BigEndianUnicode,
                _ => throw new NotSupportedException($"Entry type not supported: {type}"),
            };
            long string_values = stream.ReadInt64();
            System.String[] strings = new System.String[string_values];

            for (long str_index = 0L; str_index < string_values; str_index++)
            {
                strings[str_index] = stream.ReadString(enc, stream.ReadInt64());
            }

            return strings;
        }

        public static long EncodeNumericArrayValue(IO.DataStream stream, System.Object obj, out ENTRY_TYPE type)
        {
            long c;
            switch (obj)
            {
                case System.Byte[] b:
                    stream.WriteInt32(b.Length);
                    stream.WriteBytes(b);
                    type = ENTRY_TYPE.UNSIGNED_CHAR | ENTRY_TYPE.ARRAY;
                    return b.LongLength + sizeof(int);
                case IList<System.Byte> bl:
                    c = 0L;
                    stream.WriteInt32(bl.Count);
                    foreach (System.Byte b in bl) { c++; stream.WriteByte(b); }
                    type = ENTRY_TYPE.UNSIGNED_CHAR | ENTRY_TYPE.LIST;
                    return c + sizeof(int);
                case System.SByte[] sb:
                    type = ENTRY_TYPE.SIGNED_CHAR | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.SByte>(sb) + sizeof(int);
                case IList<System.SByte> bl:
                    c = 0L;
                    stream.WriteInt32(bl.Count);
                    foreach (System.SByte b in bl) { c++; stream.WriteSByte(b); }
                    type = ENTRY_TYPE.SIGNED_CHAR | ENTRY_TYPE.LIST;
                    return c + sizeof(int);
                case System.Int16[] i16:
                    type = ENTRY_TYPE.SHORT | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.Int16>(i16) + sizeof(int);
                case IList<System.Int16> bl:
                    c = 0L;
                    stream.WriteInt32(bl.Count);
                    foreach (System.Int16 b in bl) { c += sizeof(System.Int16); stream.WriteInt16(b); }
                    type = ENTRY_TYPE.SHORT | ENTRY_TYPE.LIST;
                    return c + sizeof(int);
                case System.UInt16[] u16:
                    type = ENTRY_TYPE.USHORT | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.UInt16>(u16) + sizeof(int);
                case IList<System.UInt16> bl:
                    c = 0L;
                    stream.WriteInt32(bl.Count);
                    foreach (System.UInt16 b in bl) { c += sizeof(System.UInt16); stream.WriteUInt16(b); }
                    type = ENTRY_TYPE.USHORT | ENTRY_TYPE.LIST;
                    return c + sizeof(int);
                case System.Int32[] i32:
                    type = ENTRY_TYPE.INT | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.Int32>(i32) + sizeof(int);
                case IList<System.Int32> bl:
                    c = 0L;
                    stream.WriteInt32(bl.Count);
                    foreach (System.Int32 b in bl) { c += sizeof(System.Int32); stream.WriteInt32(b); }
                    type = ENTRY_TYPE.INT | ENTRY_TYPE.LIST;
                    return c + sizeof(int);
                case System.UInt32[] u32:
                    type = ENTRY_TYPE.UINT | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.UInt32>(u32) + sizeof(int);
                case IList<System.UInt32> bl:
                    c = 0L;
                    stream.WriteInt32(bl.Count);
                    foreach (System.UInt32 b in bl) { c += sizeof(System.UInt32); stream.WriteUInt32(b); }
                    type = ENTRY_TYPE.UINT | ENTRY_TYPE.LIST;
                    return c + sizeof(int);
                case System.Int64[] i64:
                    type = ENTRY_TYPE.LONG | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.Int64>(i64) + sizeof(int);
                case IList<System.Int64> bl:
                    c = 0L;
                    stream.WriteInt32(bl.Count);
                    foreach (System.Int64 b in bl) { c += sizeof(System.Int64); stream.WriteInt64(b); }
                    type = ENTRY_TYPE.LONG | ENTRY_TYPE.LIST;
                    return c + sizeof(int);
                case System.UInt64[] u64:
                    type = ENTRY_TYPE.ULONG | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.UInt64>(u64) + sizeof(int);
                case IList<System.UInt64> bl:
                    c = 0L;
                    stream.WriteInt32(bl.Count);
                    foreach (System.UInt64 b in bl) { c += sizeof(System.UInt64); stream.WriteUInt64(b); }
                    type = ENTRY_TYPE.ULONG | ENTRY_TYPE.LIST;
                    return c + sizeof(int);
                case System.Single[] s:
                    type = ENTRY_TYPE.FLOAT | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.Single>(s) + sizeof(int);
                case IList<System.Single> bl:
                    c = 0L;
                    stream.WriteInt32(bl.Count);
                    foreach (System.Single b in bl) { c += sizeof(System.Single); stream.WriteSingle(b); }
                    type = ENTRY_TYPE.FLOAT | ENTRY_TYPE.LIST;
                    return c + sizeof(int);
                case System.Double[] d:
                    type = ENTRY_TYPE.DOUBLE | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<System.Double>(d) + sizeof(int);
                case IList<System.Double> bl:
                    c = 0L;
                    stream.WriteInt32(bl.Count);
                    foreach (System.Double b in bl) { c += sizeof(System.Double); stream.WriteDouble(b); }
                    type = ENTRY_TYPE.DOUBLE | ENTRY_TYPE.LIST;
                    return c + sizeof(int);
                default:
                    throw new NotSupportedException();
            }
        }

        public static System.Object DecodeNumericArrayValue(IO.DataStream stream, ENTRY_TYPE type)
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

        public static System.Object DecodeNumericListValue(IO.DataStream stream, ENTRY_TYPE type)
        {
            int length = stream.ReadInt32();
            return type switch {
                ENTRY_TYPE.UNSIGNED_CHAR => new ArrayBasedList<System.Byte>(stream.ReadBytes(length)),
                ENTRY_TYPE.SIGNED_CHAR => new ArrayBasedList<System.SByte>(stream.ReadTypedArray<System.SByte>(length)),
                ENTRY_TYPE.SHORT => new ArrayBasedList<System.Int16>(stream.ReadTypedArray<System.Int16>(length)),
                ENTRY_TYPE.USHORT => new ArrayBasedList<System.UInt16>(stream.ReadTypedArray<System.UInt16>(length)),
                ENTRY_TYPE.INT => new ArrayBasedList<System.Int32>(stream.ReadTypedArray<System.Int32>(length)),
                ENTRY_TYPE.UINT => new ArrayBasedList<System.UInt32>(stream.ReadTypedArray<System.UInt32>(length)),
                ENTRY_TYPE.LONG => new ArrayBasedList<System.Int64>(stream.ReadTypedArray<System.Int64>(length)),
                ENTRY_TYPE.ULONG => new ArrayBasedList<System.UInt64>(stream.ReadTypedArray<System.UInt64>(length)),
                ENTRY_TYPE.FLOAT => new ArrayBasedList<System.Single>(stream.ReadTypedArray<System.Single>(length)),
                ENTRY_TYPE.DOUBLE => new ArrayBasedList<System.Double>(stream.ReadTypedArray<System.Double>(length)),
                _ => throw new NotSupportedException(),
            };
        }

        public static long EncodeBooleanArrayValue(IO.DataStream stream, System.Object obj, out ENTRY_TYPE type)
        {
            switch (obj)
            {
                case BOOL[] bls:
                    type = ENTRY_TYPE.BOOL | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<BOOL>(bls) + sizeof(int);
                case IList<BOOL> bls:
                    type = ENTRY_TYPE.BOOL | ENTRY_TYPE.LIST;
                    stream.WriteInt32(bls.Count);
                    long c = 0L;
                    foreach (BOOL b in bls)
                    {
                        stream.WriteStructure(b);
                        c += sizeof(BOOL);
                    }
                    return c + sizeof(int);
                case Boolean[] bools:
                    type = ENTRY_TYPE.BOOLEAN | ENTRY_TYPE.ARRAY;
                    return stream.WriteEncodedSpan<Boolean>(bools) + sizeof(int);
                case IList<Boolean> bbs:
                    type = ENTRY_TYPE.BOOL | ENTRY_TYPE.LIST;
                    stream.WriteInt32(bbs.Count);
                    c = 0L;
                    foreach (Boolean b in bbs)
                    {
                        stream.WriteStructure(b);
                        c += sizeof(Boolean);
                    }
                    return c + sizeof(int);
                default:
                    throw new NotSupportedException();
            }
        }

        public static System.Object DecodeBooleanArrayValue(IO.DataStream stream, ENTRY_TYPE type)
        {
            int length = stream.ReadInt32();
            return type switch {
                ENTRY_TYPE.BOOL => stream.ReadTypedArray<BOOL>(length),
                ENTRY_TYPE.BOOLEAN => stream.ReadTypedArray<Boolean>(length),
                _ => throw new NotSupportedException(),
            };
        }

        public static System.Object DecodeBooleanListValue(IO.DataStream stream, ENTRY_TYPE type)
        {
            int length = stream.ReadInt32();
            return type switch {
                ENTRY_TYPE.BOOL => new ArrayBasedList<BOOL>(stream.ReadTypedArray<BOOL>(length)),
                ENTRY_TYPE.BOOLEAN => new ArrayBasedList<Boolean>(stream.ReadTypedArray<Boolean>(length)),
                _ => throw new NotSupportedException(),
            };
        }

        public static unsafe long EncodeField(IO.DataStream stream, SerializedField field)
        {
            int namelen = field.Name.Length;
            if (namelen > System.UInt16.MaxValue) {
                throw new NotSupportedException("Cannot encode the field because it's name is too large.");
            }
            long total_data_len = 16;
            SBF_ENTRY_HEADER entry = new();
            entry.NameLengthInChars = namelen.ToUInt16();
            using (IO.MemoryStream ms = new())
            {
                long length;
                if (SerializationManagerUtilities.IsStringDictAndExtractType(field.Type, out var ct)) {
                    entry.Type = ENTRY_TYPE.UTF16LE_STRING_DICT | ToEntryType(ct);
                    length = EncodeRecordArray(ms, field.RecordArrayValue);
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
                            length = EncodeRecord(ms, r);
                            entry.Type = ENTRY_TYPE.RECORD;
                            break;
                        case UninitializedRecord ur:
                            length = EncodeRecord(ms, ur.Value);
                            entry.Type = ENTRY_TYPE.RECORD;
                            break;
                        case System.String[] strs:
                            length = EncodeStringArrayValue(ms, strs, System.Text.Encoding.Unicode);
                            entry.Type = ENTRY_TYPE.STRING_LE | ENTRY_TYPE.ARRAY;
                            break;
                        case IList<System.String> sl:
                            length = EncodeStringListValue(ms, sl, System.Text.Encoding.Unicode);
                            entry.Type = ENTRY_TYPE.STRING_LE | ENTRY_TYPE.LIST;
                            break;
                        case IList<BOOL>:
                        case IList<System.Boolean>:
                            length = EncodeBooleanArrayValue(ms, field.Value, out entry.Type);
                            break;
                        case Record[] records:
                            length = EncodeRecordArray(ms, records);
                            entry.Type = ENTRY_TYPE.RECORD | ENTRY_TYPE.ARRAY;
                            break;
                        case IList<Record> r:
                            length = EncodeRecordArray(ms, r);
                            entry.Type = ENTRY_TYPE.RECORD | ENTRY_TYPE.LIST;
                            break;
                        case UninitializedRecord[] urecords:
                            length = EncodeRecordArray(ms, urecords);
                            entry.Type = ENTRY_TYPE.RECORD | ENTRY_TYPE.ARRAY;
                            break;
                        case IList<UninitializedRecord> r:
                            length = EncodeRecordArray(ms, r);
                            entry.Type = ENTRY_TYPE.RECORD | ENTRY_TYPE.LIST;
                            break;
                        default:
                            if (field.Type.HasFlag(SerializedFieldType.Array) || field.Type.HasFlag(SerializedFieldType.List)) {
                                length = EncodeNumericArrayValue(ms, field.Value, out entry.Type);
                            } else {
                                length = EncodeNumericValue(ms, field.Value, out entry.Type);
                            }
                            break;
                    }
                }
                if (length > System.UInt32.MaxValue) {
                    throw new NotSupportedException($"Value is too large to be saved on the data stream: Was {length} bytes while up to {System.UInt32.MaxValue} bytes are supported.");
                } else {
                    total_data_len += length;
                    entry.ValueLengthInBytes = length.ToUInt32();
                    total_data_len += (entry.Name = field.Name).Length * sizeof(System.Char);
                    entry.Save(stream);
                    stream.WriteByte(0); // Sync byte - verifies correct bitstream
                    total_data_len++;
                    ms.Position = 0;
                    ms.DirectCopyToStream(stream);
                }
            }
            return total_data_len;
        }
    
        public static unsafe SerializedField DecodeField(IO.DataStream stream)
        {
            SBF_ENTRY_HEADER entry = new();
            entry.Load(stream);
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
            } else if (entry.Type.HasFlag(ENTRY_TYPE.LIST)) {
                ENTRY_TYPE t;
                switch (t = (entry.Type & ~ENTRY_TYPE.LIST))
                {
                    case ENTRY_TYPE.NULL:
                        obj = null;
                        break;
                    case ENTRY_TYPE.RECORD:
                        obj = DecodeRecords_List(stream);
                        break;
                    case ENTRY_TYPE.BOOL:
                    case ENTRY_TYPE.BOOLEAN:
                        obj = DecodeBooleanListValue(stream, t);
                        break;
                    case ENTRY_TYPE.STRING_LE:
                    case ENTRY_TYPE.STRING_BE:
                    case ENTRY_TYPE.STRING_LE_UTF8:
                        obj = new ArrayBasedList<System.String>(DecodeStringArrayValue(stream, t));
                        break;
                    default:
                        obj = DecodeNumericListValue(stream, t);
                        break;
                }
            } else if (entry.Type.HasFlag(ENTRY_TYPE.UTF16LE_STRING_DICT)) {
                obj = DecodeRecords(stream);
                return new(entry.Name, obj, SerializedFieldType.StrictStringDictionary | ToSerializedFieldType(entry.Type));
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
            return new SerializedField(entry.Name , obj);
        }

        public static long EncodeRecord(IO.DataStream stream, Record rec)
        {
            long total_length = sizeof(int);
            stream.WriteInt32(rec.Count);
            foreach (SerializedField sf in rec)
            {
                total_length += EncodeField(stream, sf);
            }
            return total_length;
        }

        public static long EncodeRecordArray(IO.DataStream stream, IList<Record> recs)
        {
            long total_length = sizeof(uint);
            stream.WriteUInt32(recs.Count.ToUInt32());
            foreach (Record rc in recs)
            {
                total_length += EncodeRecord(stream, rc);
            }
            return total_length;
        }

        public static long EncodeRecordArray(IO.DataStream stream, IList<UninitializedRecord> recs)
        {
            long total_length = sizeof(uint);
            stream.WriteUInt32(recs.Count.ToUInt32());
            foreach (UninitializedRecord rc in recs)
            {
                total_length += EncodeRecord(stream, rc.Value);
            }
            return total_length;
        }

        public static Record DecodeRecord(IO.DataStream stream)
        {
            int count = stream.ReadInt32();
            SerializedField[] fields = new SerializedField[count];
            for (int I = 0; I < count; I++)  { fields[I] = DecodeField(stream); }
            return new(fields);
        }

        public static Record[] DecodeRecords(IO.DataStream stream)
        {
            uint nrecs = stream.ReadUInt32();
            Record[] rds = new Record[nrecs];
            for (uint I = 0; I < nrecs; I++)
            {
                rds[I] = DecodeRecord(stream);
            }
            return rds;
        }

        public static ArrayBasedList<Record> DecodeRecords_List(IO.DataStream stream)
        {
            uint nrecs = stream.ReadUInt32();
            ArrayBasedList<Record> r = new(nrecs > System.Int32.MaxValue ? System.Int32.MaxValue : nrecs.ToInt32());
            for (uint I = 0; I < nrecs; I++)
            {
                r.Add(DecodeRecord(stream));
            }
            return r;
        }
    }
}