


using System.Collections.Generic;

namespace MP.Serialization
{
    /// <summary>
    /// Defines common utility methods that may be proven useful when working with serialization API's.
    /// </summary>
    public static class SerializationUtilities
    {
        /// <summary>
        /// Gets the corresponding .NET type of the specified serialized field type, returning it or when it is an array it's corresponding array type.
        /// </summary>
        /// <param name="sft">The serialized field type to obtain it's corresponding .NET type.</param>
        /// <returns>The .NET type, corresponding to the specified <see cref="SerializedFieldType"/> value.</returns>
        public static System.Type GetDotNetType(this SerializedFieldType sft)
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
                SerializedFieldType.Object => typeof(ISerializedClassReader),
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

        /// <summary>
        /// Gets a value whether the specified .NET type will be matching with the current serialized field type.
        /// </summary>
        /// <param name="sft">The serialized field type to test against the .NET type for match.</param>
        /// <param name="type">The actual .NET type of the field you want to perform the match.</param>
        /// <returns><see langword="true"/> when <paramref name="type"/> can match <paramref name="sft"/>; otherwise <see langword="false" />.</returns>
        public static System.Boolean WillMostLikelyMatchWith(this SerializedFieldType sft, System.Type type)
        {
            if (sft.HasFlag(SerializedFieldType.Array))
            {
                sft &= ~SerializedFieldType.Array;
            }
            else if (sft.HasFlag(SerializedFieldType.StrictStringDictionary))
            {
                sft &= ~SerializedFieldType.StrictStringDictionary;
            }
            return sft switch
            {
                SerializedFieldType.Object => true,
                SerializedFieldType.Boolean => type == typeof(System.Boolean),
                SerializedFieldType.String => type == typeof(System.String),
                SerializedFieldType.Single => type == typeof(System.Single)
                                        || type == typeof(System.Double),
                SerializedFieldType.Double => type == typeof(System.Double),
                SerializedFieldType.Byte => type == typeof(System.Byte)
                                        || type == typeof(System.SByte)
                                        || type == typeof(System.Int16)
                                        || type == typeof(System.UInt16)
                                        || type == typeof(System.Int32)
                                        || type == typeof(System.UInt32)
                                        || type == typeof(System.Int64)
                                        || type == typeof(System.UInt64),
                SerializedFieldType.SByte => type == typeof(System.SByte)
                                        || type == typeof(System.Int16)
                                        || type == typeof(System.Int32)
                                        || type == typeof(System.Int64),
                SerializedFieldType.Int16 => type == typeof(System.Int16)
                                        || type == typeof(System.Int32)
                                        || type == typeof(System.Int64),
                SerializedFieldType.UInt16 => type == typeof(System.UInt16)
                                        || type == typeof(System.UInt32)
                                        || type == typeof(System.UInt64),
                SerializedFieldType.Int32 => type == typeof(System.Int32)
                                        || type == typeof(System.Int64),
                SerializedFieldType.UInt32 => type == typeof(System.UInt32)
                                        || type == typeof(System.UInt64),
                SerializedFieldType.Int64 => type == typeof(System.Int64),
                SerializedFieldType.UInt64 => type == typeof(System.UInt64),
                _ => false,
            };
        }
    }
}