
using MP;
using System;
using MP.Annotations;
using MP.NativeInterop.Windows;
using System.Runtime.InteropServices;

partial class Interop
{
    unsafe partial class Kernel32
    {
        [Flags]
        public enum LoadLibraryFlags : System.UInt32
        {
            None = 0,
            DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
            LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
            LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
            LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,
            LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,
            LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
            LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
            LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
        }

        [Flags]
        public enum GetModuleHandleFlags : System.UInt32
        {
            None = 0,
            GET_MODULE_HANDLE_EX_FLAG_PIN = 0x00000001,
            GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT = 0x00000002,
            GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS = 0x00000004,
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "AddDllDirectory" , ExactSpelling = true)]
        private static extern System.IntPtr AddDllDirectory_Native(System.Char* path);

        [DllImport(Libraries.Kernel32 , EntryPoint = "RemoveDllDirectory" , ExactSpelling = true)]
        public static extern BOOL RemoveDllDirectory(System.IntPtr cookie);

        public static System.IntPtr AddDllDirectory(System.String path) 
        {
            fixed (System.Char* psrc = path) {
                return AddDllDirectory_Native(psrc);
            }
        }

        [AssignsLastError]
        [DllImport(Libraries.Kernel32 , EntryPoint = "LoadLibraryExW", ExactSpelling = true)]
        private static extern System.IntPtr LoadLibraryEx_Native(System.Char* libname, System.IntPtr hfnotused , LoadLibraryFlags flags);

        public static System.IntPtr LoadLibraryEx(System.String libraryname , LoadLibraryFlags flags) 
        {
            fixed (System.Char* plib = libraryname) {
                return LoadLibraryEx_Native(plib, System.IntPtr.Zero , flags);
            }
        }

        [AssignsLastError]
        [DllImport(Libraries.Kernel32 , ExactSpelling = true)]
        public static extern BOOL FreeLibrary(System.IntPtr library);

        [AssignsLastError]
        [DllImport(Libraries.Kernel32, EntryPoint = "GetProcAddress", ExactSpelling = true)]
        private static extern void* GetProcAddress_Native(System.IntPtr library, System.Char* pprocname);

        [AssignsLastError]
        [DllImport(Libraries.Kernel32, EntryPoint = "GetModuleFileNameW", ExactSpelling = true)]
        private static extern System.UInt32 GetModuleFileName_Native(System.IntPtr hmod, System.Char* buffer, System.UInt32 bufsize);

        [AssignsLastError]
        [DllImport(Libraries.Kernel32, EntryPoint = "GetModuleHandleExW", ExactSpelling = true)]
        private static extern BOOL GetModuleHandleEx_Native(GetModuleHandleFlags flags, System.Char* plibname, System.IntPtr* phandle);

        public static void* GetProcAddress(System.IntPtr lib , System.String procName) 
        {
            fixed (System.Char* ppc = procName) {
                return GetProcAddress_Native(lib , ppc);
            }
        }
        
        public static BOOL GetModuleHandleEx(GetModuleHandleFlags flags , System.String name , out System.IntPtr pmodule)
        {
            System.IntPtr hmod;
            fixed (System.Char* plbn = name)
            {
                BOOL ret = GetModuleHandleEx_Native(flags, plbn, &hmod);
                pmodule = hmod;
                return ret;
            }
        }

        public static System.String GetModuleFileName(System.IntPtr hmod)
        {
            System.Int32 bufinsize;
            System.String pfn = new('\0', bufinsize = 512);
            System.UInt32 bufsize;
        G_Retry:
            fixed (System.Char* pfilename = pfn)
            {
                bufsize = GetModuleFileName_Native(hmod, pfilename, bufinsize.ToUInt32());
            }
            // GetModuleFileName will fail only if there is a fatal error.
            // In insufficient buffer calls, the below handling is done.
            if (bufsize == 0) { return null; }
            System.UInt32 err;
            switch (err = GetLastError())
            {
                case Errors.ERROR_INSUFFICIENT_BUFFER:
                    pfn = new('\0', bufinsize += 512);
                    goto G_Retry;
                default:
                    SetLastError(err);
                    break;
            }
            return pfn.Remove(bufsize.ToInt32());
        }
    }
}