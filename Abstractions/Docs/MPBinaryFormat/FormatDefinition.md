## The Music Player Binary Playlist Format

This document describes the MP BPL , a format designed for flexibility and it is
the storage type for storing physical playlists on disk.

This format document describes the exact structure of this format.

### The primary reason of creating the format

The format was created so that it provides meaningful playlist data to the Music Player implementation.

Unfortunately, many similar formats fail to accomplish on the side of useful data and usually have to load 
tons of other data (such as audio tags) at the moment of loading the playlist itself.

M3U , for example , is a playlist text format that only stores the raw paths of the files , providing nothing else useful 
for a factual music player.

BPL aims to cover this by defining a format that is flexible , and by it's .NET implementation , thread-safe.

However , note that the BPL implements the container format only; The implementations of how to store strings for example , 
is out of scope of this document.

This allows for complete flexibility of how the data are read and written to such files , 
because someone might always want to implement a new feature , so he can do this safely without breaking older format versions.

The format is meant to be always easily 'extensible', while trying to be backwards compatible.

### An hierarchical structure of the Binary Playlist Format

~~~mermaid
flowchart TD

A[Binary Playlist Header]
A --> B
A --> C
A --> D
B[Blob 1]
B --> E
C[Blob 2]
C --> F
D[Blob 3]
D --> G
E[Blob 1 Header]
F[Blob 2 Header]
G[Blob 3 Header]
~~~

The format is written in that way so each of the data 
can be easily be split into byte arrays , called `Blobs`.

A blob is itself a resource that contains data for the playlist.

Each blob must be independent of each other; 
one blob *MUST* be responsible for one job.

For example , in other blob you must store raw data and in anoher blob you must store strings.

By not confusing one blob with another it allows you to safely work on one blob , while ensures that all
blobs are different and seperated inside the format itself.

Blobs are of fixed length; the length of each blob *MUST* be known at read-time.

All the above allow the following statements:

-> If one blob breaks due to data corruption , the BPL reader *MAY* be able to recover if the byte length
mentioned in it's header does still have the data after it.

If the number of the bytes after the header are more or less than the indicated length , then the entire format is broken.

-> Allowing each blob to be independent allows that the format can be easily extended to include
even more blobs. A reader that does not know a new blob added by an external source for example,
it can and *SHOULD* skip reading it. However , there *are* some exceptions to this rule, described later.

-> Version indepentability. You can improve the read performance of individual blobs by introducing versioning 
inside the blobs. Each blob header also contains a special versioning field that you can take advantage of 
to update the blob reading and writing code while ensuring backwards compatibility.

-> Faster issue resolving. By knowing which blob causes incorrect reads to be performed , 
you can use this information to pinpoint the issue and resolve it quickly, and efficiently. 

-> Finally, because the music industry standards are evolving , this extensibility allows to 
efficiently consume these demands and update the playlist format by just managing it's
blobs, without necessarily breaking older behavior.

And all of these, into binary, which indeed is difficult to work with, especially if not including such extensibility.

### The Binary Playlist Format Header

A BPL format always starts with the following common header.
This header only exists to express the format itself and defines the starting point 
where a reader can then read the contained blobs.

In other words, it just instructs the reader 'how' to read the contained blobs.

Header (All fields are encoded as little-endian):

~~~mermaid
flowchart LR

A[BPL Header] --> B
B[Version field - signed short - 2 bytes]  --> C
C[Number of contained blobs - unsigned short - 2 bytes] --> D
D[Maximum Blob Type - byte - 1 byte] 
~~~

BPL Header:

The first three header bytes identify the format itself,
just a header to ensure that a reader reads the correct format.

The header is consisting of three ASCII characters,
specifically being laid out as follows:
~~~
'B' , 'P' and 'L'.
~~~

Version field:

A signed short number indicating the format version itself - currently this is set to `1`.

Note that when this field is incremented and the reader does not know how to read that version, it **MUST NOT** continue reading and **MUST** return an error.

