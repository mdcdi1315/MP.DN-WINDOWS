
using MP;
using System;
using MP.Annotations;
using MP.NativeInterop;
using MP.NativeInterop.Windows;
using System.Runtime.InteropServices;

partial class Interop
{
    unsafe partial class NtDll
    {
        public enum FS_INFORMATION_CLASS
        {
            FileFsVolumeInformation,
            FileFsLabelInformation,
            FileFsSizeInformation,
            FileFsDeviceInformation,
            FileFsAttributeInformation,
            FileFsControlInformation,
            FileFsFullSizeInformation,
            FileFsObjectIdInformation,
            FileFsDriverPathInformation,
            FileFsVolumeFlagsInformation,
            FileFsSectorSizeInformation,
            FileFsDataCopyInformation,
            FileFsMetadataSizeInformation,
            FileFsFullSizeInformationEx,
            FileFsGuidInformation,
            FileFsMaximumInformation
        }

        public enum DEVICE_TYPE
        {
            FILE_DEVICE_BEEP = 0x00000001,
            FILE_DEVICE_CD_ROM = 0x00000002,
            FILE_DEVICE_CD_ROM_FILE_SYSTEM = 0x00000003,
            FILE_DEVICE_CONTROLLER = 0x00000004,
            FILE_DEVICE_DATALINK = 0x00000005,
            FILE_DEVICE_DFS = 0x00000006,
            FILE_DEVICE_DISK = 0x00000007,
            FILE_DEVICE_DISK_FILE_SYSTEM = 0x00000008,
            FILE_DEVICE_FILE_SYSTEM = 0x00000009,
            FILE_DEVICE_INPORT_PORT = 0x0000000a,
            FILE_DEVICE_KEYBOARD = 0x0000000b,
            FILE_DEVICE_MAILSLOT = 0x0000000c,
            FILE_DEVICE_MIDI_IN = 0x0000000d,
            FILE_DEVICE_MIDI_OUT = 0x0000000e,
            FILE_DEVICE_MOUSE = 0x0000000f,
            FILE_DEVICE_MULTI_UNC_PROVIDER = 0x00000010,
            FILE_DEVICE_NAMED_PIPE = 0x00000011,
            FILE_DEVICE_NETWORK = 0x00000012,
            FILE_DEVICE_NETWORK_BROWSER = 0x00000013,
            FILE_DEVICE_NETWORK_FILE_SYSTEM = 0x00000014,
            FILE_DEVICE_NULL = 0x00000015,
            FILE_DEVICE_PARALLEL_PORT = 0x00000016,
            FILE_DEVICE_PHYSICAL_NETCARD = 0x00000017,
            FILE_DEVICE_PRINTER = 0x00000018,
            FILE_DEVICE_SCANNER = 0x00000019,
            FILE_DEVICE_SERIAL_MOUSE_PORT = 0x0000001a,
            FILE_DEVICE_SERIAL_PORT = 0x0000001b,
            FILE_DEVICE_SCREEN = 0x0000001c,
            FILE_DEVICE_SOUND = 0x0000001d,
            FILE_DEVICE_STREAMS = 0x0000001e,
            FILE_DEVICE_TAPE = 0x0000001f,
            FILE_DEVICE_TAPE_FILE_SYSTEM = 0x00000020,
            FILE_DEVICE_TRANSPORT = 0x00000021,
            FILE_DEVICE_UNKNOWN = 0x00000022,
            FILE_DEVICE_VIDEO = 0x00000023,
            FILE_DEVICE_VIRTUAL_DISK = 0x00000024,
            FILE_DEVICE_WAVE_IN = 0x00000025,
            FILE_DEVICE_WAVE_OUT = 0x00000026,
            FILE_DEVICE_8042_PORT = 0x00000027,
            FILE_DEVICE_NETWORK_REDIRECTOR = 0x00000028,
            FILE_DEVICE_BATTERY = 0x00000029,
            FILE_DEVICE_BUS_EXTENDER = 0x0000002a,
            FILE_DEVICE_MODEM = 0x0000002b,
            FILE_DEVICE_VDM = 0x0000002c,
            FILE_DEVICE_MASS_STORAGE = 0x0000002d,
            FILE_DEVICE_SMB = 0x0000002e,
            FILE_DEVICE_KS = 0x0000002f,
            FILE_DEVICE_CHANGER = 0x00000030,
            FILE_DEVICE_SMARTCARD = 0x00000031,
            FILE_DEVICE_ACPI = 0x00000032,
            FILE_DEVICE_DVD = 0x00000033,
            FILE_DEVICE_FULLSCREEN_VIDEO = 0x00000034,
            FILE_DEVICE_DFS_FILE_SYSTEM = 0x00000035,
            FILE_DEVICE_DFS_VOLUME = 0x00000036,
            FILE_DEVICE_SERENUM = 0x00000037,
            FILE_DEVICE_TERMSRV = 0x00000038,
            FILE_DEVICE_KSEC = 0x00000039,
            FILE_DEVICE_FIPS = 0x0000003A,
            FILE_DEVICE_INFINIBAND = 0x0000003B,
            FILE_DEVICE_VMBUS = 0x0000003E,
            FILE_DEVICE_CRYPT_PROVIDER = 0x0000003F,
            FILE_DEVICE_WPD = 0x00000040,
            FILE_DEVICE_BLUETOOTH = 0x00000041,
            FILE_DEVICE_MT_COMPOSITE = 0x00000042,
            FILE_DEVICE_MT_TRANSPORT = 0x00000043,
            FILE_DEVICE_BIOMETRIC = 0x00000044,
            FILE_DEVICE_PMI = 0x00000045,
            FILE_DEVICE_EHSTOR = 0x00000046,
            FILE_DEVICE_DEVAPI = 0x00000047,
            FILE_DEVICE_GPIO = 0x00000048,
            FILE_DEVICE_USBEX = 0x00000049,
            FILE_DEVICE_CONSOLE = 0x00000050,
            FILE_DEVICE_NFP = 0x00000051,
            FILE_DEVICE_SYSENV = 0x00000052,
            FILE_DEVICE_VIRTUAL_BLOCK = 0x00000053,
            FILE_DEVICE_POINT_OF_SERVICE = 0x00000054,
            FILE_DEVICE_STORAGE_REPLICATION = 0x00000055,
            FILE_DEVICE_TRUST_ENV = 0x00000056,
            FILE_DEVICE_UCM = 0x00000057,
            FILE_DEVICE_UCMTCPCI = 0x00000058,
            FILE_DEVICE_PERSISTENT_MEMORY = 0x00000059 ,
            FILE_DEVICE_NVDIMM = 0x0000005a,
            FILE_DEVICE_HOLOGRAPHIC = 0x0000005b,
            FILE_DEVICE_SDFXHCI = 0x0000005c,
            FILE_DEVICE_UCMUCSI = 0x0000005d,
            FILE_DEVICE_PRM = 0x0000005e,
            FILE_DEVICE_EVENT_COLLECTOR = 0x0000005f,
            FILE_DEVICE_USB4 = 0x00000060,
            FILE_DEVICE_SOUNDWIRE = 0x00000061,
            FILE_DEVICE_FABRIC_NVME = 0x00000062,
            FILE_DEVICE_SVM = 0x00000063,
            FILE_DEVICE_HARDWARE_ACCELERATOR = 0x00000064,
            FILE_DEVICE_I3C = 0x00000065,
        }

