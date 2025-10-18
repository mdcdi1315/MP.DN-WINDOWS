using System;

namespace MP.Graphics.Windowing
{
    /// <summary>
    /// Represents the default error handler method signature.
    /// </summary>
    /// <param name="ex">The exception that the error handler must handle</param>
    /// <returns>A value whether the exception was handled, or not.</returns>

    public delegate bool WindowDispatcherErrorHandlerCallback(Exception ex);
}