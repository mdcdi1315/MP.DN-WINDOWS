
using System;
using System.Collections.Generic;

namespace MP.Archiving
{
    public sealed class ArchiveEntry
    {
        private ArchiveEntryCompression compression;
        private List<ArchiveEntryAttribute> attributes;
        private ArchiveEntryType type;
        private MPARCHUINT len, cmplen;

        internal ArchiveEntry(MPARCHENTRY native , IEnumerable<ArchiveEntryAttribute> attributes) 
        {
            type = native.Type;
            len = native.FileSize;
            cmplen = native.CompressedSize;
            compression = native.Compression;
            this.attributes = new(attributes);
        }

        public ArchiveEntry()
        {
            attributes = new(5);
            type = ArchiveEntryType.Unknown;
            len = MPARCHUINT.Zero;
            cmplen = len.Copy();
            compression = ArchiveEntryCompression.Store;
        }

        public static ArchiveEntry PopulateFromFileSystemInfo(Microsoft.IO.FileSystemInfo fsi)
        {
            ArchiveEntry entry = new();
            entry.compression = ArchiveEntryCompression.Store;
            switch (fsi)
            {
                case Microsoft.IO.DirectoryInfo di:
                    entry.type = ArchiveEntryType.Directory;
                    entry.EntryPath = di.Name;
                    entry.len = MPARCHUINT.Zero;
                    entry.LastWriteTime = di.LastWriteTime;
                    entry.CreationTime = di.CreationTime;
                    entry.LastAccessTime = di.LastAccessTime;
                    break;
                case Microsoft.IO.FileInfo fi:
                    entry.type = ArchiveEntryType.File;
                    entry.EntryPath = fi.Name;
                    entry.len = MPARCHUINT.ToUnsignedSixByteInteger(fi.Length);
                    entry.LastWriteTime = fi.LastWriteTime;
                    entry.CreationTime = fi.CreationTime;
                    entry.LastAccessTime = fi.LastAccessTime;
                    break;
                default:
                    throw new NotSupportedException($"The given derived class is not supported: {fsi.GetType().FullName}");
            }
            return entry;
        }

        public System.String EntryPath
        {
            get => attributes.GetValueByName("NAME") as System.String;
            set => attributes.SetOrUpdate("NAME" , value);
        }

        public System.Int64 Length
        {
            get => len.ToInt64();
            set => len = MPARCHUINT.ToUnsignedSixByteInteger(value);
        }

        /// <summary>
        /// Only for when retrieveing an entry from an archive. <br />
        /// It gets the entry's compressed size in bytes.
        /// </summary>
        public System.Int64 CompressedLength => cmplen.ToInt64();

        public ArchiveEntryType Type
        {
            get => type;
            set => type = value;
        }

        public ArchiveEntryCompression Compression
        {
            get => compression;
            set => compression = value;
        }

        public System.DateTime CreationTime
        {
            get => attributes.GetTValueByName<MPARCHDATETIME>("D_CR").DateTime;
            set => attributes.SetOrUpdate("D_CR" , new MPARCHDATETIME(value));
        }

        public System.DateTime LastAccessTime
        {
            get => attributes.GetTValueByName<MPARCHDATETIME>("D_LA").DateTime;
            set => attributes.SetOrUpdate("D_LA", new MPARCHDATETIME(value));
        }

        public System.DateTime LastWriteTime
        {
            get => attributes.GetTValueByName<MPARCHDATETIME>("D_LW").DateTime;
            set => attributes.SetOrUpdate("D_LW", new MPARCHDATETIME(value));
        }
    
        public void SetCustomAttribute(System.String name , System.Object value)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name)); }
            if (name.Length != 4) { throw new ArgumentException("The programmatic name must be exactly 4 characters."); }
            attributes.SetOrUpdate(name, value);
        }

        // Gets a custom attribute with the name defined in name parameter.
        // If it does not exist the return value will be null. Additionally the return value will be null if that attribute has set the null value.
        public System.Object GetCustomAttribute(System.String name) => attributes.GetValueByName(name);

        /// <summary>
        /// Gets the number of the attributes set for this archive entry.
        /// </summary>
        public System.Int32 NumberOfAttributes => attributes.Count;

        /// <summary>
        /// Gets the currently defined attributes for this archive entry.
        /// </summary>
        public IEnumerable<ArchiveEntryAttribute> Attributes => attributes;

        internal MPARCHENTRY GetNativeHeader()
        {
            if (attributes.Count > 65535)
            {
                throw new OverflowException("Extravagant attribute count! The format allows only up to 65535 attributes to be written.");
            }
            MPARCHENTRY ret = new();
            ret.FileSize = len;
            ret.Compression = compression;
            ret.Type = type;
            ret.NumberOfAttributes = attributes.Count.ToUInt16();
            return ret;
        }
    }
}