        /// <summary>
        /// Are analytically documented at <see href="https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntifs/ns-ntifs-_file_fs_attribute_information"/>.
        /// </summary>
        [Flags]
        public enum FileSystemAttributes : System.UInt32
        {
            FILE_CASE_SENSITIVE_SEARCH = 0x00000001,
            FILE_CASE_PRESERVED_NAMES = 0x00000002,
            FILE_UNICODE_ON_DISK = 0x00000004,
            FILE_PERSISTENT_ACLS = 0x00000008,
            FILE_FILE_COMPRESSION = 0x00000010,
            FILE_VOLUME_QUOTAS = 0x00000020,
            FILE_SUPPORTS_SPARSE_FILES = 0x00000040,
            FILE_SUPPORTS_REPARSE_POINTS = 0x00000080,
            FILE_SUPPORTS_REMOTE_STORAGE = 0x00000100,
            FILE_RETURNS_CLEANUP_RESULT_INFO = 0x00000200,
            FILE_SUPPORTS_POSIX_UNLINK_RENAME = 0x00000400,
            FILE_VOLUME_IS_COMPRESSED = 0x00008000,
            FILE_SUPPORTS_OBJECT_IDS = 0x00010000,
            FILE_SUPPORTS_ENCRYPTION = 0x00020000,
            FILE_NAMED_STREAMS = 0x00040000,
            FILE_READ_ONLY_VOLUME = 0x00080000,
            FILE_SEQUENTIAL_WRITE_ONCE = 0x00100000,
            FILE_SUPPORTS_TRANSACTIONS = 0x00200000,
            FILE_SUPPORTS_HARD_LINKS = 0x00400000,
            FILE_SUPPORTS_EXTENDED_ATTRIBUTES = 0x00800000,
            FILE_SUPPORTS_OPEN_BY_FILE_ID = 0x01000000,
            FILE_SUPPORTS_USN_JOURNAL = 0x02000000,
            FILE_SUPPORTS_INTEGRITY_STREAMS = 0x04000000,
            FILE_SUPPORTS_BLOCK_REFCOUNTING = 0x08000000,
            FILE_SUPPORTS_SPARSE_VDL = 0x10000000,
            FILE_DAX_VOLUME = 0x20000000,
            FILE_SUPPORTS_GHOSTING = 0x40000000
        }

