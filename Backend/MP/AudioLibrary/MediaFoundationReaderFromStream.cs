
using System;
using MP.ComInterop;
using System.Diagnostics.CodeAnalysis;
using MP.AudioLibrary.MediaFoundation;

namespace MP.AudioLibrary
{
    public sealed class MediaFoundationReaderFromStream : MediaFoundationReader
    {
        private System.String fn;
        private IMFByteStream bytestream;
        private AbstractPropertyStream aps;

        public MediaFoundationReaderFromStream(AbstractPropertyStream aps)
        {
            ArgumentNullException.ThrowIfNull(aps);
            if (aps is not IStream && aps is not IMFByteStream) {
                throw new ArgumentException("The passed in property stream must at least implement COM's IStream interface.");
            }
            if (aps.CanRead == false) {
                throw new ArgumentException("The passed in property stream must be readable at least.");
            }
            this.aps = aps;
            fn = Microsoft.IO.Path.GetExtension(aps.GetStringAttribute("FileName"));
            Initialize();
        }

        [return: NotNull]
        protected override IMFSourceReader GetSourceReader()
        {
            if (bytestream is null)
            {
                if (aps is IMFByteStream b) {
                    bytestream = b;
                } else if (aps is IStream s) {
                    bytestream = MediaFoundationInterfacesFactory.CreateFromWrappingStream(s);
                } else {
                    throw new NotSupportedException("Cannot determine the byte stream mode to perform.");
                }
                IMFAttributes attrs = bytestream as IMFAttributes;
                if (attrs is not null && fn is not null)
                {
                    attrs.SetAttribute(IMFByteStreamAttributes.CONTENT_TYPE, fn switch {
                        ".wav" => "audio/wav",
                        ".flac" => "audio/flac",
                        ".mp3" => "audio/mp3",
                        ".m4a" => "audio/mp4",
                        ".3gpp" => "audio/3gpp",
                        ".aac" => "audio/aac",
                        ".ogg" => "audio/ogg",
                        _ => ""
                    });
                }
            }

            HRESULT hr = Interop.MfReadWrite.MFCreateSourceReaderFromByteStream(bytestream, null, out var srcr);
            if (hr.FAILED) {
                bytestream.Close();
                try { ComMarshalling.ReleaseInteropObject(bytestream); } catch (ArgumentException) { }
                bytestream = null;
                aps?.Dispose();
                aps = null;
                hr.ThrowOnFailure();
            }
            return srcr;
        }

        protected override void DisposeSourceReaderResources()
        {
            if (bytestream is not null && !Object.ReferenceEquals(bytestream , aps))
            {
                bytestream.Close();
                try { ComMarshalling.ReleaseInteropObject(bytestream); } catch (ArgumentException) { }
                bytestream = null;
            }
        }

        protected override void Dispose(bool disposing) 
        {
            base.Dispose(disposing);
            if (disposing && aps is not null) 
            {
                aps.Dispose();
                aps = null;
            }
        }
    }
}