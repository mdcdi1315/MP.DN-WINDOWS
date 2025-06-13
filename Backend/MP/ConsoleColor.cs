namespace MP
{
    /// <summary>
    /// Represents the colors that can be used for console text foreground and background colors.
    /// </summary>
    public enum ConsoleColor
    {
        Black = 0,
        DarkBlue = 1,
        DarkGreen = 2,
        DarkCyan = 3,
        DarkRed = 4,
        DarkMagenta = 5,
        DarkYellow = 6,
        Gray = 7,
        DarkGray = 8,
        Blue = 9,
        Green = 10,
        Cyan = 11,
        Red = 12,
        Magenta = 13,
        Yellow = 14,
        White = 15
    }

    internal enum ConsoleControlChars : System.Int32
    {
        FOREGROUND_BLUE = 0x0001, // text color contains blue.
        FOREGROUND_GREEN = 0x0002, // text color contains green.
        FOREGROUND_RED = 0x0004, // text color contains red.
        FOREGROUND_INTENSITY = 0x0008, // text color is intensified.
        BACKGROUND_BLUE = 0x0010, // background color contains blue.
        BACKGROUND_GREEN = 0x0020, // background color contains green.
        BACKGROUND_RED = 0x0040, // background color contains red.
        BACKGROUND_INTENSITY = 0x0080, // background color is intensified.
        COMMON_LVB_LEADING_BYTE = 0x0100, // Leading Byte of DBCS
        COMMON_LVB_TRAILING_BYTE = 0x0200, // Trailing Byte of DBCS
        COMMON_LVB_GRID_HORIZONTAL = 0x0400, // DBCS: Grid attribute: top horizontal.
        COMMON_LVB_GRID_LVERTICAL = 0x0800, // DBCS: Grid attribute: left vertical.
        COMMON_LVB_GRID_RVERTICAL = 0x1000, // DBCS: Grid attribute: right vertical.
        COMMON_LVB_REVERSE_VIDEO = 0x4000, // DBCS: Reverse fore/back ground attribute.
        COMMON_LVB_UNDERSCORE = 0x8000, // DBCS: Underscore.
        ForegroundMask = 0xF,
        BackgroundMask = 0xF0,
        ColorMask = 0xFF
    }

}
