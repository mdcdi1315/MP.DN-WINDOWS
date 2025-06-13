
using System;
using System.IO;

namespace MP.Networking
{
    internal sealed class WinInetNetworkStream : Stream
    {
        private System.Int64 length;
        private System.IntPtr internethandle;

        public WinInetNetworkStream(System.IntPtr ih)
        {
            length = -1;
            internethandle = ih;
        }

        public WinInetNetworkStream(System.IntPtr ih , System.Int64 len)
        {
            internethandle = ih;
            length = len;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length
        {
            get {
                if (length == -1) { throw new NotSupportedException(InternalResources.MP_NETWORKING_WININETSTREAM_LENNOTSUPP); }
                return length;
            }
        }

        public override long Position
        {
            get {
                throw new NotSupportedException("Not supported for network connections");
            }
            set { }
        }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (internethandle == IntPtr.Zero) { throw new ObjectDisposedException(nameof(Stream)); }
            ValidateBufferArguments(buffer, offset, count);
            if (Interop.WinInet.InternetReadFile(internethandle, buffer, offset, count, out var br) == Interop.BOOL.FALSE)
            {
                Interop.WinInet.ThrowAppropriateException();
            }
            return br;
        }

        public override int Read(Span<byte> buffer)
        {
            if (internethandle == IntPtr.Zero) { throw new ObjectDisposedException(nameof(Stream)); }
            if (Interop.WinInet.InternetReadFile(internethandle , buffer , out var br) == Interop.BOOL.FALSE)
            {
                Interop.WinInet.ThrowAppropriateException();
            }
            return br;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(InternalResources.MP_NETWORKING_WININETSTREAM_OPNOTSUPP);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(InternalResources.MP_NETWORKING_WININETSTREAM_OPNOTSUPP);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException(InternalResources.MP_NETWORKING_WININETSTREAM_OPNOTSUPP);
        }

        protected override void Dispose(bool disposing)
        {
            internethandle = IntPtr.Zero;
            base.Dispose(disposing);
        }
    }
}