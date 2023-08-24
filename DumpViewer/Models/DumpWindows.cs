using System;
using System.Linq;
using System.Windows;
using DumpViewer.Services.DumpService;

namespace DumpViewer.Models
{
    /// <summary>
    /// Содержит информацию заголовка для файла минидампа
    /// </summary>
    public class DumpWindows
    {
        private DumpStreamService _stream;
        private byte[] _signature;
        private byte[] _validDump;
        private uint _majorVersion;
        private uint _minorVersion;
        private ulong _directoryTableBase;
        private ulong _pfnDataBase;
        private ulong _psLoadedModuleList;
        private ulong _psActiveProcessHead;
        private uint _numberProcessors;
        private uint _versionArchitecture;
        private uint[] _bugCheckParameters;
        private MachineImageTypeList _machineImageType;
        private BugCheckCodeList _bugCheckCode;
        /// <summary>
        /// Подпись
        /// </summary>
        public byte[] Signature { get { return _signature; } }
        /// <summary>
        /// Проверка дампа
        /// </summary>
        public byte[] ValidDump { get { return _validDump; } }
        /// <summary>
        /// Мажорная версия системы, на которой произошел сбой
        /// </summary>
        public uint MajorVersion { get { return _majorVersion; } }
        /// <summary>
        /// Минорная версия системы, на которой произошел сбой
        /// </summary>
        public uint MinorVersion { get { return _minorVersion; } }
        /// <summary>
        /// Значение CR3 на момент сбоя системы - физический адрес каталога страниц
        /// </summary>
        public ulong DirectoryTableBase { get { return _directoryTableBase; } }
        /// <summary>
        /// Виртуальный адрес страницы физической памяти, который содержит информацию о ее состоянии
        /// </summary>
        public ulong PfnDataBase { get { return _pfnDataBase; } }
        /// <summary>
        /// Виртуальный адрес, указывающий на список загружаемых модулей
        /// </summary>
        public ulong PsLoadedModuleList { get { return _psLoadedModuleList; } }
        /// <summary>
        /// Виртуальный адрес, указывающий на список активных процессов
        /// </summary>
        public ulong PsActiveProcessHead { get { return _psActiveProcessHead; } }
        /// <summary>
        /// Версия архитектуры (32 или 64)
        /// </summary>
        public uint VersionArchitecture { get { return _versionArchitecture; } }
        /// <summary>
        /// Тип архитектуры процессора
        /// </summary>
        public MachineImageTypeList MachineImageType { get { return _machineImageType; } }
        /// <summary>
        /// Количество процессоров в системе
        /// </summary>
        public ulong NumberProcessors { get { return _numberProcessors; } }
        /// <summary>
        /// Код ошибки
        /// </summary>
        public BugCheckCodeList BugCheckCode { get { return _bugCheckCode; } }
        /// <summary>
        /// Параметры ошибки
        /// </summary>
        public uint[] BugCheckParameters { get { return _bugCheckParameters; } }
        public static DumpWindows FromFile(string fileName) => new(new DumpStreamService(fileName));
        /// <summary>
        /// Список типов архитектуры процессора
        /// </summary>
        public enum MachineImageTypeList : uint
        {
            IMAGE_FILE_MACHINE_UNKNOWN = 0x0,
            IMAGE_FILE_MACHINE_ALPHA = 0x184,
            IMAGE_FILE_MACHINE_ALPHA64 = 0x284,
            IMAGE_FILE_MACHINE_AM33 = 0x1d3,
            IMAGE_FILE_MACHINE_AMD64 = 0x8664,
            IMAGE_FILE_MACHINE_ARM = 0x1c0,
            IMAGE_FILE_MACHINE_ARM64 = 0xaa64,
            IMAGE_FILE_MACHINE_ARMNT = 0x1c4,
            IMAGE_FILE_MACHINE_AXP64 = 0x284,
            IMAGE_FILE_MACHINE_EBC = 0xebc,
            IMAGE_FILE_MACHINE_I386 = 0x014c,
            IMAGE_FILE_MACHINE_IA64 = 0x0200,
            IMAGE_FILE_MACHINE_LOONGARCH32 = 0x6232,
            IMAGE_FILE_MACHINE_LOONGARCH64 = 0x6264,
            IMAGE_FILE_MACHINE_M32R = 0x9041,
            IMAGE_FILE_MACHINE_MIPS16 = 0x266,
            IMAGE_FILE_MACHINE_MIPSFPU = 0x366,
            IMAGE_FILE_MACHINE_MIPSFPU16 = 0x466,
            IMAGE_FILE_MACHINE_POWERPC = 0x1f0,
            IMAGE_FILE_MACHINE_POWERPCFP = 0x1f1,
            IMAGE_FILE_MACHINE_R4000 = 0x166,
            IMAGE_FILE_MACHINE_RISCV32 = 0x5032,
            IMAGE_FILE_MACHINE_RISCV64 = 0x5064,
            IMAGE_FILE_MACHINE_RISCV128 = 0x5128,
            IMAGE_FILE_MACHINE_SH3 = 0x1a2,
            IMAGE_FILE_MACHINE_SH3DSP = 0x1a3,
            IMAGE_FILE_MACHINE_SH4 = 0x1a6,
            IMAGE_FILE_MACHINE_SH5 = 0x1a8,
            IMAGE_FILE_MACHINE_THUMB = 0x1c2,
            IMAGE_FILE_MACHINE_WCEMIPSV2 = 0x169,
        }
        /// <summary>
        /// Список ошибок
        /// </summary>
        public enum BugCheckCodeList : uint
        {
            APC_INDEX_MISMATCH = 0x00000001,
            DEVICE_QUEUE_NOT_BUSY = 0x00000002,
            INVALID_AFFINITY_SET = 0x00000003,
            INVALID_DATA_ACCESS_TRAP = 0x00000004,
            INVALID_PROCESS_ATTACH_ATTEMPT = 0x00000005,
            INVALID_PROCESS_DETACH_ATTEMPT = 0x00000006,
            INVALID_SOFTWARE_INTERRUPT = 0x00000007,
            IRQL_NOT_DISPATCH_LEVEL = 0x00000008,
            IRQL_NOT_GREATER_OR_EQUAL = 0x00000009,
            IRQL_NOT_LESS_OR_EQUAL = 0x0000000A,
            NO_EXCEPTION_HANDLING_SUPPORT = 0x0000000B,
            MAXIMUM_WAIT_OBJECTS_EXCEEDED = 0x0000000C,
            MUTEX_LEVEL_NUMBER_VIOLATION = 0x0000000D,
            NO_USER_MODE_CONTEXT = 0x0000000E,
            SPIN_LOCK_ALREADY_OWNED = 0x0000000F,
            SPIN_LOCK_NOT_OWNED = 0x00000010,
            THREAD_NOT_MUTEX_OWNER = 0x00000011,
            TRAP_CAUSE_UNKNOWN = 0x00000012,
            EMPTY_THREAD_REAPER_LIST = 0x00000013,
            CREATE_DELETE_LOCK_NOT_LOCKED = 0x00000014,
            LAST_CHANCE_CALLED_FROM_KMODE = 0x00000015,
            CID_HANDLE_CREATION = 0x00000016,
            CID_HANDLE_DELETION = 0x00000017,
            REFERENCE_BY_POINTER = 0x00000018,
            BAD_POOL_HEADER = 0x00000019,
            MEMORY_MANAGEMENT = 0x0000001A,
            PFN_SHARE_COUNT = 0x0000001B,
            PFN_REFERENCE_COUNT = 0x0000001C,
            NO_SPIN_LOCK_AVAILABLE = 0x0000001D,
            KMODE_EXCEPTION_NOT_HANDLED = 0x0000001E,
            SHARED_RESOURCE_CONV_ERROR = 0x0000001F,
            KERNEL_APC_PENDING_DURING_EXIT = 0x00000020,
            QUOTA_UNDERFLOW = 0x00000021,
            FILE_SYSTEM = 0x00000022,
            FAT_FILE_SYSTEM = 0x00000023,
            NTFS_FILE_SYSTEM = 0x00000024,
            NPFS_FILE_SYSTEM = 0x00000025,
            CDFS_FILE_SYSTEM = 0x00000026,
            RDR_FILE_SYSTEM = 0x00000027,
            CORRUPT_ACCESS_TOKEN = 0x00000028,
            SECURITY_SYSTEM = 0x00000029,
            INCONSISTENT_IRP = 0x0000002A,
            PANIC_STACK_SWITCH = 0x0000002B,
            PORT_DRIVER_INTERNAL = 0x0000002C,
            SCSI_DISK_DRIVER_INTERNAL = 0x0000002D,
            DATA_BUS_ERROR = 0x0000002E,
            INSTRUCTION_BUS_ERROR = 0x0000002F,
            SET_OF_INVALID_CONTEXT = 0x00000030,
            PHASE0_INITIALIZATION_FAILED = 0x00000031,
            PHASE1_INITIALIZATION_FAILED = 0x00000032,
            UNEXPECTED_INITIALIZATION_CALL = 0x00000033,
            CACHE_MANAGER = 0x00000034,
            NO_MORE_IRP_STACK_LOCATIONS = 0x00000035,
            DEVICE_REFERENCE_COUNT_NOT_ZERO = 0x00000036,
            FLOPPY_INTERNAL_ERROR = 0x00000037,
            SERIAL_DRIVER_INTERNAL = 0x00000038,
            SYSTEM_EXIT_OWNED_MUTEX = 0x00000039,
            SYSTEM_UNWIND_PREVIOUS_USER = 0x0000003A,
            SYSTEM_SERVICE_EXCEPTION = 0x0000003B,
            INTERRUPT_UNWIND_ATTEMPTED = 0x0000003C,
            INTERRUPT_EXCEPTION_NOT_HANDLED = 0x0000003D,
            MULTIPROCESSOR_CONFIGURATION_NOT_SUPPORTED = 0x0000003E,
            NO_MORE_SYSTEM_PTES = 0x0000003F,
            TARGET_MDL_TOO_SMALL = 0x00000040,
            MUST_SUCCEED_POOL_EMPTY = 0x00000041,
            ATDISK_DRIVER_INTERNAL = 0x00000042,
            NO_SUCH_PARTITION = 0x00000043,
            MULTIPLE_IRP_COMPLETE_REQUESTS = 0x00000044,
            INSUFFICIENT_SYSTEM_MAP_REGS = 0x00000045,
            DEREF_UNKNOWN_LOGON_SESSION = 0x00000046,
            REF_UNKNOWN_LOGON_SESSION = 0x00000047,
            CANCEL_STATE_IN_COMPLETED_IRP = 0x00000048,
            PAGE_FAULT_WITH_INTERRUPTS_OFF = 0x00000049,
            IRQL_GT_ZERO_AT_SYSTEM_SERVICE = 0x0000004A,
            STREAMS_INTERNAL_ERROR = 0x0000004B,
            FATAL_UNHANDLED_HARD_ERROR = 0x0000004C,
            NO_PAGES_AVAILABLE = 0x0000004D,
            PFN_LIST_CORRUPT = 0x0000004E,
            NDIS_INTERNAL_ERROR = 0x0000004F,
            PAGE_FAULT_IN_NONPAGED_AREA = 0x00000050,
            REGISTRY_ERROR = 0x00000051,
            MAILSLOT_FILE_SYSTEM = 0x00000052,
            NO_BOOT_DEVICE = 0x00000053,
            LM_SERVER_INTERNAL_ERROR = 0x00000054,
            DATA_COHERENCY_EXCEPTION = 0x00000055,
            INSTRUCTION_COHERENCY_EXCEPTION = 0x00000056,
            XNS_INTERNAL_ERROR = 0x00000057,
            FTDISK_INTERNAL_ERROR = 0x00000058,
            PINBALL_FILE_SYSTEM = 0x00000059,
            CRITICAL_SERVICE_FAILED = 0x0000005A,
            SET_ENV_VAR_FAILED = 0x0000005B,
            HAL_INITIALIZATION_FAILED = 0x0000005C,
            UNSUPPORTED_PROCESSOR = 0x0000005D,
            OBJECT_INITIALIZATION_FAILED = 0x0000005E,
            SECURITY_INITIALIZATION_FAILED = 0x0000005F,
            PROCESS_INITIALIZATION_FAILED = 0x00000060,
            HAL1_INITIALIZATION_FAILED = 0x00000061,
            OBJECT1_INITIALIZATION_FAILED = 0x00000062,
            SECURITY1_INITIALIZATION_FAILED = 0x00000063,
            SYMBOLIC_INITIALIZATION_FAILED = 0x00000064,
            MEMORY1_INITIALIZATION_FAILED = 0x00000065,
            CACHE_INITIALIZATION_FAILED = 0x00000066,
            CONFIG_INITIALIZATION_FAILED = 0x00000067,
            FILE_INITIALIZATION_FAILED = 0x00000068,
            IO1_INITIALIZATION_FAILED = 0x00000069,
            LPC_INITIALIZATION_FAILED = 0x0000006A,
            PROCESS1_INITIALIZATION_FAILED = 0x0000006B,
            REFMON_INITIALIZATION_FAILED = 0x0000006C,
            SESSION1_INITIALIZATION_FAILED = 0x0000006D,
            SESSION2_INITIALIZATION_FAILED = 0x0000006E,
            SESSION3_INITIALIZATION_FAILED = 0x0000006F,
            SESSION4_INITIALIZATION_FAILED = 0x00000070,
            SESSION5_INITIALIZATION_FAILED = 0x00000071,
            ASSIGN_DRIVE_LETTERS_FAILED = 0x00000072,
            CONFIG_LIST_FAILED = 0x00000073,
            BAD_SYSTEM_CONFIG_INFO = 0x00000074,
            CANNOT_WRITE_CONFIGURATION = 0x00000075,
            PROCESS_HAS_LOCKED_PAGES = 0x00000076,
            KERNEL_STACK_INPAGE_ERROR = 0x00000077,
            PHASE0_EXCEPTION = 0x00000078,
            MISMATCHED_HAL = 0x00000079,
            KERNEL_DATA_INPAGE_ERROR = 0x0000007A,
            INACCESSIBLE_BOOT_DEVICE = 0x0000007B,
            BUGCODE_NDIS_DRIVER = 0x0000007C,
            INSTALL_MORE_MEMORY = 0x0000007D,
            SYSTEM_THREAD_EXCEPTION_NOT_HANDLED = 0x0000007E,
            UNEXPECTED_KERNEL_MODE_TRAP = 0x0000007F,
            NMI_HARDWARE_FAILURE = 0x00000080,
            SPIN_LOCK_INIT_FAILURE = 0x00000081,
            DFS_FILE_SYSTEM = 0x00000082,
            SETUP_FAILURE = 0x00000085,
            MBR_CHECKSUM_MISMATCH = 0x0000008B,
            KERNEL_MODE_EXCEPTION_NOT_HANDLED = 0x0000008E,
            PP0_INITIALIZATION_FAILED = 0x0000008F,
            PP1_INITIALIZATION_FAILED = 0x00000090,
            WIN32K_INIT_OR_RIT_FAILURE = 0x00000091,
            UP_DRIVER_ON_MP_SYSTEM = 0x00000092,
            INVALID_KERNEL_HANDLE = 0x00000093,
            KERNEL_STACK_LOCKED_AT_EXIT = 0x00000094,
            PNP_INTERNAL_ERROR = 0x00000095,
            INVALID_WORK_QUEUE_ITEM = 0x00000096,
            BOUND_IMAGE_UNSUPPORTED = 0x00000097,
            END_OF_NT_EVALUATION_PERIOD = 0x00000098,
            INVALID_REGION_OR_SEGMENT = 0x00000099,
            SYSTEM_LICENSE_VIOLATION = 0x0000009A,
            UDFS_FILE_SYSTEM = 0x0000009B,
            MACHINE_CHECK_EXCEPTION = 0x0000009C,
            USER_MODE_HEALTH_MONITOR = 0x0000009E,
            DRIVER_POWER_STATE_FAILURE = 0x0000009F,
            INTERNAL_POWER_ERROR = 0x000000A0,
            PCI_BUS_DRIVER_INTERNAL = 0x000000A1,
            MEMORY_IMAGE_CORRUPT = 0x000000A2,
            ACPI_DRIVER_INTERNAL = 0x000000A3,
            CNSS_FILE_SYSTEM_FILTER = 0x000000A4,
            ACPI_BIOS_ERROR = 0x000000A5,
            FP_EMULATION_ERROR = 0x000000A6,
            BAD_EXHANDLE = 0x000000A7,
            BOOTING_IN_SAFEMODE_MINIMAL = 0x000000A8,
            BOOTING_IN_SAFEMODE_NETWORK = 0x000000A9,
            BOOTING_IN_SAFEMODE_DSREPAIR = 0x000000AA,
            SESSION_HAS_VALID_POOL_ON_EXIT = 0x000000AB,
            HAL_MEMORY_ALLOCATION = 0x000000AC,
            VIDEO_DRIVER_DEBUG_REPORT_REQUEST = 0x000000AD,
            VIDEO_DRIVER_INIT_FAILURE = 0x000000B4,
            ATTEMPTED_SWITCH_FROM_DPC = 0x000000B8,
            CHIPSET_DETECTED_ERROR = 0x000000B9,
            SESSION_HAS_VALID_VIEWS_ON_EXIT = 0x000000BA,
            NETWORK_BOOT_INITIALIZATION_FAILED = 0x000000BB,
            NETWORK_BOOT_DUPLICATE_ADDRESS = 0x000000BC,
            ATTEMPTED_WRITE_TO_READONLY_MEMORY = 0x000000BE,
            MUTEX_ALREADY_OWNED = 0x000000BF,
            SPECIAL_POOL_DETECTED_MEMORY_CORRUPTION = 0x000000C1,
            BAD_POOL_CALLER = 0x000000C2,
            ERROR_INVALID_MINALLOCSIZE = 0x000000C3,
            DRIVER_VERIFIER_DETECTED_VIOLATION = 0x000000C4,
            DRIVER_CORRUPTED_EXPOOL = 0x000000C5,
            DRIVER_CAUGHT_MODIFYING_FREED_POOL = 0x000000C6,
            TIMER_OR_DPC_INVALID = 0x000000C7,
            IRQL_UNEXPECTED_VALUE = 0x000000C8,
            DRIVER_VERIFIER_IOMANAGER_VIOLATION = 0x000000C9,
            PNP_DETECTED_FATAL_ERROR = 0x000000CA,
            DRIVER_LEFT_LOCKED_PAGES_IN_PROCESS = 0x000000CB,
            PAGE_FAULT_IN_FREED_SPECIAL_POOL = 0x000000CC,
            PAGE_FAULT_BEYOND_END_OF_ALLOCATION = 0x000000CD,
            DRIVER_UNLOADED_WITHOUT_CANCELLING_PENDING_OPERATIONS = 0x000000CE,
            TERMINAL_SERVER_DRIVER_MADE_INCORRECT_MEMORY_REFERENCE = 0x000000CF,
            DRIVER_CORRUPTED_MMPOOL = 0x000000D0,
            DRIVER_IRQL_NOT_LESS_OR_EQUAL = 0x000000D1,
            BUGCODE_ID_DRIVER = 0x000000D2,
            DRIVER_PORTION_MUST_BE_NONPAGED = 0x000000D3,
            SYSTEM_SCAN_AT_RAISED_IRQL_CAUGHT_IMPROPER_DRIVER_UNLOAD = 0x000000D4,
            DRIVER_PAGE_FAULT_IN_FREED_SPECIAL_POOL = 0x000000D5,
            DRIVER_PAGE_FAULT_BEYOND_END_OF_ALLOCATION = 0x000000D6,
            DRIVER_UNMAPPING_INVALID_VIEW = 0x000000D7,
            DRIVER_USED_EXCESSIVE_PTES = 0x000000D8,
            LOCKED_PAGES_TRACKER_CORRUPTION = 0x000000D9,
            SYSTEM_PTE_MISUSE = 0x000000DA,
            DRIVER_CORRUPTED_SYSPTES = 0x000000DB,
            DRIVER_INVALID_STACK_ACCESS = 0x000000DC,
            POOL_CORRUPTION_IN_FILE_AREA = 0x000000DE,
            IMPERSONATING_WORKER_THREAD = 0x000000DF,
            ACPI_BIOS_FATAL_ERROR = 0x000000E0,
            WORKER_THREAD_RETURNED_AT_BAD_IRQL = 0x000000E1,
            MANUALLY_INITIATED_CRASH = 0x000000E2,
            RESOURCE_NOT_OWNED = 0x000000E3,
            WORKER_INVALID = 0x000000E4,
            ERROR_PIPE_LOCAL = 0x000000E5,
            DRIVER_VERIFIER_DMA_VIOLATION = 0x000000E6,
            INVALID_FLOATING_POINT_STATE = 0x000000E7,
            INVALID_CANCEL_OF_FILE_OPEN = 0x000000E8,
            ACTIVE_EX_WORKER_THREAD_TERMINATION = 0x000000E9,
            THREAD_STUCK_IN_DEVICE_DRIVER = 0x000000EA,
            DIRTY_MAPPED_PAGES_CONGESTION = 0x000000EB,
            SESSION_HAS_VALID_SPECIAL_POOL_ON_EXIT = 0x000000EC,
            UNMOUNTABLE_BOOT_VOLUME = 0x000000ED,
            CRITICAL_PROCESS_DIED = 0x000000EF,
            SCSI_VERIFIER_DETECTED_VIOLATION = 0x000000F1,
            HARDWARE_INTERRUPT_STORM = 0x000000F2,
            DISORDERLY_SHUTDOWN = 0x000000F3,
            CRITICAL_OBJECT_TERMINATION = 0x000000F4,
            FLTMGR_FILE_SYSTEM = 0x000000F5,
            PCI_VERIFIER_DETECTED_VIOLATION = 0x000000F6,
            DRIVER_OVERRAN_STACK_BUFFER = 0x000000F7,
            RAMDISK_BOOT_INITIALIZATION_FAILED = 0x000000F8,
            DRIVER_RETURNED_STATUS_REPARSE_FOR_VOLUME_OPEN = 0x000000F9,
            HTTP_DRIVER_CORRUPTED = 0x000000FA,
            ATTEMPTED_EXECUTE_OF_NOEXECUTE_MEMORY = 0x000000FC,
            DIRTY_NOWRITE_PAGES_CONGESTION = 0x000000FD,
            BUGCODE_USB_DRIVER = 0x000000FE,
            RESERVE_QUEUE_OVERFLOW = 0x000000FF,
            LOADER_BLOCK_MISMATCH = 0x00000100,
            CLOCK_WATCHDOG_TIMEOUT = 0x00000101,
            DPC_WATCHDOG_TIMEOUT = 0x00000102,
            MUP_FILE_SYSTEM = 0x00000103,
            AGP_INVALID_ACCESS = 0x00000104,
            AGP_GART_CORRUPTION = 0x00000105,
            AGP_ILLEGALLY_REPROGRAMMED = 0x00000106,
            STATUS_SOME_NOT_MAPPED = 0x00000107,
            THIRD_PARTY_FILE_SYSTEM_FAILURE = 0x00000108,
            CRITICAL_STRUCTURE_CORRUPTION = 0x00000109,
            APP_TAGGING_INITIALIZATION_FAILED = 0x0000010A,
            FSRTL_EXTRA_CREATE_PARAMETER_VIOLATION = 0x0000010C,
            WDF_VIOLATION = 0x0000010D,
            VIDEO_MEMORY_MANAGEMENT_INTERNAL = 0x0000010E,
            RESOURCE_MANAGER_EXCEPTION_NOT_HANDLED = 0x0000010F,
            RECURSIVE_NMI = 0x00000111,
            MSRPC_STATE_VIOLATION = 0x00000112,
            VIDEO_DXGKRNL_FATAL_ERROR = 0x00000113,
            VIDEO_SHADOW_DRIVER_FATAL_ERROR = 0x00000114,
            AGP_INTERNAL = 0x00000115,
            VIDEO_TDR_ERROR = 0x00000116,
            VIDEO_TDR_TIMEOUT_DETECTED = 0x00000117,
            VIDEO_SCHEDULER_INTERNAL_ERROR = 0x00000119,
            EM_INITIALIZATION_FAILURE = 0x0000011A,
            DRIVER_RETURNED_HOLDING_CANCEL_LOCK = 0x0000011B,
            ATTEMPTED_WRITE_TO_CM_PROTECTED_STORAGE = 0x0000011C,
            EVENT_TRACING_FATAL_ERROR = 0x0000011D,
            DRIVER_VIOLATION = 0x00000121,
            WHEA_INTERNAL_ERROR = 0x00000122,
            WHEA_UNCORRECTABLE_ERROR = 0x00000124,
            PAGE_NOT_ZERO = 0x00000127,
            FAULTY_HARDWARE_CORRUPTED_PAGE = 0x0000012B,
            EXFAT_FILE_SYSTEM = 0x0000012C,
            UNEXPECTED_STORE_EXCEPTION = 0x00000154,
            SYSTEM_THREAD_EXCEPTION_NOT_HANDLED_M = 0x1000007E,
            UNEXPECTED_KERNEL_MODE_TRAP_M = 0x1000007F,
            KERNEL_MODE_EXCEPTION_NOT_HANDLED_M = 0x1000008E,
            THREAD_STUCK_IN_DEVICE_DRIVER_M = 0x100000EA,
            STATUS_CANNOT_LOAD_REGISTRY_FILE = 0xC0000218,
            STATUS_SYSTEM_PROCESS_TERMINATED = 0xC000021A,
            MANUALLY_INITIATED_CRASH1 = 0xDEADDEAD,
        }
        public DumpWindows(DumpStreamService stream)
        {
            _stream = stream;
            Read();
        }
        private void Read()
        {
            var count = 0;
            _signature = _stream.ReadBytes(4);
            if (!Enumerable.SequenceEqual(Signature, "PAGE"u8.ToArray()))
                MessageBox.Show("Открыть файл не удалось, так как он не соответствует сигнатуре PAGE.", "Открытие файла", MessageBoxButton.OK, MessageBoxImage.Error);
            _validDump = _stream.ReadBytes(4);
            if (!Enumerable.SequenceEqual(ValidDump, "DU64"u8.ToArray()) && !Enumerable.SequenceEqual(ValidDump, "DUMP"u8.ToArray()))
                MessageBox.Show("Открыть файл не удалось, так как в нем нет подписи DU64 или DUMP.", "Открытие файла", MessageBoxButton.OK, MessageBoxImage.Error);
            _majorVersion = _stream.ReadU4();
            _minorVersion = _stream.ReadU4();

            if (Enumerable.SequenceEqual(ValidDump, "DU64"u8.ToArray()))
            {
                _versionArchitecture = 64;
                _directoryTableBase = _stream.ReadU8();
                _pfnDataBase = _stream.ReadU8();
                _psLoadedModuleList = _stream.ReadU8();
                _psActiveProcessHead = _stream.ReadU8();
                count = 8;
            }
            else if (Enumerable.SequenceEqual(ValidDump, "DUMP"u8.ToArray()))
            {
                _versionArchitecture = 32;
                _directoryTableBase = _stream.ReadU4();
                _pfnDataBase = _stream.ReadU4();
                _psLoadedModuleList = _stream.ReadU4();
                _psActiveProcessHead = _stream.ReadU4();
                count = 4;
            }
            _machineImageType = (MachineImageTypeList)_stream.ReadU4();
            _numberProcessors = _stream.ReadU4();
            _bugCheckCode = (BugCheckCodeList)_stream.ReadU4();
            _bugCheckParameters = new uint[count];
            for (var i = 0; i < count; i++)
            {
                _bugCheckParameters[i] = _stream.ReadU4();
                if (_bugCheckParameters[i] == BitConverter.ToUInt32(_signature)) i--;
            }
        }
    }
}