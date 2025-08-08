using System;
using MP.TagReading.MP4;
using System.Collections.Generic;

namespace MP.TagReading
{
    /// <summary>
    /// Reads native M4A tags on full .NET . Pass it a stream with a M4A file , and it will read the tag if there is one. <br />
    /// M4A is actually MP4, that in turn it derived from QTFF (Apple QuickTime File Format).
    /// </summary>
    public sealed class MP4AudioTagReader : ITagReader2
    {
        private List<Box> atoms;
        private MP4HEADER header;
        private readonly long initpos;
        private System.IO.Stream stream;
        private Box extendedhdr;
        private System.String compatbrands;
        private readonly System.Text.Encoding encoding;

        private MP4AudioTagReader() 
        { 
            atoms = new(2); 
            extendedhdr = null;
            encoding = System.Text.Encoding.UTF8; 
        }

        /// <summary>
        /// Creates a new instance of the MP4 Audio Tag reader.
        /// </summary>
        /// <param name="stream">The stream to read MP4 data from</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not both readable and seekable.</exception>
        public MP4AudioTagReader(System.IO.Stream stream) : this()
        {
            if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
            if (stream.CanRead == false || stream.CanSeek == false) { throw new ArgumentException("The stream must be both readable and seekable."); }
            this.stream = stream;
            initpos = this.stream.Position;
            ReadHeader();
            ReadData();
        }

        private void ReadHeader() 
        {
            header = stream.ReadStructure<MP4HEADER>();
            if (header.IsCorrectHeader == false) { throw new FormatException("This is NOT a mp4 stream. The mp4 streams start with the word ftyp."); }
            System.Int32 size = header.RestHeaderSize.ToInt32();
            // Read the compatible brands.
            System.String[] types = ReaderUtils.MP4HeaderReadCompatBrands(stream.ReadBytes(size));
            System.Text.StringBuilder sb = new(size);
            // Create a semi-colon delimited list if more than one.
            for (System.Int32 I = 0; I < types.Length; I++)
            {
                sb.Append(types[I]);
                if (I + 1 < types.Length) { sb.Append(';'); }
            }
            compatbrands = sb.ToString();
            // From now on , only atoms will be found!
        }

        private static Box ReadAtomFromStream(System.IO.Stream stream)
        {
            if (stream.Position >= stream.Length) { return null; }
            return Box.ReadFromStream(stream);
        }

        private void ReadData()
        {
            Box d;
            // Break if the ID of the read atom is 'moov'.
            while ((d = ReadAtomFromStream(stream)) is not null) { atoms.Add(d); if (d.Type == "moov") { break; } }
            // Return the stream position back to when it started to read.
            stream.Position = initpos;
            Microsoft.IO.InternalMemoryStream mem = null;
            try {
                Box g = GetAtom("moov");
                if (g is null) { goto g_fail; }
                mem = new(g.Data);
                atoms.Clear();
                while ((g = ReadAtomFromStream(mem)) is not null) { atoms.Add(g); }
            } finally {
                mem?.Dispose();
                mem = null;
            }
            // The meta atom is residing inside the udta atom.
            // Additionally save the Xtra header if it is found now.
            try {
                Box g = GetAtom("udta");
                if (g is null) { goto g_fail; }
                mem = new(g.Data);
                atoms.Clear();
                while ((g = ReadAtomFromStream(mem)) is not null) { atoms.Add(g); if (g.Type == "Xtra") { extendedhdr = g; } }
            } finally {
                mem?.Dispose();
                mem = null;
            }
            // OK. although we have all the atoms we are not sure where the ilst atom is located.
            // The ilst atom is located inside the parent atom meta.
            // So , additional parsing must be done later.
            foreach (Box atom in atoms) 
            {
                if (atom.Type == "meta")
                {
                    // We found meta , we will read it.
                    ReadMetaHeader(atom);
                    return;
                }
            }
            g_fail:
            throw new FormatException("This M4A stream does not seem to have any data tag.");
        }

