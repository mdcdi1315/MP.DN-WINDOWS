

using System;
using System.IO;
using System.Text.Json;

namespace MP.Serialization.Json
{
    /// <summary>
    /// Defines a serialized class writer by using the JSON API's provided by .NET.
    /// </summary>
    public sealed class JsonSerializedClassWriter : ISerializedClassWriter
    {
        private System.Boolean finalized;
        private Utf8JsonWriter writer;

        private void AddSimpleField(System.String name, SerializedFieldType sft, object value)
        {
            switch (sft)
            {
                case SerializedFieldType.Boolean:
                    writer.WriteBoolean(name, (System.Boolean)value);
                    break;
                case SerializedFieldType.Byte:
                    writer.WriteNumber(name, (System.Byte)value);
                    break;
                case SerializedFieldType.SByte:
                    writer.WriteNumber(name, (System.SByte)value);
                    break;
                case SerializedFieldType.Int16:
                    writer.WriteNumber(name, (System.Int16)value);
                    break;
                case SerializedFieldType.UInt16:
                    writer.WriteNumber(name, (System.UInt16)value);
                    break;
                case SerializedFieldType.Int32:
                    writer.WriteNumber(name, (System.Int32)value);
                    break;
                case SerializedFieldType.UInt32:
                    writer.WriteNumber(name, (System.UInt32)value);
                    break;
                case SerializedFieldType.Int64:
                    writer.WriteNumber(name, (System.Int64)value);
                    break;
                case SerializedFieldType.UInt64:
                    writer.WriteNumber(name, (System.UInt64)value);
                    break;
                case SerializedFieldType.Single:
                    writer.WriteNumber(name, (System.Single)value);
                    break;
                case SerializedFieldType.Double:
                    writer.WriteNumber(name, (System.Double)value);
                    break;
                case SerializedFieldType.String:
                    writer.WriteString(name, value as string);
                    break;
            }
        }

        private void AddArrayElement(System.Object directvalue)
        {
            switch (directvalue)
            {
                case System.String s:
                    writer.WriteStringValue(s);
                    break;
                case System.Boolean bl:
                    writer.WriteBooleanValue(bl);
                    break;
                case System.Byte b:
                    writer.WriteNumberValue(b);
                    break;
                case System.SByte sb:
                    writer.WriteNumberValue(sb);
                    break;
                case System.Int16 i16:
                    writer.WriteNumberValue(i16);
                    break;
                case System.UInt16 u16:
                    writer.WriteNumberValue(u16);
                    break;
                case System.Int32 i32:
                    writer.WriteNumberValue(i32);
                    break;
                case System.UInt32 u32:
                    writer.WriteNumberValue(u32);
                    break;
                case System.Int64 i64:
                    writer.WriteNumberValue(i64);
                    break;
                case System.UInt64 u64:
                    writer.WriteNumberValue(u64);
                    break;
                case System.Single sn:
                    writer.WriteNumberValue(sn);
                    break;
                case System.Double dbl:
                    writer.WriteNumberValue(dbl);
                    break;
                case InMemorySerializedClassWriter icw:
                    writer.WriteStartObject();
                    AddObjectGraph(icw, true);
                    break;
                default:
                    throw new SerializationException("The specified serialized class writer is not the writer type returned through the GetEmptyWriter method.");
            }
        }

        private void AddObjectGraph(InMemorySerializedClassWriter cw, System.Boolean arraymode)
        {
            SerializedFieldType sft;
            foreach (var i in cw.Fields)
            {
                sft = i.Key.Type;
                if (sft.HasFlag(SerializedFieldType.Array))
                {
                    Array a = i.Value as Array;
                    int len = a.GetLength(0);
                    writer.WriteStartArray(i.Key.Name);
                    try {
                        for (int I = 0; I < len; I++)
                        {
                            AddArrayElement(a.GetValue(I));
                        }
                    } finally {
                        writer.WriteEndArray();
                    }
                } else if (sft == SerializedFieldType.Object) {
                    if (i.Value is not InMemorySerializedClassWriter cw2)
                    {
                        throw new SerializationException("The specified serialized class writer is not the writer type returned through the GetEmptyWriter method.");
                    }
                    writer.WriteStartObject(i.Key.Name);
                    AddObjectGraph(cw2, arraymode);
                } else {
                    if (arraymode) {
                        AddArrayElement(i.Value);
                    } else {
                        AddSimpleField(i.Key.Name, sft, i.Value);
                    }
                }
            }
            if (arraymode == false)
            {
                writer.WriteEndObject();
            }
        }

        /// <inheritdoc />
        public void AddField(SerializedFieldInformation sfi, object value)
        {
            SerializedFieldType sft = sfi.Type;
            if (sft.HasFlag(SerializedFieldType.Array)) {
                Array a = value as Array;
                int len = a.GetLength(0);
                writer.WriteStartArray(sfi.Name);
                try {
                    for (int I = 0; I < len; I++)
                    {
                        AddArrayElement(a.GetValue(I));
                    }
                } finally {
                    writer.WriteEndArray();
                }
            } else if (sft == SerializedFieldType.Object) {
                if (value is not InMemorySerializedClassWriter cw2)
                {
                    throw new SerializationException("The specified serialized class writer is not the writer type returned through the GetEmptyWriter method.");
                }
                writer.WriteStartObject(sfi.Name);
                AddObjectGraph(cw2, false);
            } else {
                AddSimpleField(sfi.Name, sft, value);
            }
        }

        /// <summary>
        /// Finalizes the current JSON stream by writing the object terminator and flushing the JSON text created by the JSON writer.
        /// </summary>
        public void FinalizeWriteOp()
        {
            if (finalized) { return; }
            writer.WriteEndObject();
            // Dispose implicitly calls Flush() method
            writer.Dispose();
            finalized = true;
        }

        /// <inheritdoc />
        public ISerializedClassWriter GetEmptyWriter() => new InMemorySerializedClassWriter();

        /// <inheritdoc />
        public void Initialize(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (writer is not null)
            {
                FinalizeWriteOp();
            }
            try {
                // We can skip JSON validation to save perf since this API will always generate typed JSON data
                writer = new(stream, new() { Indented = true , SkipValidation = true });
                writer.WriteStartObject();
            } catch (JsonException jex) {
                throw new SerializationException("Cannot create the JSON writer to write the current object.", jex);
            }
        }

        /// <summary>
        /// Disposes the current JSON writer, ensuring that the writer has released it's resources before returning.
        /// </summary>
        public void Dispose()
        {
            if (writer is not null)
            {
                FinalizeWriteOp();
            }
            writer = null;
        }

    }
}