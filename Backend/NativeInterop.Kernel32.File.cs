using MP;
using System;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

unsafe partial class Interop
{
    public static partial class Kernel32
    {
        public const System.Int32 REPLACEFILE_IGNORE_MERGE_ERRORS = 0x2;
        public const System.UInt32 MOVEFILE_REPLACE_EXISTING = 0x01;
        public const System.UInt32 MOVEFILE_COPY_ALLOWED = 0x02;
        public const System.Int32 MAX_PATH = 260;

        // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364228.aspx
        public enum FILE_INFO_BY_HANDLE_CLASS : System.UInt32
        {
            // Up to FileRemoteProtocolInfo available in Windows 7

            /// <summary>
            /// Returns basic information (timestamps and attributes).
            /// </summary>
            /// <remarks>
            /// Thunks to NtQueryInformationFile and FileBasicInformation.
            /// </remarks>
            FileBasicInfo,

            /// <summary>
            /// Returns file size, link count, pending delete status, and if it is a directory.
            /// </summary>
            /// <remarks>
            /// Thunks to NtQueryInformationFile and FileStandardInformation.
            /// </remarks>
            FileStandardInfo,

            /// <summary>
            /// Gets the file name.
            /// </summary>
            /// <remarks>
            /// Thunks to NtQueryInformationFile and FileNameInformation.
            /// </remarks>
            FileNameInfo,

            /// <summary>
            /// Renames a file. Allows renaming a file without having to specify a full path, if you have
            /// a handle to the directory the file resides in.
            /// </summary>
            /// <remarks>
            /// Only valid for SetFileInformationByHandle. Thunks to NtSetInformationFile and FileRenameInformation.
            /// MoveFileEx is effectively the same API.
            /// </remarks>
            FileRenameInfo,

            /// <summary>
            /// Allows marking a file handle for deletion. Handle must have been opened with Delete access.
            /// You cannot change the state of a handle opened with DeleteOnClose.
            /// </summary>
            /// <remarks>
            /// Only valid for SetFileInformationByHandle. Thunks to NtSetInformationFile and FileDispositionInformation.
            /// DeleteFile is effectively the same API.
            /// </remarks>
            FileDispositionInfo,

            /// <summary>
            /// Allows setting the allocated size of the file.
            /// </summary>
            /// <remarks>
            /// Only valid for SetFileInformationByHandle. Thunks to NtSetInformationFile.
            /// SetEndOfFile sets this after setting the logical end of file to the current position via FileEndOfFileInfo.
            /// </remarks>
            FileAllocationInfo,

            /// <summary>
            /// Allows setting the end of file.
            /// </summary>
            /// <remarks>
            /// Only valid for SetFileInformationByHandle. Thunks to NtSetInformationFile.
            /// SetEndOfFile calls this to set the logical end of file to whatever the current position is.
            /// </remarks>
            FileEndOfFileInfo,

            /// <summary>
            /// Gets stream information for the file.
            /// </summary>
            /// <remarks>
            /// Thunks to NtQueryInformationFile and FileStreamInformation.
            /// </remarks>
            FileStreamInfo,

            /// <summary>
            /// Gets compression information for the file.
            /// </summary>
            /// <remarks>
            /// Thunks to NtQueryInformationFile and FileCompressionInformation.
            /// </remarks>
            FileCompressionInfo,

            /// <summary>
            /// Gets the file attributes and reparse tag.
            /// </summary>
            /// <remarks>
            /// Thunks to NtQueryInformationFile and FileAttributeTagInformation.
            /// </remarks>
            FileAttributeTagInfo,

            /// <summary>
            /// Starts a query for file information in a directory.
            /// </summary>
            /// <remarks>
            /// Thunks to NtQueryDirectoryFile and FileIdBothDirectoryInformation with RestartScan
            /// set to false.
            /// </remarks>
            FileIdBothDirectoryInfo,

            /// <summary>
            /// Resumes a query for file information in a directory.
            /// </summary>
            /// <remarks>
            /// Thunks to NtQueryDirectoryFile and FileIdBothDirectoryInformation with RestartScan
            /// set to true.
            /// </remarks>
            FileIdBothDirectoryRestartInfo,

