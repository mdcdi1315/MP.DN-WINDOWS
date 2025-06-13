

namespace MP
{
    /// <summary>
    /// Defines the initial settings that a player instance must use.
    /// </summary>
    public interface IPlayerInstanceData
    {
        /// <summary>
        /// Left speaker's volume.
        /// </summary>
        public System.Byte VolumeLeft { get; set; }

        /// <summary>
        /// Right speaker's volume.
        /// </summary>
        public System.Byte VolumeRight { get; set; }

        /// <summary>
        /// Device latency in ms.
        /// </summary>
        public System.Byte Latency { get; set; }

        /// <summary>
        /// The playback stream to play.
        /// </summary>
        public AbstractPropertyStream Stream { get; set; }
    }
}