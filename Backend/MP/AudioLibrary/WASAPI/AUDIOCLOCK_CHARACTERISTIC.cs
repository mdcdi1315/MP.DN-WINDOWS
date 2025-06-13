

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// <see cref="IAudioClock"/> interface <see cref="IAudioClock.GetCharacteristics"/> method flags.
    /// </summary>
    public enum AUDIOCLOCK_CHARACTERISTIC : System.UInt32
    {
        // The prefix 'AUDIOCLOCK_CHARACTERISTIC_' is omitted for brevity.
        FIXED_FREQUENCY = 0x00000001
    }
}