        private void ReadMetaHeader(Box parent)
        {
            Microsoft.IO.InternalMemoryStream mem = new(parent.Data);
            mem.Position = 4;
            Box d;
            while ((d = ReadAtomFromStream(mem)) is not null) 
            { 
                if (d.Type == "ilst") 
                {
                    try {
                        mem.Dispose();
                        mem = new(d.Data);
                        atoms.Clear();
                        while ((d = ReadAtomFromStream(mem)) is not null) { atoms.Add(d); }
                    } finally { mem?.Dispose(); }
                    ReadExtendedHeader();
                    return;
                }
            }
            throw new FormatException("This M4A stream does not seem to have any data tag.");
        }

        private Box GetAtom(string pattern)
        {
            if (System.String.IsNullOrEmpty(pattern)) { return null; }
            foreach (var at in atoms)
            {
                if (at.Type.Equals(pattern, StringComparison.InvariantCultureIgnoreCase)) {
                    return at;
                }
            }
            return null;
        }

        private string GetAtomAndReturnAsString(string pattern)
        {
            Box d = GetAtom(pattern);
            if (d is null) { return System.String.Empty; }
            if (pattern.StartsWith("WM")) // Requested a Windows Media Extended Header.
            {
                // These headers are a bit different than the usual ones.
                // This is true because they define different offsets and we need UTF-16 encoding.
                return System.Text.Encoding.Unicode.GetString(d.Data, 2, d.Data.Length - 8);
            }
            foreach (MetaBox mbx in d.GetMetaBoxes())
            {
                if (mbx.ID == "data")
                {
                    return mbx.TextData;
                }
            }
            return System.String.Empty;
        }

        private byte[] GetAtomAndReturnBytes(string pattern)
        {
            Box d = GetAtom(pattern);
            if (d is null) { return null; }
            foreach (MetaBox mtb in d.GetMetaBoxes())
            {
                if (mtb.ID == "data")
                {
                    return mtb.Data;
                }
            }
            return null;
        }

        private void ReadExtendedHeader()
        {
            if (extendedhdr is null || extendedhdr.Data.LongLength == 8) { return; }
            // OK. Now read the extended header.
            byte[] data = extendedhdr.Data;
            // Keep an index for tracking positions...
            int idx = 0;
            // OK. This header will be a Windows Media Player extended header.
            // The Windows Media header is defined as follows:
            // 0000(size of whole data)->0000(size of atom name)->id of the atom(can be any length here)->atom data
            // Create a loop now...
            // The code defines +8 so as the loop cut out properly. (and avoiding exceptions to be thrown)
            while (idx + 8 < data.Length)
            {
                // This is the length of the whole block.
                // It is omitted because it is not needed.
                idx += 4;
                int hdrlen = (new byte[] { data[idx+3] , data[idx+2] , data[idx+1] , data[idx] }).ToInt32(0);
                idx += 4;
                string hdr = encoding.GetString(data, idx, hdrlen);
                idx += hdrlen; // Move to header data.
                // Next 4 bytes is the header version.
                int version = (new byte[] { data[idx + 3], data[idx + 2], data[idx + 1], data[idx] }).ToInt32(0);
                idx += 4;
                // Next 4 bytes is the header value length. -4 because it covers the next header value length too.
                int vallen = (new byte[] { data[idx + 3], data[idx + 2], data[idx + 1], data[idx] }).ToInt32(0);
                idx += 4;
                // Read the whole byte array , and store the results to a new atom.
                atoms.Add(new(hdr, data.GetBytes(idx, vallen)));
                // And move to next header...
                idx += vallen - 4; // -4 to avoid positioning issues.
            }
            data = null;
            extendedhdr = null;
        }

        /// <summary>
        /// [MP4 Reader Specific] Gets the MP4 file type. The MP4 file type is the method which this file was written. <br />
        /// For example , the M4A value indicates that the original MP4 library was used to write the stream. <br />
        /// The property in the original format is called the 'brand' of the MP4 stream.
        /// </summary>
        public System.String FileType => header.Brand;

