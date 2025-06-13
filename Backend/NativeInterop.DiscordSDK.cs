
using System;
using MP.DGSDK;
using System.Runtime.InteropServices;

partial class Interop
{
    public static unsafe class DiscordSdk
    {
        [DllImport(Libraries.DiscordSdk, ExactSpelling = true, CallingConvention = CallingConvention.Winapi , EntryPoint = "DiscordCreate")]
        private static extern Result DiscordCreate_Native(System.UInt32 version, Discord.FFICreateParams* cp, IntPtr* manager);

        public static Result DiscordCreate(UInt32 version, ref Discord.FFICreateParams createParams, out IntPtr manager)
        {
            IntPtr mgr;
            fixed (Discord.FFICreateParams * cpp = &createParams)
            {
                Result ret = DiscordCreate_Native(version, cpp, &mgr);
                manager = mgr;
                return ret;
            }
        }
    }
}