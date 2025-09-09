
## The Serialization API

This is a serialization API aimed at making a class expressible 
as any file format and in the same time being loaded as a .NET object.

This implementation takes that and pushes further this concept by adding:

-> Abstractions around the file format barriers - that is, you can save the serialized data in any format you wish, if you just implement correctly both the `ISerializedClassReader` and `ISerializedClassWriter` interfaces.

-> An extensible constraint logic to serialize objects only when their fields do have correct values, based on the constraints applied.

-> Perform concurrent read and write operations on the same object type at the same instance, at the same time.

-> The ability to deserialize from one format and serialize into another

-> And all these, into a single instance of a `SerializationManager` class.

This API will be later used for all those files that are config files 
(i.e. the ones provided for yt-dlp functionality) 
in a try to generalize it and avoid the issues aroused due to the fact 
that you had to write a new JSON parser for each one new file.

Example demonstrating the usage of the API (Assumes that a validly filled in JSON file already exists):

~~~C#

using MP.Serialization;
using MP.Serialization.Json;

public class SerClass : ISerializableClass
{
    [FieldName("type")]
    public System.String Type;

    [FieldName("default_value")]
    public System.Int64 Value;

    [FieldName("packet")]
    public Packet Packet;

    [FieldName("string_array")]
    public System.String[] Array;
}

public class Packet : ISerializableClass
{
    [FieldName("internal")]
    public byte Internal;

    [FieldName("version")]
    public byte Version;

    [FieldName("sig")]
    public String Signature;

    [FieldName("code")]
    public int Code;
}

var stream = new System.IO.FileStream("/file.json" , System.IO.FileMode.Open);

var sermgr = new SerializationManager<SerClass>();
sermgr.Reader = new JsonSerializedClassReader();
sermgr.Writer = new JsonSerializedClassWriter();

SerClass sc = new();
sermgr.Deserialize(sc , stream);
System.Console.WriteLine(sc.Type);
System.Console.WriteLine(sc.Value);
System.Console.WriteLine(sc.Packet.Signature);
stream.Dispose();

stream = new System.IO.FileStream("/file2.json" , System.IO.FileMode.Create);

sermgr.Serialize(sc , stream);

stream.Dispose();

sermgr.Dispose();
~~~
