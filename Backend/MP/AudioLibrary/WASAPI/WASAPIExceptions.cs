

using System;
using MP.ExceptionSystem;

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// Defines the base class where all the <see cref="WASAPI"/>-related exceptions do derive from.
    /// </summary>
    public abstract class WASAPIException : BaseException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="WASAPIException"/> class.
        /// </summary>
        public WASAPIException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="WASAPIException"/> class and with the specified 
        /// detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public WASAPIException(System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="WASAPIException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public WASAPIException(System.String message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when the audio device is disconnected. <br />
    /// This exception class maps 1-1 to the HRESULT code <see cref="WASAPIErrorCodes.AUDCLNT_E_DEVICE_INVALIDATED"/>.
    /// </summary>
    public sealed class AudioDeviceDisconnectedException : WASAPIException
    {
        public AudioDeviceDisconnectedException() : base("The audio device was disconnected before the audio session ends gracefully.") { }
    }

    /// <summary>
    /// Thrown when an audio session has not been created yet. <br />
    /// Usually thrown by methods that require the <see cref="IAudioClient.Initialize"/> to be successfully called before. <br />
    /// This exception class maps 1-1 to the HRESULT code <see cref="WASAPIErrorCodes.AUDCLNT_E_NOT_INITIALIZED"/>.
    /// </summary>
    public sealed class AudioSessionNotInitializedException : WASAPIException
    {
        public AudioSessionNotInitializedException() : base("The audio session has not been initialized yet.") { }
    }

    /// <summary>
    /// Thrown when attempting to create an audio session on a <see cref="IAudioClient"/> that has already a created session. <br />
    /// Usually thrown by <see cref="IAudioClient.Initialize"/> method only. <br />
    /// This exception class maps 1-1 to the HRESULT code <see cref="WASAPIErrorCodes.AUDCLNT_E_ALREADY_INITIALIZED"/>.
    /// </summary>
    public sealed class AudioSessionAlreadyInitializedException : WASAPIException
    {
        public AudioSessionAlreadyInitializedException() : base("An audio session has been successfully created before on this IAudioClient.") { }
    }


}