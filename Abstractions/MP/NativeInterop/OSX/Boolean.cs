namespace MP.NativeInterop.OSX
{
    /// <summary>
    /// A Boolean in OSX, like the <see cref="Windows.BOOLEAN"/> in Win32, is a boolean value which it can hold 2 distinct
    /// values - 0 for false and 1 for true.
    /// </summary>
    public enum Boolean : System.Byte
    {
        /// <summary>Represents the <see langword="false"/> value in C#.</summary>
        False = 0,
        /// <summary>Represents the <see langword="true"/> value in C#.</summary>
        True
    }
}