


using System;

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// Copied from Audioclient.idl of Windows SDK 10.0.19041.0 , see line 31 of the file for more information.
    /// </summary>
    [Flags]
    public enum AUDCLNT_BUFFERFLAGS
    {
        /// <summary>
        /// The data for this buffer is not correlated with the data from the previous buffer.
        /// </summary>
        // The prefix 'AUDCLNT_BUFFERFLAGS_' is omitted for brevity.
        DATA_DISCONTINUITY = 0x01,
        /// <summary>
        /// This data in this buffer should be treated as silence.
        /// </summary>
        // The prefix 'AUDCLNT_BUFFERFLAGS_' is omitted for brevity.
        AUDCLNT_BUFFERFLAGS_SILENT = 0x02,
        /// <summary>
        /// The QPC based timestamp reading for this data buffer does not correlate with the data position.
        /// </summary>
        // The prefix 'AUDCLNT_BUFFERFLAGS_' is omitted for brevity.
        AUDCLNT_BUFFERFLAGS_TIMESTAMP_ERROR = 0x04
    }
}