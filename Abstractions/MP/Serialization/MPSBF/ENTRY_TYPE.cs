
using System;

namespace MP.Serialization.MPSBF
{
    [Flags]
    internal enum ENTRY_TYPE : System.UInt16
    {
        NULL,
        RECORD, // Another record. Starts with the header normally and contains additional entries.
        BOOL, // WARN: This is C's int type, which is 4 bytes.
        BOOLEAN, // WARN: The same as above, but allocates 1 byte.
        STRING_LE, // UTF-16LE string
        STRING_BE, // UTF-16BE string
        STRING_LE_UTF8, // UTF-8LE string
        STRING_BE_UTF8, // UTF-8BE string
        UNSIGNED_CHAR, // WARN: This is C's unsigned char type, which is 1 byte.
        SIGNED_CHAR, // WARN: This is C's signed char type, which is 1 byte.
        SHORT,
        USHORT,
        INT,
        UINT,
        LONG,
        ULONG,
        FLOAT,
        DOUBLE,
        ARRAY = 1 << 9,
        UTF16LE_STRING_DICT = 1 << 10,
    }
}