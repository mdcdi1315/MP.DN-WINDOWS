

using System;
using System.IO;
using System.Xml;

namespace MP.Serialization.Xml
{
    /// <summary>
    /// Defines a <see cref="IRecordWriter"/> for encoding <see cref="Record"/>s to XML.
    /// </summary>
    public sealed class XmlRecordWriter : IRecordWriter
    {
        private XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRecordWriter"/> class.
        /// </summary>
        public XmlRecordWriter() => writer = null;

        private static void AddSimpleField(SerializedField sf , XmlWriter writer)
        {
            writer.WriteStartElement(XmlSerializationConstants.SerializedElementTagName);
            writer.WriteAttributeString(XmlSerializationConstants.ElementNameAttributeName, sf.Name);
            writer.WriteAttributeString(XmlSerializationConstants.ElementTypeAttributeName, XmlSerializationConstants.GetSimpleXmlFieldType(sf.Type));
            writer.WriteStartElement(XmlSerializationConstants.SerializedElementValueTagName);

            switch (sf.Type)
            {
                case SerializedFieldType.Boolean:
                    writer.WriteValue((System.Boolean)sf.Value);
                    break;
                case SerializedFieldType.Byte:
                    writer.WriteValue((System.Byte)sf.Value);
                    break;
                case SerializedFieldType.SByte:
                    writer.WriteValue((System.SByte)sf.Value);
                    break;
                case SerializedFieldType.Int16:
                    writer.WriteValue((System.Int16)sf.Value);
                    break;
                case SerializedFieldType.UInt16:
                    writer.WriteValue((System.UInt16)sf.Value);
                    break;
                case SerializedFieldType.Int32:
                    writer.WriteValue((System.Int32)sf.Value);
                    break;
                case SerializedFieldType.UInt32:
                    writer.WriteValue((System.UInt32)sf.Value);
                    break;
                case SerializedFieldType.Int64:
                    writer.WriteValue((System.Int64)sf.Value);
                    break;
                case SerializedFieldType.UInt64:
                    writer.WriteValue((System.Double)(System.UInt64)sf.Value);
                    break;
                case SerializedFieldType.Single:
                    writer.WriteValue((System.Single)sf.Value);
                    break;
                case SerializedFieldType.Double:
                    writer.WriteValue((System.Double)sf.Value);
                    break;
                case SerializedFieldType.String:
                    writer.WriteValue(sf.Value as string);
                    break;
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private static void AddArrayElement(System.Object value , XmlWriter writer)
        {
            writer.WriteStartElement(XmlSerializationConstants.SerializedElementValueTagName);

            switch (value)
            {
                case System.Boolean bl:
                    writer.WriteValue(bl);
                    break;
                case System.Byte b:
                    writer.WriteValue(b);
                    break;
                case System.SByte sb:
                    writer.WriteValue(sb);
                    break;
                case System.Int16 i16:
                    writer.WriteValue(i16);
                    break;
                case System.UInt16 u16:
                    writer.WriteValue(u16);
                    break;
                case System.Int32 i32:
                    writer.WriteValue(i32);
                    break;
                case System.UInt32 u32:
                    writer.WriteValue(u32);
                    break;
                case System.Int64 i64:
                    writer.WriteValue(i64);
                    break;
                case System.UInt64 u64:
                    writer.WriteValue((System.Double)u64);
                    break;
                case System.Single f:
                    writer.WriteValue(f);
                    break;
                case System.Double d:
                    writer.WriteValue(d);
                    break;
                case System.String s:
                    writer.WriteValue(s);
                    break;
                case Record rec:
                    AddRecord(rec, writer);
                    break;
                case UninitializedRecord rec2:
                    AddRecord(rec2.Value, writer);
                    break;
            }

            writer.WriteEndElement();
        }

        private static void AddRecord(Record rc , XmlWriter writer)
        {
            SerializedFieldType sft;
            foreach (var i in rc)
            {
                sft = i.Type;
                if (sft.HasFlag(SerializedFieldType.Array))
                {
                    Array a = i.Value as Array;
                    int len = a.GetLength(0);
                    writer.WriteStartElement(XmlSerializationConstants.SerializedElementTagName);
                    writer.WriteAttributeString(XmlSerializationConstants.ElementNameAttributeName, i.Name);
                    writer.WriteAttributeString(XmlSerializationConstants.ElementTypeAttributeName, XmlSerializationConstants.GetSimpleXmlFieldType(sft & ~SerializedFieldType.Array));
                    writer.WriteAttributeString(XmlSerializationConstants.ElementIsArrayAttributeName, "true");
                    for (int I = 0; I < len; I++)
                    {
                        AddArrayElement(a.GetValue(I) , writer);
                    }
                    writer.WriteEndElement();
                } else if (sft == SerializedFieldType.Object) {
                    writer.WriteStartElement(XmlSerializationConstants.SerializedElementTagName);
                    writer.WriteAttributeString(XmlSerializationConstants.ElementNameAttributeName, i.Name);
                    writer.WriteAttributeString(XmlSerializationConstants.ElementTypeAttributeName, XmlSerializationConstants.GetSimpleXmlFieldType(sft));
                    writer.WriteStartElement(XmlSerializationConstants.SerializedElementValueTagName);
                    AddRecord(i.RecordValue, writer);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                } else {
                    AddSimpleField(i , writer);
                }
            }
        }

        /// <inheritdoc />
        public void WriteNew(Stream stream, Record record)
        {
            ArgumentNullException.ThrowIfNull(record);
            try {
                writer = XmlWriter.Create(stream, new() { CloseOutput = false, ConformanceLevel = ConformanceLevel.Document, Indent = true, NewLineOnAttributes = false });
                writer.WriteStartDocument();
                writer.WriteStartElement(XmlSerializationConstants.StartDocumentTagName);
                AddRecord(record, writer);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            } finally {
                writer?.Dispose();
                writer = null;
            }
        }

        /// <summary>
        /// Disposes this <see cref="XmlRecordWriter"/> instance.
        /// </summary>
        public void Dispose()
        {
            writer?.Dispose();
            writer = null;
        }
    }
}