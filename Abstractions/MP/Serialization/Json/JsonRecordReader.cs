
using System;
using System.IO;
using System.Text.Json;

namespace MP.Serialization.Json
{
    /// <summary>
    /// Defines a <see cref="IRecordReader"/> for decoding JSON <see cref="Record"/>s.
    /// </summary>
    public sealed class JsonRecordReader : IRecordReader
    {
        private JsonDocument doc;

        /// <summary>
        /// Initializes an instance of the <see cref="JsonRecordReader"/>.
        /// </summary>
        public JsonRecordReader() {
            doc = null;
        }

        private static Record DecodeObject(JsonElement root)
        {
            Record.Builder builder = new();

            foreach (var element in root.EnumerateObject())
            {
                switch (element.Value.ValueKind)
                {
                    case JsonValueKind.Array:
                        builder.Add(new SerializedField(element.Name , DecodeArray(root)));
                        break;
                    case JsonValueKind.Object:
                        builder.Add(new SerializedField(element.Name, new UninitializedRecord(() => DecodeObject(element.Value))));
                        break;
                    default:
                        builder.Add(DecodeSimpleType(element));
                        break;
                }
            }

            return builder.Build();
        }

        private static System.Object DecodeTypeAsObject(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.String:
                    return value.GetString();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Number:
                    System.Object v = null;
                    if (value.TryGetByte(out var b)) {
                        v = b;
                    } else if (value.TryGetSByte(out var sb)) {
                        v = sb;
                    } else if (value.TryGetInt16(out var i16)) {
                        v = i16;
                    } else if (value.TryGetUInt16(out var u16)) {
                        v = u16;
                    } else if (value.TryGetInt32(out var i32)) {
                        v = i32;
                    } else if (value.TryGetUInt32(out var u32)) {
                        v = u32;
                    } else if (value.TryGetInt64(out var i64)) {
                        v = i64;
                    } else if (value.TryGetUInt64(out var u64)) {
                        v = u64;
                    } else if (value.TryGetSingle(out var fl)) {
                        v = fl;
                    } else if (value.TryGetDouble(out var dbl)) {
                        v = dbl;
                    }
                    return v;
                case JsonValueKind.Object:
                    return new UninitializedRecord(() => DecodeObject(value));
                default:
                case JsonValueKind.Null:
                    return null;
            }
        }

        private static SerializedField DecodeSimpleType(JsonProperty prop) => new(prop.Name, DecodeTypeAsObject(prop.Value));

        private static Array DecodeArray(JsonElement sourcearray)
        {
            int al = sourcearray.GetArrayLength();
            if (al == 0) { return Array.Empty<System.Object>(); }

            Array target = null;

            switch (sourcearray[0].ValueKind)
            {
                case JsonValueKind.Array:
                    throw new SerializationException("Cannot support arrays inside arrays.");
                case JsonValueKind.Object:
                    target = Array.CreateInstance(typeof(UninitializedRecord), al);
                    break;
                case JsonValueKind.String:
                    target = Array.CreateInstance(typeof(String), al);
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    target = Array.CreateInstance(typeof(Boolean), al);
                    break;
                case JsonValueKind.Number:
                    target = Array.CreateInstance(typeof(Object), al);
                    break;
            }

            try {
                for (int I = 0; I < al; I++)
                {
                    target.SetValue(DecodeObject(sourcearray[I]), I);
                }
            } catch (InvalidCastException ice) {
                throw new SerializationException("Objects of different types were found in the JSON array. This is not supported.", ice); 
            }

            return target;
        }

        /// <summary>
        /// Gets the decoded root JSON object as a <see cref="Record"/>.
        /// </summary>
        public Record Payload
        {
            get {
                JsonElement e = doc.RootElement;
                if (e.ValueKind != JsonValueKind.Object) {
                    throw new SerializationException("Attempted to decode a non-record JSON object.");
                }
                return DecodeObject(e);
            }
        }

        /// <inheritdoc />
        public void EndPayloadDecoding()
        {
            doc?.Dispose();
            doc = null;
        }

        /// <inheritdoc />
        public void InitializeForNewPayload(Stream stream)
        {
            if (doc is not null) {
                throw new InvalidOperationException("The previous operation has not yet been ended.");
            }
            doc = JsonDocument.Parse(stream, new JsonDocumentOptions() { CommentHandling = JsonCommentHandling.Skip, MaxDepth = 50 });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            doc?.Dispose();
            doc = null;
        }
    }

}