
namespace Microsoft.IO
{
    /// <summary>
    /// Defines common and handy file system extensions.
    /// </summary>
    public static class FileSystemExtensions
    {
        public static DirectoryInfo GetSubDirectory(this DirectoryInfo directory , System.String subdir)
            => new(Path.Join(directory.FullName , subdir));

        public static FileInfo CreateFile(this DirectoryInfo directory , System.String name)
        {
            System.String path = Path.Join(directory.FullName, name);
            FileInfo fi = new FileInfo(path);
            using (fi.Create()) { }
            return fi;
        }

        public static FileStream CreateFileStream(this DirectoryInfo directory , System.String name)
        {
            System.String path = Path.Join(directory.FullName, name);
            return new FileStream(path , FileMode.Create);
        }

        public static FileStream OpenReadFileStream(this DirectoryInfo directory , System.String name)
        {
            System.String path = Path.Join(directory.FullName, name);
            return new FileStream(path, FileMode.Open , FileAccess.Read);
        }

        public static FileInfo GetFile(this DirectoryInfo directory , System.String name)
        {
            FileInfo created = new(Path.Join(directory.FullName, name));
            if (created is null || created.Exists == false) { return null; }
            return created;
        }

        public static System.Boolean FileExists(this DirectoryInfo directory, System.String file) => File.Exists(Path.Join(directory.FullName, file));

        public static System.String GetFileFullPath(this DirectoryInfo directory , System.String name)
        {
            FileInfo created = new(Path.Join(directory.FullName, name));
            if (created is null || created.Exists == false) { throw new System.IO.FileNotFoundException($"The file with name {name} cannot be found in directory {directory.FullName}." , name); }
            return created.FullName;
        }

        public static void CopyTo(this DirectoryInfo directory , DirectoryInfo outdir)
        {
            if (outdir is null || outdir.Exists == false) { throw new System.IO.DirectoryNotFoundException("The target directory was not found."); }
            foreach (var inf in directory.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly)) 
            {
                if (inf is DirectoryInfo di) {
                    di.CopyTo(outdir.CreateSubdirectory(di.Name));
                } else if (inf is FileInfo fi) {
                    fi.CopyTo(outdir);
                }
            }
        }
    
        public static void CopyTo(this FileInfo file , DirectoryInfo outdir)
        {
            FileStream source = null;
            FileStream target = null;
            try
            {
                source = file.OpenRead();
                target = outdir.CreateFileStream(file.Name);
                source.Position = 0;
                target.Position = 0;
                source.CopyTo(target);
            } finally {
                source?.Dispose();
                source = null;
                target?.Dispose();
                target = null;
            }
        }
    
        public static void CopyTo(this FileInfo file , System.String path)
        {
            System.Boolean isdir = Directory.Exists(path);
            System.String pathfinal = isdir ? Path.Join(path, file.Name) : path;
            FileStream source = null;
            FileStream target = null;
            try {
                source = file.OpenRead();
                target = new FileStream(pathfinal , FileMode.Create);
                source.Position = 0;
                target.Position = 0;
                source.CopyTo(target);
            } finally {
                source?.Dispose();
                source = null;
                target?.Dispose();
                target = null;
            }
        }

        public static System.Boolean IsEncrypted(this FileInfo file) => File.IsEncrypted(file.FullName);

        public static System.Boolean IsEncryptable(this FileInfo file) => File.IsEncryptableFile(file.FullName);
    }

}

