
using MP;
using System;
using MP.ProcessLaunching;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

partial class Interop
{
    static unsafe partial class Kernel32
    {
        [Flags]
        public enum StartupInfoFlags : System.UInt32
        {
            None = 0,
            /// <summary>
            /// Indicates that the cursor is in feedback mode for two seconds after CreateProcess is called. The Working in Background cursor is displayed (see the Pointers tab in the Mouse control panel utility).
            /// If during those two seconds the process makes the first GUI call, the system gives five more seconds to the process. If during those five seconds the process shows a window, the system gives five more seconds to the process to finish drawing the window.
            /// The system turns the feedback cursor off after the first call to GetMessage, regardless of whether the process is drawing.
            /// </summary>
            STARTF_FORCEONFEEDBACK = 0x00000040,
            /// <summary>
            /// Indicates that the feedback cursor is forced off while the process is starting. The Normal Select cursor is displayed.
            /// </summary>
            STARTF_FORCEOFFFEEDBACK = 0x00000080,
            /// <summary>
            /// 	The lpTitle member contains the path of the shortcut file (.lnk) that the user invoked to start this process. This is typically set by the shell when a .lnk file pointing to the launched application is invoked. Most applications will not need to set this value.
            /// This flag cannot be used with STARTF_TITLEISAPPID.
            /// </summary>
            STARTF_TITLEISLINKNAME = 0x00000800,
            /// <summary>
            /// The command line came from an untrusted source. For more information, see Remarks.
            /// </summary>
            STARTF_UNTRUSTEDSOURCE = 0x00008000,
            /// <summary>
            /// The <see cref="STARTUPINFO.ConsoleXCountChars"/> and <see cref="STARTUPINFO.ConsoleYCountChars"/> members contain additional information.
            /// </summary>
            STARTF_USECOUNTCHARS = 0x00000008,
            /// <summary>
            /// The <see cref="STARTUPINFO.FillAttribute"/> member contains additional information.
            /// </summary>
            STARTF_USEFILLATTRIBUTE = 0x00000010,
            /// <summary>
            /// The hStdInput member contains additional information.
            /// This flag cannot be used with STARTF_USESTDHANDLES.
            /// </summary>
            STARTF_USEHOTKEY = 0x00000200,
            /// <summary>
            /// The <see cref="STARTUPINFO.X"/> and <see cref="STARTUPINFO.Y"/> members contain additional information.
            /// </summary>
            STARTF_USEPOSITION = 0x00000004,
            /// <summary>
            /// The <see cref="STARTUPINFO.ShowWindow"/> member contains additional information.
            /// </summary>
            STARTF_USESHOWWINDOW = 0x00000001,
            /// <summary>
            /// The <see cref="STARTUPINFO.XSize"/> and <see cref="STARTUPINFO.YSize"/> members contain additional information.
            /// </summary>
            STARTF_USESIZE = 0x00000002,
            /// <summary>
            /// The <see cref="STARTUPINFO.ConsoleInputHandle"/>, <see cref="STARTUPINFO.ConsoleOutputHandle"/>, and <see cref="STARTUPINFO.ConsoleErrorHandle"/> members contain additional information. <br /> <br />
            /// If this flag is specified when calling one of the process creation functions, the handles must be inheritable and the function's bInheritHandles parameter must be set to <see cref="BOOL.TRUE"/>. For more information, see <see href="https://learn.microsoft.com/en-us/windows/desktop/SysInfo/handle-inheritance">Handle Inheritance</see>. <br /> <br />
            /// If this flag is specified when calling the GetStartupInfo function, these members are either the handle value specified during process creation or -1. <br /> <br />
            /// Handles must be closed with <see cref="CloseHandle"/> when they are no longer needed. <br /> <br />
            /// This flag cannot be used with <see cref="STARTF_USEHOTKEY"/>.
            /// </summary>
            STARTF_USESTDHANDLES = 0x00000100
        }

