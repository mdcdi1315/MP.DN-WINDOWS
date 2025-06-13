using System;
using System.IO;
using System.Runtime.InteropServices;

partial class Interop
{
    public unsafe static partial class Advapi32
    {
        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use EncryptFile.
        /// </summary>
        [DllImport(Libraries.Advapi32, EntryPoint = "EncryptFileW", SetLastError = true)]
        private static extern BOOL EncryptFile_Native(System.Char* lpFileName);

        public static bool EncryptFile(string path)
        {
            path = PathInternal.EnsureExtendedPrefixIfNeeded(path) + "\0";
            fixed (System.Char* lpFileName = path) 
            {
                return EncryptFile_Native(lpFileName) != BOOL.FALSE;
            }
        }

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use DecryptFile.
        /// </summary>
        [DllImport(Libraries.Advapi32, EntryPoint = "DecryptFileW", SetLastError = true)]
        private static extern BOOL DecryptFile_Native(System.Char* lpFileName, System.Int32 dwReserved);

        public static bool DecryptFile(string path)
        {
            path = PathInternal.EnsureExtendedPrefixIfNeeded(path) + "\0";
            fixed (System.Char* lpFileName = path)
            {
                return DecryptFile_Native(lpFileName, 0) != BOOL.FALSE;
            }
        }

        public enum FileEncryptionConstants : System.UInt32
        {
            /// <summary>
            /// The file can be encrypted.
            /// </summary>
            FILE_ENCRYPTABLE = 0,
            /// <summary>
            /// The file is encrypted.
            /// </summary>
            FILE_IS_ENCRYPTED = 1,
            /// <summary>
            /// The file is a read-only file.
            /// </summary>
            FILE_READ_ONLY = 8,
            /// <summary>
            /// The file is a root directory. Root directories cannot be encrypted.
            /// </summary>
            FILE_ROOT_DIR = 3,
            /// <summary>
            /// The file is a system file. System files cannot be encrypted.
            /// </summary>
            FILE_SYSTEM_ATTR = 2,
            /// <summary>
            /// The file is a system directory. System directories cannot be encrypted.
            /// </summary>
            FILE_SYSTEM_DIR = 4,
            /// <summary>
            /// The file system does not support file encryption.
            /// </summary>
            FILE_SYSTEM_NOT_SUPPORT = 6,
            /// <summary>
            /// The encryption status is unknown. The file may be encrypted.
            /// </summary>
            FILE_UNKNOWN = 5
        }

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use FileEncryptionStatus.
        /// </summary>
        [DllImport(Libraries.Advapi32 , EntryPoint = "FileEncryptionStatusW" , SetLastError = true)]
        private static extern BOOL FileEncryptionStatusW_Native(System.Char* buffer , FileEncryptionConstants* status);

        public static System.Boolean FileEncryptionStatus(System.String path , out FileEncryptionConstants state)
        {
            state = 0;
            path = PathInternal.EnsureExtendedPrefixIfNeeded(path) + "\0";
            fixed (System.Char* src = path)
            fixed (FileEncryptionConstants* statnative = &state)
            {
                return FileEncryptionStatusW_Native(src, statnative) != BOOL.FALSE;
            }
        }
    }
}