

## Creating a serializable class suitable for the Serialization API

Serialization classes are simple classes with public non-*`readonly`* fields 
that allow to define data to be serialized.

For a class to be valid for serialization, one must make the class implement the `ISerializableClass` interface.

Do not worry, this is an empty placeholder interface so 
that you can access staticly the class object to de/serialize 
from the serialization manager. 
(Thus you do not need weird typecasts with this way, 
and this validates that the object that you will pass it 
will always be a type that you will at least know what it is.)

Along with that, you can name the fields differently of what their .NET field names are.
You do that by using the `FieldNameAttribute` class.

You may also apply constraints to simple data types.
Constraints may be the ones provided by the API or custom ones that YOU create, and these
do not need any additional registration process since these are all treated with the same unified way.

Then, you can use the serialization manager class and an object to de/serialize your class instance.

That was it!

### What is serializable with the API?

You can serialize all the types listed below:

| Type  | Serialized Field Type correspondence | 
|-------|--------------------------------------|
| `System.String` | [`SerializedFieldType.String`](../../MP/Serialization/SerializedFieldType.cs#L22)   |
| `System.Boolean` | [`SerializedFieldType.Boolean`](../../MP/Serialization/SerializedFieldType.cs#L18) |
| `System.Byte` | [`SerializedFieldType.Byte`](../../MP/Serialization/SerializedFieldType.cs#L26)       |
| `System.SByte` | [`SerializedFieldType.SByte`](../../MP/Serialization/SerializedFieldType.cs#L30)     |
| `System.Int16` | [`SerializedFieldType.Int16`](../../MP/Serialization/SerializedFieldType.cs#L34)     |
| `System.UInt16` | [`SerializedFieldType.UInt16`](../../MP/Serialization/SerializedFieldType.cs#L38)   |
| `System.Int32` | [`SerializedFieldType.Int32`](../../MP/Serialization/SerializedFieldType.cs#L42)     |
| `System.UInt32` | [`SerializedFieldType.UInt32`](../../MP/Serialization/SerializedFieldType.cs#L46)   |
| `System.Int64` | [`SerializedFieldType.Int64`](../../MP/Serialization/SerializedFieldType.cs#L50)     |
| `System.UInt64` | [`SerializedFieldType.UInt64`](../../MP/Serialization/SerializedFieldType.cs#L54)   |
| `System.Single` | [`SerializedFieldType.Single`](../../MP/Serialization/SerializedFieldType.cs#L58)   |
| `System.Double` | [`SerializedFieldType.Double`](../../MP/Serialization/SerializedFieldType.cs#L62)   |
| Any object implementing [`ISerializableClass`](../../MP/Serialization/ISerializableClass.cs) | [`SerializedFieldType.Object`](../../MP/Serialization/SerializedFieldType.cs#L14)  |
| Any enumeration type deriving from `System.Enum` | This is inherently supported by the serialization manager. The enumeration value is converted to a [`SerializedFieldType.String`](../../MP/Serialization/SerializedFieldType.cs#L22) representing the constant name. Automatically is converted back to the original value during deserialization of the field.  |
| Array of any of the above types | Flag: [`SerializedFieldType.Array`](../../MP/Serialization/SerializedFieldType.cs#L66) |

> [!NOTE]
It is not an error to serialize a `null` object or array, however, 
it will trigger a `SerializationException` to be thrown at deserialization.

> [!NOTE]
For string cases, you can assert the null case in serialization-time by using the 
`StringMustNotBeNullOrEmptyAttribute` constraint.

> [!NOTE]
You cannot apply a constraint on enumeration types because they have a fixed set of values and this logic is inherently provided by the Serialization Manager. Note that the enumerations are treated as string values containing the field name of the enumeration case.
