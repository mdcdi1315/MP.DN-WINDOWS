
using System;
using System.Runtime.InteropServices;

namespace MP.BinaryPlaylist.ArchivedPlaylist
{
    [StructLayout(LayoutKind.Explicit , Pack = 2 , Size = 8)]
    public struct ARCHIVEDPLATTRIBUTE
    {
        [FieldOffset(0)]
        public System.UInt16 AttributeNameLength;

        [FieldOffset(2)]
        public System.Int32 AttributeValueLength;

        [FieldOffset(6)]
        public ArchivedPlaylistAttributeFlags Flags;
    }

    public sealed class ArchivedPlaylistAttributeBlobReader : PlaylistBlobReader
    {
        private ArchivedPlaylistAttributeCollection attributes;

        public ArchivedPlaylistAttributeBlobReader()
        {
            attributes = null;
        }

        private void ReadAttributes()
        {
            System.Int32 ct = Header.Count.ToInt32();
            attributes = new(ct);
            ARCHIVEDPLATTRIBUTE lendat;
            for (System.Int32 I = 0; I < ct; I++)
            {
                lendat = ReadStructure<ARCHIVEDPLATTRIBUTE>();
                attributes.Add(new(ReadString(lendat.AttributeNameLength, false), ReadString(lendat.AttributeValueLength / sizeof(System.Char), false)) { Flags = lendat.Flags });
            }
        }

        public ArchivedPlaylistAttributeCollection Attributes
        {
            get {
                if (attributes is null) {
                    ReadAttributes();
                }
                return attributes;
            }
        }

        protected override void Dispose(bool disposing)
        {
            attributes = null;
            base.Dispose(disposing);
        }
    }

    public sealed class ArchivedPlaylistAttributeBlobWriter : PlaylistBlobWriter 
    {
        private System.Boolean written;

        public ArchivedPlaylistAttributeBlobWriter() 
        {
            written = false;
        }

        public void WriteAttributes(ArchivedPlaylistAttributeCollection attributes)
        {
            if (attributes is null) { throw new ArgumentNullException(nameof(attributes)); }
            written = true;
            Header = new() { Version = 1, Identifier1 = BlobFlags.ReadOnly | BlobFlags.Custom, Identifier2 = BlobTypes.MAXEMBEDDEDBLOBVAL + 3 , Count = attributes.Count.ToUInt32() };
            System.Boolean namenull, valuenull;
            foreach (var attribute in attributes) 
            {
                ARCHIVEDPLATTRIBUTE data = new();
                data.Flags = attribute.Flags;
                data.AttributeNameLength = ((namenull = attribute.Name is null) ? 0 : attribute.Name.Length).ToUInt16();
                data.AttributeValueLength = (valuenull = attribute.Value is null) ? 0 : attribute.Value.Length * sizeof(System.Char);
                WriteStructure(data);
                if (namenull == false) {
                    Write(attribute.Name, false);
                } 
                if (valuenull == false) {
                    Write(attribute.Value, false);
                }
            }
        }

        public void FinalizeWriter()
        {
            if (written == false) { throw new InvalidOperationException("Data were not written yet! Provide the attributes to write and then finalize the writer."); }
            IsCompleted = true;
        }
    }

}