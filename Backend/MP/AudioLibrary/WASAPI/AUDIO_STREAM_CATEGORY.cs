
using MP.WindowsInterop;
using System.Runtime.Versioning;

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>Audio stream categories</summary>
    public enum AUDIO_STREAM_CATEGORY
    {
        /// <summary>All other streams (default)</summary>
        AudioCategory_Other = 0,
        /// <summary>(deprecated for Win10) Music, Streaming audio</summary>
        [UnsupportedOSPlatform(WindowsVersions.NTDDI_WINTHRESHOLD)]
        AudioCategory_ForegroundOnlyMedia = 1,
        /// <summary>(deprecated for Win10) Video with audio</summary>
        [UnsupportedOSPlatform(WindowsVersions.NTDDI_WINTHRESHOLD)]
        AudioCategory_BackgroundCapableMedia = 2,
        /// <summary>VOIP, chat, phone call</summary>
        AudioCategory_Communications = 3,
        /// <summary>Alarm, Ring tones</summary>
        AudioCategory_Alerts = 4,
        /// <summary>Sound effects, clicks, dings</summary>
        AudioCategory_SoundEffects = 5,
        /// <summary>Game sound effects</summary>
        AudioCategory_GameEffects = 6,
        /// <summary>Background audio for games</summary>
        AudioCategory_GameMedia = 7,
        /// <summary>In game player chat</summary>
        AudioCategory_GameChat = 8,
        /// <summary>Speech recognition</summary>
        AudioCategory_Speech = 9,
        /// <summary>Video with audio</summary>
        AudioCategory_Movie = 10,
        /// <summary>Music, Streaming audio</summary>
        AudioCategory_Media = 11,
    }
}