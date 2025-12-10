/*
 
Portions of code are adapted from NAudio:

Copyright 2020 Mark Heath

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 
 */

using System;
using MP.ComInterop;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using MP.Annotations;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Defines the base class for MFT's. <br />
    /// It can assist you to easily create new I/O MFT's by just providing the MFT object itself.
    /// </summary>
    public abstract class MediaFoundationTransform : IInvalidatableAudioProvider
    {
        [Flags]
        private enum MEDTRANSFORMFLAGS : System.Byte
        {
            None = 0,
            Disposed = 1,
            InitedForStreaming = 2,
            FireReposition = 4,
        }

        private AudioFormat fmtout;
        private IAudioProvider provider;

        private readonly System.Byte[] sourcebuffer;

        private System.Byte[] outputbuffer;
        private System.Int32 outputbufferoffset;
        private System.Int32 outputbuffercount;

        private IMFTransform transform; // The actual COM object.
        private System.Int64 inputposition; // in ref-time, so we can timestamp the input samples
        private System.Int64 outputposition; // also in ref-time
        private MEDTRANSFORMFLAGS flags; // The transform's behavioral flags.

        private IMFSample tempsample1 , tempsample2; // Temporary samples - are pooled so that no additional samples are allocated at run time

        private IMFMediaBufferNative buffer;

        private static System.Int64 BytesToNsPosition(System.Int32 bytes, AudioFormat format) => (10000000L * bytes) / format.AverageBytesPerSecond;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private System.Boolean HasFlagFast(MEDTRANSFORMFLAGS flag) => (flags & flag) == flag;

        public MediaFoundationTransform(IAudioProvider inputprovider , AudioFormat desiredoutformat , System.Int32 sourcebufferinglatencyinms = 100)
        {
            ArgumentNullException.ThrowIfNull(inputprovider);
            ArgumentNullException.ThrowIfNull(desiredoutformat);
            if (sourcebufferinglatencyinms < 55) {
                throw new ArgumentException("Source buffering latency value is too small." , nameof(sourcebufferinglatencyinms));
            }
            fmtout = desiredoutformat;
            provider = inputprovider;
            // TODO: Maybe find a way to express the exact latency without losing up to 9 ms (possibly by doing a rem test on provider.Format.BlockAlignment instead)
            // Always round to a latency less than the actual and being multiple of 10 - this allows the data alignment
            // to work correctly and avoid race conditions about data depiction on uneven sample rates (such as the 44100 kHz).
            sourcebufferinglatencyinms -= sourcebufferinglatencyinms % 10; 
            // The source buffer length thus is transformed with this formula: (System.Int64)(sourcebufferinglatencyinms * 0.001D * provider.Format.AverageBytesPerSecond)
            sourcebuffer = new System.Byte[(System.Int64)((sourcebufferinglatencyinms * 0.001D) * provider.Format.AverageBytesPerSecond)]; 
            outputbuffer = new System.Byte[fmtout.AverageBytesPerSecond + fmtout.BlockAlignment]; // we will grow this buffer if needed, but try to make something big enough
            flags = MEDTRANSFORMFLAGS.None;
        }

        private void InitTransformForStreaming()
        {
            // Only check about the last processed message about an error.
            // If the last was failed, all the previous ones would have failed too.
            transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_COMMAND_FLUSH, 0);
            transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_NOTIFY_BEGIN_STREAMING, 0);
            transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_NOTIFY_START_OF_STREAM, 0).ThrowOnFailure();
            // Set the streaming flag, if successfull
            flags |= MEDTRANSFORMFLAGS.InitedForStreaming;
        }

        /// <summary>
        /// Creates and returns an <see cref="IMFTransform"/> instance. <br />
        /// From this method , the actual transform configuration is performed. <br />
        /// May be called twice.
        /// </summary>
        /// <returns>A COM object of type <see cref="IMFTransform"/>.</returns>
        protected abstract IMFTransform CreateTransform();

        /// <summary>
        /// Indicates the audio format under which this Media Foundation Transform should output results as
        /// </summary>
        public AudioFormat Format => fmtout;

        /// <summary>
        /// Reads data out of the source, passing it through the transform
        /// </summary>
        /// <param name="buffer">Output buffer</param>
        /// <param name="offset">Offset within buffer to write to</param>
        /// <param name="count">Desired byte count</param>
        /// <returns>Number of bytes read</returns>
        public System.Int32 Read(byte[] buffer, int offset, int count)
        {
            if (transform is null)
            {
                transform = CreateTransform();
                InitTransformForStreaming();
                // We have to create our own samples to pass into the MFT
                HRESULT hr = MediaFoundationInterfacesFactory.CreateSample(out tempsample1);
                hr.ThrowOnFailure();
                hr = MediaFoundationInterfacesFactory.CreateSample(out tempsample2);
                hr.ThrowOnFailure();
                this.buffer = CreateBufferAndReturn(sourcebuffer.Length);
            }
            // Reposition code
            if (HasFlagFast(MEDTRANSFORMFLAGS.FireReposition))
            {
                //transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_DROP_SAMPLES, 0);
                //transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_COMMAND_FLUSH, 0);
                //EndStreamAndDrain();
                ClearOutputBuffer();
                transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_DROP_SAMPLES, 0);
                //transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_COMMAND_FLUSH, 0);
                //InitTransformForStreaming();
                flags &= ~MEDTRANSFORMFLAGS.FireReposition;
            }

            // strategy will be to always read one second from the source, and give it to the MFT
            System.Int32 bytesWritten = 0;


            // read in any leftovers from last time
            if (outputbuffercount > 0)
            {
                bytesWritten += ReadFromOutputBuffer(buffer, offset, count - bytesWritten);
            }

            while (bytesWritten < count)
            {
                ReadFromSource();
                if (tempsample2 is null) // reached the end of our input
                {
                    // be good citizens and send some end messages:
                    EndStreamAndDrain();
                    // The MFT might have given us a little bit more to return
                    bytesWritten += ReadFromOutputBuffer(buffer, offset + bytesWritten, count - bytesWritten);
                    ClearOutputBuffer();
                    break;
                }

                // might need to resurrect the stream if the user has read all the way to the end,
                // and then repositioned the input backwards
                if (HasFlagFast(MEDTRANSFORMFLAGS.InitedForStreaming) == false)
                {
                    InitTransformForStreaming();
                }

                // give the input to the MFT
                // can get MF_E_NOTACCEPTING if we didn't drain the buffer properly
                transform.ProcessInput(0, tempsample2).ThrowOnFailure();

                tempsample2.RemoveAllBuffers(); // Fixes some media buffer mem leaks
                tempsample2.DeleteAllItems(); // RemoveAllItems from IMFAttributes to make a clean sample again

                System.Int32 readFromTransform;
                // n.b. in theory we ought to loop here, although we'd need to be careful as the next time into ReadFromTransform there could
                // still be some leftover bytes in outputBuffer, which would get overwritten. Only introduce this if we find a transform that 
                // needs it. For most transforms, alternating read/write should be OK

                //do
                //{
                // keep reading from transform
                readFromTransform = ReadFromTransform();
                bytesWritten += ReadFromOutputBuffer(buffer, offset + bytesWritten, count - bytesWritten);
                //} while (readFromTransform > 0);
            }

            return bytesWritten;
        }

        private void EndStreamAndDrain()
        {
            transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_NOTIFY_END_OF_STREAM, 0);
            transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_COMMAND_DRAIN, 0);
            int r;
            do { r = ReadFromTransform(); } while (r > 0);
            inputposition = 0;
            outputposition = 0;
            transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_NOTIFY_END_STREAMING, 0).ThrowOnFailure();
            flags &= ~MEDTRANSFORMFLAGS.InitedForStreaming;
        }

        private void ClearOutputBuffer()
        {
            outputbuffercount = 0;
            outputbufferoffset = 0;
        }

        private static IMFMediaBufferNative CreateBufferAndReturn(int length)
        {
            Interop.MfPlat.MFCreateMemoryBuffer_IntPtr(length, out var buf).ThrowOnFailure();
            return new IMFMediaBufferNative(buf);
        }

        /// <summary>
        /// Attempts to read from the transform
        /// Some useful info here:
        /// http://msdn.microsoft.com/en-gb/library/windows/desktop/aa965264%28v=vs.85%29.aspx#process_data
        /// </summary>
        /// <returns>The number of bytes provided into the output buffer</returns>
        private unsafe System.Int32 ReadFromTransform()
        {
            var outputDataBuffer = new MFT_OUTPUT_DATA_BUFFER();
            IMFMediaBufferNative bf_source = CreateBufferAndReturn(outputbuffer.Length);
            // AddBuffer can fail for a number of reasons, check error code.
            tempsample1.DeleteAllItems();
            tempsample1.AddBuffer(bf_source.ToNative()).ThrowOnFailure();
            tempsample1.SetSampleTime(outputposition); // hopefully this is not needed
            outputDataBuffer.pSample = Marshal.GetIUnknownForObject(tempsample1).ToPointer();
            outputDataBuffer.pEvents = null;
            
            // Currently MFT_PROCESS_OUTPUT_STATUS is not needed, so discard the variable directly
            var hr = transform.ProcessOutput(0, ref outputDataBuffer , out _);
            if (hr.FAILED) {
                // Destroy sample data.
                tempsample1.RemoveAllBuffers();
                tempsample1.DeleteAllItems();
            }
            switch (hr)
            {
                case MediaFoundationErrorCodes.MF_E_TRANSFORM_NEED_MORE_INPUT:
                    // Possibly processing ends after this error is signaled.
                    // nothing to read
                    return 0;
                default:
                    hr.ThrowOnFailure();
                    break;
            }

            System.Byte* pOutputBuffer;
            System.UInt32 outputBufferLength, maxSize;
            hr = tempsample1.ConvertToContiguousBuffer(out IMFMediaBufferNative cts);
            tempsample1.RemoveAllBuffers();
            if (hr.FAILED) {
                // In such case, the original media buffer will not be destroyed.
                while (bf_source.ToNative() is not null && bf_source.Release() > 0) { }
            }
            hr.ThrowOnFailure();
            cts.Lock(&pOutputBuffer, &maxSize, &outputBufferLength);
            if (outputbuffer.LongLength < outputBufferLength) {
                outputbuffer = new System.Byte[outputBufferLength];
            }
            Unsafe.CopyBlockUnaligned(ref outputbuffer[0], ref pOutputBuffer[0], outputBufferLength);
            cts.Unlock();
            outputbufferoffset = 0;
            while (cts.ToNative() is not null && cts.Release() > 0) ;
            // Ignore disposing the tbf1 media buffer, that has already happened by ConvertToContiguousBuffer.
            outputposition += BytesToNsPosition(outputbuffercount = outputBufferLength.ToInt32(), fmtout); // hopefully not needed
            if (outputDataBuffer.pEvents is not null) {
                while (ComMarshalling.Release(outputDataBuffer.pEvents) > 0);
                outputDataBuffer.pEvents = null;
            }
            outputDataBuffer.pSample = null;
            // Clean sample before leaving
            tempsample1.DeleteAllItems();
            return outputbuffercount;
        }

        private unsafe void ReadFromSource()
        {
            System.Int32 bytesRead = provider.Read(sourcebuffer, 0, sourcebuffer.Length);
            if (bytesRead == 0) {
                // End of stream by the audio provider, put null to indicate no more samples from the source.
                if (tempsample2 is not null)
                {
                    tempsample2.DeleteAllItems();
                    tempsample2.RemoveAllBuffers();
                    ComMarshalling.ReleaseInteropObject(tempsample2);
                    tempsample2 = null;
                }
                return;
            }

            System.Byte* pbuf;
            System.UInt32 maxlen, currentlen;
            buffer.SetCurrentLength(bytesRead.ToUInt32());
            buffer.Lock(&pbuf, &maxlen, &currentlen);
            Unsafe.CopyBlockUnaligned(ref pbuf[0], ref sourcebuffer[0], bytesRead.ToUInt32());
            buffer.Unlock();

            // This may occur if a full playback was performed and the stream was stopped.
            tempsample2 ??= MediaFoundationInterfacesFactory.CreateSample();
            tempsample2.AddBuffer(buffer.ToNative());
            // we'll set the time, I don't think it is needed for Resampler, but other MFTs might need it though.
            tempsample2.SetSampleTime(inputposition);
            long duration = BytesToNsPosition(bytesRead, provider.Format);
            tempsample2.SetSampleDuration(duration);
            inputposition += duration;
        }

        private System.Int32 ReadFromOutputBuffer(byte[] buffer, int offset, int needed)
        {
            int bytesFromOutputBuffer = Math.Min(needed, outputbuffercount);
            Array.Copy(outputbuffer, outputbufferoffset, buffer, offset, bytesFromOutputBuffer);
            outputbufferoffset += bytesFromOutputBuffer;
            outputbuffercount -= bytesFromOutputBuffer;
            if (outputbuffercount == 0)
            {
                outputbufferoffset = 0;
            }
            return bytesFromOutputBuffer;
        }

        /// <summary>
        /// Indicate that the source has been repositioned and completely drain out the transform's buffers
        /// </summary>
        public void Invalidate()
        {
            if (HasFlagFast(MEDTRANSFORMFLAGS.InitedForStreaming))
            {
                flags |= MEDTRANSFORMFLAGS.FireReposition;
            }
        }

        /// <summary>
        /// The original audio provider passed through the constructor
        /// </summary>
        protected IAudioProvider OriginalProvider => provider;

        /// <summary>
        /// Disposes this Media Foundation transform
        /// </summary>
        protected unsafe virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (transform is not null)
                {
                    ComMarshalling.ReleaseInteropObject(transform);
                    transform = null;
                }
                if (tempsample1 is not null)
                {
                    tempsample1.RemoveAllBuffers();
                    tempsample1.DeleteAllItems();
                    ComMarshalling.ReleaseInteropObject(tempsample1);
                    tempsample1 = null;
                }
                if (tempsample2 is not null)
                {
                    tempsample2.RemoveAllBuffers();
                    tempsample2.DeleteAllItems();
                    ComMarshalling.ReleaseInteropObject(tempsample2);
                    tempsample2 = null;
                }
                while (buffer.ToNative() is not null && (buffer.Release() > 0)) { }
                // while (tbf2 != IntPtr.Zero && Marshal.Release(tbf2) > 0) { }
                // Now release and the underlying audio provider too...
                provider?.Dispose();
                provider = null;
            }
        }

        /// <summary>
        /// Disposes this Media Foundation Transform
        /// </summary>
        public void Dispose()
        {
            if (flags.HasFlag(MEDTRANSFORMFLAGS.Disposed)) { return; }
            flags |= MEDTRANSFORMFLAGS.Disposed;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MediaFoundationTransform() => Dispose(false);
    }
}