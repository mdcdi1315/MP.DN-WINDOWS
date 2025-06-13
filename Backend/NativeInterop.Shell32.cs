using System;
using System.Runtime.InteropServices;

partial class Interop
{
    public static unsafe class Shell32
    {
        public const ARCONTENT ARCONTENT_PHASE_MASK = (ARCONTENT)0x70000000;
        public const ARCONTENT ARCONTENT_MASK = (ARCONTENT)0x0001FFFE;

        [Flags]
        public enum ARCONTENT : System.UInt32
        {
            NONE = 0,
            PHASE_UNKNOWN = 0,
            AUTORUNINF = 2,
            AUDIOCD = 4,
            DVDMOVIE = 8,
            BLANKCD = 0x00000010,
            BLANKDVD = 0x00000020,
            UNKNOWNCONTENT = 0x00000040,
            AUTOPLAYPIX = 0x00000080,
            AUTOPLAYMUSIC = 0x00000100,
            AUTOPLAYVIDEO = 0x00000200,
            VCD = 0x00000400,
            SVCD = 0x00000800,
            DVDAUDIO = 0x00001000,
            BLANKBD = 0x00002000,
            BLURAY = 0x00004000,
            CAMERASTORAGE = 0x00008000,
            CUSTOMEVENT = 0x00010000,
            PHASE_PRESNIFF = 0x10000000,
            PHASE_SNIFFING = 0x20000000,
            PHASE_FINAL = 0x40000000,
        }

        [Flags]
        public enum KNOWN_FOLDER_FLAG : System.UInt32
        {
            KF_FLAG_DEFAULT = 0x00000000,

            // Requires Windows 10 RS3.
            // When called from packaged app, LocalAppData/RoamingAppData folders are redirected to
            // app private locations that match the paths returned from WinRT API:
            //   Windows.Storage.ApplicationData.Current.{LocalFolder|RoamingFolder}
            // A few other folders are redirected to subdirectories of LocalAppData.
            KF_FLAG_FORCE_APP_DATA_REDIRECTION = 0x00080000,

            // <-- Require Windows 10 RS2 
            // When running in a Centennial process, some file system locations are redirected to 
            // package-specific locations by the file system. Specifying this flag will cause the
            // target of the redirection to be returned for these locations. This is useful in
            // cases where the real location in the file system needs to be known.
            KF_FLAG_RETURN_FILTER_REDIRECTION_TARGET = 0x00040000,

            // New form for KF_FLAG_FORCE_APPCONTAINER_REDIRECTION and KF_FLAG_NO_APPCONTAINER_REDIRECTION
            // The new forms should be used and will exhibit the same behavior as the old flags.

            // When running inside an AppContainer process, or when providing an AppContainer token,
            // some folders are redirected to AppContainer-specific locations within the package's location.
            // Specifying this flag will force this redirection for folders that are not normally 
            // redirected for centennial processes. Useful for sharing files between between UWA and 
            // Centennial apps that are within the same package.
            KF_FLAG_FORCE_PACKAGE_REDIRECTION = 0x00020000,

            // When running inside a packaged process (Centennial,  AppContainer, etc.), or when providing 
            // a packaged process token, some folders are redirected to package-specific locations.
            // Specifying this flag will disable redirection on locations where it applied,
            // and instead return the non-redirected location.
            KF_FLAG_NO_PACKAGE_REDIRECTION = 0x00010000,

            // see comment for KF_FLAG_FORCE_PACKAGE_REDIRECTION
            KF_FLAG_FORCE_APPCONTAINER_REDIRECTION = 0x00020000,
            // Require Windows 10 RS2 -->

            // Requires Windows 7.
            // When running inside an AppContainer, or when poviding an AppContainer token, specifying this flag will prevent redirection to AppContainer
            // folders and instead return the path that would be returned when not running inside an AppContainer
            KF_FLAG_NO_APPCONTAINER_REDIRECTION = 0x00010000,

            // Make sure that the folder already exists or create it and apply security specified in folder definition
            // If folder can not be created then function will return failure and no folder path (IDList) will be returned
            // If folder is located on the network the function may take long time to execute
            KF_FLAG_CREATE = 0x00008000,

            // If this flag is specified then the folder path is returned and no verification is performed
            // Use this flag is you want to get folder's path (IDList) and do not need to verify folder's existence
            //
            // If this flag is NOT specified then Known Folder API will try to verify that the folder exists
            //     If folder does not exist or can not be accessed then function will return failure and no folder path (IDList) will be returned
            //     If folder is located on the network the function may take long time to execute
            KF_FLAG_DONT_VERIFY = 0x00004000,

            // Set folder path as is and do not try to substitute parts of the path with environments variables.
            // If flag is not specified then Known Folder will try to replace parts of the path with some
            // known environment variables (%USERPROFILE%, %APPDATA% etc.)
            KF_FLAG_DONT_UNEXPAND = 0x00002000,

            // Get file system based IDList if available. If the flag is not specified the Known Folder API
            // will try to return aliased IDList by default. Example for FOLDERID_Documents -
            // Aliased - [desktop]\[user]\[Documents] - exact location is determined by shell namespace layout and might change
            // Non aliased - [desktop]\[computer]\[disk_c]\[users]\[user]\[Documents] - location is determined by folder location in the file system
            KF_FLAG_NO_ALIAS = 0x00001000,

