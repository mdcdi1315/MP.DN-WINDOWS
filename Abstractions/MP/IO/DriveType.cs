
using System;
using System.Runtime.Versioning;

namespace MP.IO
{
    /// <summary>
    /// Provides constants relating to the type of a drive.
    /// </summary>
    // Matches Win32's DRIVE_XXX #defines from winbase.h
    public enum DriveType
    {
        /// <summary>Unknown.</summary>
        Unknown = 0,
        /// <summary>The drive is present, but does not have a root directory.</summary>
        NoRootDirectory = 1,
        /// <summary>The drive is removable media.</summary>
        Removable = 2,
        /// <summary>The drive is a fixed drive.</summary>
        Fixed = 3,
        /// <summary>The drive is a drive accessed from the network.</summary>
        Network = 4,
        /// <summary>The drive is a CD-Rom placed on the computer's DVD drive.</summary>
        CDRom = 5,
        /// <summary>The drive is a virtual disk mapped to the computer's RAM.</summary>
        Ram = 6
    }

    /// <summary>
    /// Contains information and flags about the type of the current drive as determined by AutoPlay if it represents a 'playable' media object. <br />
    /// Typically this is accessed from the Windows OS.
    /// </summary>
    [Flags]
    [SupportedOSPlatform(MP.NativeInterop.Windows.WindowsVersions.NTDDI_VISTA)]
    public enum DriveMediaType : System.UInt32
    {
        /// <summary></summary>
        None = 0,
        /// <summary>
        /// The media type was loaded by AutoRun.inf.
        /// </summary>
        AutoRunDotInf = 2,
        /// <summary>
        /// The drive represents a CD that contains audio tracks in CDFS.
        /// </summary>
        AudioCD = 4,
        /// <summary>
        /// The drive represents a DVD that contains a playable movie.
        /// </summary>
        DVDMovie = 8,
        /// <summary>
        /// The drive represents a empty CD.
        /// </summary>
        BlankCD = 0x00000010,
        /// <summary>
        /// The drive represents a empty DVD.
        /// </summary>
        BlankDVD = 0x00000020,
        /// <summary>
        /// The drive media type could not be determined.
        /// </summary>
        Unknown = 0x00000040,
        /// <summary>
        /// The drive contains files that have .bmp or .jpg extensions , for instance.
        /// </summary>
        AutoPlayPictures = 0x00000080,
        /// <summary>
        /// The drive contains playable audio files that are music.
        /// </summary>
        AutoPlayMusic = 0x00000100,
        /// <summary>
        /// The drive contains playable video files that are movies.
        /// </summary>
        AutoPlayVideo = 0x00000200,
        /// <summary>
        /// The drive is a playable Video CD.
        /// </summary>
        VCD = 0x00000400,
        /// <summary>
        /// The drive is a playable S-Video CD.
        /// </summary>
        SVCD = 0x00000800,
        /// <summary>
        /// The drive is a playable Audio DVD.
        /// </summary>
        DVDAudio = 0x00001000,
        /// <summary>
        /// The drive is a blank Blu-Ray disc.
        /// </summary>
        BlankBDDisc = 0x00002000,
        /// <summary>
        /// The drive is a playable Blu-Ray disc.
        /// </summary>
        BLURAY = 0x00004000,
        /// <summary>
        /// The drive is possibly an SD-card that contains camera images.
        /// </summary>
        CameraStorage = 0x00008000,
    }
}
