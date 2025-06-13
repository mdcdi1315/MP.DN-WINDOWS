

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// Copied from Audioclient.idl of Windows SDK 10.0.19041.0 , see line 48 of the file for more information.
    /// </summary>
    public enum AUDCLNT_STREAMOPTIONS
    {
        // The prefix 'AUDCLNT_STREAMOPTIONS_' is omitted for brevity.
        NONE = 0x00,
        /// <summary>
        /// The audio stream is a 'raw' stream that bypasses all signal processing except for endpoint specific, always-on processing in the APO, driver and hardware
        /// </summary>
        // The prefix 'AUDCLNT_STREAMOPTIONS_' is omitted for brevity.
        AUDCLNT_STREAMOPTIONS_RAW = 0x01,
        /// <summary>
        /// The client is requesting the audio engine to match the format proposed by the client. <br />
        /// The audio engine may match this format only if the format can be accepted by the audio driver and associated APOs.
        /// </summary>
        // The prefix 'AUDCLNT_STREAMOPTIONS_' is omitted for brevity.
        AUDCLNT_STREAMOPTIONS_MATCH_FORMAT = 0x02,
        /// <summary>
        /// The client is requesting the audio client to insert Ambisonics renderer and configure the pipeline to match Ambisonics format types
        /// </summary>
        // The prefix 'AUDCLNT_STREAMOPTIONS_' is omitted for brevity.
        AUDCLNT_STREAMOPTIONS_AMBISONICS = 0x04
    }
}