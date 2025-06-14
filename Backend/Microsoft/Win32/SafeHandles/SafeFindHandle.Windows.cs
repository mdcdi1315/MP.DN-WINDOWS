// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    internal sealed class SafeFindHandle : SafeHandle
    {
        public SafeFindHandle() : base(IntPtr.Zero, true) { }

        public SafeFindHandle(System.IntPtr handle) : this() { this.handle = handle; }

        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero || handle == new IntPtr(-1);
            }
        }

        protected override bool ReleaseHandle() => Interop.Kernel32.FindClose(handle) != Interop.BOOL.FALSE;
    }
}
