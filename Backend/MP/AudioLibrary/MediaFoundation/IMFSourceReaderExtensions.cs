
using System;
using MP.ComInterop;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Extensions for the <see cref="IMFSourceReader"/> interface.
    /// </summary>
    public static unsafe class IMFSourceReaderExtensions
    {
        public static HRESULT ReadSample(this IMFSourceReader sr , 
            MF_SOURCE_READER_STREAM_SELECTION sel,
            MF_SOURCE_READER_CONTROL_FLAG f1, 
            out MF_SOURCE_READER_STREAM_SELECTION actualselectedstream , 
            out System.Int64 timestamp , 
            out MF_SOURCE_READER_FLAG srcflags , 
            out IMFSample sample)
        {
            MF_SOURCE_READER_STREAM_SELECTION outsel;
            MF_SOURCE_READER_FLAG srflag;
            System.Int64 ts;
            void* ps;
            HRESULT hr = sr.ReadSample(sel, f1, &outsel, &srflag, &ts, &ps);
            if (ps is not null) {
                // The IMFSample provided seems to be managed by the IMFSourceReader, so trust the interface implementation
                sample = ComMarshalling.CreateInteropObject(ps , -1) as IMFSample;
            } else {
                sample = null;
            }
            timestamp = ts;
            srcflags = srflag;
            actualselectedstream = outsel;
            return hr;
        }

        public static HRESULT GetStreamSelection(this IMFSourceReader sr , MF_SOURCE_READER_STREAM_SELECTION stream, out System.Boolean selected)
        {
            BOOL psel;
            HRESULT hr = sr.GetStreamSelection(stream, &psel);
            selected = psel == BOOL.TRUE;
            return hr;
        }

        public static HRESULT SetStreamSelection(this IMFSourceReader sr , MF_SOURCE_READER_STREAM_SELECTION stream , System.Boolean select)
            => sr.SetStreamSelection(stream , select ? BOOL.TRUE : BOOL.FALSE);

        public static HRESULT GetNativeMediaType(this IMFSourceReader sr , MF_SOURCE_READER_STREAM_SELECTION stream , System.UInt32 mediatypeindex , out IMFMediaType mediatype)
        {
            void* medtype;
            HRESULT hr = sr.GetNativeMediaType(stream, mediatypeindex, &medtype);
            if (hr.FAILED) {
                mediatype = null;
            } else {
                mediatype = ComMarshalling.CreateInteropObject(medtype) as IMFMediaType;
            }
            return hr;
        }

        public static HRESULT SetCurrentMediaType(this IMFSourceReader sr , MF_SOURCE_READER_STREAM_SELECTION stream , IMFMediaType medtype)
            => sr.SetCurrentMediaType(stream, null, Marshal.GetIUnknownForObject(medtype).ToPointer());

        public static HRESULT GetCurrentMediaType(this IMFSourceReader sr , MF_SOURCE_READER_STREAM_SELECTION stream , out IMFMediaType mediatype)
        {
            void* medtype;
            HRESULT hr = sr.GetCurrentMediaType(stream, &medtype);
            if (hr.FAILED) {
                mediatype = null;
            } else {
                mediatype = ComMarshalling.CreateInteropObject(medtype) as IMFMediaType;
            }
            return hr;
        }

        public static HRESULT GetServiceForStream(this IMFSourceReader sr, MF_SOURCE_READER_STREAM_SELECTION stream, Guid serviceguid, Guid interfaceid, out System.Object interfacecomobject)
        {
            GUID sg = GUID.FromGUID(serviceguid);
            GUID iid = GUID.FromGUID(interfaceid);
            void* pi;
            HRESULT hr = sr.GetServiceForStream(stream, &sg, &iid, &pi);
            if (hr.FAILED) {
               interfacecomobject = null;
            } else {
                interfacecomobject = ComMarshalling.CreateInteropObject(pi);
            }
            return hr;
        }
    }
}