
using System;

namespace MP.AudioLibrary.MMDevice
{
    [Flags]
    public enum DEVICE_STATE : System.UInt32
    {
        ACTIVE = 0x00000001,
        DISABLED = 0x00000002,
        NOTPRESENT = 0x00000004,
        UNPLUGGED = 0x00000008,
        STATEMASK_ALL = 0x0000000f
    }
}