
using System;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;
using MP.IO;

namespace MP.Serialization.Xml
{
    /// <summary>
    /// Defines a <see cref="IRecordReader"/> for decoding XML <see cref="Record"/>s.
    /// </summary>
    public sealed class XmlRecordReader : IRecordReader
    {
        private XmlReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRecordReader"/> class.
        /// </summary>
        public XmlRecordReader() {
            reader = null;
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
            try
            {
                if (sft == SerializedFieldType.Object)
                {
                    System.String data = reader.ReadInnerXml();
                    return new UninitializedRecord(() => {
                        XmlReader xr = XmlReader.Create(new StringReader($"<root>{data}</root>"), new() {
                            CloseInput = true,
                            IgnoreComments = true,
                            IgnoreProcessingInstructions = true,
                            IgnoreWhitespace = true,
                            ConformanceLevel = ConformanceLevel.Document
                        });
                        try {
                            return Decode(xr);
                        } finally {
                            xr.Dispose();
                        }
                    });
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
                    SerializedFieldType.UInt32 => System.UInt32.Parse(s, NumberStyles.Integer),
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

        private static System.Object BuildListObject(List<System.Object> b, Type needed)
        {
            Type t = typeof(List<>).MakeGenericType(needed);
            var mi = t.GetMethod("Add");
            Object o = Activator.CreateInstance(t, b.Count);
            foreach (Object eo in b) { mi.Invoke(o, new object[] { eo }); }
            return o;
        }

        private static Record Decode(XmlReader reader)
        {
            Record.Builder builder = new();

            while (MoveToNextElementNamed(reader, XmlSerializationConstants.SerializedElementTagName))
            {
                if (reader.IsEmptyElement) {
                    throw new SerializationException("Element data are empty.");
                }

                System.String name = reader.GetAttribute(XmlSerializationConstants.ElementNameAttributeName) ??
                    throw new SerializationException($"Cannot find the attribute named '{XmlSerializationConstants.ElementNameAttributeName}' in the element definition.");
                SerializedFieldType sft = XmlSerializationConstants.GetSimpleFieldType(reader.GetAttribute(XmlSerializationConstants.ElementTypeAttributeName));
                if (reader.GetAttribute(XmlSerializationConstants.ElementIsArrayAttributeName) == "true") {
                    sft |= SerializedFieldType.Array;
                } else if (reader.GetAttribute(XmlSerializationConstants.ElementIsStringDictionaryAttributeName) == "true") {
                    sft |= SerializedFieldType.StrictStringDictionary;
                } else if (reader.GetAttribute(XmlSerializationConstants.ElementIsListAttributeName) == "true") {
                    sft |= SerializedFieldType.List;
                }

                XmlReader xr = null;

                try {
                    SerializedFieldType ct;
                    xr = reader.ReadSubtree();

                    if (SerializationManagerUtilities.IsArrayAndExtractType(sft, out ct))
                    {
                        List<System.Object> list = new(10);
                        if (MoveToNextElementNamed(xr, XmlSerializationConstants.SerializedElementValueTagName))
                        {
                            list.Add(ReadSimpleElementValue(xr, ct));
                        }
                        while (xr.MoveToContent() == XmlNodeType.Element && xr.Name == XmlSerializationConstants.SerializedElementValueTagName)
                        {
                            list.Add(ReadSimpleElementValue(xr, ct));
                        }
                        Array a = Array.CreateInstance(XmlSerializationConstants.GetDotNetType(ct), list.Count);
                        for (int I = 0; I < list.Count; I++)
                        {
                            a.SetValue(list[I], I);
                        }
                        list.Clear();
                        builder.Add(new(name, a, sft));
                    } else if (SerializationManagerUtilities.IsListAndExtractType(sft, out ct)) {
                        List<System.Object> list = new(10);
                        if (MoveToNextElementNamed(xr, XmlSerializationConstants.SerializedElementValueTagName))
                        {
                            list.Add(ReadSimpleElementValue(xr, ct));
                        }
                        while (xr.MoveToContent() == XmlNodeType.Element && xr.Name == XmlSerializationConstants.SerializedElementValueTagName)
                        {
                            list.Add(ReadSimpleElementValue(xr, ct));
                        }
                        builder.Add(new(name, BuildListObject(list, XmlSerializationConstants.GetDotNetType(ct)), sft));
                    } else if (SerializationManagerUtilities.IsStringDictAndExtractType(sft, out ct)) {
                        // Strict string dictionaries will be encoded with the following way:
                        // <value name="AName" type="string" isstringdict="true"> 
                        //      <entry name="AEntryName">
                        //             <!-- Entry's value in here. -->
                        //      </entry>
                        // </value>
                        List<Record> rcd = new();
                        string entry_name;
                        if (MoveToNextElementNamed(xr, "entry"))
                        {
                            entry_name = xr.GetAttribute("name");
                            if (entry_name is null) {
                                throw new SerializationException("Name attribute was not defined for a dictionary entry.");
                            }
                            rcd.Add(new(new[] {
                                new SerializedField("key", entry_name),
                                new SerializedField("value", ReadSimpleElementValue(xr, ct)) 
                            }));
                        }
                        while (xr.MoveToContent() == XmlNodeType.Element && xr.Name == "entry")
                        {
                            entry_name = xr.GetAttribute("name");
                            if (entry_name is null)
                            {
                                throw new SerializationException("Name attribute was not defined for a dictionary entry.");
                            }
                            rcd.Add(new(new[] {
                                new SerializedField("key", entry_name),
                                new SerializedField("value", ReadSimpleElementValue(xr, ct))
                            }));
                        }
                        builder.Add(new(name, rcd.ToArray(), sft));
                    } else if (MoveToNextElementNamed(xr, XmlSerializationConstants.SerializedElementValueTagName)) {
                        builder.Add(new(name, ReadSimpleElementValue(xr, sft), sft));
                    } else {
                        throw new SerializationException($"Can't find the {XmlSerializationConstants.SerializedElementValueTagName} element in the XML element.");
                    }
                } finally {
                    xr?.Dispose();
                }

            }

            return builder.Build();
        }

        /// <summary>
        /// Gets the decoded root XML object as a <see cref="Record"/>.
        /// </summary>
        public Record Payload => Decode(reader);

        /// <inheritdoc />
        public void InitializeForNewPayload(DataStream stream)
        {
            reader?.Dispose();
            try {
                reader = XmlReader.Create(new DataStreamToStreamAdapter(stream), new() { CloseInput = false, IgnoreComments = true, IgnoreProcessingInstructions = true, ConformanceLevel = ConformanceLevel.Document, IgnoreWhitespace = true });
                if (MoveToNextElementNamed(reader, XmlSerializationConstants.StartDocumentTagName) == false)
                {
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
        public void EndPayloadDecoding()
        {
            reader?.Dispose();
            reader = null;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            reader?.Dispose();
            reader = null;
        }
    }
}