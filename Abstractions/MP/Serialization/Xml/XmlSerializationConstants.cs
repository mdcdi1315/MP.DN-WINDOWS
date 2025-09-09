
namespace MP.Serialization.Xml
{
    internal static class XmlSerializationConstants
    {
        public const System.String StartDocumentTagName = "mdcdi1315_serialization";
        public const System.String SerializedElementTagName = "element";
        public const System.String SerializedElementValueTagName = "value";

        public const System.String ElementNameAttributeName = "name";
        public const System.String ElementTypeAttributeName = "type";
        public const System.String ElementIsArrayAttributeName = "isarray";
        public const System.String ElementIsStringDictionaryAttributeName = "isstringdict";


        public static SerializedFieldType GetSimpleFieldType(System.String attributevalue)
        {
            if (attributevalue is null)
            {
                throw new SerializationException("Value of the attribute cannot be null.");
            }
            return attributevalue.ToLowerInvariant() switch
            {
                "object" => SerializedFieldType.Object,
                "string" => SerializedFieldType.String,
                "bool" => SerializedFieldType.Boolean,
                "byte" => SerializedFieldType.Byte,
                "sbyte" => SerializedFieldType.SByte,
                "i16" => SerializedFieldType.Int16,
                "u16" => SerializedFieldType.UInt16,
                "i32" => SerializedFieldType.Int32,
                "u32" => SerializedFieldType.UInt32,
                "i64" => SerializedFieldType.Int64,
                "u64" => SerializedFieldType.UInt64,
                "float" => SerializedFieldType.Single,
                "double" => SerializedFieldType.Double,
                _ => throw new SerializationException($"Cannot realize type {attributevalue} into a SerializedFieldType.")
            };
        }

        public static System.String GetSimpleXmlFieldType(SerializedFieldType sft) => sft switch
        {
            SerializedFieldType.Object => "object",
            SerializedFieldType.Boolean => "bool",
            SerializedFieldType.String => "string",
            SerializedFieldType.Byte => "byte",
            SerializedFieldType.SByte => "sbyte",
            SerializedFieldType.Int16 => "i16",
            SerializedFieldType.UInt16 => "u16",
            SerializedFieldType.Int32 => "i32",
            SerializedFieldType.UInt32 => "u32",
            SerializedFieldType.Int64 => "i64",
            SerializedFieldType.UInt64 => "u64",
            SerializedFieldType.Single => "float",
            SerializedFieldType.Double => "double",
            _ => throw new SerializationException($"This serialized field type is not supported: {sft}")
        };

    }
}