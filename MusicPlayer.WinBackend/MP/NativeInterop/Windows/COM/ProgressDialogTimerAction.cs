namespace MP.NativeInterop.Windows.COM
{
    public enum ProgressDialogTimerAction : uint
    {
        /// <summary>
        /// Reset the timer so the progress will be calculated from now until the first ::SetProgress() is called so
        /// those this time will correspond to the values passed to ::SetProgress().  Only do this before ::SetProgress() is called.
        /// </summary>
        PDTIMER_RESET = 0x00000001,
        /// <summary>Progress has been suspended</summary>
        PDTIMER_PAUSE = 0x00000002 ,
        /// <summary>Progress has resumed</summary>
        PDTIMER_RESUME = 0x00000003
    }
}