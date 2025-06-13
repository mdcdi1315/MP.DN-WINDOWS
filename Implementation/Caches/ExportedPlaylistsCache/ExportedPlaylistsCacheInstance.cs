

using System.Collections.Generic;

namespace MP.Caches.ExportedPlaylistsCache
{
    public sealed class ExportedPlaylistCacheInstance : CacheInstance<ExportedPlaylistCacheWriter , ExportedPlaylistsCacheReader>
    {
        private const System.String PlaylistCacheFileName = "ExportedPlaylistNames.mpc";
        private List<System.String> plnames;
        private List<System.String> plfolders;

        public ExportedPlaylistCacheInstance() : base() {
            plnames = new(10);
            plfolders = new(10);
        }

        public override void LoadCache(ExportedPlaylistsCacheReader reader)
        {
            if (reader is null) { throw new System.ArgumentNullException(nameof(reader)); }
            foreach (var item in reader.Playlists) 
            {
                plnames.Add(item.PlaylistName);
                plfolders.Add(item.ReferencingFolderName);
            }
        }

        public override void SaveCache(ExportedPlaylistCacheWriter writer)
        {
            if (writer is null) { throw new System.ArgumentNullException(nameof(writer)); }
            System.Int32 idx = 0;
            foreach (var item in plnames) 
            {
                writer.WritePlaylist(new() { PlaylistName = item , ReferencingFolderName = plfolders[idx] });
                idx++;
            }
        }

        public override void ClearCacheEntries() => plnames.Clear();

        public void LoadCacheFromExportedDirectory(Microsoft.IO.DirectoryInfo basedir)
        {
            if (basedir is null) { throw new System.ArgumentNullException(nameof(basedir)); }
            Microsoft.IO.FileStream fsm = null;
            try
            {
                fsm = new(Microsoft.IO.Path.Join(basedir.FullName, PlaylistCacheFileName), Microsoft.IO.FileMode.Open, Microsoft.IO.FileAccess.Read, Microsoft.IO.FileShare.None);
                LoadCacheFromStream(fsm);
            } catch (System.IO.FileNotFoundException)
            { 
                // Do not do anything in such case.
            } finally {
                fsm?.Dispose();
            }
        }

        public void SaveCacheToExportedDirectory(Microsoft.IO.DirectoryInfo basedir)
        {
            if (basedir is null) { throw new System.ArgumentNullException(nameof(basedir)); }
            Microsoft.IO.FileStream fsm = null;
            try {
                fsm = new(Microsoft.IO.Path.Join(basedir.FullName, PlaylistCacheFileName), Microsoft.IO.FileMode.Create, Microsoft.IO.FileAccess.Write, Microsoft.IO.FileShare.None);
                SaveCacheToStream(fsm);
            } finally {
                fsm?.Dispose();
            }
        }

        public void AddPlaylistNameAndFolder(System.String name , System.String foldername)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name)); }
            if (System.String.IsNullOrEmpty(foldername)) { throw new System.ArgumentNullException(nameof(foldername)); }
            plnames.Add(name);
            plfolders.Add(foldername);
        }

        public System.Int32 CountOfExportedPlaylists => plnames.Count;

        public System.String GetPlaylistNameAt(System.Int32 index) => plnames[index];

        public System.String GetPlaylistFolderAt(System.Int32 index) => plfolders[index];

        public System.String GetPlaylistFolder(System.String name)
        {
            System.Int32 idx = plnames.IndexOf(name);
            if (idx > -1)
            {
                return plfolders[idx];
            }
            return null;
        }

        public IEnumerable<System.String> PlaylistNames => plnames;

        public System.Boolean IsExportedPlaylist(System.String plname) => plnames.Contains(plname);

        public void RemovePlaylistName(System.String name)
        {
            System.Int32 idx = plnames.IndexOf(name);
            if (idx > -1)
            {
                plfolders.RemoveAt(idx);
                plnames.RemoveAt(idx);
            }
        }
    }
}