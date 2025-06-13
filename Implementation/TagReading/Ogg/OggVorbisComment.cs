namespace MP.TagReading.Ogg
{
    /// <summary>
    /// Defines a reader class for reading Ogg Vorbis comments shared by many implementations. <br />
    /// The class cannot be inherited.
    /// </summary>
    public sealed class OggVorbisComment
    {
        private System.String vendor;
        private System.Int64 readbytes;
        private System.Text.Encoding enc;
        private OggCommentEntry[] entries;

        public OggVorbisComment(System.IO.Stream stream , System.Int64 originstart , System.Int64 implicitbound)
        {
            vendor = null;
            entries = null;
            enc = System.Text.Encoding.UTF8;
            if (implicitbound == -1) { implicitbound = stream.Length; }
            Read(stream , originstart , implicitbound);
        }

        private void Read(System.IO.Stream stream, System.Int64 originstart, System.Int64 implicitbound)
        {
            stream.Seek(originstart, System.IO.SeekOrigin.Begin);
            System.UInt32 len = stream.ReadUInt32();
            if (len > originstart + implicitbound)
            {
                throw new System.IO.EndOfStreamException($"Extravagant value of {len} bytes attempted to be read");
            }
            vendor = enc.GetString(stream.ReadBytes(len));
            len = stream.ReadUInt32();
            if (len > originstart + implicitbound)
            {
                throw new System.IO.EndOfStreamException($"Extravagant value of {len} bytes attempted to be read");
            }
            entries = new OggCommentEntry[len];
            System.String cmt;
            System.Byte[] temp;
            for (System.Int32 I = 0; I < entries.Length; I++) 
            {
                len = stream.ReadUInt32();
                if (len > originstart + implicitbound)
                {
                    throw new System.IO.EndOfStreamException($"Extravagant value of {len} bytes attempted to be read");
                }
                temp = stream.ReadBytes(len);
                cmt = enc.GetString(temp);
                temp = null;
                System.Int32 offsetidx = cmt.IndexOf('=');
                if (offsetidx == -1) { throw new MP.ExceptionSystem.BaseException("Invalid comment data found"); }
                entries[I] = new() { Key = cmt.Remove(offsetidx), Value = cmt.Substring(offsetidx + 1) };
                cmt = null;
            }
            readbytes = stream.Position - originstart;
            if (readbytes > implicitbound) {
                throw new System.IO.EndOfStreamException("Stream ended prematurely.");
            }
        }

        public System.String Vendor => vendor;

        public OggCommentEntry GetEntry(System.Int32 index) => entries[index];

        public OggCommentEntry GetEntry(System.String key)
        {
            OggCommentEntry ret = default;
            if (System.String.IsNullOrEmpty(key)) { return ret; }
            for (System.Int32 I = 0; I < entries.Length; I++)
            {
                OggCommentEntry v = entries[I];
                if (v.Key == key) { ret = v; break; }
            }
            return ret;
        }

        public System.String GetValue(System.String key)
        {
            if (System.String.IsNullOrEmpty(key)) { return null; }
            foreach (var entry in entries) 
            {
                if (entry.Key.Equals(key , System.StringComparison.OrdinalIgnoreCase)) { return entry.Value; }
            }
            return System.String.Empty;
        }

        public System.String ConcatenateMultipleValues(System.String key)
        {
            if (System.String.IsNullOrEmpty(key)) { return null; }
            System.Text.StringBuilder sb = new(key.Length);
            foreach (var entry in entries) 
            {
                if (entry.Key.Equals(key , System.StringComparison.OrdinalIgnoreCase))
                {
                    if (sb.Length > 0) {
                        sb.Append(", ");
                    }
                    sb.Append(entry.Value);
                }
            }
            return sb.ToString();
        }

        public System.Collections.Generic.IEnumerable<System.String> Keys
        {
            get {
                foreach (var prop in entries)
                {
                    yield return prop.Key;
                }
            }
        }

        public System.Int32 Count => entries.Length;

        public System.Int64 CommentByteLength => readbytes;
    }
}