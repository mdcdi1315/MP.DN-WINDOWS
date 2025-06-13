using System;
using MP.Random;
using Microsoft.IO;
using System.Collections.Generic;

namespace MP
{
    internal static class CreatePlaylistArchiveInfrastracture
    {
        private static System.Byte threadopctoken;

        public enum Operation : System.Byte
        {
            FindFiles,
            DetermineTags,
            ArchiveFiles,
            ReportMaxProgress,
            Fail
        }

        public sealed class OperationEventArgs : System.EventArgs
        {
            public Operation Operation;
            public System.String OperationData;
            public System.Int64 CurrentProgress;
        }

        private static void CurrentOperationDummyMethod(System.Object send, OperationEventArgs e) { }

        static CreatePlaylistArchiveInfrastracture()
        {
            CurrentOperation = new(CurrentOperationDummyMethod);
            GenerateThreadID();
        }

        internal static void GenerateThreadID() {
            Xoroshiro256PlusPlus gen = new();
            gen.Init(3978);
            gen.Next();
            System.Int32 rolls = gen.NextInRange(4, 250);
            for (System.Int32 I = 0; I < rolls; I++) { gen.Next(); }
            threadopctoken = gen.Next().ToByte();
            gen = null;
            CancellationToken = new OperationsTasksCancellationToken(threadopctoken);
        }

        public static Threading.ICancellationToken<System.Byte> CancellationToken;
        public static event System.EventHandler<OperationEventArgs> CurrentOperation;

        public static System.Boolean IsValidCancellationToken => CancellationToken.IsValid(threadopctoken);

        private static void SendEvent(Operation op, System.String info , System.Int64 cp = 0) => CurrentOperation.Invoke(null, new() { Operation = op, OperationData = info , CurrentProgress = cp });

        private static void ReportMaxProgress(System.Int64 maxp) => CurrentOperation.Invoke(null, new() { Operation = Operation.ReportMaxProgress , CurrentProgress = maxp });

        public static IEnumerable<IPlaylistFile> FindAvailableFiles(IPlaylist playlist)
        {
            IPlaylistFile track;
            System.Int32 fd = 0 , files = playlist.TracksContained.Count;
            SendEvent(Operation.FindFiles, $"Tracking {files} files...");
            ReportMaxProgress(files);
            System.Threading.Thread.Sleep(1000);
            for (System.Int32 I = 0; I < files && CancellationToken.IsValid(threadopctoken); I++)
            {
                track = playlist.TracksContained[I];
                SendEvent(Operation.FindFiles, $"Testing file {track.Name} ..." , I-1);
                if (track is null) { continue; }
                if (track.Exists) { fd++; yield return track; }
            }
            if (fd == 0) { SendEvent(Operation.Fail, "No files could be loaded. Archive creation failed."); }
            SendEvent(Operation.FindFiles, $"{fd} of {files} were available. These files will be only archived.");
        }

        public static void DetermineTags(IPlaylist playlist , IList<IPlaylistFile> found)
        {
            ReportMaxProgress(found.Count);
            SendEvent(Operation.DetermineTags, $"Determining {found.Count} tags...");
            IPlaylistFile file;
            for (System.Int32 I = 0; I < found.Count && CancellationToken.IsValid(threadopctoken); I++)
            {
                file = found[I];
                SendEvent(Operation.DetermineTags, $"Determining tag for {file.Name} ..." , I-1);
                playlist.DetermineAudioTagAndAddToTagList(file);
            }
            SendEvent(Operation.DetermineTags, "Tag determination ended.");
        }

        public static void ArchiveFiles(TrackPlayList playlist , IList<IPlaylistFile> found , System.String fullpath , ArchivedPlaylistAttributeCollection attributes , System.String iconcachefilefullpath)
        {
            FileStream FS = null;
            MP.Archiving.MusicPlayerArchiveWriter ZO = null;
            MP.Archiving.ArchiveEntryCompression cmp = Archiving.ArchiveEntryCompression.Store;
            System.Int32 I;
            if ((I = attributes.IndexOf("CompressionMethod")) > -1)
            {
                switch ((attributes[I].Value ?? String.Empty).ToLower())
                {
                    case "store":
                        cmp = Archiving.ArchiveEntryCompression.Store;
                        break;
                    case "gzip" or "gz":
                        cmp = Archiving.ArchiveEntryCompression.GZip;
                        break;
                    case "bzip2" or "bz":
                        cmp = Archiving.ArchiveEntryCompression.BZip2;
                        break;
                    default:
                        CancellationToken.Invalidate();
                        SendEvent(Operation.Fail, "Operation failed because the compression method specified is not one of the valid values.");
                        break;
                }
            }
            try {
                FS = new(fullpath, FileMode.Create);
                ZO = new(FS);
                ZO.IsStreamOwner = false;
                ReportMaxProgress(found.Count);
                SendEvent(Operation.ArchiveFiles, $"Archiving {found.Count} tracks...");
                IPlaylistFile file;
                for (I = 0; I < found.Count && CancellationToken.IsValid(threadopctoken); I++)
                {
                    file = found[I];
                    SendEvent(Operation.ArchiveFiles, $"Archiving {file.Name} ...." , I-1);
                    Archiving.ArchiveEntry ae = new() { 
                        Compression = cmp, 
                        EntryPath = $"Data/{file.Name}",
                        CreationTime = file.CreationTimeUtc,
                        LastWriteTime = file.LastWriteTimeUtc,
                        Length = file.Length,
                        Type = Archiving.ArchiveEntryType.File
                    };
                    ae.SetCustomAttribute("CMT0", $"EDT_FILE_{file.Name}");
                    ZO.RegisterEntry(ae);
                    using (System.IO.Stream FT = file.GetStream()) { ZO.ProvideFileDataFromStream(FT); }
                    ZO.CloseEntry();
                    System.Threading.Thread.Sleep(300);
                }
                if (CancellationToken.IsValid(threadopctoken) == false) { return; }
                SendEvent(Operation.ArchiveFiles, "Adding playlist index....");
                using (MemoryStream rms = new())
                {
                    playlist.CreateArchivedPlaylist(rms, found , attributes , iconcachefilefullpath);
                    rms.Position = 0;
                    Archiving.ArchiveEntry ae = new() {
                        Compression = Archiving.ArchiveEntryCompression.GZip,
                        EntryPath = $"playlist.mpbpl",
                        CreationTime = SystemInfo.Now,
                        Length = rms.Length,
                        Type = Archiving.ArchiveEntryType.File
                    };
                    ae.SetCustomAttribute("CMT0", "Playlist file");
                    ZO.RegisterEntry(ae);
                    ZO.ProvideFileDataFromStream(rms);
                    ZO.CloseEntry();
                }
                SendEvent(Operation.ArchiveFiles, "Finishing archive....");
            } catch (System.Exception ex) {
                CancellationToken.Invalidate();
                SendEvent(Operation.Fail, $"Operation failed due to {ex.GetType().Name} :\n{ex}");
                return;
            } finally {
                System.Boolean valid = CancellationToken.IsValid(threadopctoken);
                if (valid) { SendEvent(Operation.ArchiveFiles, "Closing Archive Instance..."); }
                ZO?.Dispose();
                ZO = null;
                if (valid) { SendEvent(Operation.ArchiveFiles, "Closing Archive File..."); }
                FS?.Dispose();
                FS = null;
                if (valid == false) { try { System.IO.File.Delete(fullpath); } catch { } }
                GC.Collect(2 , GCCollectionMode.Forced , true);
            }
        }
    }
}
