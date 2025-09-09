
using System;
using System.IO;
using System.Xml;

namespace MP.Serialization.Xml
{
    /// <summary>
    /// Defines a serialized class writer that writes XML object graphs.
    /// </summary>
    public sealed class XmlSerializedClassWriter : ISerializedClassWriter
    {
        private XmlWriter writer;

        private void AddSimpleField(System.String name, SerializedFieldType sft, object value)
        {
            writer.WriteStartElement(XmlSerializationConstants.SerializedElementTagName);
            writer.WriteAttributeString(XmlSerializationConstants.ElementNameAttributeName, name);
            writer.WriteAttributeString(XmlSerializationConstants.ElementTypeAttributeName, XmlSerializationConstants.GetSimpleXmlFieldType(sft));
            writer.WriteStartElement(XmlSerializationConstants.SerializedElementValueTagName);

            switch (sft)
            {
                case SerializedFieldType.Boolean:
                    writer.WriteValue((System.Boolean)value);
                    break;
                case SerializedFieldType.Byte:
                    writer.WriteValue((System.Byte)value);
                    break;
                case SerializedFieldType.SByte:
                    writer.WriteValue((System.SByte)value);
                    break;
                case SerializedFieldType.Int16:
                    writer.WriteValue((System.Int16)value);
                    break;
                case SerializedFieldType.UInt16:
                    writer.WriteValue((System.UInt16)value);
                    break;
                case SerializedFieldType.Int32:
                    writer.WriteValue((System.Int32)value);
                    break;
                case SerializedFieldType.UInt32:
                    writer.WriteValue((System.UInt32)value);
                    break;
                case SerializedFieldType.Int64:
                    writer.WriteValue((System.Int64)value);
                    break;
                case SerializedFieldType.UInt64:
                    writer.WriteValue((System.Double)(System.UInt64)value);
                    break;
                case SerializedFieldType.Single:
                    writer.WriteValue((System.Single)value);
                    break;
                case SerializedFieldType.Double:
                    writer.WriteValue((System.Double)value);
                    break;
                case SerializedFieldType.String:
                    writer.WriteValue(value as string);
                    break;
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void AddArrayElement(System.Object value)
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
                case InMemorySerializedClassWriter icw:
                    AddObjectGraph(icw, true);
                    break;
            }

            writer.WriteEndElement();
        }

        private void AddObjectGraph(InMemorySerializedClassWriter cw, System.Boolean arraymode)
        {
            SerializedFieldType sft;
            foreach (var i in cw.Fields)
            {
                sft = i.Key.Type;
                if (sft.HasFlag(SerializedFieldType.Array)) {
                    Array a = i.Value as Array;
                    int len = a.GetLength(0);
                    writer.WriteStartElement(XmlSerializationConstants.SerializedElementTagName);
                    writer.WriteAttributeString(XmlSerializationConstants.ElementNameAttributeName, i.Key.Name);
                    writer.WriteAttributeString(XmlSerializationConstants.ElementTypeAttributeName, XmlSerializationConstants.GetSimpleXmlFieldType(sft & ~SerializedFieldType.Array));
                    writer.WriteAttributeString(XmlSerializationConstants.ElementIsArrayAttributeName, "true");
                    for (int I = 0; I < len; I++)
                    {
                        AddArrayElement(a.GetValue(I));
                    }
                    writer.WriteEndElement();
                } else if (sft == SerializedFieldType.Object) {
                    if (i.Value is not InMemorySerializedClassWriter cw2)
                    {
                        throw new SerializationException("The specified serialized class writer is not the writer type returned through the GetEmptyWriter method.");
                    }
                    if (arraymode == false)
                    {
                        writer.WriteStartElement(XmlSerializationConstants.SerializedElementTagName);
                        writer.WriteAttributeString(XmlSerializationConstants.ElementNameAttributeName, i.Key.Name);
                        writer.WriteAttributeString(XmlSerializationConstants.ElementTypeAttributeName, XmlSerializationConstants.GetSimpleXmlFieldType(sft));
                        writer.WriteStartElement(XmlSerializationConstants.SerializedElementValueTagName);
                    }
                    AddObjectGraph(cw2, arraymode);
                } else {
                    AddSimpleField(i.Key.Name, sft, i.Value);
                }
            }
            if (arraymode == false)
            {
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        /// <inheritdoc />
        public void AddField(SerializedFieldInformation sfi, object value)
        {
            SerializedFieldType sft = sfi.Type;
            if (sft.HasFlag(SerializedFieldType.Array))
            {
                Array a = value as Array;
                int len = a.GetLength(0);
                writer.WriteStartElement(XmlSerializationConstants.SerializedElementTagName);
                writer.WriteAttributeString(XmlSerializationConstants.ElementNameAttributeName, sfi.Name);
                writer.WriteAttributeString(XmlSerializationConstants.ElementTypeAttributeName, XmlSerializationConstants.GetSimpleXmlFieldType(sft & ~SerializedFieldType.Array));
                writer.WriteAttributeString(XmlSerializationConstants.ElementIsArrayAttributeName, "true");
                for (int I = 0; I < len; I++)
                {
                    AddArrayElement(a.GetValue(I));
                }
                writer.WriteEndElement();
            }
            else if (sft == SerializedFieldType.Object)
            {
                if (value is not InMemorySerializedClassWriter cw2)
                {
                    throw new SerializationException("The specified serialized class writer is not the writer type returned through the GetEmptyWriter method.");
                }
                writer.WriteStartElement(XmlSerializationConstants.SerializedElementTagName);
                writer.WriteAttributeString(XmlSerializationConstants.ElementNameAttributeName, sfi.Name);
                writer.WriteAttributeString(XmlSerializationConstants.ElementTypeAttributeName, XmlSerializationConstants.GetSimpleXmlFieldType(sft));
                writer.WriteStartElement(XmlSerializationConstants.SerializedElementValueTagName);
                AddObjectGraph(cw2, false);
            }
            else
            {
                AddSimpleField(sfi.Name, sft, value);
            }
        }

        /// <inheritdoc />
        public void FinalizeWriteOp()
        {
            if (writer is not null)
            {
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Dispose();
            }
            writer = null;
        }

        /// <inheritdoc />
        public ISerializedClassWriter GetEmptyWriter() => new InMemorySerializedClassWriter();

        /// <inheritdoc />
        public void Initialize(Stream stream)
        {
            FinalizeWriteOp();
            writer = XmlWriter.Create(stream, new() { CloseOutput = false, ConformanceLevel = ConformanceLevel.Document, Indent = true, NewLineOnAttributes = false });
            writer.WriteStartDocument();
            writer.WriteStartElement(XmlSerializationConstants.StartDocumentTagName);
        }

        /// <summary>
        /// Disposes this XML serialized class writer.
        /// </summary>
        public void Dispose() => FinalizeWriteOp();
    }
}