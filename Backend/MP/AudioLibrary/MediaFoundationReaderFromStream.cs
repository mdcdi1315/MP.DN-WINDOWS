
using System;
using MP.ComInterop;
using System.Diagnostics.CodeAnalysis;
using MP.AudioLibrary.MediaFoundation;

namespace MP.AudioLibrary
{
    public sealed class MediaFoundationReaderFromStream : MediaFoundationReader
    {
        private System.String filename;
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
            filename = null;
            aps.TryGetCustomAttribute("FileName", out filename); // We do not care whether it will be set or not, otherwise default(System.String) is known to be null.
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
                if (attrs is not null && filename is not null)
                {
                    attrs.SetAttribute(IMFByteStreamAttributes.CONTENT_TYPE, Microsoft.IO.Path.GetExtension(filename) switch {
                        ".wav" => "audio/wav",
                        ".flac" => "audio/flac",
                        ".mp3" => "audio/mp3",
                        ".m4a" => "audio/mp4",
                        ".3gpp" => "audio/3gpp",
                        ".aac" => "audio/aac",
                        ".ogg" => "audio/ogg",
                        _ => ""
                    });
                    // Since we can do it, why not also setting the ORIGIN_NAME attribute?
                    attrs.SetAttribute(IMFByteStreamAttributes.ORIGIN_NAME , $"file:///{filename}");
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
            if (bytestream is not null)
            {
                bytestream.Close();
                try { ComMarshalling.ReleaseInteropObject(bytestream); } catch (ArgumentException) { }
            }
            bytestream = null;
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