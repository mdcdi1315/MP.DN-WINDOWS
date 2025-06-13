

using System.Collections.Generic;

namespace MP
{
    public static class NativeLibraryLoader
    {
        private static List<System.IntPtr> adddllcookies;
        static NativeLibraryLoader() { adddllcookies = new(5); }

        public static void AddDLLSearchDirectory(System.String path)
        {
            if (System.String.IsNullOrEmpty(path)) { throw new System.ArgumentNullException(nameof(path)); }
            adddllcookies.Add(Interop.Kernel32.AddDllDirectory(path));
        }

        public static void ClearDLLSearchDirectories()
        {
            foreach (var dir in adddllcookies) 
            {
                Interop.Kernel32.RemoveDllDirectory(dir);
            }
            adddllcookies.Clear();
        }
    }
}