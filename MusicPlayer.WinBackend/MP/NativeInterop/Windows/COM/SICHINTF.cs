namespace MP.NativeInterop.Windows.COM
{
    public enum SICHINTF : uint
    {
        /// <summary>iOrder based on display in a folder view</summary>
        SICHINT_DISPLAY = 0x00000000,
        /// <summary>exact instance compare</summary>
        SICHINT_ALLFIELDS = 0x80000000,
        /// <summary>iOrder based on canonical name (better performance)</summary>
        SICHINT_CANONICAL = 0x10000000,
        SICHINT_TEST_FILESYSPATH_IF_NOT_EQUAL = 0x20000000,
    }
}