            // Initialize the folder with desktop.ini settings
            // If folder can not be initialized then function will return failure and no folder path will be returned
            // If folder is located on the network the function may take long time to execute
            KF_FLAG_INIT = 0x00000800,

            // Get the default path, will also verify folder existence unless KF_FLAG_DONT_VERIFY is also specified
            KF_FLAG_DEFAULT_PATH = 0x00000400,

            // Get the not-parent-relative default path. Only valid with KF_FLAG_DEFAULT_PATH
            KF_FLAG_NOT_PARENT_RELATIVE = 0x00000200,

            // Build simple IDList
            KF_FLAG_SIMPLE_IDLIST = 0x00000100,

            // only return the aliased IDLists, don't fallback to file system path
            KF_FLAG_ALIAS_ONLY = 0x80000000,
        }

        public static class ExecuteVerbs
        {
            public const System.String RunAs = "runas";

            public const System.String Print = "print";

            public const System.String Explore = "explore";

            public const System.String Find = "find";

            public const System.String Edit = "edit";

            public const System.String Open = "open";
        }

        [DllImport(Libraries.Shell32, EntryPoint = "ShellExecuteW")]
        private static extern System.Int16 ShellExecute_Native(System.IntPtr winHandle,
            System.Char* Verb,
            System.Char* Path,
            System.Char* Parameters,
            System.Char* WorkDir,
            System.Int32 WinShowArgs);

        [DllImport(Libraries.Shell32 , EntryPoint = "SHGetDriveMedia" , PreserveSig = true)]
        private static extern MP.ComInterop.HRESULT SHGetDriveMedia_Native(System.Char* driveptr , ARCONTENT* flags);

        // Docs say that ptrfolderpath must be freed with Ole32.CoTaskMemFree
        // once it is used. The native string will be copied to managed memory first then free the native string.
        [DllImport(Libraries.Shell32 , EntryPoint = "SHGetKnownFolderPath" , PreserveSig = true)]
        private static extern MP.ComInterop.HRESULT SHGetKnownFolderPath_Native(
            GUID* knownfolder, KNOWN_FOLDER_FLAG flags , System.IntPtr htokennotused , 
            System.Char** ptrfolderpath);

        [DllImport(Libraries.Shell32 , EntryPoint = "SHCreateItemFromParsingName" , PreserveSig = true , ExactSpelling = true)]
        private static extern MP.ComInterop.HRESULT SHCreateItemFromParsingName_Native(
            System.Char* path, void* unusedsettonull, GUID* refiid, void** outputinterface);

        public static MP.ComInterop.HRESULT SHCreateItemFromParsingName(System.String path, GUID interfacerefid, void** outinterface)
        {
            fixed (System.Char* pptr = path)
            {
                return SHCreateItemFromParsingName_Native(pptr , null , &interfacerefid , outinterface);
            }
        }

        public static MP.ComInterop.HRESULT SHGetKnownFolderPath(GUID knownfolder , out System.String pathcopied)
        {
            KNOWN_FOLDER_FLAG flags = KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT;
            pathcopied = System.String.Empty;
            var osinf = NtDll.RtlGetVersion().Version;
            if (osinf.Major >= 6)
            {
                flags |= KNOWN_FOLDER_FLAG.KF_FLAG_NO_APPCONTAINER_REDIRECTION;
            }
            if (osinf.Major >= 10 && osinf.Minor >= 19041)
            {
                flags |= KNOWN_FOLDER_FLAG.KF_FLAG_NO_PACKAGE_REDIRECTION;
            }
            System.Char* buffer;
            MP.ComInterop.HRESULT hrt = SHGetKnownFolderPath_Native(&knownfolder, flags, System.IntPtr.Zero, &buffer);
            if (hrt.SUCCEEDED) {
                pathcopied = new(buffer);
                Ole32.CoTaskMemFree(buffer);
            }
            return hrt;
        }

        public static MP.ComInterop.HRESULT SHGetDriveMedia(System.String drive , out ARCONTENT flags)
        {
            ARCONTENT arc;
            MP.ComInterop.HRESULT hr;
            fixed (System.Char* drvptr = drive)
            {
                hr = SHGetDriveMedia_Native(drvptr, &arc);
                if (hr.FAILED) { flags = ARCONTENT.NONE; } else { flags = arc; }
            }
            return hr;
        }

        public static System.Int16 ShellExecute(
            System.IntPtr winHandle, System.String Verb,
            System.String Path, System.String Parameters,
            System.String WorkDir, System.Int32 WinShowArgs)
        {
            fixed (System.Char* VerbPtr = Verb) 
            fixed (System.Char* PathPtr = Path)
            fixed (System.Char* ParametersPtr = Parameters)
            fixed (System.Char* WorkDirPtr = WorkDir) { 
                return ShellExecute_Native(winHandle , VerbPtr, PathPtr, ParametersPtr , WorkDirPtr , WinShowArgs);
            }
        }
    }
}