        [Flags]
        public enum CreateProcessFlags : System.UInt32
        {
            None = 0,
            /// <summary>
            /// The child processes of a process associated with a job are not associated with the job. <br />
            /// If the calling process is not associated with a job, this constant has no effect. 
            /// If the calling process is associated with a job, the job must set the JOB_OBJECT_LIMIT_BREAKAWAY_OK limit.
            /// </summary>
            CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
            /// <summary>
            /// The new process does not inherit the error mode of the calling process.Instead, the new process gets the default error mode. <br />
            /// This feature is particularly useful for multithreaded shell applications that run with hard errors disabled. <br />
            /// The default behavior is for the new process to inherit the error mode of the caller.Setting this flag changes that default behavior.
            /// </summary>
            CREATE_DEFAULT_ERROR_MODE = 0x04000000,
            /// <summary>
            /// The new process has a new console, instead of inheriting its parent's console (the default). 
            /// For more information, see <see href="https://learn.microsoft.com/en-us/windows/console/creation-of-a-console">Creation of a Console</see>. <br />
            /// This flag cannot be used with <see cref="DETACHED_PROCESS"/>.
            /// </summary>
            CREATE_NEW_CONSOLE = 0x00000010,
            /// <summary>
            /// The new process is the root process of a new process group. 
            /// The process group includes all processes that are descendants of this root process. 
            /// The process identifier of the new process group is the same as the process identifier, which is returned in the lpProcessInformation parameter.
            /// Process groups are used by the GenerateConsoleCtrlEvent function to enable sending a CTRL+BREAK signal to a group of console processes.
            /// If this flag is specified, CTRL+C signals will be disabled for all processes within the new process group. <br />
            /// This flag is ignored if specified with <see cref="CREATE_NEW_CONSOLE"/>.
            /// </summary>
            CREATE_NEW_PROCESS_GROUP = 0x00000200,
            /// <summary>
            /// The process is a console application that is being run without a console window. 
            /// Therefore, the console handle for the application is not set.
            /// This flag is ignored if the application is not a console application, or if it is used with either <see cref="CREATE_NEW_CONSOLE"/> or <see cref="DETACHED_PROCESS"/>.
            /// </summary>
            CREATE_NO_WINDOW = 0x08000000,
            /// <summary>
            /// The process is to be run as a protected process.
            /// The system restricts access to protected processes and the threads of protected processes.
            /// For more information on how processes can interact with protected processes, see <see href="https://learn.microsoft.com/en-us/windows/win32/procthread/process-security-and-access-rights">Process Security and Access Rights</see>.
            /// To activate a protected process, the binary must have a special signature.
            /// This signature is provided by Microsoft but not currently available for non-Microsoft binaries.
            /// There are currently four protected processes: media foundation, audio engine, Windows error reporting, and system.
            /// Components that load into these binaries must also be signed.
            /// Multimedia companies can leverage the first two protected processes.
            /// For more information, see <see href="https://learn.microsoft.com/en-us/windows/win32/medfound/protected-media-path">Overview of the Protected Media Path</see>. <br />
            /// Windows Server 2003 and Windows XP: This value is not supported.
            /// </summary>
            CREATE_PROTECTED_PROCESS = 0x00040000,
            /// <summary>
            /// Allows the caller to execute a child process that bypasses the process restrictions that would normally be applied automatically to the process.
            /// </summary>
            CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
            /// <summary>
            /// This flag allows secure processes, that run in the Virtualization-Based Security environment, to launch.
            /// </summary>
            CREATE_SECURE_PROCESS = 0x00400000,
            /// <summary>
            /// The primary thread of the new process is created in a suspended state, and does not run until the ResumeThread function is called.
            /// </summary>
            CREATE_SUSPENDED = 0x00000004,
            /// <summary>
            /// If this flag is set, the environment block pointed to by lpEnvironment uses Unicode characters. 
            /// Otherwise, the environment block uses ANSI characters.
            /// </summary>
            CREATE_UNICODE_ENVIRONMENT = 0x00000400,
            /// <summary>
            /// The calling thread starts and debugs the new process.
            /// It can receive all related debug events using the WaitForDebugEvent function.
            /// </summary>
            DEBUG_ONLY_THIS_PROCESS = 0x00000002,
            /// <summary>
            /// The calling thread starts and debugs the new process and all child processes created by the new process.
            /// It can receive all related debug events using the WaitForDebugEvent function.
            /// A process that uses <see cref="DEBUG_PROCESS"/> becomes the root of a debugging chain. 
            /// This continues until another process in the chain is created with <see cref="DEBUG_PROCESS"/>. <br />
            /// If this flag is combined with <see cref="DEBUG_ONLY_THIS_PROCESS"/>, the caller debugs only the new process, not any child processes.
            /// </summary>
            DEBUG_PROCESS = 0x00000001,
            /// <summary>
            /// For console processes, the new process does not inherit its parent's console (the default). 
            /// The new process can call the <see cref="AllocConsole"/> function at a later time to create a console. 
            /// For more information, see <see href="https://learn.microsoft.com/en-us/windows/console/creation-of-a-console">Creation of a Console</see>. <br />
            /// This value cannot be used with <see cref="CREATE_NEW_CONSOLE"/>.
            /// </summary>
            DETACHED_PROCESS = 0x00000008,
            /// <summary>
            /// The process is created with extended startup information; the lpStartupInfo parameter specifies a STARTUPINFOEX structure. <br />
            /// Windows Server 2003 and Windows XP: This value is not supported.
            /// </summary>
            EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
            /// <summary>
            /// The process inherits its parent's affinity. 
            /// If the parent process has threads in more than one processor group, the new process inherits the group-relative affinity of an arbitrary group in use by the parent. <br />
            /// Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP: This value is not supported.
            /// </summary>
            INHERIT_PARENT_AFFINITY = 0x00010000
        }

