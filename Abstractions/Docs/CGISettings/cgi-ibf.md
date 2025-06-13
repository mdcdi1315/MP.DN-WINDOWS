

## CGI `IBF` Format definition

This document will define and describe the CGI's default settings data reader and writer,
which do write those into a format called **CGI** **I**nterchargeable **B**inary **F**ormat.

This document targets those who wish to discover and analyze the format, and why not, to port it into other programming environments.

### About the format

The format is a compact method for conveniently saving CGI settings into a more reliable data source, such as a file.

It additionally tries to completely describe a setting by only using as less bytes as possible - so that the final result is itself pretty much 'compressed'.

The format has now reached it's second version, after some revisions on it's internal data structure.

### CGI `IBF` Header

The Interchargeable Binary Format header is described by the members of the below table.

Each next line of the table defines the next member in the header, just like how C's structs are laid out.

| Field Name                                    |  Field Size in Bytes | Field description                                                                                                                                                                                            |
|----------------------------------------|------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Header                                         |              3                | A string containing the characters `CGI`                                                                                                                                                      |
| Reserved Field                            |               1               | This field is reserved for future-use. Always set to zero.                                                                                                                         |
| Version	                                       |               4               | A 32-bit unsigned integer indicating format version.                                                                                                                                   |
| Entries Version                           |               1               | A byte indicating the maximal expected entry header version to be read from this stream.                                                                     |
| Entries Mask                                |               1               | The finally expected CGI setting type to be read from stream. For data verification.                                                                               |
| Reserved Field                            |               2               | This field is reserved for future-use. Always set to zero.                                                                                                                         |
| String Encoding                           |               4               | A 32-bit signed integer that indicates a LCID of the string data encoder and decoder.                                                                            |
| # of App Name Padding Bytes      |               4               | A 32-bit signed integer that indicates the number of bytes to skip before reading the application name that has written this stream.  |
| Reserved Field                            |               4               | This field is reserved for future-use. Always set to zero.                                                                                                                         |
| # of App Name String Bytes        |                4               | A 32-bit signed integer indicating the length of the application name string, in literal bytes.                                                                    |
| Application Name Padding Bytes  |    Variable-length   | The padding bytes before the application name. Their length is defined by the `# of App Name Padding Bytes` field.                              |
| Application Name                          |    Variable-length   | The name application that wrote this data stream. Used to verify that you are opening the correct data stream.                                 |

You can see a definition of the header's fixed-length members that both the reader and writer are using in this [file](../../MP/CGISettings/CGIStructs.cs).

After this header, only the settings headers and their values do only exist.

Note that the format does not provide a way to store how many settings are present in the stream; because it is presumed that a conforming reader
can find the end of a file.

Additionally note that headers with different versioning can coexist into a CGI IBF data stream,
it is enough that the reader does know how to read all the requested versions. 

This is identified by the `	Entries Version` field of the data stream header, which identifies up to which version a 
reader needs to know so as to reliably read their information directly from the stream.



### The CGI `IBF` Entry Header - V1 Header

This is the Version 1 Header of a CGI setting entry into the `IBF`.

There is also a Version 2 Header, that is an extended V1 header but providing the ability to provide arrays as setting data values.

The header that is now described does ONLY know how to encode a setting with a single, discrete value.

| Field Name                                    |  Field Size in Bytes | Field description                                                                                                                                                                                            |
|----------------------------------------|------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Version                                        |                1              | Identifies the version of the header itself - must be set to 1.                                                                                                                     |
| Reserved Field                            |                1              | Reserved field - must be set to zero.                                                                                                                                                            |
| Reserved Field                            |                2              | Reserved field - must be set to zero.                                                                                                                                                            |
| Setting Type                                |                2              | CGI Settings type field - See `CGISettingType` enumeration for a list of valid values.                                                                                 |
| Setting Value length in bytes      |                8              | A 64-bit signed integer indicating the setting's value length in bytes.                                                                                                         |
| Setting Name length in bytes       |                4              | A 32-bit signed integer indicating the setting's name length in bytes.                                                                                                          | 
| Setting Name # of pad bytes       |                2              | A 16-bit unsigned integer indicating the number of pad bytes to skip before reading the setting's name.                                                 |
| Reserved Field                            |                4              | Reserved field - must be set to zero.                                                                                                                                                            |
| Pad Bytes before Setting Name  |   Variable-length    | The block of pad bytes before the actual setting name.                                                                                                                              |
| Setting Name                                |   Variable-length    | The setting's name.                                                                                                                                                                                         |
| Setting Value                               |   Variable-length    | The setting's value, expressed as an array of bytes. It's size depends on the `Setting Value length in bytes` field.                              |

This is the Version 1 header of a single entry of the `IBF` format. It has the ability to store a single setting value in the stream.

The fixed-size part of the header must be 24 bytes. All the variable-length members described do not comprise the actual header.

Any reserved field must be zeroes.



### The CGI `IBF` Entry Header - V2 Header

The second version of the header introduces the setting values to have more than one elements of the same CGI setting type.

The header stores the length of the array, and the length in bytes of the value field stores all the elements size as bytes, including
their first bytes which identify a length in bytes of a single element.

| Field Name                                    |  Field Size in Bytes | Field description                                                                                                                                                                                                                                                                     |
|----------------------------------------|------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Version                                        |                1              | Identifies the version of the header itself - must be set to 1.                                                                                                                                                                                             |
| Setting Flags                               |                1              | Identifies additional behavior that the setting has. Only the Array flag is defined , and in V1 this was reserved.                                                                                                            |
| Reserved Field                            |                2              | Reserved field - must be set to zero.                                                                                                                                                                                                                                    |
| Setting Type                                |                2              | CGI Settings type field - See `CGISettingType` enumeration for a list of valid values.                                                                                                                                                         |
| Setting Value length in bytes      |                8              | A 64-bit signed integer indicating the setting's value length in bytes.                                                                                                                                                                                 |
| Setting Name length in bytes       |                4              | A 32-bit signed integer indicating the setting's name length in bytes.                                                                                                                                                                                  | 
| Setting Name # of pad bytes       |                2              | A 16-bit unsigned integer indicating the number of pad bytes to skip before reading the setting's name.                                                                                                                         |
| Setting Array length                   |                2              | A 16-bit unsigned integer indicating the number of array elements stored in the setting's value. This is valid only when the Array flag is defined and in V1 this was reserved.       | 
| Reserved Field                            |                2              | Reserved field - must be set to zero.                                                                                                                                                                                                                                    |
| Pad Bytes before Setting Name  |   Variable-length    | The block of pad bytes before the actual setting name.                                                                                                                                                                                                      |
| Setting Name                                |   Variable-length    | The setting's name.                                                                                                                                                                                                                                                                 |

