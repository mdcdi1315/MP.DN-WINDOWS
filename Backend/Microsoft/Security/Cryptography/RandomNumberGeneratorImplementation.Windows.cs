// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MP;
using System.Diagnostics;

namespace Microsoft.Security.Cryptography
{
    internal sealed partial class RandomNumberGeneratorImplementation
    {
        private static unsafe void GetBytes(byte* pbBuffer, int count)
        {
            Debug.Assert(count > 0);

            Interop.NTSTATUS status = Interop.BCrypt.BCryptGenRandom(System.IntPtr.Zero, pbBuffer, count, Interop.BCrypt.BCRYPT_USE_SYSTEM_PREFERRED_RNG);
            if (status != Interop.NTSTATUS.STATUS_SUCCESS)
                throw new MP.ExceptionSystem.NativeWindowsException(Interop.NtDll.RtlNtStatusToDosError(status).ToInt32());
        }
    }
}
