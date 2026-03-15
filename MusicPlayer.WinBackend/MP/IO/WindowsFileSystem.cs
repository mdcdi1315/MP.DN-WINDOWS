
using System;
using MP.Collections;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.IO
{
    public sealed class WindowsFileSystem : FileSystem
    {
        public static string CreateFullPath_Static(FileSystemObject fso)
        {
            ArgumentNullException.ThrowIfNull(fso);
            SingleLinkedListBasedStack<System.String> s = new();
            bool f_found = false;
            FileSystemObject g = fso;
            while (g is not null)
            {
                if (g is File) {
                    if (f_found) {
                        throw new InvalidOperationException("Cannot specify a File object after a parent directory. Check your implementation details.");
                    } else {
                        f_found = true;
                    }
                }
                s.Push(g.Name);
                g = g.Parent;
            }
            System.Text.StringBuilder sb = new();
            while (s.TryPop(out var pc))
            {
                sb.Append(pc);
                sb.Append('\\');
            }
            return sb.ToString();
        }

        public override string CreateFullPath(FileSystemObject o) => CreateFullPath_Static(o);

        public override Directory GetDirectory([NotNull] string path) => new WindowsDirectory(path);

        [return: MaybeReturnEmptyCollectionButNeverNull]
        public override Drive[] GetDrives()
        {
            throw new System.NotImplementedException();
        }

        public override File GetFile([NotNull] string path) => new WindowsFile(null, path);
    }
}