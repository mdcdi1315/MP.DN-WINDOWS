
using MP;
using System;

namespace Microsoft.Win32.SafeHandles
{
    public sealed class SafeBCryptHashHandle : CriticalHandleZeroOrMinusOneIsInvalid
    {
        internal SafeBCryptHashHandle(System.IntPtr ptr) => handle = ptr;

        public System.IntPtr Handle => handle;

        public SafeBCryptHashHandle Duplicate()
        {
            Interop.NTSTATUS nts = Interop.BCrypt.BCryptDuplicateHash(handle, out var duped);
            if (nts != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                throw new MP.ExceptionSystem.NativeWindowsException(nts);
            }
            return new(duped);
        }

        protected override bool ReleaseHandle()
        {
            if (handle == IntPtr.Zero) { return true; }
            Interop.NTSTATUS nts = Interop.BCrypt.BCryptDestroyHash(handle);
            handle = IntPtr.Zero;
            return nts == Interop.NTSTATUS.STATUS_SUCCESS;
        }
    }
}