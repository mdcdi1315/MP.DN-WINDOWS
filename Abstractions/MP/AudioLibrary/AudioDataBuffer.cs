
using System;
using MP.Collections;
using MP.Annotations;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Provides an audio buffer that can be provided audio data to write to, and read data from it as well. <br />
    /// This is provided when users write <see cref="IAudioProvider"/> instances, but the underlying
    /// sources they wrap return data which their length cannot be known prior to calling the platform method.
    /// </summary>
    /// <remarks>
    /// This class is expected to be used as follows: <br />
    /// <code>
    /// using MP.AudioLibrary;
    /// 
    /// // a class that implements IAudioProvider....
    /// private AudioDataBuffer buffer;
    /// 
    /// public int Read(Span&lt;System.Byte&gt; bytes)
    /// {
    ///     AudioDataBuffer.ReadDataContext context;
    ///     int rb = buffer.ReadData(bytes, out context);
    ///     
    ///     if (!context.BufferHasAdditionalData) {
    ///         int required = bytes.Length;
    ///         
    ///         Span&lt;System.Byte&gt; temp;
    /// 
    ///         while (required > 0) {
    ///             
    ///             // A platform-specific way that fills the 'temp' span, decrements the required counter and dispatches audio format changes.
    ///             
    ///             buffer.AddData(temp);
    ///         }
    ///         
    ///         if (!context.TargetBufferFilled) {
    ///             return buffer.ReadData(bytes, out context);
    ///         }
    ///     }
    ///     return rb;
    /// }
    /// </code>
    /// </remarks>
    [Preliminary]
    public sealed class AudioDataBuffer
    {
        private byte[] data;
        private bool change_on_next_read;
        private int actual_count, read_bytes;
        private readonly SingleLinkedListBasedQueue<AudioFormatChange> changes;
        
        private sealed class AudioFormatChange
        {
            public int Offset; // The data offset where the new format starts
            public AudioFormat New_Format;

            public AudioFormatChange(AudioFormat fmt, int offset)
            {
                Offset = offset;
                New_Format = fmt;
            }
        }

        /// <summary>
        /// Context structure for the ReadData methods.
        /// </summary>
        [Preliminary]
        public readonly struct ReadDataContext
        {
            /// <summary>
            /// Gets a value whether the <see cref="AudioDataBuffer"/> instance has some additional data to process.
            /// </summary>
            public readonly bool BufferHasAdditionalData;
            /// <summary>
            /// Gets a value whether the target audio buffer has been filled (has at least one valid byte in it).
            /// </summary>
            public readonly bool TargetBufferFilled;

            /// <summary>
            /// Initializes a new instance of the <see cref="ReadDataContext"/> structure.
            /// </summary>
            /// <param name="additional">The value for the <see cref="BufferHasAdditionalData"/> field.</param>
            /// <param name="dst_buffer_filled">The value for the <see cref="TargetBufferFilled"/> field.</param>
            public ReadDataContext(bool additional, bool dst_buffer_filled)
            {
                BufferHasAdditionalData = additional;
                TargetBufferFilled = dst_buffer_filled;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OnAudioFormatChangedEmpty(AudioFormat f) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioDataBuffer"/> class.
        /// </summary>
        public AudioDataBuffer()
        {
            read_bytes = 0;
            changes = new();
            actual_count = 0;
            data = new byte[512];
            AudioFormatChanged = new(OnAudioFormatChangedEmpty);
        }

        private void Enlarge(int by)
        {
            int new_count = unchecked(actual_count + by);
            if (new_count < 0) {
                throw new OverflowException("The audio data buffer has reached it's maximal capacity.");
            } else if (new_count > data.Length) {
                if (actual_count == 0) {
                    data = new byte[new_count];
                } else {
                    Array.Resize(ref data, new_count);
                }
            }
        }

        /// <summary>Places buffer data to the current audio data buffer.</summary>
        /// <param name="buffer">The buffer that contains the audio data to copy.</param>
        /// <param name="offset">The offset index inside <paramref name="buffer"/> to start copying data from.</param>
        /// <param name="count">The number of bytes to copy from <paramref name="buffer"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> and/or <paramref name="offset"/> parameters are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="offset"/> + <paramref name="count"/> exceeded the buffer's length.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException), typeof(ArgumentException))]
        public void AddData(byte[] buffer, int offset, int count)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            if (offset < 0) {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset value cannot be a negative value.");
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(offset), "Count value cannot be a negative value.");
            } else if (count + offset > buffer.Length) {
                throw new ArgumentException("Count and offset values cannot exceed the buffer bounds.");
            } else {
                Enlarge(count);
                Unsafe.CopyBlockUnaligned(ref data[actual_count], ref buffer[offset], count.ToUInt32());
                actual_count += count;
            }
        }

        /// <summary>Places buffer data to the current audio data buffer.</summary>
        /// <param name="buffer">The buffer that contains the audio data to copy.</param>
        public void AddData(Span<System.Byte> buffer)
        {
            int count = buffer.Length;
            Enlarge(count);
            Unsafe.CopyBlockUnaligned(ref data[actual_count], ref buffer[0], count.ToUInt32());
            actual_count += count;
        }

        /// <summary>Places buffer data to the current audio data buffer.</summary>
        /// <param name="buffer">The pointer to the memory block that contains the audio data to copy.</param>
        /// <param name="count">The number of bytes to copy from <paramref name="buffer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentOutOfRangeException), typeof(ArgumentNullException))]
        public unsafe void AddData(byte* buffer, int count)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            } else {
                Enlarge(count);
                Unsafe.CopyBlockUnaligned(ref data[actual_count], ref *buffer, count.ToUInt32());
                actual_count += count;
            }
        }

        /// <summary>Places buffer data to the current audio data buffer.</summary>
        /// <param name="buffer">The pointer to the memory block that contains the audio data to copy.</param>
        /// <param name="count">The number of bytes to copy from <paramref name="buffer"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public unsafe void AddData(byte* buffer, uint count)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            int ic = count.ToInt32();
            Enlarge(ic);
            Unsafe.CopyBlockUnaligned(ref data[actual_count], ref *buffer, count);
            actual_count += ic;
        }

        /// <summary>
        /// Places audio buffer data to <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to copy the data into.</param>
        /// <param name="offset">The offset of <paramref name="buffer"/> to start copying data into.</param>
        /// <param name="count">The number of bytes to copy to <paramref name="buffer"/>.</param>
        /// <param name="context">A context structure describing the state of the audio buffer.</param>
        /// <returns>The number of bytes processed and returned through the audio buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> and/or <paramref name="offset"/> are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="count"/> + <paramref name="offset"/> is greater than the <paramref name="buffer"/>'s length.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException), typeof(ArgumentException))]
        public int ReadData(byte[] buffer, int offset, int count, [NotNull] out ReadDataContext context)
        {
            int data_read;
            ArgumentNullException.ThrowIfNull(buffer);
            if (offset < 0) {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset value cannot be a negative value.");
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(offset), "Count value cannot be a negative value.");
            } else if (count == 0) {
                context = new(read_bytes != 0, true);
                return 0;
            } else if (count + offset > buffer.Length) {
                throw new ArgumentException("Count and offset values cannot exceed the buffer bounds.");
            } else {
                AudioFormatChange change;
                if (change_on_next_read) {
                    change_on_next_read = false;
                    changes.TryDequeue(out change);
                    AudioFormatChanged.Invoke(change.New_Format);
                }
                if (changes.TryPeek(out change)) {
                    // We have an audio format that will be changed soon.
                    int bytes_to_read = count;
                    if (read_bytes + count > change.Offset) {
                        change_on_next_read = true;
                        bytes_to_read = change.Offset - read_bytes;
                    }
                    data_read = ReadInternalBuffer(buffer, offset, bytes_to_read);
                } else {
                    data_read = ReadInternalBuffer(buffer, offset, count);
                }
            }
            context = new(read_bytes != 0, data_read > 0);
            return data_read;
        }

        /// <summary>
        /// Places audio buffer data to <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">The buffer to copy the data into.</param>
        /// <param name="context">A context structure describing the state of the audio buffer.</param>
        /// <returns>The number of bytes processed and returned through the audio buffer.</returns>
        public int ReadData(Span<System.Byte> bytes, [NotNull] out ReadDataContext context)
        {
            int data_read;
            if (bytes.Length == 0) {
                context = new(read_bytes != 0, true);
                return 0;
            } else {
                AudioFormatChange change;
                if (change_on_next_read) {
                    change_on_next_read = false;
                    changes.TryDequeue(out change);
                    AudioFormatChanged.Invoke(change.New_Format);
                }
                if (changes.TryPeek(out change)) {
                    // We have an audio format that will be changed soon.
                    int bytes_to_read = bytes.Length;
                    if (read_bytes + bytes.Length > change.Offset) {
                        change_on_next_read = true;
                        bytes_to_read = change.Offset - read_bytes;
                    }
                    data_read = ReadInternalBuffer(bytes.Slice(0, bytes_to_read));
                } else {
                    data_read = ReadInternalBuffer(bytes);
                }
            }
            context = new(read_bytes != 0, data_read > 0);
            return data_read;
        }

        private int ReadInternalBuffer(byte[] buffer, int offset, int count)
        {
            // The below will be either 0 or positive integer less or equal than actual_count.
            int actually_reading_bytes = Math.Min(actual_count, count);
            Unsafe.CopyBlockUnaligned(ref buffer[offset], ref data[read_bytes], actually_reading_bytes.ToUInt32());
            actual_count -= actually_reading_bytes;
            // We need to reset the read_bytes counter if we have processed all the bytes, or otherwise we need to update it.
            read_bytes = (actual_count == 0) ? 0 : read_bytes + actually_reading_bytes;
            return actually_reading_bytes;
        }

        private int ReadInternalBuffer(Span<System.Byte> bytes)
        {
            // The below will be either 0 or positive integer less or equal than actual_count.
            int actually_reading_bytes = Math.Min(actual_count, bytes.Length);
            Unsafe.CopyBlockUnaligned(ref bytes[0], ref data[read_bytes], actually_reading_bytes.ToUInt32());
            actual_count -= actually_reading_bytes;
            // We need to reset the read_bytes counter if we have processed all the bytes, or otherwise we need to update it.
            read_bytes = (actual_count == 0) ? 0 : read_bytes + actually_reading_bytes;
            return actually_reading_bytes;
        }

        /// <summary>
        /// Records a change in the audio format data. <br />
        /// Reading methods will ensure that they will return all the current format data, and then will dispatch the audio format change event.
        /// </summary>
        /// <param name="new_format">The new audio format that next AddData calls will record to the buffer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="new_format"/> is <see langword="null"/>.</exception>
        public void RecordAudioFormatChange(AudioFormat new_format)
        {
            ArgumentNullException.ThrowIfNull(new_format);
            changes.Enqueue(new(new_format, actual_count));
        }

        /// <summary>
        /// Provides the event that is dispatched once the audio format has been actually changed.
        /// </summary>
        public event AudioFormatChangedEventHandler AudioFormatChanged;

        /// <summary>
        /// Invalidates all the data written to the buffer so far. <br />
        /// Audio format change notifications are deleted as well.
        /// </summary>
        public void Clear()
        {
            read_bytes = 0;
            changes.Clear();
            actual_count = 0;
        }

        /// <summary>
        /// Gets the number of bytes that were read through the ReadData methods. <br />
        /// For informational purposes only.
        /// </summary>
        public int ReadBytes => read_bytes;

        /// <summary>
        /// Gets the number of remaining bytes in the underlying data buffer. <br />
        /// For informational purposes only.
        /// </summary>
        public int RemainingBytes => actual_count;

        /// <summary>
        /// Gets the buffer's capacity. <br />
        /// For informational purposes only.
        /// </summary>
        public int BufferCapacity => data.Length;

        /// <summary>
        /// Returns a string that describes the current object instance.
        /// </summary>
        public override string ToString() => $"Audio Data Buffer: {{ Read: {read_bytes} Remaining: {actual_count} Capacity: {data.LongLength} }}";
    }
}