

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// From AudioSessionTypes.h of 10.0.19041.0 SDK , see line 32.
    /// </summary>
    public enum AUDCLNT_SHAREMODE : System.Int32
    {
        /// <summary>
        /// The device will be opened in shared mode and use the WAS format.
        /// </summary>
        AUDCLNT_SHAREMODE_SHARED,
        /// <summary>
        /// The device will be opened in exclusive mode and use the application specified format.
        /// </summary>
        AUDCLNT_SHAREMODE_EXCLUSIVE
    }
}