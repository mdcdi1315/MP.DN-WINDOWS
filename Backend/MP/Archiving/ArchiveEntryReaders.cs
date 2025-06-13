
namespace MP.Archiving
{
    internal sealed class PlainArchiveEntryReader : IArchiveEntryReader
    {
        private ArchiveOffsetStream stream;
        private ArchiveEntry entry;

        public PlainArchiveEntryReader(ArchiveEntry entry , ArchiveOffsetStream offset)
        {
            this.entry = entry;
            stream = offset;
        }

        public ArchiveEntry Entry => entry;

        public System.IO.Stream EntryStream => stream;

        public void Dispose()
        {
            entry = null;
            EntryReaderHelpers.ConsumeRestBytes(stream);
            stream?.Dispose();
            stream = null;
        }
    }

    internal sealed class GZipArchiveEntryReader : IArchiveEntryReader
    {
        private System.IO.ManagedZip.GZip.GZipInputStream gzi;
        private ArchiveOffsetStream stream;
        private ArchiveEntry entry;

        public GZipArchiveEntryReader(ArchiveEntry entry, ArchiveOffsetStream offset)
        {
            this.entry = entry;
            this.stream = offset;
            // Someone might want to only read entry metadata and not the actual data ,
            // which is a costly operation since we are dealing with semaphores in the middle.
            // So, creating lazily the object at the first property call it is effectively the same.
            gzi = null;
        }

        public ArchiveEntry Entry => entry;

        public System.IO.Stream EntryStream => gzi ??= new(stream) { IsStreamOwner = false };

        public void Dispose() 
        {
            gzi?.Dispose();
            gzi = null;
            EntryReaderHelpers.ConsumeRestBytes(stream);
            stream?.Dispose();
            stream = null;
            entry = null;
        }
    }

    internal sealed class BZip2ArchiveEntryReader : IArchiveEntryReader
    {
        private System.IO.ManagedZip.BZip2.BZip2InputStream bzi;
        private ArchiveOffsetStream stream;
        private ArchiveEntry entry;

        public BZip2ArchiveEntryReader(ArchiveEntry entry, ArchiveOffsetStream offset)
        {
            this.entry = entry;
            this.stream = offset;
            // Someone might want to only read entry metadata and not the actual data ,
            // which is a costly operation since we are dealing with semaphores in the middle.
            // So, creating lazily the object at the first property call it is effectively the same.
            bzi = null;
        }

        public ArchiveEntry Entry => entry;

        public System.IO.Stream EntryStream => bzi ??= new(stream) { IsStreamOwner = false };

        public void Dispose()
        {
            bzi?.Dispose();
            bzi = null;
            EntryReaderHelpers.ConsumeRestBytes(stream);
            stream?.Dispose();
            stream = null;
            entry = null;
        }
    }

    internal static class EntryReaderHelpers
    {
        // Reads all the non-read bytes.
        // This allows the reader to continue on a next entry.
        public static void ConsumeRestBytes(ArchiveOffsetStream offset)
        {
            if (offset is null) { return; }
            System.Byte[] discardbuffer = new System.Byte[2048];
            System.Int32 rb;
            while ((rb = offset.Read(discardbuffer, 0, discardbuffer.Length)) > 0) ;
        }
    }
}