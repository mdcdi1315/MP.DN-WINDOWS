namespace MP.NativeInterop.Windows.COM
{
    public enum SIGDN : System.Int32
    {
        SIGDN_NORMALDISPLAY = 0,
        SIGDN_PARENTRELATIVEPARSING = unchecked((int)0x80018001),
        SIGDN_DESKTOPABSOLUTEPARSING = unchecked((int)0x80028000),
        SIGDN_PARENTRELATIVEEDITING = unchecked((int)0x80031001),
        SIGDN_DESKTOPABSOLUTEEDITING = unchecked((int)0x8004c000),
        SIGDN_FILESYSPATH = unchecked((int)0x80058000),
        SIGDN_URL = unchecked((int)0x80068000),
        SIGDN_PARENTRELATIVEFORADDRESSBAR = unchecked((int)0x8007c001),
        SIGDN_PARENTRELATIVE = unchecked((int)0x80080001),
        SIGDN_PARENTRELATIVEFORUI = unchecked((int)0x80094001)
    }
}