        // Must be aligned on a 8-byte boundary.
        [StructLayout(LayoutKind.Explicit)]
        public struct FILE_FS_VOLUME_INFORMATION
        {
            [FieldOffset(0)]
            public LongFileTime VolumeCreationTime; // System.Int64

            [FieldOffset(sizeof(System.Int64))]
            public System.UInt32 VolumeSerialNumber;

            [FieldOffset(sizeof(System.Int64) + sizeof(System.UInt32))]
            public System.UInt32 VolumeLabelLength;

            [FieldOffset(sizeof(System.Int64) + (sizeof(System.UInt32) * 2))]
            public BOOLEAN SupportsObjects;

            [FieldOffset(sizeof(System.Int64) + (sizeof(System.UInt32) * 2) + sizeof(BOOLEAN))]
            public fixed System.Char VolumeLabelReference[1];
        }

        // Must be aligned on a 8-byte boundary.
        [StructLayout(LayoutKind.Explicit)]
        public struct FILE_FS_SIZE_INFORMATION
        {
            [FieldOffset(0)]
            public System.Int64 TotalAllocationUnits;
            [FieldOffset(sizeof(System.Int64))]
            public System.Int64 AvailableAllocationUnits;
            [FieldOffset(sizeof(System.Int64) * 2)]
            public System.UInt32 SectorsPerAllocationUnit;
            [FieldOffset((sizeof(System.Int64) * 2) + sizeof(System.UInt32))]
            public System.UInt32 BytesPerSector;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct FILE_FS_DEVICE_INFORMATION
        {
            [FieldOffset(0)]
            public DEVICE_TYPE DeviceType;
            [FieldOffset(sizeof(DEVICE_TYPE))]
            public System.UInt32 Characteristics;
        }

        // Must be aligned on a 4-byte boundary.
        [StructLayout(LayoutKind.Explicit)]
        public struct FILE_FS_ATTRIBUTE_INFORMATION
        {
            [FieldOffset(0)]
            public FileSystemAttributes Attributes;
            [FieldOffset(sizeof(FileSystemAttributes))]
            public System.Int32 MaximumComponentNameLength;
            [FieldOffset(sizeof(FileSystemAttributes) + sizeof(System.Int32))]
            public System.UInt32 FileSystemNameLength;
            [FieldOffset(sizeof(FileSystemAttributes) + sizeof(System.UInt32) + sizeof(System.Int32))]
            public fixed System.Char FileSystemName[1];
        }

