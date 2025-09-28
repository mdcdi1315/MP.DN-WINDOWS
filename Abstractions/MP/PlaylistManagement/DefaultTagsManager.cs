
using System;
using MP.TagReading;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.PlaylistManagement
{
    /// <summary>
    /// Provides a 'default' implementation of the <see cref="TagsManager"/> class.
    /// </summary>
    public class DefaultTagsManager : TagsManager
    {
        /// <inheritdoc /> 
        public DefaultTagsManager() : base() { }

        /// <inheritdoc /> 
        public DefaultTagsManager(ICollection<KeyValuePair<PlaylistFile, PlaylistTrackTag>> tags) : base(tags) {}

        /// <summary>
        /// Provides a default implementation for determining a tag. <br />
        /// Overriding classes are free to provide their own implementations.
        /// </summary>
        /// <param name="file">The playlist file to determine it's tag.</param>
        /// <returns>The determined tag, or null if it does not have one.</returns>
        [return: MaybeNull]
        protected override PlaylistTrackTag DetermineTagImpl([DisallowNull] PlaylistFile file)
        {
            ITagReader reader = null;
            System.IO.Stream FDOut = null;
            try {
                FDOut = file.GetStream();
                FDOut.Position = 0;
                try { reader = new ID3V2DataReader(FDOut); } catch { }
                FDOut.Position = 0;
                if (reader is not null) { goto G_completed; }
                try { reader = new MP4AudioTagReader(FDOut); } catch { }
                if (reader is not null) { goto G_completed; }
                FDOut.Position = 0;
                try { reader = new FlacAudioTagReader(FDOut); } catch { }
                if (reader is not null) { goto G_completed; }
                FDOut.Position = 0;
                try { reader = new OggVorbisAudioTagReader(FDOut); } catch { }
            G_completed:
                if (reader is null) { return null; } // We cannot do something else , no tag exists
                System.String coverimage;
                System.Byte[] image = reader.Image;
                if (image is not null) {
                    coverimage = PlaylistImageRegistrar(reader.Image, file);
                } else {
                    coverimage = null;
                }
                return PlaylistTrackTag.From(PlaylistImageSupplier, reader, coverimage);
            } catch (Exception e) {
                DebugProvider.WriteLine($"DefaultTagsManager: Cannot determine the tag due to a fatal error: {e}");
            } finally { 
                FDOut?.Dispose(); 
            }
            return null;
        }
    }
}