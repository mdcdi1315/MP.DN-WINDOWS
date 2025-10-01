

## Understanding how the API works

The serialization API contains different level and kind of services, all embedded into a single class.

How that works and how you can use it for your needs is described here.


~~~mermaid
flowchart TD

OBJ[.NET object]

DRIVE[(App Storage, or saved data)]

REFLECTSERVICES[" .NET Reflection Services "]

CONSTRAINTS[" Constraints Management "]

TYPETRANSCODING[" Type transcoding services "]

OBJ <--> SERMANAGER

SERMANAGER <-->|Reads from| SERCLASSREADER

SERMANAGER <-->|Writes to| SERCLASSWRITER

DRIVE -.-> SERCLASSREADER

SERCLASSWRITER -.-> DRIVE

subgraph Serialization API

SERMANAGER <--> REFLECTSERVICES

SERMANAGER <--> CONSTRAINTS

SERMANAGER <--> TYPETRANSCODING

    subgraph Public API Surface
        SERMANAGER["` *SerializationManager{T}* class `"]
    end

    subgraph Data Access Abstraction Layer 

        SERCLASSREADER["` *IRecordReader* interface `"]

        SERCLASSWRITER["` *IRecordWriter* interface `"]

    end

end

~~~

Generally, the entire serialization process is not described by the user by any means;
The API instead specifies how the serialization process will be performed.

The user has to only worry only about two things:

-> which Record Reader and Writer will use

-> and, to create the data abstraction. (The .NET class describing the data to de/serialize) 

Due to the nature of the API, it can be used to transcode classes 
(that is, deserializing from one format and serializing to another).

Additionally it provides a constraint subsystem for creating de/serialized instances only when certain checks pass, and during at the time specified by the user (With time I mean whether during deserialization-time or serialization-time).

The kind of checks performed are up to the user to be defined, and the user can also very 
easily define it's own one without any registration or boilerplate code - just deriving from the 
[`SerializationConstraintAttribute`](../../MP/Serialization/SerializationConstraintAttribute.cs) class.

> [!NOTE]
The constraint subsystem does not check other embedded class fields due to their complexity. <br />
However, constraints on the fields of the embedded class instance are applied as usual. <br />
When testing a field for constraints and it's backing type is an array, the entire array object is instead passed to the constraint.

Finally, all the data retrieval and writing is done through `System.IO.Stream` 
instances - allowing full flexibility to how you wish to store the data as.

