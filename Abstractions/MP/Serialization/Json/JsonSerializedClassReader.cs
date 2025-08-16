

using System;
using System.IO;
using System.Text.Json;
using static System.Text.Json.JsonElement;

namespace MP.Serialization.Json
{
    /// <summary>
    /// Defines a serialized class reader by using the JSON API's provided by .NET.
    /// </summary>
    public sealed class JsonSerializedClassReader : ISerializedClassReader
    {
        private object dec;
        private JsonDocument document;
        private SerializedFieldInformation sfi;
        private ObjectEnumerator jsonobjenumerator;

        private sealed class JsonObjectClassReader : ISerializedClassReader
        {
            private ObjectEnumerator en;
            private SerializedFieldInformation sfi;

            public JsonObjectClassReader(JsonElement el)
            {
                en = el.EnumerateObject();
                sfi = null;
            }

            public object DecodeField()
            {
                _ = GetFieldInformation();
                JsonElement elem = en.Current.Value;
                SerializedFieldType sft = sfi.Type;
                if (sft.HasFlag(SerializedFieldType.Array))
                {
                    var cleantype = sft & ~SerializedFieldType.Array;
                    int len;
                    Array a = Array.CreateInstance(cleantype.GetDotNetType(), len = elem.GetArrayLength());
                    for (int I = 0; I < len; I++)
                    {
                        a.SetValue(DecodeSimpleField(elem[I], cleantype), I);
                    }
                    return a;
                }
                else
                {
                    return DecodeSimpleField(elem, sft);
                }
            }

            public void Dispose() => en.Dispose();

            public SerializedFieldInformation GetFieldInformation()
            {
                if (sfi is null)
                {
                    JsonProperty prop = en.Current;
                    sfi = new SerializedFieldInformation(prop.Name, RealizeFieldType(prop.Value));
                }
                return sfi;
            }

            public void Initialize(Stream stream) => throw new NotSupportedException("Call is not supported");

            public bool MoveNext()
            {
                sfi = null;
                return en.MoveNext();
            }
        }

        /// <inheritdoc />
        public object DecodeField()
        {
            _ = GetFieldInformation();
            if (dec is null)
            {
                JsonElement elem = jsonobjenumerator.Current.Value;
                SerializedFieldType sft = sfi.Type;
                if (sft.HasFlag(SerializedFieldType.Array))
                {
                    var cleantype = sft & ~SerializedFieldType.Array;
                    int len;
                    Array a = Array.CreateInstance(cleantype.GetDotNetType(), len = elem.GetArrayLength());
                    for (int I = 0; I < len; I++)
                    {
                        a.SetValue(DecodeSimpleField(elem[I], cleantype), I);
                    }
                    dec = a;
                }
                else
                {
                    dec = DecodeSimpleField(elem, sft);
                }
            }
            return dec;
        }

        private static System.Object DecodeSimpleField(JsonElement elem, SerializedFieldType sft) => sft switch
        {
            SerializedFieldType.Boolean => elem.GetBoolean(),
            SerializedFieldType.String => elem.GetString(),
            SerializedFieldType.Byte => elem.GetByte(),
            SerializedFieldType.SByte => elem.GetSByte(),
            SerializedFieldType.Int16 => elem.GetInt16(),
            SerializedFieldType.UInt16 => elem.GetUInt16(),
            SerializedFieldType.Int32 => elem.GetInt32(),
            SerializedFieldType.UInt32 => elem.GetUInt32(),
            SerializedFieldType.Int64 => elem.GetInt64(),
            SerializedFieldType.UInt64 => elem.GetUInt64(),
            SerializedFieldType.Single => elem.GetSingle(),
            SerializedFieldType.Double => elem.GetDouble(),
            SerializedFieldType.Object => new JsonObjectClassReader(elem),
            _ => null,
        };

        /// <summary>
        /// Disposes this <see cref="JsonSerializedClassReader"/> class.
        /// </summary>
        public void Dispose()
        {
            sfi = null;
            dec = null;
            jsonobjenumerator.Dispose();
            jsonobjenumerator = default;
            document?.Dispose();
        }

        /// <inheritdoc />
        public SerializedFieldInformation GetFieldInformation()
        {
            if (sfi is null)
            {
                JsonProperty prop = jsonobjenumerator.Current;
                sfi = new SerializedFieldInformation(prop.Name, RealizeFieldType(prop.Value));
            }
            return sfi;
        }

        private static SerializedFieldType RealizeSimpleFieldType(JsonElement value)
        {
            SerializedFieldType sft;
            switch (value.ValueKind)
            {
                case JsonValueKind.Number:
                    if (value.TryGetByte(out _))
                    {
                        sft = SerializedFieldType.Byte;
                    }
                    else if (value.TryGetSByte(out _))
                    {
                        sft = SerializedFieldType.SByte;
                    }
                    else if (value.TryGetInt16(out _))
                    {
                        sft = SerializedFieldType.Int16;
                    }
                    else if (value.TryGetUInt16(out _))
                    {
                        sft = SerializedFieldType.UInt16;
                    }
                    else if (value.TryGetInt32(out _))
                    {
                        sft = SerializedFieldType.Int32;
                    }
                    else if (value.TryGetUInt32(out _))
                    {
                        sft = SerializedFieldType.UInt32;
                    }
                    else if (value.TryGetInt64(out _))
                    {
                        sft = SerializedFieldType.Int64;
                    }
                    else if (value.TryGetUInt64(out _))
                    {
                        sft = SerializedFieldType.UInt64;
                    }
                    else if (value.TryGetSingle(out _))
                    {
                        sft = SerializedFieldType.Single;
                    }
                    else if (value.TryGetDouble(out _))
                    {
                        sft = SerializedFieldType.Double;
                    }
                    else
                    {
                        throw new SerializationException("Cannot determine the .NET type of the specified JSON number.");
                    }
                    break;
                case JsonValueKind.String:
                    sft = SerializedFieldType.String;
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    sft = SerializedFieldType.Boolean;
                    break;
                case JsonValueKind.Null:
                case JsonValueKind.Object:
                    sft = SerializedFieldType.Object;
                    break;
                default:
                    sft = 0;
                    break;
            }
            return sft;
        }

        private static SerializedFieldType RealizeFieldType(JsonElement value)
        {
            SerializedFieldType sft;
            if (value.ValueKind == JsonValueKind.Array)
            {
                sft = (value.GetArrayLength() > 0) ? RealizeSimpleFieldType(value[0]) : SerializedFieldType.Object;
                sft |= SerializedFieldType.Array;
            }
            else
            {
                sft = RealizeSimpleFieldType(value);
            }
            return sft;
        }

        /// <inheritdoc />
        public void Initialize(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            dec = null;
            sfi = null;
            jsonobjenumerator.Dispose();
            document?.Dispose();
            document = null; // So that the CLR knows that the old JsonDocument object's mem can be reclaimed.
            try {
                document = JsonDocument.Parse(stream, new JsonDocumentOptions() { CommentHandling = JsonCommentHandling.Skip });
                jsonobjenumerator = document.RootElement.EnumerateObject();
            } catch (JsonException jex) {
                throw new SerializationException("Cannot decode the specified JSON data.", jex);
            }
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            sfi = null;
            dec = null;
            return jsonobjenumerator.MoveNext();
        }
    }
}