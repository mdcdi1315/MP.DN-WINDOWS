/*
 * Copyright © 2000-2018 SharpZipLib Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.

*/


using System;

namespace System.IO.ManagedZip
{
    /// <summary>
    /// Global options to alter behavior.
    /// </summary>
    public static class SharpZipLibOptions
    {
        /// <summary>
        /// The max pool size allowed for reusing <see cref="Inflater"/> instances, defaults to 0 (disabled).
        /// </summary>
        public static int InflaterPoolSize { get; set; } = 0;
    }

    /// <summary>
    /// SharpZipBaseException is the base exception class for SharpZipLib.
    /// All library exceptions are derived from this.
    /// </summary>
    /// <remarks>NOTE: Not all exceptions thrown will be derived from this class.
    /// A variety of other exceptions are possible for example <see cref="ArgumentNullException" />.</remarks>
    public class SharpZipBaseException : MP.ExceptionSystem.BaseException
    {
        /// <summary>
        /// Initializes a new instance of the SharpZipBaseException class.
        /// </summary>
        public SharpZipBaseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SharpZipBaseException class with a specified error message.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public SharpZipBaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SharpZipBaseException class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        /// <param name="innerException">The inner exception</param>
        public SharpZipBaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Indicates that an error occurred during decoding of a input stream due to corrupt
    /// data or (unintentional) library incompatibility.
    /// </summary>
    [Serializable]
    public class StreamDecodingException : SharpZipBaseException
    {
        private const string GenericMessage = "Input stream could not be decoded";

        /// <summary>
        /// Initializes a new instance of the StreamDecodingException with a generic message
        /// </summary>
        public StreamDecodingException() : base(GenericMessage) { }

        /// <summary>
        /// Initializes a new instance of the StreamDecodingException class with a specified error message.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public StreamDecodingException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the StreamDecodingException class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        /// <param name="innerException">The inner exception</param>
        public StreamDecodingException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Indicates that the input stream could not decoded due to known library incompability or missing features
    /// </summary>
    [Serializable]
    public class StreamUnsupportedException : StreamDecodingException
    {
        private const string GenericMessage = "Input stream is in a unsupported format";

        /// <summary>
        /// Initializes a new instance of the StreamUnsupportedException with a generic message
        /// </summary>
        public StreamUnsupportedException() : base(GenericMessage) { }

        /// <summary>
        /// Initializes a new instance of the StreamUnsupportedException class with a specified error message.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public StreamUnsupportedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the StreamUnsupportedException class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        /// <param name="innerException">The inner exception</param>
        public StreamUnsupportedException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Indicates that the input stream could not decoded due to the stream ending before enough data had been provided
    /// </summary>
    [Serializable]
    public class UnexpectedEndOfStreamException : StreamDecodingException
    {
        private const string GenericMessage = "Input stream ended unexpectedly";

        /// <summary>
        /// Initializes a new instance of the UnexpectedEndOfStreamException with a generic message
        /// </summary>
        public UnexpectedEndOfStreamException() : base(GenericMessage) { }

        /// <summary>
        /// Initializes a new instance of the UnexpectedEndOfStreamException class with a specified error message.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public UnexpectedEndOfStreamException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the UnexpectedEndOfStreamException class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        /// <param name="innerException">The inner exception</param>
        public UnexpectedEndOfStreamException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Indicates that a value was outside of the expected range when decoding an input stream
    /// </summary>
    [Serializable]
    public class ValueOutOfRangeException : StreamDecodingException
    {
        /// <summary>
        /// Initializes a new instance of the ValueOutOfRangeException class naming the causing variable
        /// </summary>
        /// <param name="nameOfValue">Name of the variable, use: nameof()</param>
        public ValueOutOfRangeException(string nameOfValue)
            : base($"{nameOfValue} out of range") { }

        /// <summary>
        /// Initializes a new instance of the ValueOutOfRangeException class naming the causing variable,
        /// it's current value and expected range.
        /// </summary>
        /// <param name="nameOfValue">Name of the variable, use: nameof()</param>
        /// <param name="value">The invalid value</param>
        /// <param name="maxValue">Expected maximum value</param>
        /// <param name="minValue">Expected minimum value</param>
        public ValueOutOfRangeException(string nameOfValue, long value, long maxValue, long minValue = 0)
            : this(nameOfValue, value.ToString(), maxValue.ToString(), minValue.ToString()) { }

        /// <summary>
        /// Initializes a new instance of the ValueOutOfRangeException class naming the causing variable,
        /// it's current value and expected range.
        /// </summary>
        /// <param name="nameOfValue">Name of the variable, use: nameof()</param>
        /// <param name="value">The invalid value</param>
        /// <param name="maxValue">Expected maximum value</param>
        /// <param name="minValue">Expected minimum value</param>
        public ValueOutOfRangeException(string nameOfValue, string value, string maxValue, string minValue = "0") :
            base($"{nameOfValue} out of range: {value}, should be {minValue}..{maxValue}")
        { }

        private ValueOutOfRangeException()
        {
        }

        private ValueOutOfRangeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
