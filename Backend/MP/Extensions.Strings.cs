using Microsoft.Win32.SafeHandles;

namespace MP
{
    public static partial class Extensions
    {
        public static void Launch(this System.Uri uri)
        {
            System.String url = uri.ToString();
            if (System.String.IsNullOrEmpty(url)) { return; }
            System.String rurl;
            if (url.StartsWith("http://") || url.StartsWith("https://")) { rurl = url; } else { rurl = $"http://{url}"; }
            Interop.Shell32.ShellExecute(System.IntPtr.Zero, Interop.Shell32.ExecuteVerbs.Open, rurl, "", "", 9);
            System.Threading.Thread.Sleep(500);
        }

        public static unsafe SafeLibcMemoryHandle ToNativeUnicodeString(this System.String s)
        {
            if (s is null) { return null; }
            SafeLibcMemoryHandle mem = new((s.Length + 1) * sizeof(System.Char));
            System.Char* nativecharsp = (System.Char*)mem.MemoryPointer;
            mem[mem.MemoryLength - 2] = 0;
            mem[mem.MemoryLength - 1] = 0;
            for (System.Int32 I = 0; I < s.Length; I++ , nativecharsp++)
            {
                *nativecharsp = s[I];
            }
            return mem;
        }
    }
}
