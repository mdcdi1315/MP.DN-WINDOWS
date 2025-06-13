
using System;
using Microsoft.IO;
using MP.ExceptionSystem;
using System.Collections.Generic;

namespace MP.M3UPlaylist
{
    public sealed class M3UPlaylistReader : IDisposable
    {
        private List<System.String> files;
        private System.String plname;
        private StreamReader sr;

        public M3UPlaylistReader(System.IO.Stream stream)
        {
            sr = new StreamReader(stream);
            if (sr.ReadLine() != "#EXTM3U")
            {
                throw new InvalidM3UPlaylistFormatException("This is not the M3U playlist format.");
            }
            Read();
        }

        [System.Diagnostics.StackTraceHidden]
        private void Read()
        {
            plname = sr.ReadLine();
            if (plname is null) {
                throw new InvalidM3UPlaylistFormatException("The format ended unexpectedly.");
            }
            if (plname.StartsWith('#')) {
                plname = plname.Substring(1);
            } else {
                plname = null;
            }
            files = new(10);
            System.String fp;
            while ((fp = sr.ReadLine()) is not null)
            {
                if (System.String.IsNullOrWhiteSpace(fp)) {
                    throw new InvalidM3UPlaylistFormatException("The format was corrupted.");
                }
                files.Add(fp);
            }
            sr.Dispose();
            sr = null;
        }

        public System.String PlaylistName => plname;

        public IList<System.String> FilePaths => files;

        public System.String[] FilePathsAsArray => files.ToArray();

        /// <summary>
        /// Releases all the resources that the <see cref="M3UPlaylistReader"/> instance used.
        /// </summary>
        public void Dispose() 
        {
            sr?.Dispose();
            sr = null;
            plname = null;
            files?.Clear();
            files = null;
        }
    }
}