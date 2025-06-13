
using System;
using MP.ComInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Defines extension methods around the <see cref="IMFTransform"/> interface.
    /// </summary>
    public static unsafe class IMFTransformExtensions
    {
        public static HRESULT ProcessInput(this IMFTransform transform , System.UInt32 stream , IMFSample sample)
            // Not checking for null argument to be sure that this method would execute as fast as possible.
            => transform.ProcessInput(stream, Marshal.GetIUnknownForObject(sample).ToPointer());

        public static HRESULT ProcessOutput(this IMFTransform transform , 
            MFT_PROCESS_OUTPUT_FLAGS flags, 
            ref MFT_OUTPUT_DATA_BUFFER buffer,
            out MFT_PROCESS_OUTPUT_STATUS status)
        {
            MFT_PROCESS_OUTPUT_STATUS s;
            HRESULT hr;
            // In theory we could use Unsafe.AsPointer instead of fixed keyword, but I don't know
            // if MFT_OUTPUT_DATA_BUFFER will remain in the same place after passing it to the ProcessOutput.
            fixed (MFT_OUTPUT_DATA_BUFFER* pbf = &buffer)
            {
                hr = transform.ProcessOutput(flags, 1, pbf, &s);
            }
            status = s;
            return hr;
        }

        public static HRESULT GetStreamIDs(this IMFTransform transform , out System.UInt32[] inputids , out System.UInt32[] outputids)
        {
            inputids = null;
            outputids = null;
            System.UInt32 cin, cout;
            HRESULT hr = transform.GetStreamCount(&cin , &cout);
            if (hr.FAILED) { return hr; }
            inputids = new System.UInt32[cin];
            outputids = new System.UInt32[cout];
            fixed (System.UInt32* idsinp = inputids) 
            fixed (System.UInt32* idsoutp = outputids)
            {
                hr = transform.GetStreamIDs(cin , idsinp , cout , idsoutp);
            }
            return hr;
        }

        public static HRESULT GetStreamCount(this IMFTransform transform, out System.UInt32 instreams, out System.UInt32 outstreams) 
        {
            System.UInt32 cin ,cout;
            HRESULT hr = transform.GetStreamCount(&cin, &cout);
            instreams = cin;
            outstreams = cout;
            return hr;
        }

        public static HRESULT GetInputStreamInfo(this IMFTransform transform , System.UInt32 streamin , out MFT_INPUT_STREAM_INFO inf)
        {
            MFT_INPUT_STREAM_INFO info;
            HRESULT hr = transform.GetInputStreamInfo(streamin, &info);
            inf = info;
            return hr;
        }

        public static HRESULT GetOutputStreamInfo(this IMFTransform transform , System.UInt32 streamout , out MFT_OUTPUT_STREAM_INFO inf)
        {
            MFT_OUTPUT_STREAM_INFO info;
            HRESULT hr = transform.GetOutputStreamInfo(streamout, &info);
            inf = info;
            return hr;
        }

        public static HRESULT GetAttributes(this IMFTransform transform , out IMFAttributes attributes)
        {
            void* pattrs;
            HRESULT hr = transform.GetAttributes(&pattrs);
            if (hr.FAILED) {
                attributes = null;
            } else {
                attributes = ComMarshalling.CreateInteropObject(pattrs) as IMFAttributes;
            }
            return hr;
        }

        public static HRESULT GetInputStreamAttributes(this IMFTransform transform , System.UInt32 streamin , out IMFAttributes attributes)
        {
            void* pattrs;
            HRESULT hr = transform.GetInputStreamAttributes(streamin, &pattrs);
            if (hr.FAILED) {
                attributes = null;
            } else {
                attributes = ComMarshalling.CreateInteropObject(pattrs) as IMFAttributes;
            }
            return hr;
        }

        public static HRESULT GetOutputStreamAttributes(this IMFTransform transform, System.UInt32 streamout, out IMFAttributes attributes)
        {
            void* pattrs;
            HRESULT hr = transform.GetOutputStreamAttributes(streamout, &pattrs);
            if (hr.FAILED) {
                attributes = null;
            } else {
                attributes = ComMarshalling.CreateInteropObject(pattrs) as IMFAttributes;
            }
            return hr;
        }

        public static HRESULT AddInputStreams(this IMFTransform transform , System.UInt32[] streamids)
        {
            ArgumentNullException.ThrowIfNull(streamids);
            HRESULT hr;
            fixed (System.UInt32* pids = streamids)
            {
                hr = transform.AddInputStreams(streamids.LongLength.ToUInt32() , pids);
            }
            return hr;
        }

        public static HRESULT GetInputAvailableType(this IMFTransform transform, System.UInt32 streamid , System.UInt32 typeindex , out IMFMediaType medtype)
        {
            void* ptype;
            HRESULT hr = transform.GetInputAvailableType(streamid , typeindex , &ptype);
            if (hr.FAILED) { 
                medtype = null;
            } else {
                medtype = ComMarshalling.CreateInteropObject(ptype) as IMFMediaType;
            }
            return hr;
        }

        public static HRESULT GetOutputAvailableType(this IMFTransform transform , System.UInt32 streamid , System.UInt32 typeindex , out IMFMediaType medtype)
        {
            void* ptype;
            HRESULT hr = transform.GetOutputAvailableType(streamid, typeindex, &ptype);
            if (hr.FAILED) {
                medtype = null;
            } else {
                medtype = ComMarshalling.CreateInteropObject(ptype) as IMFMediaType;
            }
            return hr;
        }

        public static HRESULT SetInputType(this IMFTransform transform , System.UInt32 streamid , IMFMediaType medtype , MFT_SET_TYPE_FLAGS flags = MFT_SET_TYPE_FLAGS.None)
            => transform.SetInputType(streamid, medtype is null ? null : Marshal.GetIUnknownForObject(medtype).ToPointer(), flags);

        public static HRESULT SetOutputType(this IMFTransform transform , System.UInt32 streamid, IMFMediaType medtype, MFT_SET_TYPE_FLAGS flags = MFT_SET_TYPE_FLAGS.None)
            => transform.SetOutputType(streamid, medtype is null ? null : Marshal.GetIUnknownForObject(medtype).ToPointer(), flags);

        public static HRESULT GetInputCurrentType(this IMFTransform transform , System.UInt32 streamid , out IMFMediaType medtype)
        {
            void* ptype;
            HRESULT hr = transform.GetInputCurrentType(streamid, &ptype);
            if (hr.FAILED) {
                medtype = null;
            } else {
                medtype = ComMarshalling.CreateInteropObject(ptype) as IMFMediaType;
            }
            return hr;
        }

        public static HRESULT GetOutputCurrentType(this IMFTransform transform, System.UInt32 streamid, out IMFMediaType medtype)
        {
            void* ptype;
            HRESULT hr = transform.GetOutputCurrentType(streamid, &ptype);
            if (hr.FAILED) {
                medtype = null;
            } else {
                medtype = ComMarshalling.CreateInteropObject(ptype) as IMFMediaType;
            }
            return hr;
        }

        public static HRESULT GetInputStatus(this IMFTransform transform , System.UInt32 streamid , out MFT_INPUT_STATUS_FLAGS status)
        {
            MFT_INPUT_STATUS_FLAGS f;
            HRESULT hr = transform.GetInputStatus(streamid, &f);
            status = f;
            return hr;
        }

        public static HRESULT GetOutputStatus(this IMFTransform transform , out MFT_OUTPUT_STATUS_FLAGS output)
        {
            MFT_OUTPUT_STATUS_FLAGS o;
            HRESULT hr = transform.GetOutputStatus(&o);
            output = o; 
            return hr;
        }

        public static HRESULT ProcessEvent(this IMFTransform transform , System.UInt32 streamid , IMFMediaEvent evt)
        {
            ArgumentNullException.ThrowIfNull(evt);
            return transform.ProcessEvent(streamid, Marshal.GetIUnknownForObject(evt).ToPointer());
        }

        public static HRESULT GetStreamLimits(this IMFTransform transform , 
            out System.UInt32 instreamminimum,
            out System.UInt32 instreammaximum,
            out System.UInt32 outstreaminimum,
            out System.UInt32 outstreammaximum)
        {
            System.UInt32 i1, i2, o1, o2;
            HRESULT hr = transform.GetStreamLimits(&i1,&i2,&o1,&o2);
            instreamminimum = i1;
            instreammaximum = i2;
            outstreaminimum = o1;
            outstreammaximum = o2;
            return hr;
        }
    }
}