        [StructLayout(LayoutKind.Explicit , Size = 24)]
        public struct PROCESS_INFORMATION
        {
            [FieldOffset(0)]
            public System.IntPtr HProcess;

            [FieldOffset(8)]
            public System.IntPtr HThread;

            [FieldOffset(16)]
            public System.UInt32 ProcessID;

            [FieldOffset(20)]
            public System.UInt32 ThreadID;
        }

        // For a reason the same explicit layout does not work , so fall back to sequential semantics 
        // (although that this could also break at any time)
        [StructLayout(LayoutKind.Sequential)] 
        public struct STARTUPINFO
        {
            public System.UInt32 Size; // Must be set as sizeof(STARTUPINFO)

            public System.Char* RSVD0;

            public System.Char* LpDesktop;

            public System.Char* LpConsoleTitle;

            public System.UInt32 X;

            public System.UInt32 Y;

            public System.UInt32 XSize;

            public System.UInt32 YSize;

            public System.UInt32 ConsoleXCountChars;

            public System.UInt32 ConsoleYCountChars;

            public System.UInt32 ConsoleFillAttribute;

            public StartupInfoFlags Flags;

            public System.UInt16 ShowWindow;

            public System.UInt16 RSVD1;

            public System.Byte* RSVD2;

            public System.IntPtr ConsoleInputHandle;

            public System.IntPtr ConsoleOutputHandle;

            public System.IntPtr ConsoleErrorHandle;

            public STARTUPINFO() { Size = sizeof(STARTUPINFO).ToUInt32(); RSVD0 = null; RSVD1 = 0; RSVD2 = null; }
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "CreateProcessW" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL CreateProcess_Native(
            System.Char* pappname , 
            System.Char* pcommandline , 
            SECURITY_ATTRIBUTES* ppprocattrs , 
            SECURITY_ATTRIBUTES* ppthreadattrs ,
            BOOL inherithandles,
            CreateProcessFlags flags,
            System.Char* penvblock,
            System.Char* pcurrentdir,
            STARTUPINFO* pstartupinfo,
            PROCESS_INFORMATION* pprocinfo);

        public static PROCESS_INFORMATION CreateProcess(
            System.String appname,
            System.String cmdline,
            BOOL inherithandles,
            CreateProcessFlags flags,
            VariablesDictionary dict,
            System.String currentdir,
            STARTUPINFO additional)
        {
            System.Text.StringBuilder sbdict = new();
            foreach (var v in dict)
            {
                sbdict.Append($"{v.Name}={v.Value}\0");
            }
            sbdict.Append('\0');
            SafeLibcMemoryHandle envdictmem = sbdict.ToString().ToNativeUnicodeString();
            sbdict.Clear();
            sbdict = null;
            PROCESS_INFORMATION inftemp;
            BOOL ret;
            System.Int32 erc = 0;
            flags |= CreateProcessFlags.CREATE_UNICODE_ENVIRONMENT;
            cmdline += "\0";
            SafeLibcMemoryHandle cmdlmem = cmdline.ToNativeUnicodeString();
            if (appname is not null) { appname += "\0"; }
            if (currentdir is not null) { currentdir += "\0"; }
            fixed (System.Char* pappn = appname)
            fixed (System.Char* pcurrentdir = currentdir)
            {
                ret = CreateProcess_Native(pappn, (System.Char*)cmdlmem.MemoryPointer , null, null , inherithandles , flags , (System.Char*)envdictmem.MemoryPointer, pcurrentdir , &additional , &inftemp);
                if (ret == BOOL.FALSE) { erc = GetLastError(); }
            }
            cmdlmem.Dispose();
            cmdlmem = null;
            envdictmem.Dispose();
            envdictmem = null;
            if (ret == BOOL.FALSE) { throw new MP.ExceptionSystem.NativeWindowsException(erc); }
            return inftemp;
        }

        // A code of 259 indicates that the process is still active.
        [DllImport(Libraries.Kernel32 , EntryPoint = "GetExitCodeProcess" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL GetExitCodeProcess_Native(System.IntPtr procid, System.UInt32* exitcode);

        public static BOOL GetExitCodeProcess(System.IntPtr procid , out System.UInt32 exitcode)
        {
            System.UInt32 ec;
            BOOL ret = GetExitCodeProcess_Native(procid, &ec);
            exitcode = ec;
            return ret;
        }
    }
}