            /// <summary>
            /// Allows setting the priority hint for a file.
            /// </summary>
            /// <remarks>
            /// Only valid for SetFileInformationByHandle. Thunks to NtSetInformationFile.
            /// </remarks>
            FileIoPriorityHintInfo,

            /// <summary>
            /// Gets the file remote protocol information.
            /// </summary>
            /// <remarks>
            /// Thunks to NtQueryInformationFile and FileRemoteProtocolInformation.
            /// </remarks>
            FileRemoteProtocolInfo,

            /// <summary>
            /// Starts a query for file information in a directory. Uses FILE_FULL_DIR_INFO.
            /// </summary>
            /// <remarks>
            /// Thunks to NtQueryDirectoryFile and FileFullDirectoryInformation with RestartScan
            /// set to false. Windows 8 and up.
            /// </remarks>
            FileFullDirectoryInfo,

            /// <summary>
            /// Resumes a query for file information in a directory. Uses FILE_FULL_DIR_INFO.
            /// </summary>
            /// <remarks>
            /// Thunks to NtQueryDirectoryFile and FileFullDirectoryInformation with RestartScan
            /// set to true. Windows 8 and up.
            /// </remarks>
            FileFullDirectoryRestartInfo
        }

        public enum GET_FILEEX_INFO_LEVELS : System.UInt32
        {
            GetFileExInfoStandard = 0x0u,
            GetFileExMaxInfoLevel = 0x1u,
        }

        public enum FINDEX_INFO_LEVELS : System.UInt32
        {
            FindExInfoStandard = 0x0u,
            FindExInfoBasic = 0x1u,
            FindExInfoMaxInfoLevel = 0x2u,
        }

        public enum FINDEX_SEARCH_OPS : System.UInt32
        {
            FindExSearchNameMatch = 0x0u,
            FindExSearchLimitToDirectories = 0x1u,
            FindExSearchLimitToDevices = 0x2u,
            FindExSearchMaxSearchOp = 0x3u,
        }

        public enum FILE_TYPE : System.UInt32
        {
            FILE_TYPE_UNKNOWN = 0x0000,
            FILE_TYPE_DISK = 0x0001,
            FILE_TYPE_CHAR = 0x0002,
            FILE_TYPE_PIPE = 0x0003,
            FILE_TYPE_REMOTE = 0x8000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public System.UInt32 nLength;
            public IntPtr lpSecurityDescriptor;
            public BOOL bInheritHandle;
        }

        [StructLayout(LayoutKind.Explicit , Size = 36)]
        public struct WIN32_FILE_ATTRIBUTE_DATA
        {
            [FieldOffset(0)]
            private System.Byte pin;
            [FieldOffset(0)]
            public System.Int32 dwFileAttributes;
            [FieldOffset(4)]
            public FILETIME ftCreationTime;
            [FieldOffset(12)]
            public FILETIME ftLastAccessTime;
            [FieldOffset(20)]
            public FILETIME ftLastWriteTime;
            [FieldOffset(28)]
            public System.UInt32 nFileSizeHigh;
            [FieldOffset(32)]
            public System.UInt32 nFileSizeLow;

            public void PopulateFrom(ref WIN32_FIND_DATA findData)
            {
                void* src = Unsafe.AsPointer(ref findData);
                fixed (System.Byte* dst = &Unsafe.AsRef(in pin))
                {
                    Unsafe.CopyBlockUnaligned(dst, src , sizeof(WIN32_FILE_ATTRIBUTE_DATA).ToUInt32());
                }
            }
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode , Size = 592 , Pack = 4)]
        public unsafe struct WIN32_FIND_DATA
        {
            [FieldOffset(0)]
            public System.UInt32 dwFileAttributes;
            [FieldOffset(4)]
            public FILETIME ftCreationTime;
            [FieldOffset(12)]
            public FILETIME ftLastAccessTime;
            [FieldOffset(20)]
            public FILETIME ftLastWriteTime;
            [FieldOffset(28)]
            public System.UInt32 nFileSizeHigh;
            [FieldOffset(32)]
            public System.UInt32 nFileSizeLow;
            [FieldOffset(36)]
            public System.UInt32 dwReserved0;
            [FieldOffset(40)]
            public System.UInt32 dwReserved1;
            [FieldOffset(44)]
            private fixed char _cFileName[MAX_PATH];
            [FieldOffset(MAX_PATH+44)]
            private fixed char _cAlternateFileName[14];

