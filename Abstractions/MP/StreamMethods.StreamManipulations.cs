
using System;
using MP.IO.Buffers;
using System.Buffers;
using MP.Annotations.CodeAnalysis;

namespace MP
{
    unsafe partial class StreamMethods
    {
        /// <summary>
        /// Directly copies over all the bytes contained from the current to another stream. <br />
        /// Both positions will be respectively updated.
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <param name="output">The target stream.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="output"/> parameter was null.</exception>
        [Throws(
            typeof(System.IO.IOException),
            typeof(ArgumentNullException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException)
        )]
        public static void DirectCopyToStream(this MP.IO.IDataStreamAccess input, MP.IO.IDataStreamAccess output)
            => DirectCopyToStream(input, output, BUFSIZE);

        /// <summary>
        /// Directly copies over all the bytes contained from the current to another stream. <br />
        /// Both positions will be respectively updated.
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <param name="output">The target stream.</param>
        /// <param name="buffersize">The buffer size, in bytes, that the method should allocate. This buffer will be used to copy data from the one stream to the another.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="output"/> parameter was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="buffersize"/> parameter was less than 1024 bytes.</exception>
        [Throws(
            typeof(System.IO.IOException),
            typeof(ArgumentNullException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException),
            typeof(ArgumentOutOfRangeException)
        )]
        public static void DirectCopyToStream(this MP.IO.IDataStreamAccess input, MP.IO.IDataStreamAccess output, System.Int32 buffersize)
        {
            ArgumentNullException.ThrowIfNull(output);
            if (buffersize < 1024) { 
                throw new ArgumentOutOfRangeException(nameof(buffersize), "The buffersize parameter is too small and could degrade performance."); 
            } else {
                using (var cxt = ArrayPool<System.Byte>.Shared.RentByContext(buffersize))
                {
                    System.Int32 readin;
                    while ((readin = input.Read(cxt, 0, cxt.ActualLength)) > 0) { output.Write(cxt, 0, readin); }
                }
            }
        }

        /// <summary>
        /// Directly copies the specified number of bytes from the current stream to the destination stream. <br />
        /// Both positions will be respectively updated by <paramref name="bytes_copy"/>.
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <param name="output">The target stream.</param>
        /// <param name="bytes_copy">The exact number of bytes to copy from this stream to <paramref name="output"/>.</param>
        /// <param name="buffer_size">The buffer size, in bytes, that the method should allocate. This buffer will be used to copy data from the one stream to the another.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="output"/> parameter was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="buffer_size"/> parameter was less than 1024 bytes, -or- the <paramref name="bytes_copy"/> parameter was negative.</exception>
        [Throws(
            typeof(System.IO.IOException),
            typeof(ArgumentNullException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException),
            typeof(ArgumentOutOfRangeException)
        )]
        public static void CopySpecificToStream(this MP.IO.IDataStreamAccess input, MP.IO.IDataStreamAccess output, System.Int64 bytes_copy, System.Int32 buffer_size)
        {
            ArgumentNullException.ThrowIfNull(output);
            if (bytes_copy < 0) { 
                throw new ArgumentOutOfRangeException(nameof(bytes_copy), "The number of bytes to copy cannot be negative."); 
            } else if (buffer_size < 1024) { 
                throw new ArgumentOutOfRangeException(nameof(buffer_size), "The buffer_size parameter is too small and could degrade performance."); 
            } else {
                using (var buffer = ArrayPool<System.Byte>.Shared.RentByContext(buffer_size))
                {
                    int bytes_copied, buf_size = buffer.ActualLength;
                    for (long consumed = 0; consumed < bytes_copy; consumed += bytes_copied)
                    {
                        if ((bytes_copied = input.Read(buffer, 0, ComputeStreamBufferSize(consumed, bytes_copy, buf_size))) > 0) {
                            output.Write(buffer, 0, bytes_copied);
                        } else {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Directly copies the specified number of bytes from the current stream to the destination stream. <br />
        /// Both positions will be respectively updated by <paramref name="bytes_copy"/>.
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <param name="output">The target stream.</param>
        /// <param name="bytes_copy">The exact number of bytes to copy from this stream to <paramref name="output"/>.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="output"/> parameter was null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="bytes_copy"/> parameter was negative.</exception>
        [Throws(
            typeof(System.IO.IOException),
            typeof(ArgumentNullException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException),
            typeof(ArgumentOutOfRangeException)
        )]
        public static void CopySpecificToStream(this MP.IO.IDataStreamAccess input, MP.IO.IDataStreamAccess output, System.Int64 bytes_copy)
            => CopySpecificToStream(input, output, bytes_copy, BUFSIZE);
    }
}