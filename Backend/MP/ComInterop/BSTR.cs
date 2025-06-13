

using System;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    /// <summary>
    /// Defines a way to create and manage OLE Automation BSTR strings.
    /// </summary>
    public unsafe sealed class BSTR : CriticalHandle
    {
        public BSTR(System.IntPtr native) : base(0)
        {
            if (native == IntPtr.Zero) { throw new ArgumentNullException(nameof(native)); }
            handle = native;
        }

        public BSTR(System.Char* native) : base(0)
        {
            if (native is null) { throw new ArgumentNullException(nameof(native)); }
            handle = new(native);
        }

        public BSTR(System.String cpy) : base(0)
        {
            if (cpy is null) { throw new ArgumentNullException(nameof(cpy)); }
            fixed (System.Char* pcopy = cpy)
            {
                handle = new(Interop.OleAut32.SysAllocStringLen(pcopy, cpy.Length.ToUInt32()));
            }
            if (handle == IntPtr.Zero)
            {
                throw new OutOfMemoryException("Cannot allocate the requested BSTR into memory.");
            }
        }

        /// <summary>
        /// Gets the length of this <see cref="BSTR"/>.
        /// </summary>
        public System.UInt32 Length => Interop.OleAut32.SysStringLen((System.Char*)handle.ToPointer());

        public void Reallocate(System.String newstring)
        {
            if (newstring is null) { throw new ArgumentNullException(nameof(newstring)); }
            Interop.BOOL ret;
            System.Char* pn = (System.Char*)handle.ToPointer();
            fixed (System.Char* pnew = newstring)
            {
                ret = Interop.OleAut32.SysReAllocStringLen(&pn, pnew, newstring.Length.ToUInt32());
            }
            if (ret == Interop.BOOL.FALSE)
            {
                throw new OutOfMemoryException("Cannot reallocate the string.");
            }
            handle = new(pn);
        }

        public override System.Boolean IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            Interop.OleAut32.SysFreeString((System.Char*)handle.ToPointer());
            handle = IntPtr.Zero;
            return true;
        }

        /// <summary>
        /// Gets a managed representation of the contents of this <see cref="BSTR"/>.
        /// </summary>
        /// <returns>A managed reprsentation of the <see cref="BSTR"/> contents.</returns>
        public override System.String ToString() => new((System.Char*)handle.ToPointer(), 0, Length.ToInt32());
    }

}