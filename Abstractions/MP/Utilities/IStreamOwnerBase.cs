
using System;

namespace MP.Utilities
{
    /// <summary>
    /// Defines a simple interface for whether a disposable object that holds <br />
    /// a stream should dispose that holding stream.
    /// </summary>
    public interface IStreamOwnerBase : IDisposable
    {
        /// <summary>
        /// Gets or sets a value whether the implementing class should dispose the underlying stream <br />
        /// when <see cref="IDisposable.Dispose"/> will be called.
        /// </summary>
        public System.Boolean IsStreamOwner { get; set; }
    }
}