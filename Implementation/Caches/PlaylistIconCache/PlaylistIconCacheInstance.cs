

using Microsoft.IO;
using System.Drawing;
using MP.Graphics.Imaging;
using System.Collections.Generic;

namespace MP.Caches.PlaylistIconCache
{
    public sealed class PlaylistIconCacheInstance : CacheInstance<IconCacheWriter , IconCacheReader>
    {
        private struct PlaylistIconItem
        {
            public System.String IconFileName;
            public System.String IconSmallFileName;
        }

        private DirectoryInfo basedir;
        private Dictionary<System.String, PlaylistIconItem> icons;

        public PlaylistIconCacheInstance(DirectoryInfo di) 
        {
            if (di is null) {
                throw new System.ArgumentNullException(nameof(di));
            }
            icons = new(8);
            basedir = di;
            if (basedir.Exists == false) { throw new System.IO.DirectoryNotFoundException($"Cannot locate directory {di.FullName} ."); }
        }

        public override void LoadCache(IconCacheReader reader)
        {
            if (reader is null) { 
                throw new System.ArgumentNullException(nameof(reader));
            }
            icons.EnsureCapacity(icons.Count + reader.Count);
            foreach (var icr in reader.CachedItems)
            {
                icons.Add(icr.ReferencingPlaylistName, new() { IconFileName = icr.LargeIconFileName, IconSmallFileName = icr.SmallIconFileName });
            }
        }

        public override void SaveCache(IconCacheWriter writer)
        {
            if (writer is null) {
                throw new System.ArgumentNullException(nameof(writer));
            }
            PlaylistIconCacheItem[] items = new PlaylistIconCacheItem[icons.Count];
            System.Int32 I = 0;
            PlaylistIconItem itm;
            foreach (var k in icons.Keys)
            {
                itm = icons[k];
                items[I] = new() { ReferencingPlaylistName = k , LargeIconFileName = itm.IconFileName , SmallIconFileName = itm.IconSmallFileName };
            }
            writer.WriteEntries(items);
            items = null;
        }

        public System.Boolean RemovePlaylist(System.String name)
        {
            if (icons.TryGetValue(name , out var data))
            {
                try {
                    basedir.GetFile(data.IconFileName).Delete();
                    basedir.GetFile(data.IconSmallFileName).Delete();
                } catch { }
                return icons.Remove(name);
            }
            return false;
        }

        public System.String GetLargeIconFile(System.String name) => icons[name].IconFileName;

        public System.String GetLargeIconFileFullPath(System.String name) => Path.Join(basedir.FullName, icons[name].IconFileName);

        public System.String GetSmallIconFile(System.String name) => icons[name].IconSmallFileName;

        public System.String GetSmallIconFileFullPath(System.String name) => Path.Join(basedir.FullName, icons[name].IconSmallFileName);

        public IEnumerable<System.String> PlaylistNames => icons.Keys;

        public System.Int32 EntriesCount => icons.Count;

        public override void ClearCacheEntries() => icons.Clear();

        public System.Boolean PlaylistRegistered(System.String name) => icons.ContainsKey(name);

        public void AddPlaylistIcon(System.String iconfile , System.String playlistname)
        {
            PlaylistIconItem item = new();
            try {
                icons.Add(playlistname, item);
            } catch (System.ArgumentException e) {
                throw new System.InvalidOperationException($"This playlist has already been registered!\nName: {playlistname}" , e);
            }
            try {
                System.String generatedlargename = MP.SystemInfo.Now.Ticks.ToString("x2");
                System.String generatedsmallname = generatedlargename + "-small.bmp";
                generatedlargename += ".bmp";
                using (var bm = new Bitmap(iconfile))
                {
                    using (var img = bm.GetThumbnailImage(32, 32, null, System.IntPtr.Zero))
                    {
                        img.Save(Path.Join(basedir.FullName, generatedsmallname), System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    using (var resultimg = bm.ToImage())
                    using (var fs = new FileStream(Path.Join(basedir.FullName, generatedlargename), FileMode.Create))
                    using (var bitmapwriter = new Graphics.Imaging.WindowsBitmap.RawBitmapWriter(resultimg))
                    {
                        bitmapwriter.Save(fs);
                    }
                }
                item.IconFileName = generatedlargename;
                item.IconSmallFileName = generatedsmallname;
                icons[playlistname] = item;
            } catch { 
                // On any exception, delete the added information.
                icons.Remove(playlistname);
            }
        }
    }
}