
using System;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.PlaylistManagement
{
    /// <summary>
    /// Defines the audio tags manager, a playlist service for managing audio tags.
    /// </summary>
    public abstract class TagsManager
    {
        private ImageRegistrar imageregistrar;
        private ImageSupplier imagesupplier;
        private readonly Dictionary<System.String, PlaylistTrackTag> tags;

        /// <summary>
        /// Creates a new and default instance of the <see cref="TagsManager"/> class.
        /// </summary>
        public TagsManager()
        {
            tags = new(10);
            imageregistrar = null;
            imagesupplier = null;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TagsManager"/> class from existing tag entries.
        /// </summary>
        /// <param name="tags">The loaded tag instances as a collection containing the playlist file as the key and a <see cref="PlaylistTrackTag"/> as the value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tags"/> was <see langword="null"/>.</exception>
        public TagsManager(ICollection<KeyValuePair<PlaylistFile , PlaylistTrackTag>> tags)
        {
            ArgumentNullException.ThrowIfNull(tags);
            this.tags = new(tags.Count);
            imageregistrar = null;
            imagesupplier = null;
            foreach (var kvp in tags) {
                this.tags.Add(kvp.Key.Name , kvp.Value);
            }
        }

        /// <summary>
        /// Gets a cached audio tag for the specified playlist file. <br />
        /// If the tag does not exist, <see cref="KeyNotFoundException"/> is thrown.
        /// </summary>
        /// <param name="file">The <see cref="PlaylistFile"/> to return tag information for.</param>
        /// <returns>The cached audio tag for <paramref name="file"/>. If <see langword="null"/>, it indicates that no audio tag exists for the file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> was <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">The <paramref name="file"/> passed does not contain an audio tag.</exception>
        [return: MaybeNull]
        public PlaylistTrackTag Get(PlaylistFile file)
        {
            ArgumentNullException.ThrowIfNull(file);
            return tags[file.Name];
        }

        /// <summary>
        /// Determines the audio tag for the specified file. <br />
        /// The tag will be overwritten if there was one existing before.
        /// </summary>
        /// <param name="pf">The playlist file to determine it's audio tag.</param>
        /// <returns>The determined audio tag for the file. Can be also retrieved later by using the <see cref="Get(PlaylistFile)"/> method.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pf"/> was <see langword="null"/>.</exception>
        [return: MaybeNull]
        public PlaylistTrackTag DetermineTag(PlaylistFile pf)
        {
            ArgumentNullException.ThrowIfNull(pf);
            return DetermineTagShared(pf);
        }

        /// <summary>
        /// Provides the implementation for getting a <see cref="PlaylistTrackTag"/> instance from the specified playlist file. <br />
        /// This function must load the data stream and use the playlist tag decoders to decode the file.
        /// </summary>
        /// <param name="file">The playlist file to determine it's tag.</param>
        /// <returns>The determined tag. If the file does not have an audio tag, this should return <see langword="null"/> rather than failing with an exception.</returns>
        [return: MaybeNull]
        [MustNotReportException]
        protected abstract PlaylistTrackTag DetermineTagImpl([DisallowNull] PlaylistFile file);

        /// <summary>
        /// Determines the tags for the specified playlist file(s) and adds them to the current manager instance.
        /// </summary>
        /// <param name="files">The playlist files to determine their tags.</param>
        public void DetermineTags(IEnumerable<PlaylistFile> files)
        {
            if (files is ICollection<PlaylistFile> pf) {
                tags.EnsureCapacity(tags.Count + pf.Count);
            }
            foreach (var file in files) {
                DetermineTagShared(file);
            }
        }

        /// <summary>
        /// Use this method to clean the manager's cache and to re-determine the tags contained in the playlist.
        /// </summary>
        public void Clear() => tags.Clear();

        /// <summary>
        /// Provides the <see cref="ImageSupplier"/> that does provide the playlist cover images for the tags.
        /// </summary>
        public ImageSupplier PlaylistImageSupplier
        {
            get => imagesupplier;
            set {
                ArgumentNullException.ThrowIfNull(value);
                imagesupplier = value;
            }
        }

        /// <summary>
        /// Provides the <see cref="ImageRegistrar"/> that does register new cover images and associates them with the current playlist. <br />
        /// This function does also remove cover images previously registered by passing the image parameter a null value.
        /// </summary>
        public ImageRegistrar PlaylistImageRegistrar
        {
            get => imageregistrar;
            set {
                ArgumentNullException.ThrowIfNull(value);
                imageregistrar = value;
            }
        }

        [System.Diagnostics.StackTraceHidden]
        private PlaylistTrackTag DetermineTagShared(PlaylistFile pf)
        {
            PlaylistTrackTag ptt = DetermineTagImpl(pf);
            if (tags.TryGetValue(pf.Name, out var tag) && tag is not null) {
                // Destroy the tag if found successfully.
                // Additionally unregister the cover image.
                PlaylistImageRegistrar(null, pf);
                tag.Dispose();
            }
            tags[pf.Name] = ptt;
            return ptt;
        }
    }
}
