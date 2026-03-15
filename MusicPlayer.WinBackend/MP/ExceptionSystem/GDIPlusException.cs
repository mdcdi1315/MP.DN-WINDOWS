namespace MP.ExceptionSystem
{
    public sealed class GDIPlusException : BaseException, INativeException
    {
        internal static void ThrowIfError(Interop.GdiPlus.GpStatus status)
        {
            switch (status)
            {
                case Interop.GdiPlus.GpStatus.Ok:
                    return;
                case Interop.GdiPlus.GpStatus.OutOfMemory:
                    throw new System.OutOfMemoryException();
                case Interop.GdiPlus.GpStatus.InvalidParameter:
                    throw new System.ArgumentException("An invalid parameter was passed.");
                default:
                    throw new GDIPlusException(status);
            }
        }

        private GDIPlusException(Interop.GdiPlus.GpStatus status) : base(
            status switch {
                Interop.GdiPlus.GpStatus.Ok => "All OK!",
                Interop.GdiPlus.GpStatus.GenericError => "A generic error was occurred.",
                Interop.GdiPlus.GpStatus.InvalidParameter => "An invalid parameter was passed to a function.",
                Interop.GdiPlus.GpStatus.OutOfMemory => "Out of memory.",
                Interop.GdiPlus.GpStatus.ObjectBusy => "Object is busy.",
                Interop.GdiPlus.GpStatus.InsufficientBuffer => "Insufficient buffer.",
                Interop.GdiPlus.GpStatus.NotImplemented => "Function is not implemented.",
                Interop.GdiPlus.GpStatus.Win32Error => "A Win32 error was occurred.",
                Interop.GdiPlus.GpStatus.WrongState => "Detected a wrong object state.",
                Interop.GdiPlus.GpStatus.Aborted => "The call was aborted.",
                Interop.GdiPlus.GpStatus.FileNotFound => "The specified file was not found.",
                Interop.GdiPlus.GpStatus.ValueOverflow => "An artithemtic overflow was occurred.",
                Interop.GdiPlus.GpStatus.AccessDenied => "Access is denied.",
                Interop.GdiPlus.GpStatus.UnknownImageFormat => "Unknown image format.",
                Interop.GdiPlus.GpStatus.FontFamilyNotFound => "Font family was not found.",
                Interop.GdiPlus.GpStatus.FontStyleNotFound => "Font style was not found.",
                Interop.GdiPlus.GpStatus.NotTrueTypeFont => "Not a TrueType font.",
                Interop.GdiPlus.GpStatus.UnsupportedGdiplusVersion => "The specified GDI+ version is unsupported.",
                Interop.GdiPlus.GpStatus.GdiplusNotInitialized => "GDI+ is not yet initialized.",
                Interop.GdiPlus.GpStatus.PropertyNotFound => "The property was not found.",
                Interop.GdiPlus.GpStatus.PropertyNotSupported => "The property is not supported.",
                Interop.GdiPlus.GpStatus.ProfileNotFound => "The specified profile was not found.",
                _ => $"Unknown error code {status:x2}"
            }
        ) { }
    }
}