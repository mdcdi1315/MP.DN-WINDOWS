
// Windows Console Stream class.
// Helps you out to use the returned data as you wish.
// From this stream you can always read and write data but you cannot 
// get it's byte position, setting that and getting it's length.

// The stream provides two primary API's:
// The default API inherited by the Stream class
// and the Console API which delegates to the native side to get the data as a string.

// Additionally, the console stream may be created by a file, and any operations will be done based on that file.

using MP;
using System;
using System.IO;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.IO
{
    /// <summary>
    /// Defines a special stream that can handle reading from , and writing to, to a console. <br />
    /// As an additional point , you can use this stream to work with files instead of the operating system-provided handles.
    /// </summary>
    // The good with this stream is that we can directly read and write using the classic ReadFile and WriteFile functions respectively.
    public sealed class ConsoleStream : Stream
    {
        private System.Boolean isconsole; // Indicates that these are handles returned by GetStdHandle
        private Encoding encodingin , encodingout; // Encoding used for stringy operations.
        // We can use MPSafeFileHandle on these even when these are truly created by a console.
        private RedistSafeFileHandle handleout, handlein;

        /// <summary>
        /// Creates a new console stream, retrieveing information directly from the console.
        /// </summary>
        public ConsoleStream()
        {
            System.IntPtr handleout = Interop.Kernel32.GetStdHandle(Interop.Console.ConsoleHandleOptions.Output);
            if (handleout == System.IntPtr.Zero) { throw new MP.ExceptionSystem.NativeWindowsException(); }
            System.IntPtr handlein = Interop.Kernel32.GetStdHandle(Interop.Console.ConsoleHandleOptions.Input);
            if (handlein == System.IntPtr.Zero) { throw new MP.ExceptionSystem.NativeWindowsException(); }
            // Note that these handles are owned by the system.
            // Even attempting to CloseHandle them would cause a kernel failure
            this.handleout = new(handleout, false);
            this.handlein = new(handlein, false);
            isconsole = true;
            System.UInt32 cp = Interop.Kernel32.GetConsoleCP();
            if (cp == 437) { // ANSI encoding , but we can use ASCII encoding for the heavy lifting.
                encodingin = Encoding.ASCII;
            } else {
                encodingin = Encoding.GetEncoding(cp.ToInt32());
            }
            cp = Interop.Kernel32.GetConsoleOutputCP();
            if (cp == 437) { // ANSI encoding , but we can use ASCII encoding for the heavy lifting.
                encodingout = Encoding.ASCII;
            } else {
                encodingout = Encoding.GetEncoding(cp.ToInt32());
            }
        }

        public ConsoleStream(RedistSafeFileHandle outhandle , RedistSafeFileHandle inhandle = null)
        {
            if (outhandle is null) { throw new ArgumentNullException(nameof(outhandle)); }
            if (outhandle.IsInvalid) { throw new ArgumentException("Cannot use a closed handle." , nameof(outhandle)); }
            if (outhandle.GetFileAccess() == FileAccess.Read)
            {
                throw new ArgumentException("The console's output handle must be writeable!");
            }
            handleout = outhandle;
            if (inhandle is not null)
            {
                if (inhandle.IsInvalid) { throw new ArgumentException("Cannot use a closed handle.", nameof(inhandle)); }
                if (inhandle.GetFileAccess() == FileAccess.Write)
                {
                    throw new ArgumentException("The console's output handle must be readable!");
                }
                handlein = inhandle;
            } else {
                handlein = null;
            }
            CreateDefaultEncoding();
        }

        // Helper used for file handle ctors of ConsoleStream
        private void CreateDefaultEncoding() => encodingin = encodingout = new UnicodeEncoding(false, false);

        /// <summary>
        /// Gets or sets the console encoding that is used when reading data from the console. <br />
        /// Note that when setting a different encoding, support by the OS might be unavailable. <br />
        /// However if the codepage is not supported, the property performs failsafe and the stream does still use the old encoding.
        /// </summary>
        public Encoding InputEncoding
        {
            get => encodingin;
            set {
                VerifyNotDisposed();
                if (value is null) { throw new System.ArgumentNullException(nameof(value)); }
                if (handlein is null) { throw new System.InvalidOperationException("The input handle was not set during construction time."); }
                if (isconsole)
                {
                    // When we are using files we can just play around with the encodings.
                    System.UInt32 cp = value.CodePage.ToUInt32();
                    if (Interop.Kernel32.IsValidCodePage(cp) == Interop.BOOL.FALSE)
                    {
                        throw new System.ArgumentException($"The code page with code {cp} is not supported by the system.");
                    }
                    if (Interop.Kernel32.SetConsoleCP(cp) == Interop.BOOL.FALSE)
                    {
                        throw new MP.ExceptionSystem.NativeWindowsException();
                    }
                }
                encodingin = value;
            }
        }

        /// <summary>
        /// Gets or sets the console encoding that is used when writing data to the console. <br />
        /// Note that when setting a different encoding, support by the OS might be unavailable. <br />
        /// However if the codepage is not supported, the property performs failsafe and the stream does still use the old encoding.
        /// </summary>
        public Encoding OutputEncoding
        {
            get => encodingout;
            set {
                VerifyNotDisposed();
                if (value is null) { throw new System.ArgumentNullException(nameof(value)); }
                if (isconsole)
                {
                    // When we are using files we can just play around with the encodings.
                    System.UInt32 cp = value.CodePage.ToUInt32();
                    if (Interop.Kernel32.IsValidCodePage(cp) == Interop.BOOL.FALSE)
                    {
                        throw new System.ArgumentException($"The code page with code {cp} is not supported by the system.");
                    }
                    if (Interop.Kernel32.SetConsoleOutputCP(cp) == Interop.BOOL.FALSE)
                    {
                        throw new MP.ExceptionSystem.NativeWindowsException();
                    }
                }
                encodingout = value;
            }
        }

        /// <summary>
        /// Returns the handle that represents the console's output.
        /// </summary>
        public RedistSafeFileHandle SafeOutputFileHandle
        {
            get {
                VerifyNotDisposed();
                return handleout;
            }
        }

        /// <summary>
        /// Returns the handle that represents the console's input.
        /// </summary>
        public RedistSafeFileHandle SafeInputFileHandle
        {
            get {
                VerifyNotDisposed();
                return handlein;
            }
        }

        // Such stream is unseekable.
        public override System.Boolean CanSeek => false;

        // Always the output console handle must be provided.
        // However, on disposed streams signal that the consumer cannot read from this stream anymore.
        public override System.Boolean CanRead => handleout is not null;

        // Ability to write must have been provided during object creation
        public override System.Boolean CanWrite => handlein is not null && handlein.IsInvalid == false;

        public override System.Boolean CanTimeout => false;

        /// <summary>
        /// Defines whether the console stream is connected to the spawned console. <br />
        /// If <see langword="false"/> , it means that the stream was opened using files.
        /// </summary>
        public System.Boolean IsTrueConsole => isconsole;

        // Setting length is not supported but leave it empty to do anything else but nothing.
        public override void SetLength(System.Int64 value) {}

        // Flushing is always done automatically, so no need to perform anything else but nothing.
        public override void Flush() {}

        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            // We otherwise create the span , so call the Read with the Span overload,
            // and allow Span users to benefit.
            return Read(new System.Span<System.Byte>(buffer, offset, count));
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // We still need to validate the buffer on either case
            ValidateBufferArguments(buffer, offset, count);
            Write(new System.Span<System.Byte>(buffer, offset, count));
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            VerifyNotDisposed();
            System.Int32 wb = FileOperations.WriteFileNative(handleout, buffer, out System.Int32 erc);
            if (wb == -1) { throw new MP.ExceptionSystem.NativeWindowsException(erc); }
        }

        public override int Read(Span<byte> buffer)
        {
            VerifyNotDisposed();
            // Even if read requests happen on the stream , this will discard it effectively.
            // Additionally it will return 0 , indicating that no data could be read from the stream.
            // Note also that here we must always read from the end of stream.
            System.Int32 rb;
            if (handlein is not null) {
                rb = FileOperations.ReadFileNative(handlein, buffer, out System.Int32 erc);
                if (rb == -1)
                {
                    throw new MP.ExceptionSystem.NativeWindowsException(erc);
                }
            } else {
                rb = 0;
            }
            return rb;
        }

        public System.Char NextCharacterFromInput()
        {
            VerifyNotDisposed();
            if (handlein is null) { throw new InvalidOperationException("Input handle was not specified during the creation time."); }
            System.Boolean hasrecords;
            Interop.Kernel32.INPUT_RECORD record = default;
            while (record.Type != Interop.Kernel32.InputRecordType.KEY_EVENT)
            {
                if (Interop.Kernel32.ReadConsoleInputOneOnly(handlein.Handle, out hasrecords , out record) == Interop.BOOL.FALSE)
                {
                    throw new MP.ExceptionSystem.NativeWindowsException();
                }
                if (hasrecords == false) 
                {
                    if (Interop.Kernel32.WaitForSingleObject(handlein.Handle , 0) == Interop.Kernel32.WaitObjectReturnValue.WAIT_FAILED)
                    {
                        throw new MP.ExceptionSystem.NativeWindowsException();
                    }
                }
            }
            return record.KeyEvent.UnicodeChar;
        }

        public System.String ReadLine()
        {
            VerifyNotDisposed();
            System.Byte[] buf = new System.Byte[4096];
            System.Int32 rb = Read(buf);
            if (rb == 0) {
                return System.String.Empty;
            }
            return encodingin.GetString(buf , 0 , rb);
        }

        public void WriteText(System.String text)
        {
            VerifyNotDisposed();
            // Must check if it is null or empty
            if (System.String.IsNullOrEmpty(text)) { return; }
            // Now we can freely write our line...
            System.Byte[] b = encodingout.GetBytes(text);
            Write(b, 0, b.Length);
        }

        public void WriteLine(System.String line)
        {
            WriteText(line);
            System.Int32 cp = encodingout.CodePage;
            if (cp >= 1200 && cp <= 1201)
            {
                Write([13, 0, 10, 0], 0, 4); // CRLF line terminator, UTF-16 encoding
            } else {
                Write([13, 10], 0, 2); // CRLF line terminator
            }
        }

        public override System.Int64 Length => throw new System.NotSupportedException("Cannot get the length in bytes of a console handle.");

        public override System.Int64 Position
        {
            get => throw new System.NotSupportedException("Cannot get the byte position in a console stream.");
            set => throw new System.NotSupportedException("Cannot set the byte position in a console stream.");
        }

        public override System.Int64 Seek(System.Int64 offset, SeekOrigin origin) => throw new System.NotSupportedException("Cannot seek in a console stream.");

        private void VerifyNotDisposed() => ObjectDisposedException.ThrowIf(handleout is null, this);

        protected override void Dispose(bool disposing)
        {
            handlein?.Dispose();
            handlein = null;
            handleout?.Dispose();
            handleout = null;
            if (disposing)
            {
                encodingin = null;
                encodingout = null;
            }
            base.Dispose(disposing);
        }
    }
}