            public readonly ReadOnlySpan<char> cFileName {
                get { fixed (char* c = _cFileName) return new ReadOnlySpan<char>(c, MAX_PATH); }
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 40, Pack = 8)]
        public struct FILE_BASIC_INFO
        {
            [FieldOffset(0)]
            public System.Int64 CreationTime;
            [FieldOffset(8)]
            public System.Int64 LastAccessTime;
            [FieldOffset(16)]
            public System.Int64 LastWriteTime;
            [FieldOffset(24)]
            public System.Int64 ChangeTime;
            [FieldOffset(32)]
            public System.UInt32 FileAttributes;
        }

        [StructLayout(LayoutKind.Explicit , Size = 24 , Pack = 8)]
        public struct FILE_STANDARD_INFO
        { 
            [FieldOffset(0)]
            public System.Int64 AllocationSize;

            [FieldOffset(8)]
            public System.Int64 EndOfFile;

            [FieldOffset(16)]
            public System.UInt32 NumberOfLinks;

            [FieldOffset(20)]
            public BOOLEAN DeletePending;

            [FieldOffset(21)]
            public BOOLEAN Directory;
        }

        [StructLayout(LayoutKind.Explicit , Size = 28 , Pack = 8)]
        public struct STORAGE_READ_CAPACITY
        {
            [FieldOffset(0)]
            public System.UInt32 Version;

            [FieldOffset(4)]
            public System.UInt32 Size;

            [FieldOffset(8)]
            public System.UInt32 BlockLength;

            [FieldOffset(12)]
            public System.Int64 NumberOfBlocks;

