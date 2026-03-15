using System;

namespace MP.NativeInterop.Windows.COM
{
    [Flags]
    public enum SIATTRIBFLAGS
    {
        SIATTRIBFLAGS_AND = 0x00000001, // if multiple items and the attirbutes together.
        SIATTRIBFLAGS_OR = 0x00000002, // if multiple items or the attributes together.
        SIATTRIBFLAGS_APPCOMPAT = 0x00000003, // Call GetAttributes directly on the ShellFolder for multiple attributes
        SIATTRIBFLAGS_MASK = 0x00000003, // for the AND/OR/APPCOMPAT value
        SIATTRIBFLAGS_ALLITEMS = 0x00004000, // normally only the first few items are used to compute the attributes, pass this to force all of them
                                             // doing all will result in poor performance for large arrays so use this carefuly
    }
}