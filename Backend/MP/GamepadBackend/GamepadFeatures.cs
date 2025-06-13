namespace MP.GamepadBackend
{
    /// <summary>
    /// Gamepad features is a class that holds several critical values when you request a controller device , 
    /// or when you query the controller device feature support.
    /// </summary>
    public sealed class GamepadFeatures
    {
        private System.Boolean ffb, pmd, nav, voice;

        /// <summary>
        /// Creates a default instance of the <see cref="GamepadFeatures"/> class.
        /// </summary>
        public GamepadFeatures() {
            ffb = false;
            pmd = false;
            nav = false;
            voice = false;
        }

        /// <summary>
        /// Gets or sets whether FFB features are available.
        /// </summary>
        public System.Boolean FFB { get => ffb; set => ffb = value; }

        /// <summary>
        /// Gets or sets whether PMD features are available.
        /// </summary>
        public System.Boolean PMD { get => pmd; set => pmd = value; }

        /// <summary>
        /// Gets or sets whether the current gamepad has a full controller layout or not. <br />
        /// Set this value to true to require full controller support.
        /// </summary>
        public System.Boolean Navigation { get => nav; set => nav = value; }

        /// <summary>
        /// Gets or sets whether controller voice device features are available.
        /// </summary>
        public System.Boolean VoiceDeviceSupport { get => voice; set => voice = value; }
    }
}
