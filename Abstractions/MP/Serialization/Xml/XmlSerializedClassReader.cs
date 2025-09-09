

using System;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;

namespace MP.Serialization.Xml
{
    /// <summary>
    /// Defines a serialized class reader written with XML.
    /// </summary>
    public sealed class XmlSerializedClassReader : ISerializedClassReader
    {
        private System.Object dec;
        private XmlReader reader;
        private SerializedFieldInformation sfi;

        private sealed class RecursiveXmlSerializedClassReader : ISerializedClassReader
        {
            private System.Object dec;
            private XmlReader reader;
            private SerializedFieldInformation sfi;

            public RecursiveXmlSerializedClassReader(System.String subtree)
            {
                reader = XmlReader.Create(new StringReader($"<root>{subtree}</root>") , new() {
                    CloseInput = true,
                    IgnoreComments = true,
                    IgnoreProcessingInstructions = true,
                    IgnoreWhitespace = true,
                    ConformanceLevel = ConformanceLevel.Document
                });
                dec = null;
                sfi = null;
            }

            public object DecodeField() => dec ??= DecodeFieldInternal(reader, GetFieldInformation());

            public void Dispose()
            {
                dec = null;
                sfi = null;
                reader?.Dispose();
                reader = null;
            }

            public SerializedFieldInformation GetFieldInformation() => sfi ??= DecodeFieldInformationInternal(reader);

            public void Initialize(Stream stream) => throw new NotSupportedException("This reader has been already initialized");

            public bool MoveNext()
            {
                dec = null;
                sfi = null;
                return MoveToNextElementNamed(reader, XmlSerializationConstants.SerializedElementTagName);
            }
        }

        /// <inheritdoc/> 
        public object DecodeField() => dec ??= DecodeFieldInternal(reader, GetFieldInformation());

        /// <inheritdoc/> 
        public SerializedFieldInformation GetFieldInformation() => sfi ??= DecodeFieldInformationInternal(reader);

        /// <inheritdoc/> 
        public void Initialize(Stream stream)
        {
            reader?.Dispose();
            dec = null;
            sfi = null;
            try {
                reader = XmlReader.Create(stream, new() { CloseInput = false, IgnoreComments = true, IgnoreProcessingInstructions = true, ConformanceLevel = ConformanceLevel.Document , IgnoreWhitespace = true });
                if (MoveToNextElementNamed(reader, XmlSerializationConstants.StartDocumentTagName) == false) {
                    throw new SerializationException($"The starting document tag was not {XmlSerializationConstants.StartDocumentTagName} .");
                }
                if (reader.EOF) {
                    throw new SerializationException("XML stream ended unexpectedly.");
                }
            } catch (Exception except) {
                reader?.Dispose();
                reader = null;
                switch (except)
                {
                    case XmlException:
                        throw new SerializationException("Cannot create the XML reader.", except);
                    default:
                        throw;
                }
            }
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            dec = null;
            sfi = null;
            return MoveToNextElementNamed(reader, XmlSerializationConstants.SerializedElementTagName);
        }

        private static System.Boolean MoveToNextElementNamed(XmlReader reader, System.String name)
        {
            System.Boolean found = false;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == name)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        private static System.Object ReadSimpleElementValue(XmlReader reader, SerializedFieldType sft)
        {
            try {
                if (sft == SerializedFieldType.Object)
                {
                    return new RecursiveXmlSerializedClassReader(reader.ReadInnerXml());
                }
                System.String s = reader.ReadElementContentAsString();
                return sft switch
                {
                    SerializedFieldType.String => s,
                    SerializedFieldType.Boolean => System.Boolean.Parse(s),
                    SerializedFieldType.Byte => System.Byte.Parse(s, NumberStyles.Integer),
                    SerializedFieldType.SByte => System.SByte.Parse(s, NumberStyles.Integer),
                    SerializedFieldType.Int16 => System.Int16.Parse(s, NumberStyles.Integer),
                    SerializedFieldType.UInt16 => System.UInt16.Parse(s, NumberStyles.Integer),
                    SerializedFieldType.Int32 => System.Int32.Parse(s, NumberStyles.Integer),
                    SerializedFieldType.UInt32 => System.UInt32.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite),
                    SerializedFieldType.Int64 => System.Int64.Parse(s, NumberStyles.Integer),
                    SerializedFieldType.UInt64 => System.UInt64.Parse(s, NumberStyles.Integer),
                    SerializedFieldType.Single => System.Single.Parse(s, NumberStyles.Float),
                    SerializedFieldType.Double => System.Double.Parse(s, NumberStyles.Float),
                    _ => null,
                };
            } catch (FormatException fe) {
                throw new SerializationException("Cannot deserialize the specified value.", fe);
            }
        }

        private static System.Object DecodeFieldInternal(XmlReader reader, SerializedFieldInformation sfi)
        {
            XmlReader xr = reader.ReadSubtree();
            try
            {
                if (sfi.Type.HasFlag(SerializedFieldType.Array))
                {
                    var ct = sfi.Type & ~SerializedFieldType.Array;
                    List<System.Object> list = new(10);
                    if (MoveToNextElementNamed(xr, XmlSerializationConstants.SerializedElementValueTagName))
                    {
                        list.Add(ReadSimpleElementValue(xr, ct));
                    }
                    while (xr.MoveToContent() == XmlNodeType.Element && xr.Name == XmlSerializationConstants.SerializedElementValueTagName)
                    {
                        list.Add(ReadSimpleElementValue(xr, ct));
                    }
                    Array a = Array.CreateInstance(ct.GetDotNetType(), list.Count);
                    for (int I = 0; I < list.Count; I++)
                    {
                        a.SetValue(list[I], I);
                    }
                    list.Clear();
                    return a;
                }
                else if (sfi.Type.HasFlag(SerializedFieldType.StrictStringDictionary))
                {
                    throw new SerializationException("Currently not supported");
                }
                else if (MoveToNextElementNamed(xr, XmlSerializationConstants.SerializedElementValueTagName))
                {
                    return ReadSimpleElementValue(xr, sfi.Type);
                }
                else
                {
                    throw new SerializationException($"Can't find the {XmlSerializationConstants.SerializedElementValueTagName} element in the XML element.");
                }
            }
            finally
            {
                xr?.Dispose();
            }
        }

        private static SerializedFieldInformation DecodeFieldInformationInternal(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                throw new SerializationException("Element data are empty.");
            }
            System.String name = reader.GetAttribute(XmlSerializationConstants.ElementNameAttributeName) ?? throw new SerializationException($"Cannot find the attribute named '{XmlSerializationConstants.ElementNameAttributeName}' in the element definition.");
            SerializedFieldType sft = XmlSerializationConstants.GetSimpleFieldType(reader.GetAttribute(XmlSerializationConstants.ElementTypeAttributeName));
            if (reader.GetAttribute(XmlSerializationConstants.ElementIsArrayAttributeName) == "true")
            {
                sft |= SerializedFieldType.Array;
            }
            else if (reader.GetAttribute(XmlSerializationConstants.ElementIsStringDictionaryAttributeName) == "true")
            {
                sft |= SerializedFieldType.StrictStringDictionary;
            }
            return new(name, sft);
        }

        /// <summary>
        /// Disposes this XML serialized class reader instance.
        /// </summary>
        public void Dispose()
        {
            reader?.Dispose();
            reader = null;
        }
    }
}