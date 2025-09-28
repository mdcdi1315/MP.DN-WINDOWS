
using System;

namespace MP.PlaylistManagement.AudioTagSerialization
{
    /// <summary>
    /// Defines a writer that can serialize <see cref="PlaylistTrackTag"/> instances.
    /// </summary>
    public static class PlaylistTrackTagWriter
    {
        private static void SerializeInternal(PlaylistTrackTag tag , System.IO.Stream stream)
        {
            PLAYLISTTRACKTAGHEADER header = new();
            if (tag.PropertyCount > System.UInt16.MaxValue)
            {
                throw new ArgumentException("Extravagant number of properties; the writer does only support up to 65535 entries.");
            }
            header.NumberOfEntries = tag.PropertyCount.ToUInt16();
            stream.WriteStructure(header);
            System.String entryvalue;
            PLAYLISTTRACKTAGENTRY entry;
            foreach (var item in tag)
            {
                entryvalue = item.Value;
                entry = new() { Key = item.Key, ValueLength = (entryvalue.Length * sizeof(System.Char)).ToUInt32() };
                stream.WriteUTF16LEString(entryvalue);
            }
        }

        /// <summary>
        /// Serializes the specified <see cref="PlaylistTrackTag"/> instance and returns a <see cref="System.IO.Stream"/> representing the serialized result.
        /// </summary>
        /// <param name="tag">The <see cref="PlaylistTrackTag"/> to serialize.</param>
        /// <returns>An internal memory-based stream containing the serialized data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="tag"/> was <see langword="null"/>.</exception>
        public static System.IO.Stream Serialize(this PlaylistTrackTag tag)
        {
            ArgumentNullException.ThrowIfNull(tag);
            IO.MemoryStream stream = new();
            SerializeInternal(tag, stream);
            return stream;
        }

        /// <summary>
        /// Serializes the specified <see cref="PlaylistTrackTag"/> instance and returns a <see cref="System.Byte"/>[] representing the serialized result.
        /// </summary>
        /// <param name="tag">The <see cref="PlaylistTrackTag"/> to serialize.</param>
        /// <returns>A byte array containing the serialized data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="tag"/> was <see langword="null"/>.</exception>
        public static System.Byte[] SerializeAsArray(this PlaylistTrackTag tag)
        {
            ArgumentNullException.ThrowIfNull(tag);
            using IO.MemoryStream ms = new();
            SerializeInternal(tag, ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Serializes the specified <see cref="PlaylistTrackTag"/> instance to the specified stream.
        /// </summary>
        /// <param name="tag">The playlist track tag to serialize.</param>
        /// <param name="stream">The stream to write the serialized data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unwriteable.</exception>
        public static void SerializeTo(this PlaylistTrackTag tag , System.IO.Stream stream)
        {
            ArgumentNullException.ThrowIfNull(tag);
            ArgumentNullException.ThrowIfNull(stream);
            if (stream.CanWrite == false) {
                throw new ArgumentException("Stream is unwriteable.");
            }
            SerializeInternal(tag, stream);
        }
    }
}