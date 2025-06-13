

using System;
using System.Drawing;
using Microsoft.Win32.SafeHandles;


namespace MP.ProcessLaunching
{
    public unsafe sealed class ProcessLauncher : IDisposable
    {
        private System.UInt32 exitcode;
        private ProcessCreationFlags pcf;
        private VariablesDictionary variables;
        private SafeLibcMemoryHandle lptitlemem;
        private Interop.Kernel32.STARTUPINFO startupinf;
        private System.String apppath, currentdir, cmdline;
        private Interop.Kernel32.PROCESS_INFORMATION createdprocinf;

        public ProcessLauncher() {
            variables = new();
            lptitlemem = null;
            apppath = currentdir = cmdline = null;
            createdprocinf = default;
            startupinf = new();
            exitcode = System.UInt32.MaxValue;
        }

        public VariablesDictionary EnvironmentBlock => variables;

        public System.String ExecutablePath
        {
            get => apppath;
            set => apppath = value;
        }

        public System.String ProcessCurrentDirectory
        {
            get => currentdir;
            set => currentdir = value;
        }

        public System.String CommandLine
        {
            get => cmdline;
            set => cmdline = value;
        }

        public System.String ConsoleTitle
        {
            get {
                if (lptitlemem is null) { return null; }
                return new((System.Char*)lptitlemem.MemoryPointer, 0, ((lptitlemem.MemoryLength - 1) / sizeof(System.Char)));
            }
            set {
                lptitlemem?.Dispose();
                lptitlemem = value.ToNativeUnicodeString();
            }
        }

        public Point InitialWindowPoint
        {
            get => new(startupinf.X.ToInt32() , startupinf.Y.ToInt32());
            set {
                startupinf.X = value.X.ToUInt32();
                startupinf.Y = value.Y.ToUInt32();
            }
        }

        public Size InitialWindowSize
        {
            get => new(startupinf.XSize.ToInt32(), startupinf.YSize.ToInt32());
            set {
                startupinf.XSize = value.Width.ToUInt32();
                startupinf.YSize = value.Height.ToUInt32();
            }
        }

        public Size ConsoleBufferSize
        {
            get => new(startupinf.ConsoleXCountChars.ToInt32() , startupinf .ConsoleYCountChars.ToInt32());
            set {
                startupinf.ConsoleXCountChars = value.Width.ToUInt32();
                startupinf.ConsoleYCountChars = value.Height.ToUInt32();
            }
        }

        public System.Boolean IsUntrustedLaunch
        {
            get => startupinf.Flags.HasFlag(Interop.Kernel32.StartupInfoFlags.STARTF_UNTRUSTEDSOURCE);
            set {
                if (value) {
                    startupinf.Flags |= Interop.Kernel32.StartupInfoFlags.STARTF_UNTRUSTEDSOURCE;
                } else if (startupinf.Flags.HasFlag(Interop.Kernel32.StartupInfoFlags.STARTF_UNTRUSTEDSOURCE)) {
                    startupinf.Flags ^= Interop.Kernel32.StartupInfoFlags.STARTF_UNTRUSTEDSOURCE;
                }
            }
        }

        public ProcessCreationFlags CreationFlags
        {
            get => pcf;
            set => pcf = value;
        }

        // This may be IntPtr.Zero indicating that a process has not been launched or that we are on a pseudo-disposed state.
        public System.IntPtr ProcessHandle => createdprocinf.HProcess;

        public System.IntPtr MainThreadHandle => createdprocinf.HThread;

        public System.UInt32 ProcessID => createdprocinf.ProcessID;

        public System.UInt32 ThreadID => createdprocinf.ThreadID;

        public System.UInt32 ExitCode
        {
            get {
                if (createdprocinf.HProcess != System.IntPtr.Zero) 
                {
                    var ec = exitcode;
                    if (Interop.Kernel32.GetExitCodeProcess(createdprocinf.HProcess, out exitcode) == Interop.BOOL.FALSE)
                    {
                        // We do not care if the native call has an error , but we do care if failed so that 
                        // we can apply the pre-existing exit code back (On error the exit code will be flushed with zero.)
                        exitcode = ec;
                    }
                }
                return exitcode;
            }
        }

