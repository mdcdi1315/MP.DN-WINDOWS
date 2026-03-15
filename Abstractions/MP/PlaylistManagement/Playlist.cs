
using System;
using System.Threading;

namespace MP.PlaylistManagement
{
    /// <summary>
    /// Provides an abstraction for a playlist, that is 
    /// a collection of tracks that can be played back. <br /> <br />
    /// 
    /// Typical lifecycle and usage of a <see cref="Playlist"/> instance: <br />
    /// Playlist instances are intermediary objects providing services for managing a set of tracks, learning the tracks
    /// they do contain as well as various metadata defined through them. <br />
    /// When done using it, it should be destroyed by calling the <see cref="Dispose()"/> method on it. <br />
    /// <see cref="Playlist"/> instances are designed so that they can be created from any location.  <br />
    /// As such, they should not expose any internal information (such as file format data and implementation). <br /> <br />
    /// 
    /// Playlist objects that can be reconstructed back should extend instead the <see cref="SaveablePlaylist"/> class. <br />
    /// Music player implementations using this subsystem can detect with that way that they must save the playlist before exiting.
    /// </summary>
    public abstract class Playlist : IDisposable
    {
        /// <summary>
        /// Gets the files (tracks) that are comprising this playlist.
        /// </summary>
        public abstract PlaylistFileCollection Files { get; }

        /// <summary>
        /// Gets the metadata that are comprising this playlist.
        /// </summary>
        public abstract PlaylistMetadata Metadata { get; }

        /*
        /// <summary>
        /// Gets a class instance that is able to manage the audio tags that are associated with the playlist's files.
        /// </summary>
        public abstract TagsManager Tags { get; }
        */

        /// <summary>
        /// Gets a value whether the audio tags were determined for this playlist object or not. <br />
        /// Usually this will be a value from the playlist metadata that will identify this.
        /// </summary>
        public abstract System.Boolean AudioTagsDetermined { get; }

        /// <summary>
        /// Gets the informal name of the playlist. <br />
        /// Usually this will be a value retrieved from the playlist's metadata.
        /// </summary>
        public abstract System.String Name { get; }

        /// <summary>
        /// Gets or sets the playlist's cover image. <br />
        /// Also loaded on demand, but it is an internal aspect of the playlist and should be irrelevant to the data blob.
        /// </summary>
        public abstract System.Byte[] Cover { get; set; }

        /// <summary>Disposes this playlist instance.</summary>
        /// <param name="disposing">A value whether the unmanaged resources should be disposed as well.</param>
        protected virtual void Dispose(bool disposing) { }

        /// <summary>
        /// Default finalizer for <see cref="Playlist"/> instances.
        /// </summary>
        ~Playlist()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
             Dispose(disposing: false);
        }

        /// <summary>
        /// Disposes this <see cref="Playlist"/> instance. <br />
        /// Extending classes should declare their disposal code by overriding the <see cref="Dispose(bool)"/> method.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Monitor.Enter(this);
            try {
                Dispose(disposing: true);
            } finally {
                GC.SuppressFinalize(this);
                Monitor.Exit(this);
            }
        }
    }

}