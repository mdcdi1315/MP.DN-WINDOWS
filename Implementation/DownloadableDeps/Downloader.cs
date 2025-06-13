
using MP.Utilities;
using Microsoft.IO;
using MP.Threading;
using MP.Networking;
using System.IO.ManagedZip.Zip;
using System.Collections.Generic;

namespace MP.DownloadableDeps
{
    public sealed class Downloader
    {
        private DirectoryInfo di;
        private DownloadableDependency[] deps;

        private static void FailedDummyMethod(System.Exception ex) { }

        private static void CommonStringDummyMethod(System.String str) { }

        private static void DownloadProgressDummyMethod(System.Byte d) { }

        private static void ExtractProgressDummyMethod(System.Int32 d) { }

        public Downloader(DownloadableDependency maindep , DownloadableDependency[] childdeps)
        {
            var cmp = new DownloadDependencyComputer(maindep , childdeps);
            deps = cmp.ComputeDependencies();
            cmp = null;
            Failed = new(FailedDummyMethod);
            Succeeded = new(CommonStringDummyMethod);
            Progress = new(DownloadProgressDummyMethod);
            RequestInitiated = new(CommonStringDummyMethod);
            DownloadStarted = new(CommonStringDummyMethod);
            ExtractingFiles = new(ExtractProgressDummyMethod);
        }

        public DirectoryInfo ResultDirectory
        {
            get {
                if (di is null) {
                    di = new(SystemInfo.CurrentDirectory);
                }
                return di;
            }
            set {
                if (value is null) { return; }
                di = value;
            }
        }

        public DownloadableDependency[] DependenciesRequired => deps;

        public void ProcessAll(WinInetApplication appmain , DirectoryInfo tempdir , ICancellationToken<System.Byte> token)
        {
            if (appmain is null) { throw new System.ArgumentNullException(nameof(appmain)); }
            if (tempdir is null) { throw new System.ArgumentNullException(nameof(tempdir)); }
            tempdir.Create();
            di.Create();
            foreach (var dep in deps) 
            {
                if (token.IsValid(token.InstanceId) == false) { break; }
                RequestInitiated.Invoke(dep.Name);
                var connection = appmain.CreateNewGenericConnection(new() { URL = dep.DownloadURL });
                System.Int64 downloadsize;
                try {
                    connection.Submit();
                    downloadsize = connection.ContentLength;
                    if (dep.IsZipFile && (connection.ContentType != "application/zip" && connection.ContentType != "application/octet-stream"))
                    {
                        Failed.Invoke(new System.NotSupportedException($"Zip files must be marked with application/zip MIME type.\nInstead the MIME type returned was: {connection.ContentType}."));
                        return;
                    }
                } catch (System.Exception ex) { Failed.Invoke(ex); return; }
                if (token.IsValid(token.InstanceId) == false) { break; }
                DownloadStarted.Invoke(dep.Name);
                System.String temppathname = Path.Join(tempdir.FullName, SystemInfo.Now.Ticks.ToString("x2"));
                if (CopyToFileAndReportProgress(temppathname , connection.GetResultStream() , token) == false) {
                    connection.Dispose();
                    return; 
                }
                connection.Dispose();
                if (token.IsValid(token.InstanceId) == false) { break; }
                if (dep.SavePath.Type == PathDetailSaveType.File)
                {
                    File.Move(temppathname , Path.Join(di.FullName, dep.SavePath.RelativePath));
                } else if (dep.SavePath.Type == PathDetailSaveType.Directory && dep.IsZipFile)
                {
                    ExtractingFiles.Invoke(0);
                    if (ExtractZipFile(temppathname , di.CreateSubdirectory(dep.SavePath.RelativePath) , dep.ImportantFileList , token) == false) { return; }
                    try { File.Delete(temppathname); } catch {  }
                }
                if (token.IsValid(token.InstanceId) == false) { break; }
                Succeeded.Invoke(dep.Name);
                System.Threading.Thread.Sleep(800);
            }
        }

        private System.Boolean CopyToFileAndReportProgress(System.String fp , System.IO.Stream networkstream , ICancellationToken<System.Byte> token)
        {
            FileStream fsm = new(fp, FileMode.Create);
            try
            {
                System.Byte[] buffer = new System.Byte[4096];
                System.Int32 rb;
                System.Int64 progresscurrent = 0;
                while ((rb = networkstream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (token.IsValid(token.InstanceId) == false) { break; }
                    progresscurrent += rb;
                    fsm.Write(buffer, 0, rb);
                    Progress.Invoke((System.Byte)(progresscurrent * (100f / networkstream.Length)));
                }
            } catch (System.Exception ex) { 
                Failed.Invoke(ex); 
                return false; 
            } finally {
                networkstream.Dispose();
                fsm.Dispose();
            }
            return true;
        }

        private static System.Boolean IsValidEntryOrMatchesPattern(System.String zipentryname , IList<System.String> filekeeplist)
        {
            foreach (var entry in filekeeplist) 
            {
                if (entry.Equals(zipentryname , System.StringComparison.OrdinalIgnoreCase) ||
                    Microsoft.IO.Enumeration.FileSystemName.MatchesWin32Expression(entry, zipentryname)) { return true; }
            }
            return false;
        }

        private System.Boolean ExtractZipFile(System.String sourcezip , DirectoryInfo basedata , IList<System.String> filekeeplist , ICancellationToken<System.Byte> token)
        {
            filekeeplist ??= new List<System.String>();
            FileStream fsm = new(sourcezip, FileMode.Open) , temp;
            ZipInputStream zi = null;
            try {
                zi = new(fsm, 8192);
                ZipEntry ent;
                System.Int32 processed = 0;
                while ((ent = zi.GetNextEntry()) is not null)
                {
                    if (token.IsValid(token.InstanceId) == false) { break; }
                    ExtractingFiles.Invoke(processed);
                    if (ent.IsDirectory) {
                        processed++;
                        zi.CloseEntry();
                        continue;
                    }
                    if (filekeeplist.Count > 0 && IsValidEntryOrMatchesPattern(ent.Name , filekeeplist) == false) 
                    {
                        processed++;
                        zi.CloseEntry();
                        continue; 
                    }
                    if (ent.CanDecompress == false) { Failed.Invoke(new System.InvalidOperationException($"Cannot decompress a required entry.\nEntry name: {ent.Name}")); return false; }
                    if (token.IsValid(token.InstanceId) == false) { break; }
                    temp = basedata.CreateFileStream(Path.GetFileName(ent.Name));
                    try {
                        zi.CopyToExactly(temp, 4096, ent.Size);
                    } finally { temp.Dispose(); }
                    zi.CloseEntry();
                    processed++;
                }
            } catch (System.Exception ex) {
                Failed.Invoke(ex);
                return false;
            } finally {
                fsm.Dispose();
                zi?.Dispose();
                zi = null;
            }
            return true;
        }

        public event DownloadFailedDelegate Failed;

        public event DownloadSucceededDelegate Succeeded;

        public event DownloadReportProgressDelegate Progress;

        public event DownloadExtractingFilesDelegate ExtractingFiles;

        public event DownloadRequestInitiatedDelegate RequestInitiated;

        public event DownloadStartsDownloadDelegate DownloadStarted;
    }
}