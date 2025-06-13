
using System.Text.Json;

namespace MP
{
    public static class CommonTagUtils
    {
        public static void WriteTo(this SavedDataTag sdt, Utf8JsonWriter writer, System.String FN)
        {
            if (sdt is null) { throw new System.NullReferenceException(); }
            writer.WriteStartObject();
            writer.WriteString("FilePath", FN);
            writer.WriteBoolean("TagDataExist", sdt.DataExist);
            if (sdt.DataExist)
            {
                writer.WriteString(nameof(SavedDataTag.WebSiteEncoderUrl), sdt.WebSiteEncoderUrl);
                writer.WriteString(nameof(SavedDataTag.EncodedBy), sdt.EncodedBy);
                writer.WriteString(nameof(SavedDataTag.AlbumName), sdt.AlbumName);
                writer.WriteString(nameof(SavedDataTag.Title1), sdt.Title1);
                writer.WriteString(nameof(SavedDataTag.Title2), sdt.Title2);
                writer.WriteString(nameof(SavedDataTag.DiscOrdinal), sdt.DiscOrdinal);
                writer.WriteString(nameof(SavedDataTag.TrackNumber), sdt.TrackNumber);
                writer.WriteString(nameof(SavedDataTag.ContributingArtists), sdt.ContributingArtists);
                writer.WriteString(nameof(SavedDataTag.AlbumArtist), sdt.AlbumArtist);
                writer.WriteString(nameof(SavedDataTag.Comments), sdt.Comments);
                writer.WriteString(nameof(SavedDataTag.SubTitle), sdt.SubTitle);
                writer.WriteString(nameof(SavedDataTag.PublisherURL), sdt.PublisherURL);
                writer.WriteString(nameof(SavedDataTag.Genre), sdt.Genre);
                writer.WriteString(nameof(SavedDataTag.ImageFormat), sdt.ImageFormat);
                writer.WriteString(nameof(SavedDataTag.Copyright), sdt.Copyright);
                writer.WriteString(nameof(SavedDataTag.CreationDate), sdt.CreationDate);
                writer.WriteString(nameof(SavedDataTag.Publisher), sdt.Publisher);
                writer.WriteBase64String(nameof(SavedDataTag.Image), sdt.Image);
            }
            writer.WriteEndObject();
        }

        public static void WriteEmptyTag(Utf8JsonWriter writer, System.String FN)
        {
            writer.WriteStartObject();
            writer.WriteString("FilePath", FN);
            writer.WriteBoolean("TagDataExist", false);
            writer.WriteEndObject();
        }

        public static SavedDataTag FromJsonElement(JsonElement el)
        {
            if (el.GetProperty("TagDataExist").GetBoolean())
            {
                return SavedDataTag.FromRawData([
                    new("P1", el.GetProperty(nameof(SavedDataTag.EncodedBy)).GetString()),
                    new("P2", el.GetProperty(nameof(SavedDataTag.Genre)).GetString()),
                    new("P3", el.GetProperty(nameof(SavedDataTag.TrackNumber)).GetString()),
                    new("P4", el.GetProperty(nameof(SavedDataTag.Title1)).GetString()),
                    new("P5", el.GetProperty(nameof(SavedDataTag.Title2)).GetString()),
                    new("P6", el.GetProperty(nameof(SavedDataTag.AlbumArtist)).GetString()),
                    new("P7", el.GetProperty(nameof(SavedDataTag.PublisherURL)).GetString()),
                    new("P8", el.GetProperty(nameof(SavedDataTag.AlbumName)).GetString()),
                    new("P9", el.GetProperty(nameof(SavedDataTag.Comments)).GetString()),
                    new("P10", el.GetProperty(nameof(SavedDataTag.ContributingArtists)).GetString()),
                    new("P11", el.GetProperty(nameof(SavedDataTag.DiscOrdinal)).GetString()),
                    new("P12", el.GetProperty(nameof(SavedDataTag.SubTitle)).GetString()),
                    new("P13" , el.GetProperty(nameof(SavedDataTag.WebSiteEncoderUrl)).GetString()),
                    new("P14", el.GetProperty(nameof(SavedDataTag.Publisher)).GetString()),
                    new("P15", el.GetProperty(nameof(SavedDataTag.Copyright)).GetString()),
                    new("P16", el.GetProperty(nameof(SavedDataTag.CreationDate)).GetString()),
                    new("P17", el.GetProperty(nameof(SavedDataTag.ImageFormat)).GetString())
                    ], el.GetProperty(nameof(SavedDataTag.Image)).GetBytesFromBase64());
            } else {
                return null;
            }
        }
    }
}