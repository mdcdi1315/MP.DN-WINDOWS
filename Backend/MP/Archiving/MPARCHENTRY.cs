
using System.Runtime.InteropServices;

namespace MP.Archiving
{
    [StructLayout(LayoutKind.Explicit , Pack = 2)]
    internal struct MPARCHENTRY
    {
        // Note that directories may only contain attributes - 
        // but may also act as files recording only secondary data that are about the directory entry itself.
        [FieldOffset(0)]
        public ArchiveEntryType Type;

        [FieldOffset(1)]
        public ArchiveEntryCompression Compression;

        // Gets the file's size in bytes , if this entry is a file after all.
        // Note that this should represent the file's uncompressed size,
        // when compression was specified in Compression member.
        [FieldOffset(2)]
        public MPARCHUINT FileSize;

        // Gets the file's compressed size in bytes , if the Compression member is not zero.
        // Otherwise this value must be zero.
        [FieldOffset(8)]
        public MPARCHUINT CompressedSize;

        // Gets the number of additional attributes defined for this archive file.
        // There are some standard attributes that do elsewise need to be defined - 
        // see more information about this in the archive entry class.
        [FieldOffset(14)]
        public System.UInt16 NumberOfAttributes;

        // Reserved field , set to zero.
        [FieldOffset(16)]
        public System.UInt16 RSVD0;
    }
}