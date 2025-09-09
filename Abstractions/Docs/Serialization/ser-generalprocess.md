

## Understanding how the API works

The serialization API contains different level and kind of services, all embedded into a single class.

How that works and how you can use it for your needs is described here.


~~~mermaid
flowchart TD

OBJ[.NET object]

DRIVE[(App Storage, or saved data)]

REFLECTSERVICES[" .NET Reflection Services "]

CONSTRAINTS[" Constraints Management "]

OBJ <--> SERMANAGER

SERMANAGER <-->|Reads from| SERCLASSREADER

SERMANAGER <-->|Writes to| SERCLASSWRITER

DRIVE -.-> SERCLASSREADER

SERCLASSWRITER -.-> DRIVE

subgraph Serialization API

SERMANAGER <--> REFLECTSERVICES

SERMANAGER <--> CONSTRAINTS

    subgraph Public API Surface
        SERMANAGER["` *SerializationManager{T}* class `"]
    end

    subgraph Data Access Abstraction Layer 

        SERCLASSREADER["` *ISerializedClassReader* interface `"]

        SERCLASSWRITER["` *ISerializedClassWriter* interface `"]

    end

end

~~~

Generally, the entire serialization process is not described by the user by any means;
The API instead specifies how the serialization process will be performed.

The user has to only worry only about two things:

-> which Serialized Class Reader and Writer will use

-> and, to create the data abstraction. (The .NET class describing the data to de/serialize) 

Due to the nature of the API, you can also use it to transcode classes 
(that is, deserialize from one format and serialize to another).

Additionally it provides a constraint subsystem for creating de/serialized instances only when certain checks pass, and during at the time specified by the user (With time I mean whether during deserialization-time or serialization-time).

The kind of checks performed are up to the user to be defined, and the user can also very 
easily define it's own one without any registration or boilerplate code - just deriving from the 
[`SerializationConstraintAttribute`](../../MP/Serialization/SerializationConstraintAttribute.cs) class.

> [!NOTE]
The constraint subsystem does not check other embedded class fields 
nor the array itself and/or it's elements due to their complexity.
However, constraints on the fields of the embedded class instance are applied as usual.

Finally, all the data retrieval and writing is done through `System.IO.Stream` 
instances - allowing full flexibility to how you wish to store the data as.

