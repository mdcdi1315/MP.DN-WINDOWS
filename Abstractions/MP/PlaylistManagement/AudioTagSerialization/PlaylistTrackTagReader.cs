
using System;
using System.Collections.Generic;

namespace MP.PlaylistManagement.AudioTagSerialization
{
    /// <summary>
    /// Defines a writer that can deserialize <see cref="PlaylistTrackTag"/> data written with the <see cref="PlaylistTrackTagWriter"/> class.
    /// </summary>
    public static class PlaylistTrackTagReader
    {
        private static IDictionary<PlaylistTrackTagKey, String> DecodeInternal(System.IO.Stream stream)
        {
            Dictionary<PlaylistTrackTagKey, String> dict;

            PLAYLISTTRACKTAGENTRY entry;

            PLAYLISTTRACKTAGHEADER header = stream.ReadStructure<PLAYLISTTRACKTAGHEADER>();

            if (!header.IsValidHeader) {
                throw new ArgumentException("This stream does not contain serialized playlist track tag data.");
            }

            if (header.Version != 1) {
                throw new ArgumentException($"Invalid track tag data version: {header.Version}.");
            }

            dict = new(header.NumberOfEntries);

            for (short I = 0; I < header.NumberOfEntries; I++)
            {
                entry = stream.ReadStructure<PLAYLISTTRACKTAGENTRY>();
                dict.Add(entry.Key, stream.ReadUTF16LEString(entry.ValueLength));
            }

            return dict;
        }

        /// <summary>
        /// Deserializes previously serialized track tag data from the specified stream and returns them in a dictionary object.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> to deserialize from. Only permission to read is required.</param>
        /// <returns>A new dictionary object containing the deserialized data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unreadable.</exception>
        public static IDictionary<PlaylistTrackTagKey, String> Deserialize(System.IO.Stream stream) 
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (stream.CanRead == false) {
                throw new ArgumentException("Stream was unreadable.");
            }
            return DecodeInternal(stream);
        }

        /// <summary>
        /// Deserializes previously serialized track tag data from the specified byte array and returns them in a dictionary object.
        /// </summary>
        /// <param name="array">The byte array to read data from.</param>
        /// <returns>A new dictionary object containing the deserialized data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> was <see langword="null"/>.</exception>
        public static IDictionary<PlaylistTrackTagKey , String> Deserialize(System.Byte[] array)
        {
            ArgumentNullException.ThrowIfNull(array);
            using (IO.MemoryStream mem = new(array)) {
                return DecodeInternal(mem);
            }
        }

    }
}