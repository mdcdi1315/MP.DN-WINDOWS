
using MP.Annotations;
using System.Collections.Generic;

namespace MP
{
    /// <summary>
    /// Represents a tag reader for playlist abstractions, whose backing store is the memory.
    /// </summary>
    [DeprecatedMayBeRemoved("2.0.0.0")]
    public sealed class SavedDataTag : ITagReader2
    {
        private Dictionary<System.String, System.String> strings;
        private System.Boolean dtfound;
        private System.Byte[] imgdata;

        private SavedDataTag() 
        {
            strings = new(13);
            imgdata = null; 
            dtfound = false;
        }

        /// <summary>
        /// Creates a new in-memory tag from a specified audio tag.
        /// </summary>
        /// <param name="reader">The audio tag to create from.</param>
        public SavedDataTag(ITagReader reader) : this()
        {
            if (reader is null) { dtfound = false; return; }
            SaveV1TagReader(reader);
            if (reader is ITagReader2 rd) { SaveV2TagReader(rd); }
        }

        /// <summary>
        /// Creates a new in-memory tag from a specified audio tag.
        /// </summary>
        /// <param name="tgr2">The audio tag to create from.</param>
        public SavedDataTag(ITagReader2 tgr2) : this()
        {
            if (tgr2 is null) { dtfound = false; return; }
            SaveV1TagReader(tgr2);
            SaveV2TagReader(tgr2);
        }

        /// <summary>
        /// Initializes a new <see cref="SavedDataTag"/> class instance from raw reader data.
        /// </summary>
        /// <param name="props">The raw properties of this tag.</param>
        /// <param name="image">The raw image data bytes.</param>
        /// <returns>A new <see cref="SavedDataTag"/> class instance.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="props"/> was null. Note that <paramref name="props"/> can be the empty array but cannot be null.</exception>
        public static SavedDataTag FromRawData(SavedDataTagProperty[] props, System.Byte[] image)
        {
            if (props is null) { throw new System.ArgumentNullException(nameof(props)); }
            SavedDataTag tag = new();
            tag.dtfound = true;
            tag.strings = new(props.Length);
            foreach (SavedDataTagProperty prop in props) 
            {
                tag.strings.Add(prop.Name, prop.Value);
            }
            tag.imgdata = image;
            return tag;
        }

        private void SaveV1TagReader(ITagReader reader)
        {
            try { strings.Add("P1", reader.EncodedBy); } catch { }
            try { strings.Add("P2", reader.Genre); } catch { }
            try { strings.Add("P3", reader.TrackNumber); } catch { }
            try { strings.Add("P4", reader.Title1); } catch { }
            try { strings.Add("P5", reader.Title2); } catch { }
            try { strings.Add("P6", reader.AlbumArtist); } catch { }
            try { strings.Add("P7", reader.PublisherURL); } catch { }
            try { strings.Add("P8", reader.AlbumName); } catch { }
            try { strings.Add("P9", reader.Comments); } catch { }
            try { strings.Add("P10", reader.ContributingArtists); } catch { }
            try { strings.Add("P11", reader.DiscOrdinal); } catch { }
            try { strings.Add("P12", reader.SubTitle); } catch { }
            try { strings.Add("P13", reader.WebSiteEncoderUrl); } catch { }
            try { imgdata = reader.Image; } catch { imgdata = null; }
            dtfound = true;
        }

        private void SaveV2TagReader(ITagReader2 tgr2)
        {
            try { strings.Add("P14", tgr2.Publisher); } catch { }
            try { strings.Add("P15", tgr2.Copyright); } catch { }
            try { strings.Add("P16", tgr2.CreationDate); } catch { }
            try { strings.Add("P17", tgr2.ImageFormat); } catch { }
            if (tgr2 is IID3V2TagReaderBase dr)
            {
                System.String ls = dr.Lyrics;
                if (System.String.IsNullOrEmpty(ls) == false && ls.Length <= 65535)
                {
                    strings.Add("P-1", ls);
                }
            }
            if (tgr2 is IOggTagReaderBase oggtb)
            {
                System.String vend = oggtb.Vendor;
                if (System.String.IsNullOrEmpty(vend) == false && vend.Length <= 65535)
                {
                    strings.Add("P-2", vend);
                }
            }
            dtfound = true;
        }

        private System.String GetString(System.String id)
        {
            System.String data;
            if (strings.TryGetValue(id, out data)) { return data; }
            return System.String.Empty;
        }

        /// <inheritdoc />
        public string ImageFormat => GetString("P17");

        /// <inheritdoc />
        public string Publisher => GetString("P14");

        /// <inheritdoc />
        public string Copyright => GetString("P15");

        /// <inheritdoc />
        public string CreationDate => GetString("P16");

        /// <inheritdoc />
        public string WebSiteEncoderUrl => GetString("P13");

        /// <inheritdoc />
        public string EncodedBy => GetString("P1");

        /// <inheritdoc />
        public string AlbumName => GetString("P8");

        /// <inheritdoc />
        public string Title1 => GetString("P4");

        /// <inheritdoc />
        public string Title2 => GetString("P5");

        /// <inheritdoc />
        public string DiscOrdinal => GetString("P11");

        /// <inheritdoc />
        public string TrackNumber => GetString("P3");
        
        /// <inheritdoc />
        public string ContributingArtists => GetString("P10");

        /// <inheritdoc />
        public string AlbumArtist => GetString("P6");

        /// <inheritdoc />
        public string Comments => GetString("P9");

        /// <inheritdoc />
        public string SubTitle => GetString("P12");

        /// <inheritdoc />
        public string PublisherURL => GetString("P7");

        /// <inheritdoc />
        public string Genre => GetString("P2");

        /// <inheritdoc />
        public byte[] Image => imgdata;

        /// <summary>
        /// If this value is <see langword="true"/> it indicates that the data contained in this instance are valid.
        /// </summary>
        public System.Boolean DataExist 
        { 
            get => dtfound; 
            set => dtfound = value; 
        }

        /// <summary>
        /// Returns an array of properties suitable for writers which need to save data tags.
        /// </summary>
        public IEnumerable<SavedDataTagProperty> NativeProperties
        {
            get {
                foreach (var prop in strings) { 
                    yield return new(prop.Key , prop.Value);
                }
            }
        }

        /// <summary>
        /// Represents a tag that does not have any information within. <br />
        /// In other words, the tag is empty.
        /// </summary>
        public static SavedDataTag Empty => new() { dtfound = false };

        /// <summary>
        /// Destroys all the data used by the current <see cref="SavedDataTag"/> instance.
        /// </summary>
        public void Dispose() 
        { 
            imgdata = null; 
            strings?.Clear(); 
            strings = null; 
        }
    }
}