        private void VerifyCorrectCmdLine()
        {
            if (System.String.IsNullOrEmpty(cmdline))
            {
                if (System.String.IsNullOrEmpty(apppath) == false) { return; }
                throw new InvalidOperationException("A command line must have been specified at least!");
            }
        }

        public void Launch()
        {
            VerifyCorrectCmdLine();
            // With this class design you may spawn a lot of processes using essentially the same object 
            // but this will not work when the two essential components have been freed out
            if (lptitlemem is null && variables is null) 
            {
                throw new ObjectDisposedException(nameof(ProcessLauncher));
            }
            DisposeHandles();
            if (lptitlemem is not null) {
                startupinf.LpConsoleTitle = (System.Char*)lptitlemem.MemoryPointer;
            }
            if (startupinf.X > 0 || startupinf.Y > 0) {
                startupinf.Flags |= Interop.Kernel32.StartupInfoFlags.STARTF_USEPOSITION;
            }
            if (startupinf.XSize > 0 || startupinf.YSize > 0) {
                startupinf.Flags |= Interop.Kernel32.StartupInfoFlags.STARTF_USESIZE;
            }
            if (startupinf.ConsoleXCountChars > 0 || startupinf.ConsoleYCountChars > 0) {
                startupinf.Flags |= Interop.Kernel32.StartupInfoFlags.STARTF_USECOUNTCHARS;
            }
            Interop.Kernel32.CreateProcessFlags cpf = Interop.Kernel32.CreateProcessFlags.None;
            if (pcf.HasFlag(ProcessCreationFlags.CreateWithNewConsole))
            {
                cpf |= Interop.Kernel32.CreateProcessFlags.CREATE_NEW_CONSOLE;
            } 
            if (pcf.HasFlag(ProcessCreationFlags.DoNotUseConsole))
            {
                cpf |= Interop.Kernel32.CreateProcessFlags.CREATE_NO_WINDOW;
            }
            if (pcf.HasFlag(ProcessCreationFlags.CreateNewProcessGroup))
            {
                cpf |= Interop.Kernel32.CreateProcessFlags.CREATE_NEW_PROCESS_GROUP;
            }
            if (pcf.HasFlag(ProcessCreationFlags.UseSecureProcessSemantics))
            {
                cpf |= Interop.Kernel32.CreateProcessFlags.CREATE_SECURE_PROCESS;
            }
            if (pcf.HasFlag(ProcessCreationFlags.WithDefaultErrorMode))
            {
                cpf |= Interop.Kernel32.CreateProcessFlags.CREATE_DEFAULT_ERROR_MODE;
            }
            startupinf.LpDesktop = null;

            createdprocinf = Interop.Kernel32.CreateProcess(apppath, $" {cmdline}", Interop.BOOL.FALSE, cpf, variables, currentdir, startupinf);
            if (createdprocinf.HProcess != System.IntPtr.Zero) {
                Interop.Kernel32.GetExitCodeProcess(createdprocinf.HProcess, out exitcode);
            }
        }

        public void WaitUntilTerminated()
        {
            if (createdprocinf.HProcess == System.IntPtr.Zero) { return; }
            while (exitcode == 259)
            {
                Interop.BOOL failure = Interop.Kernel32.GetExitCodeProcess(createdprocinf.HProcess, out exitcode);
                if (failure == Interop.BOOL.FALSE) { throw new MP.ExceptionSystem.NativeWindowsException(); }
                System.Threading.Thread.Sleep(10);
            }
        }

        private void DisposeHandles()
        {
            if (createdprocinf.HProcess != System.IntPtr.Zero)
            {
                Interop.Kernel32.CloseHandle(createdprocinf.HProcess);
                createdprocinf.HProcess = System.IntPtr.Zero;
            }
            if (createdprocinf.HThread != System.IntPtr.Zero)
            {
                Interop.Kernel32.CloseHandle(createdprocinf.HThread);
                createdprocinf.HThread = System.IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            DisposeHandles();
            lptitlemem?.Dispose();
            lptitlemem = null;
            variables?.Clear();
            variables = null;
        }
    }
}