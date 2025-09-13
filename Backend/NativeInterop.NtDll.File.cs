
using MP;
using System;
using Microsoft.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

unsafe partial class Interop
{
    public static partial class NtDll
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct IO_STATUS_BLOCK
        {
            /// <summary>Status data</summary>
            public IO_STATUS Status;

            /// <summary>Request dependent value.</summary>
            public System.IntPtr Information;
        }

        // This isn't an actual Windows type, it is a union within IO_STATUS_BLOCK. We *have* to separate it out as
        // the size of IntPtr varies by architecture and we can't specify the size at compile time to offset the
        // Information pointer in the status block.
        [StructLayout(LayoutKind.Explicit)]
        public struct IO_STATUS
        {
            /// <summary>
            /// The completion status, either STATUS_SUCCESS if the operation was completed successfully or
            /// some other informational, warning, or error status.
            /// </summary>
            [FieldOffset(0)]
            public NTSTATUS Status;

            /// <summary>
            /// Reserved for internal use.
            /// </summary>
            [FieldOffset(0)]
            public System.IntPtr Pointer;
        }

        /// <summary>
        /// <a href="https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/content/wdm/ns-wdm-_file_full_ea_information">FILE_FULL_EA_INFORMATION</a> structure.
        /// Provides extended attribute (EA) information. This structure is used primarily by network drivers.
        /// </summary>
        [StructLayout(LayoutKind.Explicit , Size = 8 , Pack = 8)]
        public struct FILE_FULL_EA_INFORMATION
        {
            /// <summary>
            /// The offset of the next FILE_FULL_EA_INFORMATION-type entry. This member is zero if no other entries follow this one.
            /// </summary>
            [FieldOffset(0)]
            public System.UInt32 NextEntryOffset;

            /// <summary>
            /// Can be zero or can be set with FILE_NEED_EA, indicating that the file to which the EA belongs cannot be interpreted without understanding the associated extended attributes.
            /// </summary>
            [FieldOffset(4)]
            public System.Byte Flags;

            /// <summary>
            /// The length in bytes of the EaName array. This value does not include a null-terminator to EaName.
            /// </summary>
            [FieldOffset(5)]
            public System.Byte EaNameLength;

            /// <summary>
            /// The length in bytes of each EA value in the array.
            /// </summary>
            [FieldOffset(6)]
            public System.UInt16 EaValueLength;
        }

