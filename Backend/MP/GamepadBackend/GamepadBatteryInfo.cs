namespace MP.GamepadBackend
{
    /// <summary>
    /// The Gamepad Battery Info class returns values for the controller and/or headset battery status.
    /// </summary>
    public sealed class GamepadBatteryInfo
    {
        private GamepadBatteryType type;
        private GamepadBatteryLevel level;

        internal GamepadBatteryInfo(Interop.XInput.XINPUT_BATTERY_INFORMATION info) 
        {
            type = info.BatteryType switch { 
                Interop.XInput.XInputBatteryType.DISCONNECTED => GamepadBatteryType.Disconnected,
                Interop.XInput.XInputBatteryType.WIRED => GamepadBatteryType.NoBattery,
                Interop.XInput.XInputBatteryType.ALKALINE => GamepadBatteryType.Alkaline,
                Interop.XInput.XInputBatteryType.NIMH => GamepadBatteryType.NiMH,
                Interop.XInput.XInputBatteryType.UNKNOWN => GamepadBatteryType.Unknown,
                _ => GamepadBatteryType.Unknown
            };
            level = info.BatteryLevel switch { 
                Interop.XInput.XInputBatteryLevel.EMPTY => GamepadBatteryLevel.Empty,
                Interop.XInput.XInputBatteryLevel.LOW => GamepadBatteryLevel.Low,
                Interop.XInput.XInputBatteryLevel.MEDIUM => GamepadBatteryLevel.Medium,
                Interop.XInput.XInputBatteryLevel.FULL => GamepadBatteryLevel.Full,
                _ => GamepadBatteryLevel.Empty
            };
        }

        public GamepadBatteryType Type => type;

        public GamepadBatteryLevel Level => level;

        public override string ToString() => $"GamepadBatteryInfo {{ Type={type} , Level={level} }}";
    }
}