        /// <summary>
        /// [MP4 Reader Specific] Gets the additionally supported MP4 types. <br />
        /// The property in the original format is called the 'compatible types' of the MP4 stream. <br />
        /// If more than one supported types are found , the property returns the types as a semi-colon delimited list.
        /// </summary>
        public System.String AdditionalSupportedTypes => compatbrands;

        /// <summary>
        /// [MP4 Reader Specific] Gets the revision of the format under which the format is written to.
        /// </summary>
        public System.UInt32 FormatRevision => header.Revision;

        /// <inheritdoc />
        public string PublisherURL => GetAtomAndReturnAsString("purl");

        /// <summary>
        /// Returns the Author URL of the current MP4 tag. <br />
        /// Returns <see cref="string.Empty"/> if it does not exist.
        /// </summary>
        // Map this property to WM/AuthorURL instead.
        // Only supported through the Windows Media Extended Header.
        public string WebSiteEncoderUrl => GetAtomAndReturnAsString("WM/AuthorURL");

        /// <inheritdoc />
        public string EncodedBy
        {
            get
            {
                string result = GetAtomAndReturnAsString("WM/EncodedBy");
                if (string.IsNullOrEmpty(result)) { result = GetAtomAndReturnAsString("too"); }
                return result;
            }
        }

        /// <inheritdoc />
        public string AlbumName => GetAtomAndReturnAsString("alb");

        /// <summary>
        /// This property is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        public string Title1 => throw new NotSupportedException("The MP4 tags do not honor or support the Title1 property.");

        /// <inheritdoc />
        public string Title2 => GetAtomAndReturnAsString("nam");

        /// <inheritdoc />
        public string DiscOrdinal
        {
            get
            {
                byte[] dt = GetAtomAndReturnBytes("disk");
                if (dt is null) { return string.Empty; }
                dt = dt.GetBytes(0, 4);
                if (BitConverter.IsLittleEndian) { dt.Reverse(); }
                return dt.ToInt32(0).ToString();
            }
        }
        
        /// <inheritdoc />
        public string TrackNumber
        {
            get
            {
                byte[] dt = GetAtomAndReturnBytes("trkn");
                if (dt is null) { return string.Empty; }
                dt = dt.GetBytes(0, 4);
                if (BitConverter.IsLittleEndian) { dt.Reverse(); }
                return dt.ToInt32(0).ToString();
            }
        }

        /// <inheritdoc />
        public string ContributingArtists => GetAtomAndReturnAsString("ART");

        /// <inheritdoc />
        public string AlbumArtist => GetAtomAndReturnAsString("aART");

        /// <inheritdoc />
        public string Comments => GetAtomAndReturnAsString("cmt");

        /// <inheritdoc />
        // Only supported through the Windows Media Extended Header.
        public string SubTitle => GetAtomAndReturnAsString("WM/SubTitle");

        /// <inheritdoc />
        public string Genre => GetAtomAndReturnAsString("gen");

        /// <summary>
        /// This property is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        public string ImageFormat => throw new NotSupportedException("The MP4 tags do not honor or support the ImageFormat property.");

        /// <inheritdoc />
        // Only supported through the Windows Media Extended Header.
        public string Publisher => GetAtomAndReturnAsString("WM/Publisher");

        /// <inheritdoc />
        public string Copyright => GetAtomAndReturnAsString("cprt");

        /// <inheritdoc />
        public string CreationDate => GetAtomAndReturnAsString("day");

        /// <inheritdoc />
        public byte[] Image => GetAtomAndReturnBytes("covr");

        /// <summary>
        /// Clears all the internal references held by the current tag reader.
        /// </summary>
        public void Dispose()
        {
            atoms?.Clear();
            atoms = null;
            stream = null;
            compatbrands = null;
            extendedhdr = null;
        }
    }
}
