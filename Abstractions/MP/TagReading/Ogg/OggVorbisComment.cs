using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Reads an Ogg Vorbis Comment from the specified data stream, starting in a specified position, and specifying an implicit upper bound length to 
        /// protect the user from reading invalid data.
        /// </summary>
        /// <param name="stream">The data stream to read the comment from.</param>
        /// <param name="originstart">The start position to read the comment, relative to the stream's beginning.</param>
        /// <param name="implicitbound">
        /// The estimated length of the comment. <br />
        /// The actual length of bytes that will be finally read might be less than the number provided here. <br />
        /// Specify -1 to this argument to say that the comment is the entire stream. 
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="originstart"/> was less than zero or <paramref name="implicitbound"/> was less than -1.</exception>
        /// <exception cref="System.IO.EndOfStreamException">Early end of stream detected before the entire comment was read out.</exception>
        public OggVorbisComment(IO.DataStream stream, System.Int64 originstart, System.Int64 implicitbound = -1)
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (originstart < 0) {
                throw new ArgumentOutOfRangeException(nameof(originstart), "Starting position must not be negative.");
            }
            if (implicitbound < -1) {
                throw new ArgumentOutOfRangeException(nameof(implicitbound), "Invalid value.");
            }
            vendor = null;
            entries = null;
            enc = System.Text.Encoding.UTF8;
            if (implicitbound == -1) { implicitbound = stream.Length; }
            Read(stream, originstart, implicitbound);
        }

        private void Read(IO.DataStream stream, System.Int64 originstart, System.Int64 implicitbound)
        {
            stream.Seek(originstart, IO.SeekDisplacement.Begin);
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

        /// <summary>
        /// Gets the vendor of the audio file.
        /// </summary>
        public System.String Vendor => vendor;

        /// <summary>
        /// Retrieves an <see cref="OggCommentEntry"/> with the ordinal specified. <br />
        /// The ordinal is a zero-based index. The entries are retrieved in the same way as read from the stream.
        /// </summary>
        /// <param name="index">The index in the internal array to read the comment.</param>
        /// <returns>The retrieved comment at <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> was outside of the internal array bounds.</exception>
        public OggCommentEntry GetEntry(System.Int32 index)
        {
            try {
                return entries[index];
            } catch (IndexOutOfRangeException) {
                throw new ArgumentOutOfRangeException(nameof(index) , "Index must not be negative and be less than the internal array length.");
            }
        }

        /// <summary>
        /// Retrieves the first <see cref="OggCommentEntry"/> whose <see cref="OggCommentEntry.Key"/> member matches the contents of the <paramref name="key"/> parameter.
        /// </summary>
        /// <param name="key">The key of the entry to find.</param>
        /// <returns>The requested entry, or a default structure whose <see cref="OggCommentEntry.IsNull"/> property returns <see langword="true"/>.</returns>
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

        /// <summary>
        /// Gets the value of the requested entry indexed by it's key. <br />
        /// The <paramref name="key"/> parameter is compared with all the entries with the <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </summary>
        /// <param name="key">The key of the entry to retrieve it's value.</param>
        /// <returns>The value of the entry, or the empty string if no entry is defined with this key.</returns>
        /// <remarks><see langword="null"/> is returned if <paramref name="key"/> is null or the empty string.</remarks>
        public System.String GetValue(System.String key)
        {
            if (System.String.IsNullOrEmpty(key)) { return null; }
            foreach (var entry in entries)
            {
                if (entry.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) { return entry.Value; }
            }
            return System.String.Empty;
        }

        /// <summary>
        /// Gets the values of the requested entry indexed by it's key. <br />
        /// The <paramref name="key"/> parameter is compared with all the entries with the <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </summary>
        /// <param name="key">The key of the entry to retrieve all the values.</param>
        /// <returns>All the values of the entries which have as a key the contents of <paramref name="key"/> parameter.</returns>
        /// <remarks><see langword="null"/> is returned if <paramref name="key"/> is null or the empty string.</remarks>
        public System.String[] GetValues(System.String key)
        {
            if (System.String.IsNullOrEmpty(key)) { return null; }
            List<System.String> strings = new(5);
            foreach (var entry in entries)
            {
                if (entry.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) { strings.Add(entry.Value); }
            }
            return strings.ToArray();
        }

        /// <summary>
        /// Works as the <see cref="GetValues"/> method, but concatenates and comma-delimiters all the found values in a single string. <br />
        /// This is useful for presentation in a GUI.
        /// </summary>
        /// <param name="key">The key of the entry to retrieve all the values.</param>
        /// <returns>All the values of <paramref name="key"/> in a single string that contains the values as a comma-delimited list.</returns>
        public System.String ConcatenateMultipleValues(System.String key)
        {
            if (System.String.IsNullOrEmpty(key)) { return null; }
            System.Text.StringBuilder sb = new(key.Length);
            foreach (var entry in entries)
            {
                if (entry.Key.Equals(key, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(entry.Value);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets a collection of the comment keys contained in the current Ogg comment. <br />
        /// Note that multiple occurences of a single key may be defined, so do not use these for binary search and sorting.
        /// </summary>
        public IEnumerable<System.String> Keys
        {
            get
            {
                foreach (var prop in entries)
                {
                    yield return prop.Key;
                }
            }
        }

        /// <summary>
        /// The number of Ogg comments existing in the Ogg Vorbis comment block.
        /// </summary>
        public System.Int32 Count => entries.Length;

        /// <summary>
        /// Gets the entire length of this Ogg Vorbis comment block.
        /// </summary>
        public System.Int64 CommentByteLength => readbytes;
    }
}