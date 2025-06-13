namespace System.Windows.Forms
{
    /// <summary>
    /// A very bare implementation of <see cref="IWin32Window"/> interface to cope with interopability.
    /// </summary>
    public readonly struct ReadOnlyWindowHandle : IWin32Window , MP.Utilities.INullable
    {
        private readonly System.IntPtr hwnd;

        public ReadOnlyWindowHandle(System.IntPtr hwnd) => this.hwnd = hwnd;

        public static ReadOnlyWindowHandle Null => new(System.IntPtr.Zero);

        public readonly System.IntPtr Handle => hwnd;

        public readonly System.Boolean IsNull => hwnd == IntPtr.Zero;
    }
}