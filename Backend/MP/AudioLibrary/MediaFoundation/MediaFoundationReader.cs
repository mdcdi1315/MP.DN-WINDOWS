/*
 
Portions of code are adapted from NAudio:

Copyright 2020 Mark Heath

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 
 */

using System;
using MP.ComInterop;
using MP.WindowsInterop;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.AudioLibrary.MediaFoundation
{
    public abstract class MediaFoundationReader : AudioStream
    {
        [Flags]
        private enum ReaderFlags : System.Byte
        {
            None = 0,
            SeekSupported = 1,
            NeedsReposition = 2,
        }

        private TimeSpan length;
        private ReaderFlags flags;
        private AudioFormat format;
        private GUID pddurationguid;
        private System.Int64 position;
        private IMFSourceReader reader;
        private System.Byte[] decoderoutputbuffer , encodedmedtype;
        private System.Int32 decoderoutputoffset , decoderoutputcount;

        private void EnsureBuffer(System.Int32 bytesRequired)
        {
            if (decoderoutputbuffer is null || decoderoutputbuffer.Length < bytesRequired)
            {
                decoderoutputbuffer = new byte[bytesRequired];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private System.Boolean FlagsHaveFlag(ReaderFlags flg) => (flags & flg) != 0;

        protected MediaFoundationReader() 
        {
            format = null;
            reader = null;
            length = new(0);
            position = 0;
            flags = ReaderFlags.None;
            pddurationguid = GUID.FromGUID(MediaFoundationAttributes.PD_DURATION);
        }

        protected unsafe void Initialize()
        {
            try {
                CreateReaderInternal();
                AssignCurrentAudioFormat();
                length = GetLength();
                // Test whether we can seek. 
                // We will just seek to the beginning of the file to see if supported
                var pv = PROPVARIANT.FromLong(0);
                GUID empty = GUID.Empty;
                // Depending on this seek we will know whether the file is then supported for seeking
                if (reader.SetCurrentPosition(&empty, &pv).SUCCEEDED) {
                    flags |= ReaderFlags.SeekSupported;
                }
            } finally {
                ComMarshalling.ReleaseInteropObject(reader);
                reader = null;
                DisposeSourceReaderResources();
            }
        }

        [return: NotNull]
        protected abstract IMFSourceReader GetSourceReader();

        /// <summary>
        /// Disposes only the Source Reader-associated resources. <br />
        /// This is done because the Source Reader will be created twice.
        /// </summary>
        protected abstract void DisposeSourceReaderResources();

        private void CreateReaderInternal()
        {
            MediaFoundationMediaType partialmediatype = MediaFoundationMediaType.CreateEmpty();
            partialmediatype.MajorType = MediaFoundationMediaType.AudioMajorType;
            partialmediatype.SubType = MediaFoundationMediaType.CreateMediaSubTypeGUIDFromFormatTag(WAVEFORMATTAG.Pcm);

            MediaFoundationMediaType currenttype = null; 

            reader = GetSourceReader();
            try {
                 currenttype = GetCurrentMediaType();

                // mono, low sample rate files can go wrong on Windows 10 unless we specify here
                partialmediatype.Channels = currenttype.Channels;
                partialmediatype.SampleRate = currenttype.SampleRate;

                // Close all other irrelevant streams
                reader.SetStreamSelection(MF_SOURCE_READER_STREAM_SELECTION.ALL_STREAMS, false).ThrowOnFailure();
                // Only activate the first audio stream
                reader.SetStreamSelection(MF_SOURCE_READER_STREAM_SELECTION.FIRST_AUDIO_STREAM, true).ThrowOnFailure();

                // set the media type
                // can return MF_E_INVALIDMEDIATYPE if not supported
                HRESULT hr = reader.SetCurrentMediaType(MF_SOURCE_READER_STREAM_SELECTION.FIRST_AUDIO_STREAM, partialmediatype.NativeMediaType);

                switch (hr)
                {
                    case MediaFoundationErrorCodes.MF_E_INVALIDMEDIATYPE:
                        // HE-AAC (and v2) seems to halve the samplerate
                        if (currenttype.SubType == MediaFoundationMediaType.CreateMediaSubTypeGUIDFromFormatTag(WAVEFORMATTAG.MPEG_HEAAC) && currenttype.Channels == 1)
                        {
                            partialmediatype.SampleRate = currenttype.SampleRate *= 2;
                            partialmediatype.Channels = currenttype.Channels *= 2;
                            reader.SetCurrentMediaType(MF_SOURCE_READER_STREAM_SELECTION.FIRST_AUDIO_STREAM, partialmediatype.NativeMediaType).ThrowOnFailure();
                        }
                        break;
                    default:
                        hr.ThrowOnFailure();
                        break;
                }
                encodedmedtype = partialmediatype.Serialize();
            } finally {
                partialmediatype?.Dispose();
                partialmediatype = null;
                currenttype?.Dispose();
                currenttype = null;
            }
        }

        private void CreateReaderOnRead()
        {
            reader = GetSourceReader();

            MediaFoundationMediaType partialmediatype = MediaFoundationMediaType.DeserializeFromBuffer(encodedmedtype);
            encodedmedtype = null;

            // Close all other irrelevant streams
            reader.SetStreamSelection(MF_SOURCE_READER_STREAM_SELECTION.ALL_STREAMS, false).ThrowOnFailure();
            // Only activate the first audio stream
            reader.SetStreamSelection(MF_SOURCE_READER_STREAM_SELECTION.FIRST_AUDIO_STREAM, true).ThrowOnFailure();

            // No need to check again for format errors since we have encoded the media type itself. Initialize directly.
            reader.SetCurrentMediaType(MF_SOURCE_READER_STREAM_SELECTION.FIRST_AUDIO_STREAM, partialmediatype.NativeMediaType);
        }

        private MediaFoundationMediaType GetCurrentMediaType()
        {
            reader.GetCurrentMediaType(MF_SOURCE_READER_STREAM_SELECTION.FIRST_AUDIO_STREAM, out IMFMediaType mt).ThrowOnFailure();

            return MediaFoundationMediaType.FromExisting(mt);
        }

        private void AssignCurrentAudioFormat()
        {
            MediaFoundationMediaType mti = null;
            try {
                mti = GetCurrentMediaType();
                format = mti.ToAudioFormat();
            } finally {
                mti?.Dispose();
                mti = null;
            }
        }

        private unsafe TimeSpan GetLength()
        {
            PROPVARIANT pv;
            GUID cpy = pddurationguid;
            // http://msdn.microsoft.com/en-gb/library/windows/desktop/dd389281%28v=vs.85%29.aspx#getting_file_duration
            HRESULT hResult = reader.GetPresentationAttribute(MF_SOURCE_READER_STREAM_SELECTION.MEDIASOURCE, &cpy, &pv);
            if (hResult == MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND)
            {
                // this doesn't support telling us its duration (might be streaming)
                return new(0);
            }
            hResult.ThrowOnFailure();

            return TimeSpan.FromSeconds(pv.PPVTValue.uhVal / 10000000UL);
        }

        private unsafe void Reposition()
        {
            var pv = PROPVARIANT.FromLong((10000000L * position) / format.AverageBytesPerSecond);
            GUID empty = GUID.Empty;
            // should pass in a variant of type VT_I8 which is a long containing time in 100nanosecond units
            reader.SetCurrentPosition(&empty, &pv);
            reader.Flush(MF_SOURCE_READER_STREAM_SELECTION.FIRST_AUDIO_STREAM);
            decoderoutputcount = 0;
            decoderoutputoffset = 0;
            FireCurrentTimeInvalidatedEvent();
            flags &= ~ReaderFlags.NeedsReposition; // Clear the flag
        }

        private System.Int32 ReadFromDecoderBuffer(System.Byte[] buffer, System.Int32 offset, System.Int32 needed)
        {
            System.Int32 bytesFromDecoderOutput = Math.Min(needed, decoderoutputcount);
            Array.Copy(decoderoutputbuffer, decoderoutputoffset, buffer, offset, bytesFromDecoderOutput);
            decoderoutputoffset += bytesFromDecoderOutput;
            decoderoutputcount -= bytesFromDecoderOutput;
            if (decoderoutputcount == 0)
            {
                decoderoutputoffset = 0;
            }
            return bytesFromDecoderOutput;
        }

        /// <summary>
        /// Cleans up after finishing with this reader
        /// </summary>
        /// <param name="disposing">true if called from Dispose</param>
        protected override void Dispose(bool disposing)
        {
            encodedmedtype = null;
            if (reader is not null)
            {
                ComMarshalling.ReleaseInteropObject(reader);
                reader = null;
            }
            DisposeSourceReaderResources();
            base.Dispose(disposing);
        }
        
        /// <summary>Reads from this audio stream</summary>
        /// <param name="buffer">Buffer to read into</param>
        /// <param name="offset">Offset in buffer</param>
        /// <param name="count">Bytes required</param>
        /// <returns>Number of bytes read; 0 indicates end of stream</returns>
        public unsafe override System.Int32 Read(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
        {
            if (reader is null) { CreateReaderOnRead(); }
            if (FlagsHaveFlag(ReaderFlags.NeedsReposition)) { Reposition(); }

            int bytesWritten = 0;
            // read in any leftovers from last time
            if (decoderoutputcount > 0)
            {
                bytesWritten += ReadFromDecoderBuffer(buffer, offset, count - bytesWritten);
            }

            HRESULT hr = default;
            IMFSample pSample = null;
            IMFMediaBufferNative pBuffer;
            while (bytesWritten < count)
            {
                hr = reader.ReadSample(MF_SOURCE_READER_STREAM_SELECTION.FIRST_AUDIO_STREAM, 0,
                    out _,
                    out long timestamp,
                    out MF_SOURCE_READER_FLAG dwFlags,
                    out pSample);
                if (hr.FAILED) { break; }
                if ((dwFlags & MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_ENDOFSTREAM) != 0)
                {
                    // reached the end of the stream
                    break;
                }
                else if ((dwFlags & MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_CURRENTMEDIATYPECHANGED) != 0)
                {
                    AssignCurrentAudioFormat();
                    //OnWaveFormatChanged();
                    // carry on, but user must handle the change of format
                }
                else if (dwFlags != 0)
                {
                    throw new InvalidOperationException($"MediaFoundationReadError {dwFlags}");
                }

                pSample.ConvertToContiguousBuffer(out pBuffer).ThrowOnFailure();
                System.Byte* pd;
                System.UInt32 maxlen, bufsize;
                decoderoutputoffset = 0;
                pBuffer.Lock(&pd, &maxlen, &bufsize);
                EnsureBuffer(decoderoutputcount = bufsize.ToInt32());
                Unsafe.CopyBlockUnaligned(ref decoderoutputbuffer[0], ref pd[0], bufsize);
                pBuffer.Unlock();

                while (pBuffer.Release() > 0) ;
                ComMarshalling.ReleaseInteropObject(pSample);

                bytesWritten += ReadFromDecoderBuffer(buffer, offset + bytesWritten, count - bytesWritten);
            }
            hr.ThrowOnFailure();
            position += bytesWritten;
            return bytesWritten;
        }

        public override AudioFormat Format => format;

        public override TimeSpan CurrentTime 
        { 
            get => TimeSpan.FromSeconds((double)position / format.AverageBytesPerSecond);
            set {
                if (FlagsHaveFlag(ReaderFlags.SeekSupported) == false) {
                    throw new NotSupportedException("Seeking in the audio stream is not supported by this reader object, or it's current configuration.");
                }
                if (value > length) {
                    throw new ArgumentException("The passed time interval represents a time beyond the end of the audio file."  , nameof(value));
                }
                flags |= ReaderFlags.NeedsReposition;
                position = (System.Int64)(value.TotalSeconds * format.AverageBytesPerSecond);
            }
        }

        public override TimeSpan TotalTime => length;
    }
}