            [FieldOffset(20)]
            public System.Int64 DiskLength;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct FILE_ALLOCATION_INFO
        {
            [FieldOffset(0)]
            public System.Int64 AllocationSize;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct FILE_END_OF_FILE_INFO
        {
            [FieldOffset(0)]
            public System.Int64 EndOfFile;
        }

        public static class IOReparseOptions
        {
            public const System.UInt32 IO_REPARSE_TAG_FILE_PLACEHOLDER = 0x80000015;
            public const System.UInt32 IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;
        }

        public static class FileOperations
        {
            public const System.Int32 OPEN_EXISTING = 3;
            public const System.Int32 COPY_FILE_FAIL_IF_EXISTS = 0x00000001;

            public const System.Int32 FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
            public const System.Int32 FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000;
            public const System.Int32 FILE_FLAG_OVERLAPPED = 0x40000000;

            public const System.Int32 FILE_LIST_DIRECTORY = 0x0001;
        }

        public static class GenericOperations
        {
            public const int GENERIC_READ = unchecked((int)0x80000000);
            public const int GENERIC_WRITE = 0x40000000;
        }

        public static class FileAttributes
        {
            public const int FILE_ATTRIBUTE_NORMAL = 0x00000080;
            public const int FILE_ATTRIBUTE_READONLY = 0x00000001;
            public const int FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
            public const int FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
        }

        public static class SecurityOptions
        {
            internal const int SECURITY_SQOS_PRESENT = 0x00100000;
            internal const int SECURITY_ANONYMOUS = 0 << 16;
            internal const int SECURITY_IDENTIFICATION = 1 << 16;
            internal const int SECURITY_IMPERSONATION = 2 << 16;
            internal const int SECURITY_DELEGATION = 3 << 16;
        }

        [DllImport(Libraries.Kernel32, SetLastError = true)]
        public static extern BOOL FindClose(IntPtr hFindFile);

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use FindFirstFile.
        /// </summary>
        [DllImport(Libraries.Kernel32, EntryPoint = "FindFirstFileExW", SetLastError = true, ExactSpelling = true)]
        private static extern System.IntPtr FindFirstFile_Native(System.Char* lpFileName, FINDEX_INFO_LEVELS fInfoLevelId, WIN32_FIND_DATA* lpFindFileData, FINDEX_SEARCH_OPS fSearchOp, IntPtr lpSearchFilter, int dwAdditionalFlags);

        public static SafeFindHandle FindFirstFile(string fileName, ref WIN32_FIND_DATA data)
        {
            fileName = PathInternal.EnsureExtendedPrefixIfNeeded(fileName);

            // use FindExInfoBasic since we don't care about short name and it has better perf
            fixed (System.Char* pfn = fileName)
            fixed (WIN32_FIND_DATA* pdt = &data)
            {
                return new(FindFirstFile_Native(pfn, FINDEX_INFO_LEVELS.FindExInfoBasic, pdt, FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, 0));
            }
        }

        private static string GetAndTrimString(Span<char> buffer)
        {
            int length = buffer.Length;
            while (length > 0 && buffer[length - 1] <= 32)
            {
                length--; // trim off spaces and non-printable ASCII chars at the end of the resource
            }
            return buffer.Slice(0, length).ToString();
        }

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use GetFileAttributesEx.
        /// </summary>
        [DllImport(Libraries.Kernel32, EntryPoint = "GetFileAttributesExW", SetLastError = true, ExactSpelling = true)]
        private static extern BOOL GetFileAttributes_Native(System.Char* name, GET_FILEEX_INFO_LEVELS fileInfoLevel, WIN32_FILE_ATTRIBUTE_DATA* lpFileInformation);

        public static System.Boolean GetFileAttributesEx(string name, GET_FILEEX_INFO_LEVELS fileInfoLevel, ref WIN32_FILE_ATTRIBUTE_DATA lpFileInformation)
        {
            name = PathInternal.EnsureExtendedPrefixIfNeeded(name);
            fixed (System.Char* pname = name)
            fixed (WIN32_FILE_ATTRIBUTE_DATA* pinfo = &lpFileInformation)
            {
                return GetFileAttributes_Native(pname, fileInfoLevel, pinfo) != BOOL.FALSE;
            }
        }

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use GetFullPathName or PathHelper.
        /// </summary>
        [DllImport(Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false, ExactSpelling = true)]
        public static extern System.UInt32 GetFullPathNameW(ref System.Char lpFileName, System.UInt32 nBufferLength, ref System.Char lpBuffer, IntPtr lpFilePart);

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use GetFullPath/PathHelper.
        /// </summary>
        [DllImport(Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false, ExactSpelling = true)]
        public static extern System.UInt32 GetLongPathNameW(ref System.Char lpszShortPath, ref System.Char lpszLongPath, System.UInt32 cchBuffer);

        [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
        public static extern System.UInt32 GetTempFileNameW(ref System.Char lpPathName, System.String lpPrefixString, System.UInt32 uUnique, ref System.Char lpTempFileName);

        [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, BestFitMapping = false)]
        public static extern System.UInt32 GetTempPathW(System.Int32 bufferLen, ref System.Char buffer);

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use CopyFileEx.
        /// </summary>
        [DllImport(Libraries.Kernel32, EntryPoint = "CopyFileExW", SetLastError = true, ExactSpelling = true)]
        private static extern BOOL CopyFileEx_Native(
            System.Char* src,
            System.Char* dst,
            IntPtr progressRoutine,
            IntPtr progressData,
            int* cancel,
            int flags);

        public static System.Boolean CopyFileEx(
            string src,
            string dst,
            IntPtr progressRoutine,
            IntPtr progressData,
            ref int cancel,
            int flags)
        {
            src = PathInternal.EnsureExtendedPrefixIfNeeded(src);
            dst = PathInternal.EnsureExtendedPrefixIfNeeded(dst);
            fixed (System.Char* psrc = src)
            fixed (System.Char* pdst = dst)
            fixed (System.Int32* pcl = &cancel)
            {
                return CopyFileEx_Native(psrc, pdst, progressRoutine, progressData, pcl, flags) != BOOL.FALSE;
            }
        }

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use CreateDirectory.
        /// </summary>
        [DllImport(Libraries.Kernel32, EntryPoint = "CreateDirectoryW", SetLastError = true, ExactSpelling = true)]
        private static extern BOOL CreateDirectory_Native(System.Char* path, SECURITY_ATTRIBUTES* lpSecurityAttributes);

        public static bool CreateDirectory(string path, ref SECURITY_ATTRIBUTES lpSecurityAttributes)
        {
            // We always want to add for CreateDirectory to get around the legacy 248 character limitation
            path = PathInternal.EnsureExtendedPrefix(path) + "\0";
            fixed (System.Char* ppath = path)
            fixed (SECURITY_ATTRIBUTES* pattrs = &lpSecurityAttributes)
            {
                return CreateDirectory_Native(ppath, pattrs) != BOOL.FALSE;
            }
        }

        public static int CopyFile(string src, string dst, bool failIfExists)
        {
            int copyFlags = failIfExists ? FileOperations.COPY_FILE_FAIL_IF_EXISTS : 0;
            int cancel = 0;
            if (CopyFileEx(src, dst, IntPtr.Zero, IntPtr.Zero, ref cancel, copyFlags) == false)
            {
                return Interop.Kernel32.GetLastError();
            }

            return Errors.ERROR_SUCCESS;
        }

        // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858.aspx
        /// <summary>
        /// WARNING: The private methods do not implicitly handle long paths. Use CreateFile.
        /// </summary>
        [DllImport(Libraries.Kernel32, EntryPoint = "CreateFileW", SetLastError = true, ExactSpelling = true)]
        private unsafe static extern IntPtr CreateFilePrivate(
            System.Char* lpFileName,
            System.Int32 dwDesiredAccess,
            Microsoft.IO.FileShare dwShareMode,
            SECURITY_ATTRIBUTES* securityAttrs,
            Microsoft.IO.FileMode dwCreationDisposition,
            System.Int32 dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        public static RedistSafeFileHandle CreateFile(
            string lpFileName,
            int dwDesiredAccess,
            Microsoft.IO.FileShare dwShareMode,
            ref SECURITY_ATTRIBUTES securityAttrs,
            Microsoft.IO.FileMode dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile)
        {
            lpFileName = PathInternal.EnsureExtendedPrefixIfNeeded(lpFileName);
            fixed (SECURITY_ATTRIBUTES* sa = &securityAttrs)
            fixed (System.Char* pfn = lpFileName)
            {
                IntPtr handle = CreateFilePrivate(pfn, dwDesiredAccess, dwShareMode, sa, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
                try
                {
                    return new RedistSafeFileHandle(handle, ownsHandle: true);
                }
                catch
                {
                    CloseHandle(handle);
                    throw;
                }
            }
        }

        public static RedistSafeFileHandle CreateFile(
            string lpFileName,
            int dwDesiredAccess,
            Microsoft.IO.FileShare dwShareMode,
            Microsoft.IO.FileMode dwCreationDisposition,
            int dwFlagsAndAttributes)
        {
            IntPtr handle = CreateFile_IntPtr(lpFileName, dwDesiredAccess, dwShareMode, dwCreationDisposition, dwFlagsAndAttributes);
            try
            {
                return new RedistSafeFileHandle(handle, ownsHandle: true);
            }
            catch
            {
                CloseHandle(handle);
                throw;
            }
        }

        public static IntPtr CreateFile_IntPtr(
            string lpFileName,
            int dwDesiredAccess,
            Microsoft.IO.FileShare dwShareMode,
            Microsoft.IO.FileMode dwCreationDisposition,
            int dwFlagsAndAttributes)
        {
            lpFileName = PathInternal.EnsureExtendedPrefixIfNeeded(lpFileName);
            fixed (System.Char* pfn = lpFileName)
            {
                return CreateFilePrivate(pfn, dwDesiredAccess, dwShareMode, null, dwCreationDisposition, dwFlagsAndAttributes, IntPtr.Zero);
            }
        }

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use DeleteFile.
        /// </summary>
        [DllImport(Libraries.Kernel32, EntryPoint = "DeleteFileW", SetLastError = true, ExactSpelling = true)]
        private static extern BOOL DeleteFile_Native(System.Char* path);

        public static bool DeleteFile(string path)
        {
            path = PathInternal.EnsureExtendedPrefixIfNeeded(path);
            fixed (System.Char* ppath = path)
            {
                return DeleteFile_Native(ppath) != BOOL.FALSE;
            }
        }

        [DllImport(Libraries.Kernel32, EntryPoint = "FindNextFileW", SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false)]
        public static extern BOOL FindNextFile(SafeFindHandle hndFindFile, ref WIN32_FIND_DATA lpFindFileData);

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use MoveFile.
        /// </summary>
        [DllImport(Libraries.Kernel32, EntryPoint = "MoveFileExW", SetLastError = true, ExactSpelling = true)]
        private static extern BOOL MoveFile_Native(System.Char* src, System.Char* dst, System.UInt32 flags);

        /// <summary>
        /// Moves a file or directory, optionally overwriting existing destination file. NOTE: overwrite must be false for directories.
        /// </summary>
        /// <param name="src">Source file or directory</param>
        /// <param name="dst">Destination file or directory</param>
        /// <param name="overwrite">True to overwrite existing destination file. NOTE: must pass false for directories as overwrite of directories is not supported.</param>
        /// <returns></returns>
        public static bool MoveFile(string src, string dst, bool overwrite)
        {
            src = PathInternal.EnsureExtendedPrefixIfNeeded(src);
            dst = PathInternal.EnsureExtendedPrefixIfNeeded(dst);

            uint flags = MOVEFILE_COPY_ALLOWED;
            if (overwrite)
            {
                flags |= MOVEFILE_REPLACE_EXISTING;
            }

            fixed (System.Char* psrc = src)
            fixed (System.Char* pdst = dst)
            {
                return MoveFile_Native(psrc, pdst, flags) != BOOL.FALSE;
            }
        }

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use RemoveDirectory.
        /// </summary>
        [DllImport(Libraries.Kernel32, EntryPoint = "RemoveDirectoryW", SetLastError = true, ExactSpelling = true)]
        private static extern BOOL RemoveDirectory_Native(System.Char* path);

        public static bool RemoveDirectory(string path)
        {
            path = PathInternal.EnsureExtendedPrefixIfNeeded(path);
            fixed (System.Char* ppath = path)
            {
                return RemoveDirectory_Native(ppath) != BOOL.FALSE;
            }
        }

        [DllImport(Libraries.Kernel32, EntryPoint = "ReplaceFileW", SetLastError = true, ExactSpelling = true)]
        private static extern BOOL ReplaceFile_Native(
            System.Char* replacedFileName, 
            System.Char* replacementFileName, 
            System.Char* backupFileName,
            System.Int32 dwReplaceFlags, 
            IntPtr lpExclude, 
            IntPtr lpReserved);

        public static bool ReplaceFile(
            string replacedFileName, string replacementFileName, string backupFileName,
            int dwReplaceFlags, IntPtr lpExclude, IntPtr lpReserved)
        {
            replacedFileName = PathInternal.EnsureExtendedPrefixIfNeeded(replacedFileName);
            replacementFileName = PathInternal.EnsureExtendedPrefixIfNeeded(replacementFileName);
            backupFileName = PathInternal.EnsureExtendedPrefixIfNeeded(backupFileName);

            fixed (System.Char* preplaced = replacedFileName)
            fixed (System.Char* preplacement = replacementFileName)
            fixed (System.Char* pbackup = backupFileName)
            {
                return ReplaceFile_Native(preplaced, preplacement, pbackup, dwReplaceFlags, lpExclude, lpReserved) != BOOL.FALSE;
            }
        }

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use SetFileAttributes.
        /// </summary>
        [DllImport(Libraries.Kernel32, EntryPoint = "SetFileAttributesW", SetLastError = true, ExactSpelling = true)]
        private static extern BOOL SetFileAttributes_Native(System.Char* name, System.Int32 attr);

        public static bool SetFileAttributes(string name, int attr)
        {
            name = PathInternal.EnsureExtendedPrefixIfNeeded(name);
            fixed (System.Char* pname = name)
            {
                return SetFileAttributes_Native(pname, attr) != BOOL.FALSE;
            }
        }

        // https://msdn.microsoft.com/en-us/library/windows/desktop/aa365539.aspx
        [DllImport(Libraries.Kernel32, EntryPoint = "SetFileInformationByHandle", SetLastError = true, ExactSpelling = true)]
        private static extern BOOL SetFileInformationByHandle_Native(System.IntPtr hFile, FILE_INFO_BY_HANDLE_CLASS FileInformationClass, void* lpFileInformation, uint dwBufferSize);

        public static BOOL SetFileInformationByHandle(System.IntPtr file, FILE_ALLOCATION_INFO lpallocationinfo)
            => SetFileInformationByHandle_Native(file, FILE_INFO_BY_HANDLE_CLASS.FileAllocationInfo, &lpallocationinfo, sizeof(FILE_ALLOCATION_INFO).ToUInt32());

        public static BOOL SetFileInformationByHandle(System.IntPtr file , FILE_BASIC_INFO lpbasicinfo)
            => SetFileInformationByHandle_Native(file, FILE_INFO_BY_HANDLE_CLASS.FileBasicInfo, &lpbasicinfo, sizeof(FILE_BASIC_INFO).ToUInt32());

        public static BOOL SetFileInformationByHandle(System.IntPtr file, FILE_END_OF_FILE_INFO lpendoffile)
            => SetFileInformationByHandle_Native(file, FILE_INFO_BY_HANDLE_CLASS.FileEndOfFileInfo, &lpendoffile, sizeof(FILE_END_OF_FILE_INFO).ToUInt32());

        // Default values indicate "no change".  Use defaults so that we don't force callsites to be aware of the default values
        public static unsafe bool SetFileTime(
            RedistSafeFileHandle hFile,
            long creationTime = -1,
            long lastAccessTime = -1,
            long lastWriteTime = -1,
            long changeTime = -1,
            uint fileAttributes = 0)
        {
            FILE_BASIC_INFO basicInfo = new FILE_BASIC_INFO() {
                CreationTime = creationTime,
                LastAccessTime = lastAccessTime,
                LastWriteTime = lastWriteTime,
                ChangeTime = changeTime,
                FileAttributes = fileAttributes
            };

            return SetFileInformationByHandle(hFile.Handle, basicInfo) != BOOL.FALSE;
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "ReadFile" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL ReadFile_Native(System.IntPtr hfe, System.Byte* pbuf, System.UInt32 numbytestoread, System.UInt32* pnumbytesread, OVERLAPPED* poverlapped);
    
        public static BOOL ReadFile(System.IntPtr hfe , System.Byte* pbuf , System.UInt32 nbtr , OVERLAPPED overlapped, out System.UInt32 numbytesread)
        {
            System.UInt32 pnr;
            BOOL ret = ReadFile_Native(hfe, pbuf, nbtr, &pnr, &overlapped);
            numbytesread = pnr;
            return ret;
        }

        public static BOOL ReadFile(System.IntPtr hfe, System.Byte* pbuf, System.UInt32 nbtr, OVERLAPPED* overlapped, out System.UInt32 numbytesread)
        {
            System.UInt32 pnr;
            BOOL ret = ReadFile_Native(hfe, pbuf, nbtr, &pnr, overlapped);
            numbytesread = pnr;
            return ret;
        }

        public static BOOL ReadFile(System.IntPtr hfe , System.Byte* pbuf , System.UInt32 nbtr , out System.UInt32 numbytesread)
        {
            System.UInt32 pnr;
            BOOL ret = ReadFile_Native(hfe, pbuf, nbtr, &pnr, null);
            numbytesread = pnr;
            return ret;
        }

        [DllImport(Libraries.Kernel32, EntryPoint = "WriteFile" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL WriteFile_Native(System.IntPtr hfe, System.Byte* pbuf, System.UInt32 numbytestorwrite, System.UInt32* pnumbyteswritten, OVERLAPPED* poverlapped);

        public static BOOL WriteFile(System.IntPtr hfe , System.Byte* pbuf , System.UInt32 nbtw , OVERLAPPED overlapped , out System.UInt32 nbw)
        {
            System.UInt32 pnw;
            BOOL ret = WriteFile_Native(hfe, pbuf, nbtw, &pnw, &overlapped);
            nbw = pnw;
            return ret;
        }

        public static BOOL WriteFile(System.IntPtr hfe, System.Byte* pbuf, System.UInt32 nbtw, OVERLAPPED* overlapped, out System.UInt32 nbw)
        {
            System.UInt32 pnw;
            BOOL ret = WriteFile_Native(hfe, pbuf, nbtw, &pnw, overlapped);
            nbw = pnw;
            return ret;
        }

        public static BOOL WriteFile(System.IntPtr hfe, System.Byte* pbuf, System.UInt32 nbtw, out System.UInt32 nbw)
        {
            System.UInt32 pnw;
            BOOL ret = WriteFile_Native(hfe, pbuf, nbtw, &pnw, null);
            nbw = pnw;
            return ret;
        }

        [DllImport(Libraries.Kernel32 , ExactSpelling = true , SetLastError = true)]
        public static extern FILE_TYPE GetFileType(System.IntPtr hfe);

        [DllImport(Libraries.Kernel32 , ExactSpelling = true , SetLastError = true)]
        public static extern BOOL DeviceIoControl(
            System.IntPtr hfe , 
            IOCTL ctlcode , 
            void* InputBuffer,
            System.UInt32 InputBufferSize,
            void* OutBuffer,
            System.UInt32 OutBufferSize,
            System.UInt32* pbytesret,
            OVERLAPPED* poverlapped);

        [DllImport(Libraries.Kernel32 , EntryPoint = "GetFileInformationByHandleEx" , SetLastError = true , ExactSpelling = true)]
        private static extern BOOL GetFileInformationByHandle_Native(System.IntPtr hfe , FILE_INFO_BY_HANDLE_CLASS filecls , void* pfileinfo , System.UInt32 bufsize);

        public static BOOL GetFileInformationByHandle(System.IntPtr file , out FILE_STANDARD_INFO standardinf)
        {
            FILE_STANDARD_INFO fstandard;
            BOOL ret = GetFileInformationByHandle_Native(file, FILE_INFO_BY_HANDLE_CLASS.FileStandardInfo, &fstandard, sizeof(FILE_STANDARD_INFO).ToUInt32());
            standardinf = fstandard;
            return ret;
        }

        public static BOOL GetFileInformationByHandle(System.IntPtr file , out FILE_BASIC_INFO basicinf)
        {
            FILE_BASIC_INFO snative;
            BOOL ret = GetFileInformationByHandle_Native(file, FILE_INFO_BY_HANDLE_CLASS.FileBasicInfo, &snative, sizeof(FILE_BASIC_INFO).ToUInt32());
            basicinf = snative;
            return ret;
        }

        public static BOOL GetFileInformationByHandle(System.IntPtr file , out System.String filename)
        {
            // For FILE_NAME_INFO we have to work with the following way:
            // The FILE_NAME_INFO is the buffer that contains the name string.
            // So we will create a byte array and will get the required information,
            // emulating the structure.
            System.Byte[] datatemp = new System.Byte[1004]; // The file name along it's buffer size.
        g_retry:
            (datatemp.Length - 4).GetBytes().Copy(0, datatemp, 0, sizeof(System.Int32).ToUInt32());
            BOOL bret;
            fixed (System.Byte* pdat = datatemp)
            {
                bret = GetFileInformationByHandle_Native(file, FILE_INFO_BY_HANDLE_CLASS.FileNameInfo, pdat, datatemp.Length.ToUInt32());
                if (bret == BOOL.FALSE)
                {
                    System.Int32 erc = GetLastError();
                    if (erc == Errors.ERROR_MORE_DATA) {
                        datatemp = new System.Byte[datatemp.ToUInt32(0) + 4];
                        goto g_retry;
                    } else {
                        throw new MP.ExceptionSystem.NativeWindowsException(erc);
                    }
                }
            }
            fixed (System.Byte* pdat = &datatemp[4])
            {
                // And return the file name by the length...
                filename = new((System.Char*)pdat , 0 , (datatemp.ToUInt32(0) / sizeof(System.Char)).ToInt32());
            }
            return bret;
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "SetFilePointerEx" , SetLastError = true , ExactSpelling = true)]
        private static extern BOOL SetFilePointer_Native(System.IntPtr hfe, System.Int64 disttomove, System.Int64* pdistmoved, SeekOrigin origin);

        public static BOOL SetFilePointer(System.IntPtr hfe , System.Int64 offset , SeekOrigin origin , out System.Int64 movedat)
        {
            System.Int64 pmv;
            BOOL ret = SetFilePointer_Native(hfe, offset, &pmv, origin);
            movedat = pmv;
            return ret;
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "CancelIoEx" , SetLastError = true , ExactSpelling = true)]
        private static extern BOOL CancelIo_Native(System.IntPtr hfe, OVERLAPPED* poverlapped);

        public static BOOL CancelIo(System.IntPtr hfe) => CancelIo_Native(hfe, null);

        public static BOOL CancelIo(System.IntPtr hfe , OVERLAPPED* overlapped) => CancelIo_Native(hfe , overlapped);

        [DllImport(Libraries.Kernel32 , ExactSpelling = true , SetLastError = true)]
        public static extern BOOL FlushFileBuffers(System.IntPtr hfe);
    }
}