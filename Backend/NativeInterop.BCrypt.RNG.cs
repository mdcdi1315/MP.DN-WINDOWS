
using System.Runtime.InteropServices;

partial class Interop
{
    /// <summary>
    /// Defines BCrypt.dll native interopability methods.
    /// </summary>
    static unsafe partial class BCrypt
    {
        internal const int BCRYPT_USE_SYSTEM_PREFERRED_RNG = 0x00000002;

        [DllImport(Libraries.BCrypt, CharSet = CharSet.Unicode)]
        public static extern NTSTATUS BCryptGenRandom(System.IntPtr hAlgorithm, byte* pbBuffer, int cbBuffer, int dwFlags);
    }

    public static unsafe void GetRandomBytes(byte* buffer, int length)
    {
        System.Diagnostics.Debug.Assert(buffer != null);
        System.Diagnostics.Debug.Assert(length >= 0);

        NTSTATUS status = BCrypt.BCryptGenRandom(System.IntPtr.Zero, buffer, length, BCrypt.BCRYPT_USE_SYSTEM_PREFERRED_RNG);
        if (status != NTSTATUS.STATUS_SUCCESS)
        {
            if (status == NTSTATUS.STATUS_NO_MEMORY) {
                throw new System.OutOfMemoryException();
            } else {
                throw new System.InvalidOperationException();
            }
        }
    }
}