        // Must be aligned on a 8-byte boundary.
        [StructLayout(LayoutKind.Explicit)]
        public struct FILE_FS_OBJECTID_INFORMATION
        {
            [FieldOffset(0)]
            public GUID ObjectId;
            [FieldOffset(16)]
            public fixed System.Byte ExtendedInfo[48];
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct FILE_FS_GUID_INFORMATION
        {
            [FieldOffset(0)]
            public GUID FsGuid;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct FILE_FS_DRIVER_PATH_INFORMATION
        {
            [FieldOffset(0)]
            public BOOLEAN DriverInPath;

            [FieldOffset(sizeof(BOOLEAN))]
            public System.UInt32 DriverNameLength;

            [FieldOffset(sizeof(BOOLEAN) + sizeof(System.UInt32))]
            public fixed System.Char DriverName[1];
        }

        [DllImport(Libraries.NtDll, EntryPoint = "NtQueryVolumeInformationFile", ExactSpelling = true)]
        private static extern NTSTATUS NtQueryVolumeInformationFile_Native(
            System.IntPtr FileHandle,
            IO_STATUS_BLOCK* IoStatusBlock,
            void* FsInformation,
            System.UInt32 Length,
            FS_INFORMATION_CLASS FsInformationClass
        );

        [RequiresNativeLayer]
        public static NTSTATUS NtQueryVolumeInformationFile(System.IntPtr file_handle, out FILE_FS_VOLUME_INFORMATION fsvi, out System.String volume_label, out IO_STATUS_BLOCK block)
        {
            IO_STATUS_BLOCK io;
            uint fsv_size = sizeof(FILE_FS_VOLUME_INFORMATION).ToUInt32();
            uint actual_size = fsv_size + 64;
            FILE_FS_VOLUME_INFORMATION* mh = (FILE_FS_VOLUME_INFORMATION*)SystemInfo.GetDefaultMemoryManager().AllocateAligned(actual_size, 8U);
            try {
            g_begin:
                NTSTATUS nts = NtQueryVolumeInformationFile_Native(file_handle, &io, mh, actual_size, FS_INFORMATION_CLASS.FileFsVolumeInformation);
                fsvi = *mh;
                block = io;
                if (nts == NTSTATUS.STATUS_SUCCESS) {
                    if (fsvi.VolumeLabelLength > actual_size - fsv_size) {
                        SystemInfo.GetDefaultMemoryManager().FreeAligned(mh);
                        actual_size += fsvi.VolumeLabelLength;
                        mh = (FILE_FS_VOLUME_INFORMATION*)SystemInfo.GetDefaultMemoryManager().AllocateAligned(actual_size, 8U);
                        goto g_begin;
                    } else {
                        volume_label = new(mh->VolumeLabelReference, 0, (mh->VolumeLabelLength / sizeof(System.Char)).ToInt32());
                    }
                } else {
                    volume_label = null;
                }
                return nts;
            } finally {
                SystemInfo.GetDefaultMemoryManager().FreeAligned(mh);
            }
        }

        [RequiresNativeLayer]
        public static NTSTATUS NtQueryVolumeInformationFile(System.IntPtr file_handle, out FILE_FS_SIZE_INFORMATION fsvi, out IO_STATUS_BLOCK block)
        {
            int size = sizeof(FILE_FS_SIZE_INFORMATION);
            FILE_FS_SIZE_INFORMATION* pff = (FILE_FS_SIZE_INFORMATION*)SystemInfo.GetDefaultMemoryManager().AllocateAligned(size, 8);
            try {
                IO_STATUS_BLOCK io;
                NTSTATUS nts = NtQueryVolumeInformationFile_Native(file_handle, &io, pff, size.ToUInt32(), FS_INFORMATION_CLASS.FileFsSizeInformation);
                block = io;
                fsvi = *pff;
                return nts;
            } finally {
                SystemInfo.GetDefaultMemoryManager().FreeAligned(pff);
            }
        }

        public static NTSTATUS NtQueryVolumeInformationFile(System.IntPtr file_handle, out FILE_FS_DEVICE_INFORMATION fsdv, out IO_STATUS_BLOCK block)
        {
            IO_STATUS_BLOCK io;
            FILE_FS_DEVICE_INFORMATION fd;
            NTSTATUS nts = NtQueryVolumeInformationFile_Native(file_handle, &io, &fd, sizeof(FILE_FS_DEVICE_INFORMATION).ToUInt32(), FS_INFORMATION_CLASS.FileFsDeviceInformation);
            block = io;
            fsdv = fd;
            return nts;
        }

        public static NTSTATUS NtQueryVolumeInformationFile(System.IntPtr file_handle, out FILE_FS_GUID_INFORMATION fsg,  out IO_STATUS_BLOCK block)
        {
            IO_STATUS_BLOCK io;
            FILE_FS_GUID_INFORMATION fd;
            NTSTATUS nts = NtQueryVolumeInformationFile_Native(file_handle, &io, &fd, 16U, FS_INFORMATION_CLASS.FileFsGuidInformation);
            block = io;
            fsg = fd;
            return nts;
        }

        [RequiresNativeLayer]
        public static NTSTATUS NtQueryVolumeInformationFile(System.IntPtr file_handle, out FILE_FS_ATTRIBUTE_INFORMATION fsai, out System.String fs_name, out IO_STATUS_BLOCK block)
        {
            IO_STATUS_BLOCK io;
            uint fsa_size = sizeof(FILE_FS_ATTRIBUTE_INFORMATION).ToUInt32();
            uint actual_size = fsa_size + 64U;
            FILE_FS_ATTRIBUTE_INFORMATION* mh = (FILE_FS_ATTRIBUTE_INFORMATION*)SystemInfo.GetDefaultMemoryManager().AllocateAligned(actual_size, 4U);
            try {
            g_begin:
                NTSTATUS nts = NtQueryVolumeInformationFile_Native(file_handle, &io, mh, actual_size, FS_INFORMATION_CLASS.FileFsAttributeInformation);
                fsai = *mh;
                block = io;
                if (nts == NTSTATUS.STATUS_SUCCESS) {
                    if (fsai.FileSystemNameLength > actual_size - fsa_size) {
                        SystemInfo.GetDefaultMemoryManager().FreeAligned(mh);
                        actual_size += fsai.FileSystemNameLength;
                        mh = (FILE_FS_ATTRIBUTE_INFORMATION*)SystemInfo.GetDefaultMemoryManager().AllocateAligned(actual_size, 4U);
                        goto g_begin;
                    } else {
                        fs_name = new(mh->FileSystemName, 0, (mh->FileSystemNameLength / sizeof(System.Char)).ToInt32());
                    }
                } else {
                    fs_name = null;
                }
                return nts;
            } finally {
                SystemInfo.GetDefaultMemoryManager().FreeAligned(mh);
            }
        }

        [RequiresNativeLayer]
        public static NTSTATUS NtQueryVolumeInformationFile(System.IntPtr file_handle, out FILE_FS_OBJECTID_INFORMATION fso, out IO_STATUS_BLOCK block)
        {
            FILE_FS_OBJECTID_INFORMATION* pgi = (FILE_FS_OBJECTID_INFORMATION*)SystemInfo.GetDefaultMemoryManager().AllocateAligned(sizeof(FILE_FS_OBJECTID_INFORMATION), 8);
            try {
                IO_STATUS_BLOCK io;
                NTSTATUS nts = NtQueryVolumeInformationFile_Native(file_handle, &io, pgi, sizeof(FILE_FS_OBJECTID_INFORMATION).ToUInt32(), FS_INFORMATION_CLASS.FileFsObjectIdInformation);
                block = io;
                fso = *pgi;
                return nts;
            } finally {
                SystemInfo.GetDefaultMemoryManager().FreeAligned(pgi);
            }
        }

        [RequiresNativeLayer]
        public static NTSTATUS NtQueryVolumeInformationFile(System.IntPtr file_handle, out FILE_FS_DRIVER_PATH_INFORMATION fsdi, out System.String driver_name, out IO_STATUS_BLOCK block)
        {
            uint s_size = sizeof(FILE_FS_DRIVER_PATH_INFORMATION).ToUInt32();
            uint allocated_size = s_size + 64U;
            FILE_FS_DRIVER_PATH_INFORMATION* pi = (FILE_FS_DRIVER_PATH_INFORMATION*)SystemInfo.GetDefaultMemoryManager().AllocateAligned(allocated_size, 8U);
            NTSTATUS nts;
            IO_STATUS_BLOCK io;
            try {
            g_begin:
                nts = NtQueryVolumeInformationFile_Native(file_handle, &io, &pi, allocated_size, FS_INFORMATION_CLASS.FileFsDriverPathInformation);
                if (pi->DriverNameLength > allocated_size - s_size) {
                    allocated_size += pi->DriverNameLength;
                    SystemInfo.GetDefaultMemoryManager().FreeAligned(pi);
                    pi = (FILE_FS_DRIVER_PATH_INFORMATION*)SystemInfo.GetDefaultMemoryManager().AllocateAligned(allocated_size, 8U);
                    goto g_begin;
                } else {
                    fsdi = *pi;
                    driver_name = new(pi->DriverName, 0, (pi->DriverNameLength / sizeof(System.Char)).ToInt32());
                }
            } finally {
                SystemInfo.GetDefaultMemoryManager().FreeAligned(pi);
            }
            block = io;
            return nts;
        }
    }
}