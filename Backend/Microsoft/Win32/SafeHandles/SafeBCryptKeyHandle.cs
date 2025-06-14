// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    public sealed class SafeBCryptKeyHandle : CriticalHandle
    {
        internal SafeBCryptKeyHandle(System.IntPtr ptr) : base(0)
        {
            handle = ptr;
        }

        public System.IntPtr Handle => handle;

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected sealed override bool ReleaseHandle()
        {
            Interop.NTSTATUS ntStatus = Interop.BCrypt.BCryptDestroyKey(handle);
            return ntStatus == Interop.NTSTATUS.STATUS_SUCCESS;
        }
    }
}
