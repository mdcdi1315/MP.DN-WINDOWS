
using System;
using MP.ExceptionSystem;

namespace MP.Graphics
{
    public static class GDIPlusLibrary
    {
        private static UIntPtr token;

        public static void Initialize()
        {
            if (token == UIntPtr.Zero) {
                GDIPlusException.ThrowIfError(Interop.GdiPlus.GdiplusStartup(out token, new(), out _));
            }
        }

        public static void Destroy()
        {
            if (token == UIntPtr.Zero) { return; }
            Interop.GdiPlus.GdiplusShutdown(token);
            token = UIntPtr.Zero;
        }
    }
}