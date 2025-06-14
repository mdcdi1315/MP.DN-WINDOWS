
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    public unsafe sealed class SafeWindowsLibraryHandle : CriticalHandle , MP.ILibraryHandle
    {
        internal SafeWindowsLibraryHandle(System.IntPtr hlib) : base(System.IntPtr.Zero) => handle = hlib;

        public static SafeWindowsLibraryHandle FromLibrary(System.String name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            System.IntPtr hl = Interop.Kernel32.LoadLibraryEx(name, 
                Interop.Kernel32.LoadLibraryFlags.LOAD_LIBRARY_SEARCH_USER_DIRS | 
                Interop.Kernel32.LoadLibraryFlags.LOAD_LIBRARY_SEARCH_SYSTEM32 | 
                Interop.Kernel32.LoadLibraryFlags.LOAD_LIBRARY_SEARCH_APPLICATION_DIR |
                Interop.Kernel32.LoadLibraryFlags.LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR);
            if (hl == IntPtr.Zero) { throw new MP.ExceptionSystem.NativeWindowsException(); }
            return new SafeWindowsLibraryHandle(hl);
        }

        public void* GetFunction(System.String name)
        {
            ObjectDisposedException.ThrowIf(handle == IntPtr.Zero, this);
            void* ptr = Interop.Kernel32.GetProcAddress(handle, name);
            if (ptr is null) { throw new MP.ExceptionSystem.NativeWindowsException(); }
            return ptr;
        }

        public System.IntPtr Handle => handle;

        /// <summary>
        /// Gets the full name of the currently loaded library.
        /// </summary>
        public System.String Name
        {
            get {
                ObjectDisposedException.ThrowIf(handle == IntPtr.Zero, this);
                return Interop.Kernel32.GetModuleFileName(handle);
            }
        }

        public override System.Boolean IsInvalid => handle == System.IntPtr.Zero || handle == -1;

        protected override System.Boolean ReleaseHandle()
        {
            Interop.BOOL bret = Interop.Kernel32.FreeLibrary(handle);
            handle = System.IntPtr.Zero;
            return bret != Interop.BOOL.FALSE;
        }
    }
}