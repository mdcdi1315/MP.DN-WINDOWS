

using System;

namespace MP.AudioLibrary.Windows
{
    /// <summary>
    /// For the use of <see cref="WAVEFORMATEXTENSIBLE"/>. <br />
    /// The values are retrieved from <see href="https://learn.microsoft.com/en-us/windows/win32/api/mmreg/ns-mmreg-waveformatextensible#remarks"/>
    /// </summary>
    [Flags]
    public enum SPEAKERASSIGNMENT : System.UInt32
    {
        /// <summary>Undefined value.</summary>
        None = 0,
        /// <summary>Front left speaker flag.</summary>
        FRONT_LEFT = 0x1,
        /// <summary>Front right speaker flag.</summary>
        FRONT_RIGHT = 0x2,
        /// <summary>Front center speaker flag.</summary>
        FRONT_CENTER = 0x4,
        /// <summary>Low frequency (subwoofer) speaker flag.</summary>
        LOW_FREQUENCY = 0x8,  
        /// <summary>Back left speaker flag.</summary>
        BACK_LEFT = 0x10,
        /// <summary>Back right speaker flag.</summary>
        BACK_RIGHT = 0x20,
        /// <summary>Front left of center speaker flag.</summary>
        FRONT_LEFT_OF_CENTER = 0x40,
        /// <summary>Front right of center speaker flag.</summary>
        FRONT_RIGHT_OF_CENTER = 0x80,
        /// <summary>Back center speaker flag.</summary>
        BACK_CENTER = 0x100,
        /// <summary>Side left speaker flag.</summary>
        SIDE_LEFT = 0x200,
        /// <summary>Side right speaker flag.</summary>
        SIDE_RIGHT = 0x400,
        /// <summary>Top center speaker flag.</summary>
        TOP_CENTER = 0x800,
        /// <summary>Top front left speaker flag.</summary>
        TOP_FRONT_LEFT = 0x1000,
        /// <summary>Top front center speaker flag.</summary>
        TOP_FRONT_CENTER = 0x2000,
        /// <summary>Top front right speaker flag.</summary>
        TOP_FRONT_RIGHT = 0x4000,
        /// <summary>Top back left speaker flag.</summary>
        TOP_BACK_LEFT = 0x8000,
        /// <summary>Top back center speaker flag.</summary>
        TOP_BACK_CENTER = 0x10000,
        /// <summary>Top back right speaker flag.</summary>
        TOP_BACK_RIGHT = 0x20000
    }
}