Number of blobs field:

An unsigned short number indicating the number of the blobs expected to be read after this header.

Note that is perfectly valid to say 0 blobs , 
but it is a bit useless in terms that you cannot save anything else in the format itself.

Note that when you add or remove blobs from the format , you must update this field too.

The format currently has predicted for 65535 blobs , however I do consider them a lot , especially for such a format.

This field value is dynamic, it does not have any common value because it is dependent on the number of the blobs contained into a stream that implements this format.

Maximum Blob Type field:

A byte indicating the last blob type that *may* be found in the format blobs.

Blob types will be described in the blob header doc , but you should know from now
that the value is completely optional , but when it supported by the reader , 
it adds an extra layer of security to the format itself.

Also needs to be updated like the Number of Blobs field as well , especially when you add new blobs past to the currently defined type.

### The Binary Playlist Blob Header

Before each blob begins , it contains a header for identifying itself 'as a blob'.

Additionally , it contains metadata that a blob reader and writer can take advantage of in order to identify it and how the reading/writing should be handled.

Header (All fields are encoded as little-endian):

~~~mermaid
flowchart LR

A[Blob Flags - Enumeration - 1 byte] --> B
B[Blob Type - Enumeration - 1 byte] --> C
C[Blob Version - unsigned short - 2 bytes] --> D
D[Blob Elements Count - unsigned integer - 4 bytes] --> E
E[Blob Length in bytes - unsigned long integer - 8 bytes]
~~~

Blob flags field:

This field identifies additional behavior to the blob reader or even the binary playlist reader if needed to.

A blob writer and reader may also define their own and private flags here too.

However , there is also a set of common flags applied to all the defined blobs in the format , 
whether they are using that field or not.

You can see the implicitly-defined flags into this file: [Blob Flags](../../MP/BinaryPlaylist/BlobFlags.cs)

:notebook: Special considerations for the `RelocationBlob` flag:

The `RelocationBlob` flag , when specified , it means that both the blob reader and the writer implement
a logic where the blob contains offset data for fast seeking into the rest of the blob.

When this flag is defined the Binary Playlist Reader is ought to:

-> Aknownledge that the blob reader may perform a lot of seek requests into it's data and thus should try to improve performance for such cases on the cost of reliability.

-> Prioritize the read calls happened by this blob than others that may be requesting data access at the same time.

Note that the .NET implementation does not provide support for both of the above statements , but an implementing reader of the format should implement appropriate logic for the above statements.

Blob types field:

This field identifies the blob's type, that is, what kind of data this binary blob holds.

There is a set of [common values](../../MP/BinaryPlaylist/BlobTypes.cs) that may be found in such a playlist file, 
but custom blob types can be also specified after the type constant `MAXEMBEDDEDBLOBVAL`.

:notebook: About defining custom blob types

Custom blob types designate blobs that this format version has 'not predicted' or that they are 'extensions of the base format'.

As such, these custom blob types may and could break between BPL major revisions.

Thus, all such custom blobs should be additionally flagged with the `Custom` flag,
indicating to an implementing BPL reader that this blob is subject to break at any versioning change,
and thus, should avoid to provide it to the user if deemed necessary.

Blob version field:

A user-defined value that indicates the version of the blob data themselves.

Typically it is used by the blob implementer to extend the blob's behavior or to improve performance.

This is typically set to 1 and is incremented between every change.

Blob elements count field:

Another user-defined value that indicates how many user-defined elements are contained in the blob data.

For example, you use this when you define a blob that is essentially a collection of same things,
like integer arrays.

Note that you do *ANYTHING* you want with this field; you can even leave it to zero if you deem that you cannot use it.

Blob length in bytes field:

This is set during write-time by the implementing BPL writer and it designates
the blob's absolute length in bytes.

This field is a 64-bit integer, meaning that this can expand to very large values,
even more larger than 4 GB.

However, typically such a large blob does not exist for any reason.

Even the raw array blob should not be far larger than 2 GB in typical cases.
