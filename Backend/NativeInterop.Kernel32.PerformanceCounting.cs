

using System.Runtime.InteropServices;

partial class Interop
{
    unsafe partial class Kernel32
    {
        [DllImport(Libraries.Kernel32 , EntryPoint = "QueryPerformanceCounter", ExactSpelling = true)]
        private static extern BOOL QueryPerformanceCounter_Native(System.Int64* pcount);

        [DllImport(Libraries.Kernel32, EntryPoint = "QueryPerformanceFrequency", ExactSpelling = true)]
        private static extern BOOL QueryPerformanceFrequency_Native(System.Int64* pcount);

        public static System.Int64 QueryPerformanceCounter()
        {
            System.Int64 ret;

            BOOL bret = QueryPerformanceCounter_Native(&ret);

            System.Diagnostics.Debug.Assert(bret != BOOL.FALSE);

            return ret;
        }

        public static System.Int64 QueryPerformanceFrequency()
        {
            System.Int64 ret;

            BOOL bret = QueryPerformanceFrequency_Native(&ret);

            System.Diagnostics.Debug.Assert(bret != BOOL.FALSE);

            return ret;
        }
    }
}