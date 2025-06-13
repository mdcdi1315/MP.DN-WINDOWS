
using System;

namespace MP.Dialogs
{
    /// <summary>
    /// An enumeration of <see cref="System.Int32" /> that hold valid icon images allowed to be shown.
    /// </summary>
    public enum IconSelection : System.Int32
    {
        None = 0,
        Error = 1,
        Info = 2,
        Info2 = 3,
        Warning = 4,
        Notice = 5,
        InvalidOperation = 6,
        Question = 7
    }

    /// <summary>
    /// An enumeration of <see cref="System.Int32" /> that keeps valid button patterns for returning the button selected.
    /// </summary>
    public enum ButtonSelection : System.Int32
    {
        OK = 0,
        YesNo = 1,
        OKCancel = 2,
        AbortRetry = 3,
        RetryCancel = 4,
        IgnoreCancel = 5,
        YesNoCancel = 6,
        YesNoRetry = 7,
        YesCancelAbort = 8
    }

    /// <summary>
    /// An enumeration of <see cref="System.Int32" /> values which indicates which button pressed or presents an error.
    /// </summary>
    public enum ButtonReturned : System.Int32
    {
        None = 0,
        Error = 1,
        OK = 2,
        Cancel = 3,
        Yes = 4,
        No = 5,
        Retry = 6,
        Abort = 7,
        Ignore = 8,
        NotAnAnswer = 9
    }

    /// <summary>
    /// Defines the different file operation types that the file operations dialog can handle.
    /// </summary>
    public enum FileOperationType : System.Byte
    {
        MoveFile,
        CopyFile,
        DeleteFile,
        MoveDirectory,
        CopyDirectory,
        DeleteDirectory
    }
}