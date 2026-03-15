

using MP.IO;
using System;
using System.Text.Json;
using System.Collections;

namespace MP.Serialization.Json
{
    /// <summary>
    /// Defines a <see cref="IRecordWriter"/> for encoding <see cref="Record"/>s to JSON.
    /// </summary>
    public sealed class JsonRecordWriter : IRecordWriter
    {
        private Utf8JsonWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRecordWriter"/> class.
        /// </summary>
        public JsonRecordWriter() => writer = null;

        /// <inheritdoc/>
        public void WriteNew(DataStream stream, Record record)
        {
            ArgumentNullException.ThrowIfNull(record);
            writer = new(new DataStreamToStreamAdapter(stream), new JsonWriterOptions() { Indented = true, SkipValidation = true, MaxDepth = 50 });
            try {
                writer.WriteStartObject();
                WriteRecord(writer, record);
                writer.WriteEndObject();
                writer.Flush();
            } finally {
                writer.Dispose();
                writer = null;
            }
        }

        private static void WriteRecord(Utf8JsonWriter writer , Record rc)
        {
            SerializedFieldType sft;

            foreach (var fd in rc)
            {
                if (SerializationManagerUtilities.IsArrayAndExtractType(fd.Type, out sft)) {
                    Record[] rcs = fd.RecordArrayValue;

                    writer.WriteStartArray(fd.Name);
                    if (rcs is not null) {
                        foreach (var r in rcs)
                        {
                            writer.WriteStartObject();
                            WriteRecord(writer, r);
                            writer.WriteEndObject();
                        }
                    } else {
                        WriteArrayData(writer, fd, sft);
                    }
                    writer.WriteEndArray();
                } else if (SerializationManagerUtilities.IsListAndExtractType(fd.Type, out sft)) {
                    Record[] rcs = fd.RecordArrayValue;

                    writer.WriteStartArray(fd.Name);
                    if (rcs is not null) {
                        foreach (var r in rcs)
                        {
                            writer.WriteStartObject();
                            WriteRecord(writer, r);
                            writer.WriteEndObject();
                        }
                    } else {
                        WriteArrayData(writer, fd, sft);
                    }
                    writer.WriteEndArray();
                } else if (SerializationManagerUtilities.IsStringDictAndExtractType(fd.Type, out sft)) {
                    // String-strict dictionaries are encoded as Record arrays.
                    writer.WriteStartArray(fd.Name);
                    foreach (var r in fd.RecordArrayValue)
                    {
                        writer.WriteStartObject();
                        WriteRecord(writer, r);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                } else {
                    WriteSimple(writer, fd);
                }
            }
        }

        private static void WriteArrayData(Utf8JsonWriter writer, SerializedField fd, SerializedFieldType sft)
        {
            foreach (Object value in (fd.Value as IEnumerable))
            {
                switch (sft)
                {
                    case SerializedFieldType.String:
                        writer.WriteStringValue(value.ToString());
                        break;
                    case SerializedFieldType.Boolean:
                        writer.WriteBooleanValue((System.Boolean)value);
                        break;
                    case SerializedFieldType.Byte:
                        writer.WriteNumberValue((System.Byte)value);
                        break;
                    case SerializedFieldType.SByte:
                        writer.WriteNumberValue((System.SByte)value);
                        break;
                    case SerializedFieldType.Int16:
                        writer.WriteNumberValue((System.Int16)value);
                        break;
                    case SerializedFieldType.UInt16:
                        writer.WriteNumberValue((System.UInt16)value);
                        break;
                    case SerializedFieldType.Int32:
                        writer.WriteNumberValue((System.Int32)value);
                        break;
                    case SerializedFieldType.UInt32:
                        writer.WriteNumberValue((System.UInt32)value);
                        break;
                    case SerializedFieldType.Int64:
                        writer.WriteNumberValue((System.Int64)value);
                        break;
                    case SerializedFieldType.UInt64:
                        writer.WriteNumberValue((System.UInt64)value);
                        break;
                    case SerializedFieldType.Single:
                        writer.WriteNumberValue((System.Single)value);
                        break;
                    case SerializedFieldType.Double:
                        writer.WriteNumberValue((System.Double)value);
                        break;
                }
            }
        }

        private static void WriteSimple(Utf8JsonWriter writer , SerializedField fd)
        {
            switch (fd.Type)
            {
                case SerializedFieldType.Empty:
                    writer.WriteNull(fd.Name);
                    break;
                case SerializedFieldType.Object:
                    writer.WriteStartObject(fd.Name);
                    WriteRecord(writer, fd.RecordValue);
                    writer.WriteEndObject();
                    break;
                case SerializedFieldType.Boolean:
                    writer.WriteBoolean(fd.Name, (System.Boolean)fd.Value);
                    break;
                case SerializedFieldType.Byte:
                    writer.WriteNumber(fd.Name, (System.Byte)fd.Value);
                    break;
                case SerializedFieldType.SByte:
                    writer.WriteNumber(fd.Name, (System.SByte)fd.Value);
                    break;
                case SerializedFieldType.Int16:
                    writer.WriteNumber(fd.Name, (System.Int16)fd.Value);
                    break;
                case SerializedFieldType.UInt16:
                    writer.WriteNumber(fd.Name, (System.UInt16)fd.Value);
                    break;
                case SerializedFieldType.Int32:
                    writer.WriteNumber(fd.Name, (System.Int32)fd.Value);
                    break;
                case SerializedFieldType.UInt32:
                    writer.WriteNumber(fd.Name, (System.UInt32)fd.Value);
                    break;
                case SerializedFieldType.Int64:
                    writer.WriteNumber(fd.Name, (System.Int64)fd.Value);
                    break;
                case SerializedFieldType.UInt64:
                    writer.WriteNumber(fd.Name, (System.UInt64)fd.Value);
                    break;
                case SerializedFieldType.Single:
                    writer.WriteNumber(fd.Name, (System.Single)fd.Value);
                    break;
                case SerializedFieldType.Double:
                    writer.WriteNumber(fd.Name, (System.Double)fd.Value);
                    break;
                case SerializedFieldType.String:
                    writer.WriteString(fd.Name, fd.Value.ToString());
                    break;
            }
        }

        /// <summary>
        /// Disposes this <see cref="JsonRecordWriter"/> instance.
        /// </summary>
        public void Dispose()
        {
            writer?.Dispose();
            writer = null;
        }
    }
}