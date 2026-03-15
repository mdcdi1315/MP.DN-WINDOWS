
using System;
using MP.Utilities;
using MP.TagReading;
using MP.TagReading.ID3;
using MP.PlaylistManagement.TagSupport.MP_TCF;

namespace MP.PlaylistManagement.TagSupport
{
    public sealed class CompressedFormatTagWriter : IDisposable, IStreamOwnerBase
    {
        private bool stream_owner;
        private IO.DataStream stream;

        public CompressedFormatTagWriter(IO.DataStream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            this.stream = stream;
            stream_owner = false;
        }

        private void WriteStringField(string name, string value, bool last = false)
        {
            if (name.Length > ushort.MaxValue) {
                throw new ArgumentException("Name string is too large.", nameof(name));
            } else {
                stream.WriteUInt16(0);
                stream.WriteStructure(new ENTRY() {
                    Flags = last ? ENTRY_FLAGS.LastEntry : ENTRY_FLAGS.None,
                    NameEncoding = STRING_ENCODING.UTF16_LE,
                    NameLength = name.Length.ToUInt16(),
                    ValueLength = value.Length.ToUInt32()
                });
                stream.WriteString(name, System.Text.Encoding.Unicode);
                stream.WriteStructure(STRING_ENCODING.UTF16_LE);
                stream.WriteString(value, System.Text.Encoding.Unicode);
            }
        }

        private void WriteStreamFieldAsByteArray(string name, IO.DataStream value, bool last = false)
        {
            if (name.Length > ushort.MaxValue) {
                throw new ArgumentException("Name string is too large.", nameof(name));
            } else if (value is null) {
                throw new ArgumentNullException(nameof(value));
            } else if (value.CanRead == false) {
                throw new ArgumentException("Stream is unreadable.", nameof(value));
            } else {
                var ed = new ENTRY() {
                    Flags = last ? ENTRY_FLAGS.LastEntry : ENTRY_FLAGS.None,
                    NameEncoding = STRING_ENCODING.UTF16_LE,
                    NameLength = name.Length.ToUInt16(),
                    ValueLength = value.Length.ToUInt32()
                };
                stream.WriteUInt16(0);
                stream.WriteStructure(ed);
                stream.WriteString(name, System.Text.Encoding.Unicode);
                value.DirectCopyToStream(stream);
            }
        }

        public void AddData(ITagReader reader)
        {
            WriteStringField(nameof(ITagReader.Title), reader.Title);
            WriteStringField(nameof(ITagReader.Genre), reader.Genre);
            WriteStringField(nameof(ITagReader.SubTitle), reader.SubTitle);
            WriteStringField(nameof(ITagReader.Comments), reader.Comments);
            WriteStringField(nameof(ITagReader.Copyright), reader.Copyright);
            WriteStringField(nameof(ITagReader.AlbumName), reader.AlbumName);
            WriteStringField(nameof(ITagReader.EncodedBy), reader.EncodedBy);
            WriteStringField(nameof(ITagReader.AlbumArtist), reader.AlbumArtist);
            WriteStringField(nameof(ITagReader.DiscOrdinal), reader.DiscOrdinal);
            WriteStreamFieldAsByteArray(nameof(ITagReader.Image), reader.Image);
            WriteStringField(nameof(ITagReader.ImageFormat), reader.ImageFormat);
            WriteStringField(nameof(ITagReader.CreationDate), reader.CreationDate);
            WriteStringField(nameof(ITagReader.TrackNumber), reader.TrackNumber);
            WriteStringField(nameof(ITagReader.PublisherURL), reader.PublisherURL);
            WriteStringField(nameof(ITagReader.WebSiteEncoderUrl), reader.WebSiteEncoderUrl);
            if (reader is IID3V2TagReaderBase id3v2) { 
                AddID3V2Data(id3v2); 
            }
            WriteStringField(nameof(ITagReader.ContributingArtists), reader.ContributingArtists, true);
        }

        private unsafe void WriteID3V2CommentBlock(CommentFrameData cfd)
        {
            stream.WriteUInt16(0);
            stream.WriteStructure(new ENTRY() {
                Flags = ENTRY_FLAGS.None,
                NameEncoding = STRING_ENCODING.UTF16_LE,
                ValueLength = cfd.Text.Length.ToUInt32() * sizeof(char) + cfd.Language.Length.ToUInt32() * sizeof(char) + cfd.ContentDescription.Length.ToUInt32() * sizeof(char) + sizeof(ID3V2CMTBLOCK).ToUInt32()
            });
            stream.WriteStructure(new ID3V2CMTBLOCK() {
                Encoding = STRING_ENCODING.UTF16_LE,
                ContentDescriptionLength = cfd.ContentDescription.Length.ToUInt16(),
                LanguageLength = cfd.Language.Length.ToUInt16(),
                TextLength = cfd.Text.Length.ToUInt32()
            });
            stream.WriteString(cfd.Language, System.Text.Encoding.Unicode);
            stream.WriteString(cfd.ContentDescription, System.Text.Encoding.Unicode);
            stream.WriteString(cfd.Text, System.Text.Encoding.Unicode);
        }

        private void AddID3V2Data(IID3V2TagReaderBase reader)
        {
            if (reader.TryGetComment(out var cmt)) {
                WriteID3V2CommentBlock(cmt);
            }
            if (reader.TryGetLyrics(out var lyrics)) {

            }
        }

        public bool IsStreamOwner 
        {
            get => stream_owner;
            set => stream_owner = value;
        }

        public void Dispose()
        {
            if (stream_owner && stream is not null) { stream.Dispose(); }
            stream = null;
        }
    }
}
