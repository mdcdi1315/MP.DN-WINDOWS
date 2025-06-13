
using MP;
using System.IO;
using Microsoft.Build.Utilities;

namespace MusicPlayer.BuildTasks
{
    public static class SharedUtilities
    {
        public static void LogErrorWithCode(this TaskLoggingHelper task , System.String code , System.String format , params System.Object[] replacements)
            => task.LogError("", code, "", "", "", 0, 0, 0, 0, format, replacements);

        public static void LogWarningWithCode(this TaskLoggingHelper task , System.String code, System.String format , params System.Object[] replacements)
            => task.LogWarning("", code, "", "", "", 0, 0, 0, 0, format, replacements);

        public static void CopyToDirectory(this FileInfo fi , DirectoryInfo directory)
        {
            System.IO.FileStream fss = fi.OpenRead(), fst = null;
            try {
                fst = new(Path.Combine(directory.FullName, fi.Name), FileMode.Create);
                fss.DirectCopyToStream(fst);
            } finally {
                fss.Dispose();
                fst.Dispose();
            }
        }
  
        public static void MoveToDirectory(this FileInfo fi , DirectoryInfo directory)
        {
            System.IO.FileStream fss = fi.OpenRead(), fst = null;
            try {
                fst = new(Path.Combine(directory.FullName, fi.Name), FileMode.Create);
                fss.DirectCopyToStream(fst);
            } finally {
                fss.Dispose();
                fst.Dispose();
            }
            fi.Delete();
        }
    }
}