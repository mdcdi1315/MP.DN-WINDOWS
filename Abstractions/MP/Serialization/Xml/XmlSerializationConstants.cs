
using System.Collections.Generic;

namespace MP.Serialization.Xml
{
    internal static class XmlSerializationConstants
    {
        public const System.String StartDocumentTagName = "mdcdi1315_serialization";
        public const System.String SerializedElementTagName = "element";
        public const System.String SerializedElementValueTagName = "value";

        public const System.String ElementNameAttributeName = "name";
        public const System.String ElementTypeAttributeName = "type";
        public const System.String ElementIsListAttributeName = "islist";
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

        /// <summary>
        /// Gets the corresponding .NET type of the specified serialized field type, returning it or when it is an array it's corresponding array type.
        /// </summary>
        /// <param name="sft">The serialized field type to obtain it's corresponding .NET type.</param>
        /// <returns>The .NET type, corresponding to the specified <see cref="SerializedFieldType"/> value.</returns>
        public static System.Type GetDotNetType(SerializedFieldType sft)
        {
            System.Boolean isarray, isstringdict;
            SerializedFieldType simple;
            simple = (isarray = sft.HasFlag(SerializedFieldType.Array)) ? sft & ~SerializedFieldType.Array : sft;
            simple = (isstringdict = sft.HasFlag(SerializedFieldType.StrictStringDictionary)) ? sft & ~SerializedFieldType.StrictStringDictionary : sft;
            System.Type build = simple switch
            {
                SerializedFieldType.Boolean => typeof(System.Boolean),
                SerializedFieldType.String => typeof(System.String),
                SerializedFieldType.Byte => typeof(System.Byte),
                SerializedFieldType.SByte => typeof(System.SByte),
                SerializedFieldType.Int16 => typeof(System.Int16),
                SerializedFieldType.UInt16 => typeof(System.UInt16),
                SerializedFieldType.Int32 => typeof(System.Int32),
                SerializedFieldType.UInt32 => typeof(System.UInt32),
                SerializedFieldType.Int64 => typeof(System.Int64),
                SerializedFieldType.UInt64 => typeof(System.UInt64),
                SerializedFieldType.Single => typeof(System.Single),
                SerializedFieldType.Double => typeof(System.Double),
                SerializedFieldType.Object => typeof(UninitializedRecord),
                _ => null,
            };
            if (isarray)
            {
                return build.MakeArrayType();
            }
            else if (isstringdict)
            {
                return typeof(IDictionary<,>).MakeGenericType(typeof(System.String), build);
            }
            else
            {
                return build;
            }
        }

    }
}