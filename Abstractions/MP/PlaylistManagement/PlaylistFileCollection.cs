
using System;
using MP.Random;
using MP.Collections;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.PlaylistManagement
{
    /// <summary>
    /// Defines services as well the base interface for managing files of a playlist object.
    /// </summary>
    public abstract class PlaylistFileCollection : ICollection<PlaylistFile> , IGettableSettable<System.Int32 , PlaylistFile>
    {
        private volatile CurrentPlaylistFileData current; // Modify this through Interlocked only!!
        private readonly ArrayBasedList<PlaylistFile> list;

        private sealed class CurrentPlaylistFileData
        {
            public System.Int32 Index;
            public PlaylistFile File;

            public CurrentPlaylistFileData(int index, PlaylistFile file)
            {
                Index = index;
                File = file;
            }
        }

        /// <summary>
        /// Constructs a default instance of the <see cref="PlaylistFileCollection"/> class instance.
        /// </summary>
        public PlaylistFileCollection() {
            list = new(10);
            current = null;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="PlaylistFileCollection"/> class instance from the specified pre-constructed playlist file entries.
        /// </summary>
        /// <param name="files">The playlist file entries to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="files"/> is <see langword="null"/>.</exception>
        public PlaylistFileCollection(IEnumerable<PlaylistFile> files) {
            list = new(files);
            current = null;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="PlaylistFileCollection"/> class instance from the specified 
        /// pre-constructed playlist file entries, plus specifying the current track to use.  <br />
        /// This is to be used to load back data from a data storage. <br />
        /// If <paramref name="index"/> is less than 0 then it is presumed that no current track is selected.
        /// </summary>
        /// <param name="files">The playlist file entries to add.</param>
        /// <param name="index">The ordinal of the current track.</param>
        /// <exception cref="ArgumentNullException"><paramref name="files"/> is <see langword="null"/>.</exception>
        public PlaylistFileCollection(IEnumerable<PlaylistFile> files , int index) {
            list = new(files);
            current = index > -1 ? new(index, list[index]) : null;
        }

        /// <summary>
        /// Gets a playlist file at <paramref name="index"/>. <br />
        /// The setter of this property is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="index">The numeric ordinal in the backing collection where this item is located to.</param>
        /// <returns>The specified playlist file at <paramref name="index"/>.</returns>
        /// <exception cref="NotSupportedException">Setting directly a playlist file is not supported.</exception>
        public PlaylistFile this[int index] 
        {
            get => list[index];
            set => throw new NotSupportedException("This operation is not supported."); 
        }

        /// <summary>
        /// Gets the number of playlist items contained in this playlist file collection.
        /// </summary>
        public int Count => list.Count;

        /// <summary>
        /// Gets a value whether the playlist items are read-only (that is , you are using a read-only playlist)
        /// </summary>
        public abstract bool IsReadOnly { get; }

        /// <summary>
        /// Adds a new playlist file to the collection of valid playlist tracks.
        /// </summary>
        /// <remarks>
        /// Adding new playlist files while playback is performed should 
        /// be considered a thread-safe operation, because the index 
        /// of the currently playing track remains untouched.
        /// </remarks>
        /// <param name="item">The playlist file to add. Must be a valid instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public void Add(PlaylistFile item)
        {
            ArgumentNullException.ThrowIfNull(item);
            list.Add(item);
        }

        /// <summary>
        /// Removes all the playlist files from the current collection object. <br />
        /// Plus, the current selection is also cleared. <br />
        /// Throws if the playlist happens to be read-only.
        /// </summary>
        /// <exception cref="InvalidOperationException">The playlist is read-only; that is, a playlist loaded from a remote location or from somewhere that no write permissions are not inherently allowed.</exception>
        public void Clear()
        {
            if (IsReadOnly) { throw new InvalidOperationException("The list is read-only."); }
            list.Clear();
            Interlocked.Exchange(ref current, null);
        }

        /// <summary>
        /// Determines whether the specified file is part of this playlist. <br />
        /// <see langword="false"/> is additionally returned if <paramref name="item"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="item">The playlist file to search.</param>
        /// <returns>A value whether the specified file does belong to the playlist associated with this object.</returns>
        public bool Contains(PlaylistFile item) => item is not null && list.Contains(item);

        /// <summary>
        /// Copies the entire <see cref="PlaylistFileCollection"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="PlaylistFileCollection"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="PlaylistFileCollection"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        public void CopyTo(PlaylistFile[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        /// <summary>
        /// Gets an enumerator able to enumerate through the playlist entries of this <see cref="PlaylistFileCollection"/> class.
        /// </summary>
        /// <returns>An enumerator instance returning <see cref="PlaylistFile"/> instances belonging to the current object.</returns>
        public IEnumerator<PlaylistFile> GetEnumerator() => list.GetEnumerator();

        /// <summary>
        /// Removes the specified playlist file from the current object.
        /// </summary>
        /// <param name="item">The playlist file to remove.</param>
        /// <returns>A value whether the playlist file was removed from this object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> was <see langword="null"/>.</exception>
        public bool Remove(PlaylistFile item)
        {
            ArgumentNullException.ThrowIfNull(item);
            if (item.Equals(current.File)) { return false; } // Removing the currently selected playlist item is invalid operation.
            if (list.Remove(item)) {
                OnRemove(item);
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Removes the specified playlist file by index. <br />
        /// If it happens the file to remove to be the currently selected file, the selection is cleared.
        /// </summary>
        /// <param name="index">The index of the playlist file to remove.</param>
        public void RemoveAtIndex(int index)
        {
            if (current is not null && current.Index == index) { Interlocked.Exchange(ref current, null); }
            PlaylistFile pf = list[index];
            list.RemoveAt(index);
            OnRemove(pf);
        }

        /// <summary>
        /// Sets the current playlist track of the specified index, and it returns the current and the previous current playlist file. 
        /// </summary>
        /// <param name="index">The index of the new track to select as the current one.</param>
        /// <param name="old">An object reference to an <see cref="PlaylistFile"/> to retrieve the previously current playlist file. If no previous current file was set , it will be <see langword="null"/>.</param>
        /// <returns>The current playlist file that was selected. This is like doing <see cref="this[int]"/> with <paramref name="index"/> as the value of the parameter.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> was out of the collection bounds.</exception>
        public PlaylistFile SetCurrentPlaylistTrack(int index , [MaybeNull] out PlaylistFile old)
        {
            if (index < 0 || index >= list.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), "Playlist index was invalid.");
            }
            CurrentPlaylistFileData cpfd;
            old = (cpfd = Interlocked.Exchange(ref current, new(index, list[index]))) is not null ? cpfd.File : null;
            return current.File;
        }

        /// <summary>
        /// Sets the current playlist track to the next item in the playlist. <br />
        /// If the current track item is the last item, the index resets back to zero, and the first item is returned.
        /// </summary>
        /// <param name="old">An object reference to an <see cref="PlaylistFile"/> to retrieve the previously current playlist file. If no previous current file was set , it will be <see langword="null"/>.</param>
        /// <returns>The current playlist file that was selected.</returns>
        public PlaylistFile SetCurrentPlaylistTrackToNextItem([MaybeNull] out PlaylistFile old)
        {
            int count = list.Count;
            if (count == 0) { old = null; return null; }
            int index = current is null ? 0 : current.Index;
            CurrentPlaylistFileData cpfd;
            if (++index >= count) { index = 0; }
            old = (cpfd = Interlocked.Exchange(ref current, new(index, list[index]))) is not null ? cpfd.File : null;
            return current.File;
        }

        /// <summary>
        /// Retrieves the current playlist track and it's ordinal within the playlist. <br />
        /// If not a playlist file is selected for this object, <see langword="null"/> is returned and <paramref name="index"/> will contain the value -1.
        /// </summary>
        /// <param name="index">The ordinal of the current file in the playlist.</param>
        /// <returns>The current playlist file.</returns>
        [return: MaybeNull]
        public PlaylistFile GetCurrentPlaylistTrack(out int index)
        {
            if (current is null) { index = -1; return null; }
            index = current.Index;
            return current.File;
        }

        /// <summary>
        /// Sets the current playlist track randomly. This is like shuffling the playlist, but with by a random value (that means, tracks can be re-selected).
        /// </summary>
        /// <param name="random">The random source instance to use.</param>
        /// <param name="old">An object reference to an <see cref="PlaylistFile"/> to retrieve the previously current playlist file. If no previous current file was set , it will be <see langword="null"/>.</param>
        /// <returns>The current playlist file that was selected.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> was <see langword="null"/>.</exception>
        public PlaylistFile SetCurrentPlaylistTrackRandomly(IRandomSource random, [MaybeNull] out PlaylistFile old)
        {
            ArgumentNullException.ThrowIfNull(random);
            int count = list.Count;
            if (count == 0) { old = null; return null; }
            int index = random.NextInRange(0 , count);
            CurrentPlaylistFileData cpfd;
            old = (cpfd = Interlocked.Exchange(ref current, new(index, list[index]))) is not null ? cpfd.File : null;
            return current.File;
        }

        /// <summary>
        /// This method is called if any of the Remove methods are called. <br />
        /// This should be overriden by your implementing class if you need additional behavior to be performed just after the file is removed.
        /// </summary>
        /// <param name="file">The file that was removed.</param>
        protected virtual void OnRemove([DisallowNull] PlaylistFile file) { }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}