        /// <summary>
        /// <a href="https://msdn.microsoft.com/en-us/library/windows/hardware/ff540289.aspx">FILE_FULL_DIR_INFORMATION</a> structure.
        /// Used with GetFileInformationByHandleEx and FileIdBothDirectoryInfo/RestartInfo as well as NtQueryFileInformation.
        /// Equivalent to <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/hh447298.aspx">FILE_FULL_DIR_INFO</a> structure.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct FILE_FULL_DIR_INFORMATION
        {
            /// <summary>
            /// Offset in bytes of the next entry, if any.
            /// </summary>
            [FieldOffset(0)]
            public System.UInt32 NextEntryOffset;

            /// <summary>
            /// Byte offset within the parent directory, undefined for NTFS.
            /// </summary>
            [FieldOffset(4)]
            public System.UInt32 FileIndex;
            [FieldOffset(8)]
            public LongFileTime CreationTime;
            [FieldOffset(16)]
            public LongFileTime LastAccessTime;
            [FieldOffset(24)]
            public LongFileTime LastWriteTime;
            [FieldOffset(32)]
            public LongFileTime ChangeTime;
            [FieldOffset(40)]
            public System.Int64 EndOfFile;
            [FieldOffset(48)]
            public System.Int64 AllocationSize;

            /// <summary>
            /// File attributes.
            /// </summary>
            /// <remarks>
            /// Note that MSDN documentation isn't correct for this- it can return
            /// any FILE_ATTRIBUTE that is currently set on the file, not just the
            /// ones documented.
            /// </remarks>
            [FieldOffset(56)]
            public System.IO.FileAttributes FileAttributes;

            /// <summary>
            /// The length of the file name in bytes (without null).
            /// </summary>
            [FieldOffset(60)]
            public System.UInt32 FileNameLength;

            /// <summary>
            /// The extended attribute size OR the reparse tag if a reparse point.
            /// </summary>
            [FieldOffset(64)]
            public System.UInt32 EaSize;

            [FieldOffset(68)]
            private System.Char _fileName;

            public ReadOnlySpan<char> FileName => MemoryMarshal.CreateReadOnlySpan(ref _fileName, (FileNameLength / sizeof(System.Char)).ToInt32());

            /// <summary>
            /// Gets the next info pointer or null if there are no more.
            /// </summary>
            public unsafe static FILE_FULL_DIR_INFORMATION* GetNextInfo(FILE_FULL_DIR_INFORMATION* info)
            {
                if (info is null)
                    return null;

                uint nextOffset = info->NextEntryOffset;
                if (nextOffset == 0)
                    return null;

                return (FILE_FULL_DIR_INFORMATION*)((byte*)info + nextOffset);
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct FILE_ACCESS_INFORMATION
        {
            [FieldOffset(0)]
            public DesiredAccess AccessFlags;
        }

        [StructLayout(LayoutKind.Explicit , Pack = 4)]
        public struct FILE_ID_INFORMATION
        {
            [FieldOffset(0)]
            public System.UInt64 VolumeID;

            [FieldOffset(8)]
            public fixed System.Byte FileID[16];
        }

        [StructLayout(LayoutKind.Explicit , Pack = 4)]
        public struct FILE_STANDARD_INFORMATION
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

        [StructLayout(LayoutKind.Explicit, Size = 40, Pack = 8)]
        public struct FILE_BASIC_INFORMATION
        {
            [FieldOffset(0)]
            public LongFileTime CreationTime;
            [FieldOffset(8)]
            public LongFileTime LastAccessTime;
            [FieldOffset(16)]
            public LongFileTime LastWriteTime;
            [FieldOffset(24)]
            public LongFileTime ChangeTime;
            [FieldOffset(32)]
            public FileAttributes FileAttributes;
        }

        [StructLayout(LayoutKind.Explicit , Size = 8 , Pack = 8)]
        public struct FILE_POSITION_INFORMATION
        {
            [FieldOffset(0)]
            public System.Int64 CurrentByteOffset;
        }

        [StructLayout(LayoutKind.Explicit , Size = 8 , Pack = 8)]
        public struct FILE_END_OF_FILE_INFORMATION
        {
            [FieldOffset(0)]
            public System.Int64 EndOfFile;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct FILE_MODE_INFORMATION
        {
            [FieldOffset(0)]
            public CreateOptions Mode;
        }

        [StructLayout(LayoutKind.Sequential , Pack = 8)]
        private struct FILE_RENAME_INFORMATION_OLD
        {
            public BOOLEAN ReplaceIfExists;
            public System.IntPtr RootDirectory;
            public System.UInt32 FileNameLength;
            // This must be treated as a reference.
            public System.Char ReferenceToName;

            public static SafeLibcMemoryHandle GetRenameInformation(System.Boolean replaceifexists , System.String name, System.IntPtr root)
            {
                System.Int32 size = sizeof(FILE_RENAME_INFORMATION_OLD);
                System.Int32 leninbytes = name.Length * sizeof(System.Char);
                SafeLibcMemoryHandle mem = new(size + leninbytes);
                FILE_RENAME_INFORMATION_OLD* p = (FILE_RENAME_INFORMATION_OLD*)mem.MemoryPointer;
                p->ReplaceIfExists = replaceifexists ? BOOLEAN.TRUE : BOOLEAN.FALSE;
                p->RootDirectory = root;
                p->FileNameLength = leninbytes.ToUInt32();
                ref System.Char pref = ref Unsafe.AsRef(in name.GetPinnableReference());
                Unsafe.CopyBlockUnaligned(ref Unsafe.As<System.Char, System.Byte>(ref p->ReferenceToName), ref Unsafe.As<System.Char, System.Byte>(ref pref), p->FileNameLength);
                return mem;
            }
        }

        [StructLayout(LayoutKind.Sequential , Pack = 8)]
        private struct FILE_RENAME_INFORMATION_NEW
        {
            [StructLayout(LayoutKind.Explicit , Size = 4)]
            public struct DUMMYUNIONNAME
            {
                [FieldOffset(0)]
                public BOOLEAN ReplaceIfExists;
                [FieldOffset(0)]
                public System.UInt32 Flags;
            }

            public DUMMYUNIONNAME Union;
            public System.IntPtr RootDirectory;
            public System.UInt32 FileNameLength;
            // This must be treated as a reference.
            public System.Char ReferenceToName;

            public static SafeLibcMemoryHandle GetRenameInformation(System.Boolean replaceifexists, System.String name, System.IntPtr root)
            {
                System.Int32 size = sizeof(FILE_RENAME_INFORMATION_NEW);
                System.Int32 leninbytes = name.Length * sizeof(System.Char);
                SafeLibcMemoryHandle mem = new(size + leninbytes);
                FILE_RENAME_INFORMATION_NEW* p = (FILE_RENAME_INFORMATION_NEW*)mem.MemoryPointer;
                p->Union.ReplaceIfExists = replaceifexists ? BOOLEAN.TRUE : BOOLEAN.FALSE;
                p->RootDirectory = root;
                p->FileNameLength = leninbytes.ToUInt32();
                ref System.Char pref = ref Unsafe.AsRef(in name.GetPinnableReference());
                Unsafe.CopyBlockUnaligned(ref Unsafe.As<System.Char, System.Byte>(ref p->ReferenceToName), ref Unsafe.As<System.Char , System.Byte>(ref pref), p->FileNameLength);
                return mem;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FILE_NAME_INFORMATION
        {
            public System.UInt32 FileNameLength;

            public static System.String ReadName(FILE_NAME_INFORMATION* pi)
                => new((System.Char*)((System.Byte*)pi + sizeof(System.UInt32)) , 0, (pi->FileNameLength / sizeof(System.Char)).ToInt32());
        }

        /// <summary>Specifies file information attributes.</summary>
        public enum FILE_INFORMATION_CLASS : System.UInt32
        {
            FileDirectoryInformation = 1,
            FileFullDirectoryInformation = 2,
            FileBothDirectoryInformation = 3,
            FileBasicInformation = 4,
            FileStandardInformation = 5,
            FileInternalInformation = 6,
            FileEaInformation = 7,
            FileAccessInformation = 8,
            FileNameInformation = 9,
            FileRenameInformation = 10,
            FileLinkInformation = 11,
            FileNamesInformation = 12,
            FileDispositionInformation = 13,
            FilePositionInformation = 14,
            FileFullEaInformation = 15,
            FileModeInformation = 16,
            FileAlignmentInformation = 17,
            FileAllInformation = 18,
            FileAllocationInformation = 19,
            FileEndOfFileInformation = 20,
            FileAlternateNameInformation = 21,
            FileStreamInformation = 22,
            FilePipeInformation = 23,
            FilePipeLocalInformation = 24,
            FilePipeRemoteInformation = 25,
            FileMailslotQueryInformation = 26,
            FileMailslotSetInformation = 27,
            FileCompressionInformation = 28,
            FileObjectIdInformation = 29,
            FileCompletionInformation = 30,
            FileMoveClusterInformation = 31,
            FileQuotaInformation = 32,
            FileReparsePointInformation = 33,
            FileNetworkOpenInformation = 34,
            FileAttributeTagInformation = 35,
            FileTrackingInformation = 36,
            FileIdBothDirectoryInformation = 37,
            FileIdFullDirectoryInformation = 38,
            FileValidDataLengthInformation = 39,
            FileShortNameInformation = 40,
            FileIoCompletionNotificationInformation = 41,
            FileIoStatusBlockRangeInformation = 42,
            FileIoPriorityHintInformation = 43,
            FileSfioReserveInformation = 44,
            FileSfioVolumeInformation = 45,
            FileHardLinkInformation = 46,
            FileProcessIdsUsingFileInformation = 47,
            FileNormalizedNameInformation = 48,
            FileNetworkPhysicalNameInformation = 49,
            FileIdGlobalTxDirectoryInformation = 50,
            FileIsRemoteDeviceInformation = 51,
            FileUnusedInformation = 52,
            FileNumaNodeInformation = 53,
            FileStandardLinkInformation = 54,
            FileRemoteProtocolInformation = 55,
            FileRenameInformationBypassAccessCheck = 56,
            FileLinkInformationBypassAccessCheck = 57,
            FileVolumeNameInformation = 58,
            FileIdInformation = 59,
            FileIdExtdDirectoryInformation = 60,
            FileReplaceCompletionInformation = 61,
            FileHardLinkFullIdInformation = 62,
            FileIdExtdBothDirectoryInformation = 63,
            FileDispositionInformationEx = 64,
            FileRenameInformationEx = 65,
            FileRenameInformationExBypassAccessCheck = 66,
            FileDesiredStorageClassInformation = 67,
            FileStatInformation = 68
        }

        /// <summary>
        /// File creation disposition when calling directly to NT APIs.
        /// </summary>
        public enum CreateDisposition : System.UInt32
        {
            /// <summary>
            /// Default. Replace or create. Deletes existing file instead of overwriting.
            /// </summary>
            /// <remarks>
            /// As this potentially deletes it requires that DesiredAccess must include Delete.
            /// This has no equivalent in CreateFile.
            /// </remarks>
            FILE_SUPERSEDE = 0,

            /// <summary>
            /// Open if exists or fail if doesn't exist. Equivalent to OPEN_EXISTING or
            /// <see cref="System.IO.FileMode.Open"/>.
            /// </summary>
            /// <remarks>
            /// TruncateExisting also uses Open and then manually truncates the file
            /// by calling NtSetInformationFile with FileAllocationInformation and an
            /// allocation size of 0.
            /// </remarks>
            FILE_OPEN = 1,

            /// <summary>
            /// Create if doesn't exist or fail if does exist. Equivalent to CREATE_NEW
            /// or <see cref="System.IO.FileMode.CreateNew"/>.
            /// </summary>
            FILE_CREATE = 2,

            /// <summary>
            /// Open if exists or create if doesn't exist. Equivalent to OPEN_ALWAYS or
            /// <see cref="System.IO.FileMode.OpenOrCreate"/>.
            /// </summary>
            FILE_OPEN_IF = 3,

            /// <summary>
            /// Open and overwrite if exists or fail if doesn't exist. Equivalent to
            /// TRUNCATE_EXISTING or <see cref="System.IO.FileMode.Truncate"/>.
            /// </summary>
            FILE_OVERWRITE = 4,

            /// <summary>
            /// Open and overwrite if exists or create if doesn't exist. Equivalent to
            /// CREATE_ALWAYS or <see cref="System.IO.FileMode.Create"/>.
            /// </summary>
            FILE_OVERWRITE_IF = 5
        }

        /// <summary>
        /// Options for creating/opening files with NtCreateFile.
        /// </summary>
        public enum CreateOptions : System.UInt32
        {
            /// <summary>
            /// File being created or opened must be a directory file. Disposition must be FILE_CREATE, FILE_OPEN,
            /// or FILE_OPEN_IF.
            /// </summary>
            /// <remarks>
            /// Can only be used with FILE_SYNCHRONOUS_IO_ALERT/NONALERT, FILE_WRITE_THROUGH, FILE_OPEN_FOR_BACKUP_INTENT,
            /// and FILE_OPEN_BY_FILE_ID flags.
            /// </remarks>
            FILE_DIRECTORY_FILE = 0x00000001,

            /// <summary>
            /// Applications that write data to the file must actually transfer the data into
            /// the file before any requested write operation is considered complete. This flag
            /// is set automatically if FILE_NO_INTERMEDIATE_BUFFERING is set.
            /// </summary>
            FILE_WRITE_THROUGH = 0x00000002,

            /// <summary>
            /// All accesses to the file are sequential.
            /// </summary>
            FILE_SEQUENTIAL_ONLY = 0x00000004,

            /// <summary>
            /// File cannot be cached in driver buffers. Cannot use with AppendData desired access.
            /// </summary>
            FILE_NO_INTERMEDIATE_BUFFERING = 0x00000008,

            /// <summary>
            /// All operations are performed synchronously. Any wait on behalf of the caller is
            /// subject to premature termination from alerts.
            /// </summary>
            /// <remarks>
            /// Cannot be used with FILE_SYNCHRONOUS_IO_NONALERT.
            /// Synchronous DesiredAccess flag is required. I/O system will maintain file position context.
            /// </remarks>
            FILE_SYNCHRONOUS_IO_ALERT = 0x00000010,

            /// <summary>
            /// All operations are performed synchronously. Waits in the system to synchronize I/O queuing
            /// and completion are not subject to alerts.
            /// </summary>
            /// <remarks>
            /// Cannot be used with FILE_SYNCHRONOUS_IO_ALERT.
            /// Synchronous DesiredAccess flag is required. I/O system will maintain file position context.
            /// </remarks>
            FILE_SYNCHRONOUS_IO_NONALERT = 0x00000020,

            /// <summary>
            /// File being created or opened must not be a directory file. Can be a data file, device,
            /// or volume.
            /// </summary>
            FILE_NON_DIRECTORY_FILE = 0x00000040,

            /// <summary>
            /// Create a tree connection for this file in order to open it over the network.
            /// </summary>
            /// <remarks>
            /// Not used by device and intermediate drivers.
            /// </remarks>
            FILE_CREATE_TREE_CONNECTION = 0x00000080,

            /// <summary>
            /// Complete the operation immediately with a success code of STATUS_OPLOCK_BREAK_IN_PROGRESS if
            /// the target file is oplocked.
            /// </summary>
            /// <remarks>
            /// Not compatible with ReserveOpfilter or OpenRequiringOplock.
            /// Not used by device and intermediate drivers.
            /// </remarks>
            FILE_COMPLETE_IF_OPLOCKED = 0x00000100,

            /// <summary>
            /// If the extended attributes on an existing file being opened indicate that the caller must
            /// understand extended attributes to properly interpret the file, fail the request.
            /// </summary>
            /// <remarks>
            /// Not used by device and intermediate drivers.
            /// </remarks>
            FILE_NO_EA_KNOWLEDGE = 0x00000200,

            // Behavior undocumented, defined in headers
            // FILE_OPEN_REMOTE_INSTANCE = 0x00000400,

            /// <summary>
            /// Accesses to the file can be random, so no sequential read-ahead operations should be performed
            /// on the file by FSDs or the system.
            /// </summary>
            FILE_RANDOM_ACCESS = 0x00000800,

            /// <summary>
            /// Delete the file when the last handle to it is passed to NtClose. Requires Delete flag in
            /// DesiredAccess parameter.
            /// </summary>
            FILE_DELETE_ON_CLOSE = 0x00001000,

            /// <summary>
            /// Open the file by reference number or object ID. The file name that is specified by the ObjectAttributes
            /// name parameter includes the 8 or 16 byte file reference number or ID for the file in the ObjectAttributes
            /// name field. The device name can optionally be prefixed.
            /// </summary>
            /// <remarks>
            /// NTFS supports both reference numbers and object IDs. 16 byte reference numbers are 8 byte numbers padded
            /// with zeros. ReFS only supports reference numbers (not object IDs). 8 byte and 16 byte reference numbers
            /// are not related. Note that as the UNICODE_STRING will contain raw byte data, it may not be a "valid" string.
            /// Not used by device and intermediate drivers.
            /// </remarks>
            /// <example>
            /// \??\C:\{8 bytes of binary FileID}
            /// \device\HardDiskVolume1\{16 bytes of binary ObjectID}
            /// {8 bytes of binary FileID}
            /// </example>
            FILE_OPEN_BY_FILE_ID = 0x00002000,

            /// <summary>
            /// The file is being opened for backup intent. Therefore, the system should check for certain access rights
            /// and grant the caller the appropriate access to the file before checking the DesiredAccess parameter
            /// against the file's security descriptor.
            /// </summary>
            /// <remarks>
            /// Not used by device and intermediate drivers.
            /// </remarks>
            FILE_OPEN_FOR_BACKUP_INTENT = 0x00004000,

            /// <summary>
            /// When creating a file, specifies that it should not inherit the compression bit from the parent directory.
            /// </summary>
            FILE_NO_COMPRESSION = 0x00008000,

            /// <summary>
            /// The file is being opened and an opportunistic lock (oplock) on the file is being requested as a single atomic
            /// operation.
            /// </summary>
            /// <remarks>
            /// The file system checks for oplocks before it performs the create operation and will fail the create with a
            /// return code of STATUS_CANNOT_BREAK_OPLOCK if the result would be to break an existing oplock.
            /// Not compatible with CompleteIfOplocked or ReserveOpFilter. Windows 7 and up.
            /// </remarks>
            FILE_OPEN_REQUIRING_OPLOCK = 0x00010000,

            /// <summary>
            /// CreateFile2 uses this flag to prevent opening a file that you don't have access to without specifying 
            /// FILE_SHARE_READ. (Preventing users that can only read a file from denying access to other readers.)
            /// </summary>
            /// <remarks>
            /// Windows 7 and up.
            /// </remarks>
            FILE_DISALLOW_EXCLUSIVE = 0x00020000,

            /// <summary>
            /// The client opening the file or device is session aware and per session access is validated if necessary.
            /// </summary>
            /// <remarks>
            /// Windows 8 and up.
            /// </remarks>
            FILE_SESSION_AWARE = 0x00040000,

            /// <summary>
            /// This flag allows an application to request a filter opportunistic lock (oplock) to prevent other applications
            /// from getting share violations.
            /// </summary>
            /// <remarks>
            /// Not compatible with CompleteIfOplocked or OpenRequiringOplock.
            /// If there are already open handles, the create request will fail with STATUS_OPLOCK_NOT_GRANTED.
            /// </remarks>
            FILE_RESERVE_OPFILTER = 0x00100000,

            /// <summary>
            /// Open a file with a reparse point attribute, bypassing the normal reparse point processing.
            /// </summary>
            FILE_OPEN_REPARSE_POINT = 0x00200000,

            /// <summary>
            /// Causes files that are marked with the Offline attribute not to be recalled from remote storage.
            /// </summary>
            /// <remarks>
            /// More details can be found in Remote Storage documentation (see Basic Concepts).
            /// https://technet.microsoft.com/en-us/library/cc938459.aspx
            /// </remarks>
            FILE_OPEN_NO_RECALL = 0x00400000

            // Behavior undocumented, defined in headers
            // FILE_OPEN_FOR_FREE_SPACE_QUERY = 0x00800000
        }

        /// <summary>
        /// System.IO.FileAccess looks up these values when creating handles
        /// </summary>
        /// <remarks>
        /// File Security and Access Rights
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa364399.aspx
        /// </remarks>
        [Flags]
        public enum DesiredAccess : System.UInt32
        {
            // File Access Rights Constants
            // https://msdn.microsoft.com/en-us/library/windows/desktop/gg258116.aspx

            /// <summary>
            /// For a file, the right to read data from the file.
            /// </summary>
            /// <remarks>
            /// Directory version of this flag is <see cref="FILE_LIST_DIRECTORY"/>.
            /// </remarks>
            FILE_READ_DATA = 0x0001,

            /// <summary>
            /// For a directory, the right to list the contents.
            /// </summary>
            /// <remarks>
            /// File version of this flag is <see cref="FILE_READ_DATA"/>.
            /// </remarks>
            FILE_LIST_DIRECTORY = 0x0001,

            /// <summary>
            /// For a file, the right to write data to the file.
            /// </summary>
            /// <remarks>
            /// Directory version of this flag is <see cref="FILE_ADD_FILE"/>.
            /// </remarks>
            FILE_WRITE_DATA = 0x0002,

            /// <summary>
            /// For a directory, the right to create a file in a directory.
            /// </summary>
            /// <remarks>
            /// File version of this flag is <see cref="FILE_WRITE_DATA"/>.
            /// </remarks>
            FILE_ADD_FILE = 0x0002,

            /// <summary>
            /// For a file, the right to append data to a file. <see cref="FILE_WRITE_DATA"/> is needed
            /// to overwrite existing data.
            /// </summary>
            /// <remarks>
            /// Directory version of this flag is <see cref="FILE_ADD_SUBDIRECTORY"/>.
            /// </remarks>
            FILE_APPEND_DATA = 0x0004,

            /// <summary>
            /// For a directory, the right to create a subdirectory.
            /// </summary>
            /// <remarks>
            /// File version of this flag is <see cref="FILE_APPEND_DATA"/>.
            /// </remarks>
            FILE_ADD_SUBDIRECTORY = 0x0004,

            /// <summary>
            /// For a named pipe, the right to create a pipe instance.
            /// </summary>
            FILE_CREATE_PIPE_INSTANCE = 0x0004,

            /// <summary>
            /// The right to read extended attributes.
            /// </summary>
            FILE_READ_EA = 0x0008,

            /// <summary>
            /// The right to write extended attributes.
            /// </summary>
            FILE_WRITE_EA = 0x0010,

            /// <summary>
            /// The right to execute the file.
            /// </summary>
            /// <remarks>
            /// Directory version of this flag is <see cref="FILE_TRAVERSE"/>.
            /// </remarks>
            FILE_EXECUTE = 0x0020,

            /// <summary>
            /// For a directory, the right to traverse the directory.
            /// </summary>
            /// <remarks>
            /// File version of this flag is <see cref="FILE_EXECUTE"/>.
            /// </remarks>
            FILE_TRAVERSE = 0x0020,

            /// <summary>
            /// For a directory, the right to delete a directory and all
            /// the files it contains, including read-only files.
            /// </summary>
            FILE_DELETE_CHILD = 0x0040,

            /// <summary>
            /// The right to read attributes.
            /// </summary>
            FILE_READ_ATTRIBUTES = 0x0080,

            /// <summary>
            /// The right to write attributes.
            /// </summary>
            FILE_WRITE_ATTRIBUTES = 0x0100,

            /// <summary>
            /// All standard and specific rights. [FILE_ALL_ACCESS]
            /// </summary>
            FILE_ALL_ACCESS = DELETE | READ_CONTROL | WRITE_DAC | WRITE_OWNER | 0x1FF,

            /// <summary>
            /// The right to delete the object.
            /// </summary>
            DELETE = 0x00010000,

            /// <summary>
            /// The right to read the information in the object's security descriptor.
            /// Doesn't include system access control list info (SACL).
            /// </summary>
            READ_CONTROL = 0x00020000,

            /// <summary>
            /// The right to modify the discretionary access control list (DACL) in the
            /// object's security descriptor.
            /// </summary>
            WRITE_DAC = 0x00040000,

            /// <summary>
            /// The right to change the owner in the object's security descriptor.
            /// </summary>
            WRITE_OWNER = 0x00080000,

            /// <summary>
            /// The right to use the object for synchronization. Enables a thread to wait until the object
            /// is in the signaled state. This is required if opening a synchronous handle.
            /// </summary>
            SYNCHRONIZE = 0x00100000,

            /// <summary>
            /// Same as READ_CONTROL.
            /// </summary>
            STANDARD_RIGHTS_READ = READ_CONTROL,

            /// <summary>
            /// Same as READ_CONTROL.
            /// </summary>
            STANDARD_RIGHTS_WRITE = READ_CONTROL,

            /// <summary>
            /// Same as READ_CONTROL.
            /// </summary>
            STANDARD_RIGHTS_EXECUTE = READ_CONTROL,

            /// <summary>
            /// Maps internally to <see cref="FILE_READ_ATTRIBUTES"/> | <see cref="FILE_READ_DATA"/> | <see cref="FILE_READ_EA"/>
            /// | <see cref="STANDARD_RIGHTS_READ"/> | <see cref="SYNCHRONIZE"/>.
            /// (For directories, <see cref="FILE_READ_ATTRIBUTES"/> | <see cref="FILE_LIST_DIRECTORY"/> | <see cref="FILE_READ_EA"/>
            /// | <see cref="STANDARD_RIGHTS_READ"/> | <see cref="SYNCHRONIZE"/>.)
            /// </summary>
            FILE_GENERIC_READ = 0x80000000, // GENERIC_READ

            /// <summary>
            /// Maps internally to <see cref="FILE_APPEND_DATA"/> | <see cref="FILE_WRITE_ATTRIBUTES"/> | <see cref="FILE_WRITE_DATA"/>
            /// | <see cref="FILE_WRITE_EA"/> | <see cref="STANDARD_RIGHTS_READ"/> | <see cref="SYNCHRONIZE"/>.
            /// (For directories, <see cref="FILE_ADD_SUBDIRECTORY"/> | <see cref="FILE_WRITE_ATTRIBUTES"/> | <see cref="FILE_ADD_FILE"/> AddFile
            /// | <see cref="FILE_WRITE_EA"/> | <see cref="STANDARD_RIGHTS_READ"/> | <see cref="SYNCHRONIZE"/>.)
            /// </summary>
            FILE_GENERIC_WRITE = 0x40000000, // GENERIC WRITE

            /// <summary>
            /// Maps internally to <see cref="FILE_EXECUTE"/> | <see cref="FILE_READ_ATTRIBUTES"/> | <see cref="STANDARD_RIGHTS_EXECUTE"/>
            /// | <see cref="SYNCHRONIZE"/>.
            /// (For directories, <see cref="FILE_DELETE_CHILD"/> | <see cref="FILE_READ_ATTRIBUTES"/> | <see cref="STANDARD_RIGHTS_EXECUTE"/>
            /// | <see cref="SYNCHRONIZE"/>.)
            /// </summary>
            FILE_GENERIC_EXECUTE = 0x20000000 // GENERIC_EXECUTE
        }

        // https://msdn.microsoft.com/en-us/library/bb432380.aspx
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff566424.aspx
        [DllImport(Libraries.NtDll, CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern NTSTATUS NtCreateFile(
            IntPtr* FileHandle,
            DesiredAccess DesiredAccess,
            OBJECT_ATTRIBUTES* ObjectAttributes,
            IO_STATUS_BLOCK* IoStatusBlock,
            System.Int64* AllocationSize,
            FileAttributes FileAttributes,
            FileShare ShareAccess,
            CreateDisposition CreateDisposition,
            CreateOptions CreateOptions,
            void* EaBuffer,
            System.UInt32 EaLength);

        public static (NTSTATUS status, IntPtr handle) CreateFile(
            ReadOnlySpan<char> path,
            IntPtr rootDirectory,
            CreateDisposition createDisposition,
            DesiredAccess desiredAccess = DesiredAccess.FILE_GENERIC_READ | DesiredAccess.SYNCHRONIZE,
            FileShare shareAccess = FileShare.ReadWrite | FileShare.Delete,
            FileAttributes fileAttributes = 0,
            CreateOptions createOptions = CreateOptions.FILE_SYNCHRONOUS_IO_NONALERT,
            ObjectAttributes objectAttributes = ObjectAttributes.OBJ_CASE_INSENSITIVE,
            void* eaBuffer = null,
            uint eaLength = 0)
        {
            UNICODE_STRING name = UNICODE_STRING.CreateFromSpan(path);

            OBJECT_ATTRIBUTES attributes = new(&name, objectAttributes, rootDirectory);

            System.IntPtr handle;
            IO_STATUS_BLOCK sbb;

            var status = NtCreateFile(
                &handle,
                desiredAccess,
                &attributes,
                &sbb,
                AllocationSize: null,
                fileAttributes,
                shareAccess,
                createDisposition,
                createOptions,
                eaBuffer,
                eaLength);

            // Even on NtCreateFile success or failure , the created string will always be successfully freed
            name.FreeCreatedString();

            return (status, handle);
        }

        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff556633.aspx
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff567047.aspx
        [DllImport(Libraries.NtDll , ExactSpelling = true , EntryPoint = "NtQueryDirectoryFile")]
        private static extern NTSTATUS NtQueryDirectoryFile_Native(
            IntPtr filehandle,
            IntPtr evnt,
            IntPtr ApcRoutine,
            IntPtr ApcContext,
            IO_STATUS_BLOCK* iostatus,
            void* fileinformation,
            System.UInt32 length,
            FILE_INFORMATION_CLASS fileinfoclass,
            BOOLEAN ReturnSingleEntry,
            UNICODE_STRING* filename,
            BOOLEAN RestartScan
        );

        public static NTSTATUS NtQueryDirectoryFile(
            IntPtr filehandle,
            IntPtr evnt,
            IntPtr ApcRoutine,
            IntPtr ApcContext,
            out IO_STATUS_BLOCK iostatus,
            IntPtr fileinformation,
            System.UInt32 length,
            FILE_INFORMATION_CLASS fileinfoclass,
            BOOLEAN ReturnSingleEntry,
            System.String filename,
            BOOLEAN RestartScan)
        {
            IO_STATUS_BLOCK blk;
            NTSTATUS nts;
            UNICODE_STRING pstr = default;
            if (filename is not null) {
                pstr = UNICODE_STRING.CreateFromString(filename);
            }
            nts = NtQueryDirectoryFile_Native(
                        filehandle,
                        evnt,
                        ApcRoutine,
                        ApcContext,
                        &blk,
                        fileinformation.ToPointer(),
                        length,
                        fileinfoclass,
                        ReturnSingleEntry,
                        filename is null ? null : &pstr,
                        RestartScan
                );
            // Even if being 'default', FreeCreatedString will elsewise bail out because on 'default' all fields are zeroes
            pstr.FreeCreatedString();
            iostatus = blk;
            return nts;
        }

        [DllImport(Libraries.NtDll, ExactSpelling = true)]
        public static extern System.UInt32 RtlNtStatusToDosError(NTSTATUS Status);

        [DllImport(Libraries.NtDll, ExactSpelling = true , EntryPoint = "NtQueryInformationFile")]
        private static extern NTSTATUS NtQueryInformationFile_Native(System.IntPtr hfe, IO_STATUS_BLOCK* blk, void* fileinfo, System.UInt32 length, FILE_INFORMATION_CLASS cls);

        [DllImport(Libraries.NtDll , ExactSpelling = true , EntryPoint = "NtSetInformationFile")]
        private static extern NTSTATUS NtSetInformationFile_Native(System.IntPtr hfe , IO_STATUS_BLOCK* blk , void* fileinfo , System.UInt32 length, FILE_INFORMATION_CLASS cls);

        [DllImport(Libraries.NtDll, ExactSpelling = true, EntryPoint = "NtReadFile")]
        private static extern NTSTATUS NtReadFile_Native(
            IntPtr filehandle,
            IntPtr eventtofireatend, // Optional
            IntPtr ApcRoutine, // Optional
            void* ApcContext, // Optional
            IO_STATUS_BLOCK* IoStatus,
            System.Byte* Buffer,
            System.UInt32 Length,
            System.Int64* ByteOffset,
            System.UInt32* Key // Unused, set this to NULL
        );

        [DllImport(Libraries.NtDll, ExactSpelling = true, EntryPoint = "NtWriteFile")]
        private static extern NTSTATUS NtWriteFile_Native(
            IntPtr filehandle,
            IntPtr eventtofireatend, // Optional
            IntPtr ApcRoutine, // Optional
            void* ApcContext, // Optional
            IO_STATUS_BLOCK* IoStatus,
            System.Byte* Buffer,
            System.UInt32 Length,
            System.Int64* ByteOffset,
            System.UInt32* Key // Unused, set this to NULL
        );

        public static NTSTATUS NtQueryInformationFile(System.IntPtr hfe , out FILE_MODE_INFORMATION options , out IO_STATUS_BLOCK stat)
        {
            IO_STATUS_BLOCK blk;
            FILE_MODE_INFORMATION opts;
            NTSTATUS status = NtQueryInformationFile_Native(hfe, &blk, &opts, sizeof(FILE_MODE_INFORMATION).ToUInt32(), FILE_INFORMATION_CLASS.FileModeInformation);
            stat = blk;
            options = opts;
            return status;
        }

        public static NTSTATUS NtQueryInformationFile(System.IntPtr hfe , out FILE_ACCESS_INFORMATION accessinfo , out IO_STATUS_BLOCK stat)
        {
            IO_STATUS_BLOCK blk;
            FILE_ACCESS_INFORMATION native;
            NTSTATUS status = NtQueryInformationFile_Native(hfe, &blk, &native, sizeof(FILE_ACCESS_INFORMATION).ToUInt32(), FILE_INFORMATION_CLASS.FileAccessInformation);
            stat = blk;
            accessinfo = native;
            return status;
        }
        
        public static NTSTATUS NtQueryInformationFile(System.IntPtr hfe , out FILE_ID_INFORMATION fileidinfo , out IO_STATUS_BLOCK stat)
        {
            IO_STATUS_BLOCK blk;
            FILE_ID_INFORMATION native;
            NTSTATUS status = NtQueryInformationFile_Native(hfe, &blk, &native, sizeof(FILE_ID_INFORMATION).ToUInt32(), FILE_INFORMATION_CLASS.FileIdInformation);
            stat = blk;
            fileidinfo = native;
            return status;
        }

        public static NTSTATUS NtQueryInformationFile(System.IntPtr hfe , out FILE_STANDARD_INFORMATION fsinfo , out IO_STATUS_BLOCK stat)
        {
            IO_STATUS_BLOCK blk;
            FILE_STANDARD_INFORMATION native;
            NTSTATUS status = NtQueryInformationFile_Native(hfe, &blk, &native, sizeof(FILE_STANDARD_INFORMATION).ToUInt32(), FILE_INFORMATION_CLASS.FileStandardInformation);
            stat = blk;
            fsinfo = native;
            return status;
        }

        public static NTSTATUS NtQueryInformationFile(System.IntPtr hfe , out FILE_BASIC_INFORMATION basicinf , out IO_STATUS_BLOCK stat)
        {
            IO_STATUS_BLOCK blk;
            FILE_BASIC_INFORMATION native;
            NTSTATUS status = NtQueryInformationFile_Native(hfe, &blk, &native, sizeof(FILE_BASIC_INFORMATION).ToUInt32(), FILE_INFORMATION_CLASS.FileBasicInformation);
            stat = blk;
            basicinf = native;
            return status;
        }
    
        public static NTSTATUS NtQueryInformationFile(System.IntPtr hfe , out FILE_POSITION_INFORMATION posinf , out IO_STATUS_BLOCK stat)
        {
            IO_STATUS_BLOCK blk;
            FILE_POSITION_INFORMATION native;
            NTSTATUS nts = NtQueryInformationFile_Native(hfe, &blk, &native, sizeof(FILE_POSITION_INFORMATION).ToUInt32(), FILE_INFORMATION_CLASS.FilePositionInformation);
            stat = blk;
            posinf = native;
            return nts;
        }

        public static NTSTATUS NtQueryInformationFile(System.IntPtr hfe , out System.String filename)
        {
            IO_STATUS_BLOCK blk;
            SafeLibcMemoryHandle mem = new(504);
            FILE_NAME_INFORMATION* pf = (FILE_NAME_INFORMATION*)mem.MemoryPointer;
            System.UInt32 lennow = 500;
            pf->FileNameLength = lennow;
            NTSTATUS nts;

        G_retry:
            nts = NtQueryInformationFile_Native(hfe, &blk, pf, mem.MemoryLength.ToUInt32(), FILE_INFORMATION_CLASS.FileNameInformation);

            switch (nts)
            {
                case NTSTATUS.STATUS_SUCCESS:
                    filename = FILE_NAME_INFORMATION.ReadName(pf);
                    mem.Dispose();
                    return NTSTATUS.STATUS_SUCCESS;
                case NTSTATUS.STATUS_BUFFER_OVERFLOW:
                    mem.Reallocate(mem.MemoryLength + 500);
                    pf = (FILE_NAME_INFORMATION*)mem.MemoryPointer;
                    lennow += 500;
                    pf->FileNameLength = lennow;
                    goto G_retry;
                default:
                    mem.Dispose();
                    filename = null;
                    return nts;
            }
        }

        public static NTSTATUS NtSetInformationFile(System.IntPtr hfe , FILE_BASIC_INFORMATION basic , out IO_STATUS_BLOCK stat)
        {
            IO_STATUS_BLOCK blk;
            NTSTATUS nts = NtSetInformationFile_Native(hfe, &blk, &basic, sizeof(FILE_BASIC_INFORMATION).ToUInt32(), FILE_INFORMATION_CLASS.FileBasicInformation);
            stat = blk;
            return nts;
        }

        public static NTSTATUS NtSetInformationFile(System.IntPtr hfe , FILE_POSITION_INFORMATION posinf, out IO_STATUS_BLOCK stat)
        {
            IO_STATUS_BLOCK blk;
            NTSTATUS nts = NtSetInformationFile_Native(hfe, &blk, &posinf, sizeof(FILE_POSITION_INFORMATION).ToUInt32(), FILE_INFORMATION_CLASS.FilePositionInformation);
            stat = blk;
            return nts;
        }

        public static NTSTATUS NtSetInformationFile(System.IntPtr hfe , FILE_END_OF_FILE_INFORMATION feof ,  out IO_STATUS_BLOCK stat)
        {
            IO_STATUS_BLOCK blk;
            NTSTATUS nts = NtSetInformationFile_Native(hfe, &blk, &feof, sizeof(FILE_END_OF_FILE_INFORMATION).ToUInt32(), FILE_INFORMATION_CLASS.FileEndOfFileInformation);
            stat = blk;
            return nts;
        }

        public static NTSTATUS NtSetInformationFile(System.IntPtr hfe , System.Boolean replaceifexisting , System.IntPtr root , System.String name , out IO_STATUS_BLOCK stat)
        {
            IO_STATUS_BLOCK blk;
            NTSTATUS nts;
            SafeLibcMemoryHandle temp = null;
            try {
                var v = RtlGetVersion().Version;
                if (v.Major >= 10 && v.Build >= 12000) {
                    // Then the newer FILE_RENAME_INFORMATION_NEW must be used
                    temp = FILE_RENAME_INFORMATION_NEW.GetRenameInformation(replaceifexisting , name , root);
                } else {
                    // Otherwise fall back to the older version of the structure
                    temp = FILE_RENAME_INFORMATION_OLD.GetRenameInformation(replaceifexisting, name, root);
                }
                nts = NtSetInformationFile_Native(hfe, &blk, temp.MemoryPointer, temp.MemoryLength.ToUInt32(), FILE_INFORMATION_CLASS.FileRenameInformation);
            } finally {
                temp?.Dispose();
                temp = null;
            }
            stat = blk;
            return nts;
        }

        public static NTSTATUS NtReadFile(System.IntPtr filehandle , System.IntPtr hevent , System.Byte* pBuffer , System.UInt32 length , out IO_STATUS_BLOCK stat)
        {
            IO_STATUS_BLOCK iosb;
            NTSTATUS nts = NtReadFile_Native(filehandle, hevent, IntPtr.Zero, null, &iosb, pBuffer, length, null, null);
            stat = iosb;
            return nts;
        }

        public static NTSTATUS NtWriteFile(System.IntPtr filehandle, System.IntPtr hevent, System.Byte* pBuffer, System.UInt32 length, out IO_STATUS_BLOCK stat)
        {
            IO_STATUS_BLOCK iosb;
            NTSTATUS nts = NtWriteFile_Native(filehandle, hevent, IntPtr.Zero, null, &iosb, pBuffer, length, null, null);
            stat = iosb;
            return nts;
        }
    }
}

