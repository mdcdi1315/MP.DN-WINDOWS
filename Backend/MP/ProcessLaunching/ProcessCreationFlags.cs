

using System;

namespace MP.ProcessLaunching
{
    [Flags]
    public enum ProcessCreationFlags : System.UInt16
    {
        None = 0,
        CreateWithNewConsole = 1,
        DoNotUseConsole = 2,
        CreateNewProcessGroup = 4,
        WithDefaultErrorMode = 8,
        UseSecureProcessSemantics = 16,
    }
}