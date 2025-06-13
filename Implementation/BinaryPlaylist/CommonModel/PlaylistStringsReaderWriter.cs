using System;
using System.Collections.Generic;

namespace MP.BinaryPlaylist
{
    public sealed class PlaylistStringsWriter : PlaylistBlobWriter
    {
        private System.UInt32 lastid;

        public PlaylistStringsWriter() { lastid = 0; }

        /// <summary>
        /// Writes a string to the string blob and returns it's associated index.
        /// </summary>
        /// <param name="value">The string to write in the string blob.</param>
        /// <returns>A unique index inside on the string blob so as to identify this string.</returns>
        public System.UInt32 WriteString(System.String value)
        {
            if (lastid == System.UInt32.MaxValue) { throw new OutOfMemoryException("The string blob was filled up and cannot allocate more strings!"); }
            if (System.String.IsNullOrEmpty(value)) { value = "\0"; }
            Write(value.Length.ToUInt16());
            for (System.Int32 I = 0; I < value.Length; I++)
            {
                Write(value[I]);
            }
            Write(0.ToByte());
            return lastid++;
        }

        /// <summary>
        /// Finalizes the string blob.
        /// </summary>
        public void FinalizeWriter()
        {
            Header = new() { Count = lastid, Identifier1 = BlobFlags.ReadOnly, Identifier2 = BlobTypes.STRINGBLOB, Version = 1 };
            IsCompleted = true;
        }
    }

    public sealed class PlaylistStringsReader : PlaylistBlobReader
    {
        private System.Text.StringBuilder shared;
        private Dictionary<System.UInt32, System.String> stringsfast;

        public PlaylistStringsReader() : base() { stringsfast = new(30); shared = new(500); }

        private System.String ReadString(System.UInt32 idx)
        {
            if (idx >= Header.Count) { throw new ArgumentOutOfRangeException(nameof(idx)); }
            System.UInt16 length;
            System.Int32 marker;
            Position = 0;
            for (System.UInt32 I = 0; I < Header.Count && I < idx; I++)
            {
                length = ReadNumber<System.UInt16>();
                if (length < 0) { throw new InvalidStringDataException("The string length could not be retrieved."); }
                Seek(length * 2, System.IO.SeekOrigin.Current);
                marker = ReadByte();
                if (marker != 0) { throw new InvalidStringDataException(); }
            }
            length = ReadNumber<System.UInt16>();
            shared.Clear();
            System.Byte[] stringnative = new System.Byte[length * 2];
            if (Read(stringnative, 0, stringnative.Length) != stringnative.Length) { throw new System.IO.IOException($"Could not read {stringnative.Length} bytes from the stream."); }
            for (System.Int32 I = 0; I < stringnative.Length; I += 2)
            {
                shared.Append(stringnative.ToChar(I));
            }
            stringnative = null;
            marker = ReadByte();
            if (marker != 0) { throw new InvalidStringDataException(); }
            return shared.ToString();
        }

        public System.String Get(System.UInt32 index)
        {
            if (stringsfast.TryGetValue(index, out System.String g)) { return g; }
            if (stringsfast.Count > 30) { stringsfast.Clear(); }
            System.String val = ReadString(index);
            stringsfast.Add(index, val);
            return val;
        }

        protected override void Dispose(bool disposing)
        {
            shared?.Clear();
            shared = null;
            stringsfast?.Clear();
            stringsfast = null;
            base.Dispose(disposing);
        }
    }
}
