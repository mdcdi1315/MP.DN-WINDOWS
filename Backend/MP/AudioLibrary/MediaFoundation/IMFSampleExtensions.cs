
using System;
using MP.ComInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Defines marshalling extension methods around the <see cref="IMFSample"/> interface.
    /// </summary>
    public static unsafe class IMFSampleExtensions
    {
        public static HRESULT ConvertToContiguousBuffer(this IMFSample sample, out IMFMediaBuffer buffer)
        {
            void* pbuffer;
            HRESULT hr = sample.ConvertToContiguousBuffer(&pbuffer);
            if (hr.FAILED) {
                buffer = null;
            } else {
                buffer = ComMarshalling.CreateInteropObject(pbuffer) as IMFMediaBuffer;
            }
            return hr;
        }

        public static HRESULT AddBuffer(this IMFSample sample , IMFMediaBuffer buffer)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            return sample.AddBuffer(Marshal.GetIUnknownForObject(buffer).ToPointer());
        }

        public static HRESULT CopyToBuffer(this IMFSample sample , IMFMediaBuffer buffer)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            return sample.CopyToBuffer(Marshal.GetIUnknownForObject(buffer).ToPointer());
        }

        public static HRESULT GetTotalLength(this IMFSample sample, out System.UInt32 pcbTotalLength)
        {
            System.UInt32 tl;
            HRESULT hr = sample.GetTotalLength(&tl);
            pcbTotalLength = tl;
            return hr;
        }

        public static HRESULT GetBufferByIndex(this IMFSample sample, System.UInt32 dwIndex, out IMFMediaBuffer ppBuffer)
        {
            void* pb;
            HRESULT hr = sample.GetBufferByIndex(dwIndex, &pb);
            if (hr.FAILED) {
                ppBuffer = null;
            } else {
                ppBuffer = ComMarshalling.CreateInteropObject(pb) as IMFMediaBuffer;
            }
            return hr;
        }

        public static HRESULT GetBufferCount(this IMFSample sample , out System.UInt32 pdwBufferCount)
        {
            System.UInt32 bc;
            HRESULT hr = sample.GetBufferCount(&bc);
            pdwBufferCount = bc;
            return hr;
        }
    
        public static HRESULT GetSampleTime(this IMFSample sample , out System.Int64 sampletime)
        {
            System.Int64 time;
            HRESULT hr = sample.GetSampleTime(&time);
            sampletime = time;
            return hr;
        }
    }
}