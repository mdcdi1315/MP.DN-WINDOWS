namespace MP.ExceptionSystem
{
    public sealed class GamepadDisconnectedException : BaseException
    {
        public GamepadDisconnectedException() : base("The gamepad is disconnected.") { }

        public GamepadDisconnectedException(System.String msg) : base(msg) { }
    }
}
