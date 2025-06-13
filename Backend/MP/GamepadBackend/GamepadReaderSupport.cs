namespace MP.GamepadBackend
{
    /// <summary>
    /// Defines the gamepad button that was pressed.
    /// </summary>
    public enum GamepadButton : System.UInt32
    {
        /// <summary>No buttons were pressed.</summary>
        None = 0,
        /// <summary>The menu button was pressed.</summary>
        Menu = 0x1,
        /// <summary>The view button was pressed.</summary>
        View = 0x2,
        /// <summary>The A button was pressed.</summary>
        A = 0x4,
        /// <summary>The B button was pressed.</summary>
        B = 0x8,
        /// <summary>The X button was pressed.</summary>
        X = 0x10,
        /// <summary>The Y button was pressed.</summary>
        Y = 0x20,
        /// <summary>The D-Pad up arrow button was pressed.</summary>
        DPadUp = 0x40,
        /// <summary>The D-Pad down arrow button was pressed.</summary>
        DPadDown = 0x80,
        /// <summary>The D-Pad left arrow button was pressed.</summary>
        DPadLeft = 0x100,
        /// <summary>The D-Pad right arrow button was pressed.</summary>
        DPadRight = 0x200,
        /// <summary></summary>
        LeftShoulder = 0x400,
        /// <summary></summary>
        RightShoulder = 0x800,
        /// <summary></summary>
        LeftThumbstick = 0x1000,
        /// <summary></summary>
        RightThumbstick = 0x2000,
        /// <summary></summary>
        [System.Runtime.Versioning.SupportedOSPlatform("windows10.0.14393.0")]
        Paddle1 = 0x4000,
        /// <summary></summary>
        [System.Runtime.Versioning.SupportedOSPlatform("windows10.0.14393.0")]
        Paddle2 = 0x8000,
        /// <summary></summary>
        [System.Runtime.Versioning.SupportedOSPlatform("windows10.0.14393.0")]
        Paddle3 = 0x10000,
        /// <summary></summary>
        [System.Runtime.Versioning.SupportedOSPlatform("windows10.0.14393.0")]
        Paddle4 = 0x20000,
    }

    /// <summary>
    /// Defines the type of the activated event in the gamepad.
    /// </summary>
    public enum GamepadMode : System.Byte
    {
        /// <summary>Dummy field to define that the event is possibly invalid and should be ignored.</summary>
        None = 0,
        /// <summary>A button was pressed.</summary>
        Button = 1,
        /// <summary>The left trigger was activated.</summary>
        LeftTrigger = 2,
        /// <summary>The right trigger was activated.</summary>
        RightTrigger = 3,
        /// <summary>The left thumbstick was activated.</summary>
        LeftThumbstick = 4,
        /// <summary>The right thumbstick was activated.</summary>
        RightThumbstick = 5,
    }

    /// <summary>
    /// Determines the type of battery used in the given controller device.
    /// </summary>
    public enum GamepadBatteryType : System.Byte
    {
        /// <summary>The device is no longer connected.</summary>
        Disconnected = 0,
        /// <summary>The device uses alkaline batteries.</summary>
        Alkaline = 1,
        /// <summary>The device uses nickel metal hydride batteries.</summary>
        NiMH = 2,
        /// <summary>The device does not make use of any batteries. It is connected to a cable where it takes power from.</summary>
        NoBattery = 3,
        /// <summary>The battery type cannot be queried.</summary>
        Unknown = 255
    }

    /// <summary>
    /// Determines the battery level of the given controller device.
    /// </summary>
    public enum GamepadBatteryLevel : System.Byte
    {
        /// <summary>The battery is empty and needs replacement or re-charge.</summary>
        Empty = 0,
        /// <summary>The battery level is low and it will require soon a replacement or a re-charge.</summary>
        Low = 1,
        /// <summary>The battery has still enough power.</summary>
        Medium = 2,
        /// <summary>The battery is fully charged.</summary>
        Full = 3,
    }

    /// <summary>
    /// Provides the event data when a new gamepad event is available for consumption.
    /// </summary>
    public sealed class GamepadEventEventArgs : System.EventArgs
    {
        private GamepadMode mode;
        private System.Int16 cx, cy;
        private GamepadButton button;

        /// <summary>
        /// Initializes an empty event argument class instance.
        /// </summary>
        public GamepadEventEventArgs() : base()
        {
            mode = GamepadMode.None;
            cx = cy = 0;
            button = GamepadButton.None;
        }

        /// <summary>
        /// Defines the fired event type.
        /// </summary>
        public GamepadMode Type { get => mode; internal set => mode = value; }

        /// <summary>
        /// Defines a numeric value describing the position for triggers
        /// and the X coordinate for the thumbsticks.
        /// </summary>
        public System.Int16 X { get => cx; internal set => cx = value; }

        /// <summary>
        /// Defines a numeric value describing the Y coordinate for the thumbsticks.
        /// </summary>
        public System.Int16 Y { get => cy; internal set => cy = value; }

        /// <summary>
        /// Defines the gamepad button that was pressed when the event is of <see cref="GamepadMode.Button"/> type.
        /// </summary>
        public GamepadButton Button { get => button; internal set => button = value; }
    }

    /// <summary>
    /// Defines data when applying a vibration scheme to the controller. <br />
    /// The class defines the vibration duration and the required motor speeds to apply.
    /// </summary>
    public sealed class GamepadVibrationTimeLine
    {
        private System.Int32 timeout;
        private System.UInt16 left, right;

        public GamepadVibrationTimeLine() { timeout = 20; left = 0; right = 0; }

        /// <summary>
        /// Specifies the duration in milliseconds of the current vibration.
        /// </summary>
        public System.Int32 Duration 
        { 
            get => timeout; 
            set { 
                if (timeout < 20) { throw new System.ArgumentException("Gamepad vibration durations must be more than 20 ms."); } 
                timeout = value; 
            }
        }

        /// <summary>
        /// Specifies the left motor speed when this vibration takes effect. <br />
        /// A value of 65535 specifies full speed.
        /// </summary>
        public System.UInt16 LeftMotorSpeed { get => left; set => left = value; }

        /// <summary>
        /// Specifies the right motor speed when this vibration takes effect. <br />
        /// A value of 65535 specifies full speed.
        /// </summary>
        public System.UInt16 RightMotorSpeed { get => right; set => right = value; }
    }
}
