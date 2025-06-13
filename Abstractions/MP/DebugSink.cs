

using System;

namespace MP
{
    /// <summary>
    /// Defines a single debugging sink (Where debugging data will be written to)
    /// </summary>
    public abstract class DebugSink : IDisposable
    {
        /// <summary>
        /// Creates a new debugging sink.
        /// </summary>
        protected DebugSink() { }

        /// <summary>
        /// Gets a value whether this sink can accept log writing requests.
        /// </summary>
        public abstract System.Boolean IsActive { get; }

        /// <summary>
        /// Writes a line to the target that this sink represents.
        /// </summary>
        /// <param name="message">The line to write.</param>
        public virtual void WriteLine(System.String message) => Write($"{message}\n");

        /// <summary>
        /// Writes text to the target that this sink represents.
        /// </summary>
        /// <param name="message">The text to write.</param>
        public abstract void Write(System.String message);

        /// <summary>
        /// Disposes this debug sink, if applicable.
        /// </summary>
        public abstract void Dispose();
    }
}