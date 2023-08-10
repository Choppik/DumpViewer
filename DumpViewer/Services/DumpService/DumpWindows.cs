using System.Collections.Generic;

namespace DumpViewer.Services.DumpService
{
    /// <summary>
    /// Содержит информацию заголовка для файла минидампа
    /// </summary>
    /// <remarks>
    /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_header">Source</a>
    /// </remarks>
    public class DumpWindows : DumpStructService
    {
        private bool f_streams;
        private List<Dir> _streams;
        public List<Dir> Streams
        {
            get
            {
                if (f_streams)
                    return _streams;
                long _pos = m_io.Pos;
                m_io.Seek(MinorVersion);
                _streams = new();
                for (var i = 0; i < MajorVersion; i++)
                {
                    _streams.Add(new(m_io, this, m_root));
                }
                m_io.Seek(_pos);
                f_streams = true;
                return _streams;
            }
        }
        private byte[] _signature;
        private byte[] _validDump;
        //private uint _sign;
        //private ushort _version;
        private uint _majorVersion;
        private uint _minorVersion;
        private uint _directoryTableBase;
        private uint _pfnDataBase;
        private uint _bugCheckCode;
        private DumpWindows m_root;
        private DumpStructService m_parent;
        public byte[] Signature { get { return _signature; } }
        public byte[] ValidDump { get { return _validDump; } }
        //public uint Sign { get { return _sign; } }
        /// <summary>
        /// Версия формата минидампа. Младшее слово — MINIDUMP_VERSION. Старшее слово — это внутреннее значение, зависящее от реализации
        /// </summary>
        //public ushort Version { get { return _version; } }
        /// <summary>
        /// Количество потоков в каталоге минидампа
        /// </summary>
        public uint MajorVersion { get { return _majorVersion; } }
        /// <summary>
        /// Базовый RVA каталога минидампа. Каталог представляет собой массив структур MINIDUMP_DIRECTORY.
        /// </summary>
        public uint MinorVersion { get { return _minorVersion; } }
        /// <summary>
        /// Контрольная сумма файла минидампа. Этот член может быть нулевым
        /// </summary>
        public uint DirectoryTableBase { get { return _directoryTableBase; } }
        /// <summary>
        /// Время и дата в формате time_t
        /// </summary>
        public uint PfnDataBase { get { return _pfnDataBase; } }
        /// <summary>
        /// Одно или несколько значений перечисляемого типа MINIDUMP_TYPE
        /// </summary>
        public uint BugCheckCode { get { return _bugCheckCode; } }
        public DumpWindows M_Root { get { return m_root; } }
        public DumpStructService M_Parent { get { return m_parent; } }
        public static DumpWindows FromFile(string fileName) => new(new DumpStreamService(fileName));

        public enum StreamTypes : uint
        {
            Unused = 0,
            Reserved0 = 1,
            Reserved1 = 2,
            ThreadList = 3,
            ModuleList = 4,
            MemoryList = 5,
            Exception = 6,
            SystemInfo = 7,
            ThreadExList = 8,
            Memory64List = 9,
            CommentA = 10,
            CommentW = 11,
            HandleData = 12,
            FunctionTable = 13,
            UnloadedModuleList = 14,
            MiscInfo = 15,
            MemoryInfoList = 16,
            ThreadInfoList = 17,
            HandleOperationList = 18,
            Token = 19,
            JavaScriptData = 20,
            SystemMemoryInfo = 21,
            ProcessVmCounters = 22,
            IptTrace = 23,
            ThreadNames = 24,
            CeNull = 32768,
            CeSystemInfo = 32769,
            CeException = 32770,
            CeModuleList = 32771,
            CeProcessList = 32772,
            CeThreadList = 32773,
            CeThreadContextList = 32774,
            CeThreadCallStackList = 32775,
            CeMemoryVirtualList = 32776,
            CeMemoryPhysicalList = 32777,
            CeBucketParameters = 32778,
            CeProcessModuleMap = 32779,
            CeDiagnosisList = 32780,
            MdCrashpadInfoStream = 1129316353,
            MdRawBreakpadInfo = 1197932545,
            MdRawAssertionInfo = 1197932546,
            MdLinuxCpuInfo = 1197932547,
            MdLinuxProcStatus = 1197932548,
            MdLinuxLsbRelease = 1197932549,
            MdLinuxCmdLine = 1197932550,
            MdLinuxEnviron = 1197932551,
            MdLinuxAuxv = 1197932552,
            MdLinuxMaps = 1197932553,
            MdLinuxDsoDebug = 1197932554,
        }
        public DumpWindows(DumpStreamService p__io, DumpStructService p__parent = null, DumpWindows p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            f_streams = false;
            _read();
        }
        private void _read()
        {
            _signature = m_io.ReadBytes(4);
            /*if (!(DumpStreamService.ByteArrayCompare(Magic1, "PAGE"u8.ToArray()) == 0))
            {
                throw new ValidationNotEqualError("PAGE"u8.ToArray(), Magic1, M_Io, "/seq/0");
            }*/
            _validDump = m_io.ReadBytes(4);
            /*if (!(DumpStreamService.ByteArrayCompare(Magic2, "DU"u8.ToArray()) == 0))
            {
                throw new ValidationNotEqualError("DUMP"u8.ToArray(), Magic2, M_Io, "/seq/1");
            }*/
            //_sign = m_io.ReadUInt32();
            //_version = m_io.ReadUInt16();
            _majorVersion = m_io.ReadU4();
            _minorVersion = m_io.ReadU4();
            _directoryTableBase = m_io.ReadU4();
            //_reser = m_io.ReadU4();
            _pfnDataBase = m_io.ReadU4();
            _validDump = m_io.ReadBytes(32);

            _bugCheckCode = m_io.ReadU4();
        }

        /// <summary>
        /// Содержит сведения, необходимые для доступа 
        /// к определенному потоку данных в файле минидампа
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_directory">Source</a>
        /// </remarks>
        public class Dir : DumpStructService
        {
            public static Dir FromFile(string fileName) => new(new DumpStreamService(fileName));
            
            public Dir(DumpStreamService p__io, DumpWindows p__parent = null, DumpWindows p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_data = false;
                Read();
            }
            private void Read()
            {
                _streamType = (StreamTypes)m_io.ReadU4();
                _lenData = m_io.ReadU4();
                _ofsData = m_io.ReadU4();
            }
            private bool f_data;
            private object _data;
            public object Data
            {
                get
                {
                    if (f_data)
                        return _data;
                    long _pos = m_io.Pos;
                    m_io.Seek(OfsData);
                    switch (StreamType)
                    {
                        case DumpWindows.StreamTypes.MemoryList:
                            {
                                __raw_data = m_io.ReadBytes(LenData);
                                var io___raw_data = new DumpStreamService(__raw_data);
                                _data = new MemoryList(io___raw_data, this, m_root);
                                break;
                            }
                        case DumpWindows.StreamTypes.MiscInfo:
                            {
                                __raw_data = m_io.ReadBytes(LenData);
                                var io___raw_data = new DumpStreamService(__raw_data);
                                _data = new MiscInfo(io___raw_data, this, m_root);
                                break;
                            }
                        case DumpWindows.StreamTypes.ThreadList:
                            {
                                __raw_data = m_io.ReadBytes(LenData);
                                var io___raw_data = new DumpStreamService(__raw_data);
                                _data = new ThreadList(io___raw_data, this, m_root);
                                break;
                            }
                        case DumpWindows.StreamTypes.Exception:
                            {
                                __raw_data = m_io.ReadBytes(LenData);
                                var io___raw_data = new DumpStreamService(__raw_data);
                                _data = new ExceptionStream(io___raw_data, this, m_root);
                                break;
                            }
                        case DumpWindows.StreamTypes.SystemInfo:
                            {
                                __raw_data = m_io.ReadBytes(LenData);
                                var io___raw_data = new DumpStreamService(__raw_data);
                                _data = new SystemInfo(io___raw_data, this, m_root);
                                break;
                            }
                        default:
                            {
                                _data = m_io.ReadBytes(LenData);
                                break;
                            }
                    }
                    m_io.Seek(_pos);
                    f_data = true;
                    return _data;
                }
            }
            private StreamTypes _streamType;
            private uint _lenData;
            private uint _ofsData;
            private DumpWindows m_root;
            private DumpWindows m_parent;
            private byte[] __raw_data;
            /// <summary>
            /// Тип потока данных. 
            /// Этот элемент может быть одним из значений в перечислении MINIDUMP_STREAM_TYPE.
            /// </summary>
            public StreamTypes StreamType { get { return _streamType; } }
            public uint LenData { get { return _lenData; } }
            public uint OfsData { get { return _ofsData; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows M_Parent { get { return m_parent; } }
            public byte[] M_RawData { get { return __raw_data; } }
        }

        /// <summary>
        /// Содержит список потоков
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_thread_list">Source</a>
        /// </remarks>
        public class ThreadList : DumpStructService
        {
            public static ThreadList FromFile(string fileName)
            {
                return new ThreadList(new DumpStreamService(fileName));
            }

            public ThreadList(DumpStreamService p__io, DumpWindows.Dir p__parent = null, DumpWindows p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _numberOfThreads = m_io.ReadU4();
                _threads = new List<Thread>();
                for (var i = 0; i < NumberOfThreads; i++)
                {
                    _threads.Add(new Thread(m_io, this, m_root));
                }
            }
            private uint _numberOfThreads;
            private List<Thread> _threads;
            private DumpWindows m_root;
            private DumpWindows.Dir m_parent;
            /// <summary>
            /// Количество структур в массиве Threads.
            /// </summary>
            public uint NumberOfThreads { get { return _numberOfThreads; } }
            /// <summary>
            /// Массив MINIDUMP_THREAD структур
            /// </summary>
            public List<Thread> Threads { get { return _threads; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.Dir M_Parent { get { return m_parent; } }
        } //1

        /// <summary>
        /// Содержит сведения для конкретного потока
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_thread">Source</a>
        /// </remarks>
        public class Thread : DumpStructService
        {
            public static Thread FromFile(string fileName)
            {
                return new Thread(new DumpStreamService(fileName));
            }

            public Thread(DumpStreamService p__io, DumpWindows.ThreadList p__parent = null, DumpWindows p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _threadId = m_io.ReadU4();
                _suspendCount = m_io.ReadU4();
                _priorityClass = m_io.ReadU4();
                _priority = m_io.ReadU4();
                _teb = m_io.ReadU8();
                _stack = new MemoryDescriptor(m_io, this, m_root);
                _threadContext = new LocationDescriptor(m_io, this, m_root);
            }
            private uint _threadId;
            private uint _suspendCount;
            private uint _priorityClass;
            private uint _priority;
            private ulong _teb;
            private MemoryDescriptor _stack;
            private LocationDescriptor _threadContext;
            private DumpWindows m_root;
            private DumpWindows.ThreadList m_parent;
            /// <summary>
            /// Идентификатор потока
            /// </summary>
            public uint ThreadId { get { return _threadId; } }
            /// <summary>
            /// Счетчик приостановки для потока. Если число приостановок больше нуля, 
            /// поток приостанавливается; В противном случае нить не подвешивается. 
            /// Максимальное значение — MAXIMUM_SUSPEND_COUNT.
            /// </summary>
            public uint SuspendCount { get { return _suspendCount; } }
            /// <summary>
            /// Класс приоритета потока
            /// </summary>
            public uint PriorityClass { get { return _priorityClass; } }
            /// <summary>
            /// Уровень приоритета потока
            /// </summary>
            public uint Priority { get { return _priority; } }
            /// <summary>
            /// Блок окружения потока
            /// </summary>
            public ulong Teb { get { return _teb; } }
            public MemoryDescriptor Stack { get { return _stack; } }
            public LocationDescriptor ThreadContext { get { return _threadContext; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.ThreadList M_Parent { get { return m_parent; } }
        } //1.1

        /// <summary>
        /// Содержит список диапазонов памяти
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_memory64_list">Source</a>
        /// </remarks>
        public class MemoryList : DumpStructService
        {
            public static MemoryList FromFile(string fileName)
            {
                return new MemoryList(new DumpStreamService(fileName));
            }

            public MemoryList(DumpStreamService p__io, DumpWindows.Dir p__parent = null, DumpWindows p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _numberOfMemoryRanges = m_io.ReadU4();
                _memRanges = new List<MemoryDescriptor>();
                for (var i = 0; i < NumberOfMemoryRanges; i++)
                {
                    _memRanges.Add(new MemoryDescriptor(m_io, this, m_root));
                }
            }
            private uint _numberOfMemoryRanges;
            private List<MemoryDescriptor> _memRanges;
            private DumpWindows m_root;
            private DumpWindows.Dir m_parent;
            /// <summary>
            /// Количество структур в массиве MemoryRanges.
            /// </summary>
            public uint NumberOfMemoryRanges { get { return _numberOfMemoryRanges; } }
            public List<MemoryDescriptor> MemRanges { get { return _memRanges; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.Dir M_Parent { get { return m_parent; } }
        } //2

        /// <summary>
        /// Описывает диапазон памяти
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_memory_descriptor">Source</a>
        /// </remarks>
        public class MemoryDescriptor : DumpStructService
        {
            public static MemoryDescriptor FromFile(string fileName)
            {
                return new MemoryDescriptor(new DumpStreamService(fileName));
            }

            public MemoryDescriptor(DumpStreamService p__io, DumpStructService p__parent = null, DumpWindows p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _startOfMemoryRange = m_io.ReadU8();
                _memory = new LocationDescriptor(m_io, this, m_root);
            }
            private ulong _startOfMemoryRange;
            private LocationDescriptor _memory;
            private DumpWindows m_root;
            private DumpStructService m_parent;
            /// <summary>
            /// Начальный адрес диапазона памяти
            /// </summary>
            public ulong StartOfMemoryRange { get { return _startOfMemoryRange; } }
            public LocationDescriptor Memory { get { return _memory; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpStructService M_Parent { get { return m_parent; } }
        } //2.1 , 1.1.2

        /// <summary>
        /// Содержит сведения, описывающие расположение потока данных в файле мини-дампа
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_location_descriptor">Source</a>
        /// </remarks>
        public class LocationDescriptor : DumpStructService
        {
            public static LocationDescriptor FromFile(string fileName)
            {
                return new LocationDescriptor(new DumpStreamService(fileName));
            }

            public LocationDescriptor(DumpStreamService p__io, DumpStructService p__parent = null, DumpWindows p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_data = false;
                _read();
            }
            private void _read()
            {
                _lenData = m_io.ReadU4();
                _ofsData = m_io.ReadU4();
            }
            private bool f_data;
            private byte[] _data;
            public byte[] Data
            {
                get
                {
                    if (f_data)
                        return _data;
                    DumpStreamService io = M_Root.M_Io;
                    long _pos = io.Pos;
                    io.Seek(OfsData);
                    _data = io.ReadBytes(LenData);
                    io.Seek(_pos);
                    f_data = true;
                    return _data;
                }
            }
            private uint _lenData;
            private uint _ofsData;
            private DumpWindows m_root;
            private DumpStructService m_parent;
            /// <summary>
            /// Размер потока данных в байтах
            /// </summary>
            public uint LenData { get { return _lenData; } }
            /// <summary>
            /// Относительный виртуальный адрес (RVA) данных. 
            /// Это смещение байтов потока данных от начала файла минидампа
            /// </summary>
            public uint OfsData { get { return _ofsData; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpStructService M_Parent { get { return m_parent; } }
        }

        /// <summary>
        /// Описывает строку
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_string">Source</a>
        /// </remarks>
        public class MinidumpString : DumpStructService
        {
            public static MinidumpString FromFile(string fileName)
            {
                return new MinidumpString(new DumpStreamService(fileName));
            }

            public MinidumpString(DumpStreamService p__io, DumpWindows.SystemInfo p__parent = null, DumpWindows p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _lenStr = m_io.ReadU4();
                _str = System.Text.Encoding.GetEncoding("UTF-16LE").GetString(m_io.ReadBytes(LenStr));
            }
            private uint _lenStr;
            private string _str;
            private DumpWindows m_root;
            private DumpWindows.SystemInfo m_parent;
            /// <summary>
            /// Размер строки в члене Buffer в байтах.
            /// Этот размер не включает символ, заканчивающийся нулем
            /// </summary>
            public uint LenStr { get { return _lenStr; } }
            /// <summary>
            /// Строка, заканчивающаяся значением NULL
            /// </summary>
            public string Str { get { return _str; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.SystemInfo M_Parent { get { return m_parent; } }
        } //3.1

        /// <summary>
        /// Содержит информацию о процессоре и операционной системе
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_system_info">Source</a>
        /// </remarks>
        public class SystemInfo : DumpStructService
        {
            public static SystemInfo FromFile(string fileName)
            {
                return new SystemInfo(new DumpStreamService(fileName));
            }

            public enum CpuArchs
            {
                Intel = 0,
                Arm = 5,
                Ia64 = 6,
                Amd64 = 9,
                Unknown = 65535,
            }
            public SystemInfo(DumpStreamService p__io, DumpWindows.Dir p__parent = null, DumpWindows p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_servicePack = false;
                _read();
            }
            private void _read()
            {
                _processorArchitecture = (CpuArchs)m_io.ReadU2();
                _processorLevel = m_io.ReadU2();
                _processorRevision = m_io.ReadU2();
                _numCpus = m_io.ReadU1();
                _osType = m_io.ReadU1();
                _osVerMajor = m_io.ReadU4();
                _osVerMinor = m_io.ReadU4();
                _osBuild = m_io.ReadU4();
                _osPlatform = m_io.ReadU4();
                _ofsServicePack = m_io.ReadU4();
                _osSuiteMask = m_io.ReadU2();
                _reserved2 = m_io.ReadU2();
            }
            private bool f_servicePack;
            private MinidumpString _servicePack;
            public MinidumpString ServicePack
            {
                get
                {
                    if (f_servicePack)
                        return _servicePack;
                    if (OfsServicePack > 0)
                    {
                        DumpStreamService io = M_Root.M_Io;
                        long _pos = io.Pos;
                        io.Seek(OfsServicePack);
                        _servicePack = new MinidumpString(io, this, m_root);
                        io.Seek(_pos);
                        f_servicePack = true;
                    }
                    return _servicePack;
                }
            }
            private CpuArchs _processorArchitecture;
            private ushort _processorLevel;
            private ushort _processorRevision;
            private byte _numCpus;
            private byte _osType;
            private uint _osVerMajor;
            private uint _osVerMinor;
            private uint _osBuild;
            private uint _osPlatform;
            private uint _ofsServicePack;
            private ushort _osSuiteMask;
            private ushort _reserved2;
            private DumpWindows m_root;
            private DumpWindows.Dir m_parent;
            /// <summary>
            /// Архитектура процессора системы
            /// </summary>
            public CpuArchs ProcessorArchitecture { get { return _processorArchitecture; } }
            /// <summary>
            /// Уровень процессора, зависящий от архитектуры системы.
            /// Если ProcessorArchitecture имеет значение PROCESSOR_ARCHITECTURE_INTEL, ProcessorLevel может иметь одно из следующих значений
            /// </summary>
            public ushort ProcessorLevel { get { return _processorLevel; } }
            /// <summary>
            /// Версия процессора, зависящая от архитектуры
            /// </summary>
            public ushort ProcessorRevision { get { return _processorRevision; } }
            public byte NumCpus { get { return _numCpus; } }
            public byte OsType { get { return _osType; } }
            public uint OsVerMajor { get { return _osVerMajor; } }
            public uint OsVerMinor { get { return _osVerMinor; } }
            public uint OsBuild { get { return _osBuild; } }
            public uint OsPlatform { get { return _osPlatform; } }
            public uint OfsServicePack { get { return _ofsServicePack; } }
            public ushort OsSuiteMask { get { return _osSuiteMask; } }
            public ushort Reserved2 { get { return _reserved2; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.Dir M_Parent { get { return m_parent; } }
        } //3

        /// <summary>
        /// Содержит сведения об исключениях
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_exception">Source</a>
        /// </remarks>
        public class ExceptionRecord : DumpStructService
        {
            public static ExceptionRecord FromFile(string fileName)
            {
                return new ExceptionRecord(new DumpStreamService(fileName));
            }

            public ExceptionRecord(DumpStreamService p__io, DumpWindows.ExceptionStream p__parent = null, DumpWindows p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _code = m_io.ReadU4();
                _flags = m_io.ReadU4();
                _innerException = m_io.ReadU8();
                _addr = m_io.ReadU8();
                _numParams = m_io.ReadU4();
                _reserved = m_io.ReadU4();
                _params = new List<ulong>();
                for (var i = 0; i < 15; i++)
                {
                    _params.Add(m_io.ReadU8());
                }
            }
            private uint _code;
            private uint _flags;
            private ulong _innerException;
            private ulong _addr;
            private uint _numParams;
            private uint _reserved;
            private List<ulong> _params;
            private DumpWindows m_root;
            private DumpWindows.ExceptionStream m_parent;
            /// <summary>
            /// Причина, по которой произошло исключение. Это код, сгенерированный аппаратным исключением, или код, указанный в функции RaiseException для программного исключения
            /// </summary>
            public uint Code { get { return _code; } }
            /// <summary>
            /// Этот элемент может быть либо нулевым, что указывает на непрерывное исключение, либо EXCEPTION_NONCONTINUABLE, указывающим на непрерывное исключение. 
            /// Любая попытка продолжить выполнение после непрерывистого исключения вызывает EXCEPTION_NONCONTINUABLE_EXCEPTION исключение.
            /// </summary>
            public uint Flags { get { return _flags; } }
            /// <summary>
            /// Указатель на связанную MINIDUMP_EXCEPTION структуру. Записи исключений могут быть объединены в цепочки, 
            /// чтобы предоставить дополнительную информацию при возникновении вложенных исключений
            /// </summary>
            public ulong InnerException { get { return _innerException; } }
            /// <summary>
            /// Адрес памяти, где произошло исключение
            /// </summary>
            public ulong Addr { get { return _addr; } }
            /// <summary>
            /// Количество параметров, связанных с исключением. Это количество определенных элементов в массиве ExceptionInformation.
            /// </summary>
            public uint NumParams { get { return _numParams; } }
            /// <summary>
            /// Зарезервировано для кроссплатформенного выравнивания элементов структуры. Не устанавливать.
            /// </summary>
            public uint Reserved { get { return _reserved; } }
            /// <summary>
            /// Дополнительные параметры, передаваемые вместе с вызовом исключения
            /// функция (для WinAPI это `RaiseException`). Значение
            /// зависит от исключения. Учитывая, что этот тип изначально
            /// определяется структурой C, там он описывается как массив
            /// фиксированное количество элементов (`EXCEPTION_MAXIMUM_PARAMETERS` =
            /// 15), но на самом деле будет использоваться только первый `num_params`.
            /// </summary>
            public List<ulong> Params { get { return _params; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.ExceptionStream M_Parent { get { return m_parent; } }
        } //5.1

        /// <summary>
        /// Класс представляет поток сведений об исключении
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_exception_stream">Source</a>
        /// </remarks>
        public class ExceptionStream : DumpStructService
        {
            public static ExceptionStream FromFile(string fileName)
            {
                return new ExceptionStream(new DumpStreamService(fileName));
            }

            public ExceptionStream(DumpStreamService p__io, DumpWindows.Dir p__parent = null, DumpWindows p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _threadId = m_io.ReadU4();
                _reserved = m_io.ReadU4();
                _exceptionRec = new ExceptionRecord(m_io, this, m_root);
                _threadContext = new LocationDescriptor(m_io, this, m_root);
            }
            private uint _threadId;
            private uint _reserved;
            private ExceptionRecord _exceptionRec;
            private LocationDescriptor _threadContext;
            private DumpWindows m_root;
            private DumpWindows.Dir m_parent;
            /// <summary>
            /// Идентификатор потока, вызвавшего исключение
            /// </summary>
            public uint ThreadId { get { return _threadId; } }
            /// <summary>
            /// Переменная для выравнивания
            /// </summary>
            public uint Reserved { get { return _reserved; } }
            public ExceptionRecord ExceptionRec { get { return _exceptionRec; } }
            public LocationDescriptor ThreadContext { get { return _threadContext; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.Dir M_Parent { get { return m_parent; } }
        } //5

        /// <summary>
        /// Класс содержит разнообразную информацию
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_misc_info">Source</a>
        /// </remarks>
        public class MiscInfo : DumpStructService
        {
            public static MiscInfo FromFile(string fileName)
            {
                return new MiscInfo(new DumpStreamService(fileName));
            }

            public MiscInfo(DumpStreamService p__io, DumpWindows.Dir p__parent = null, DumpWindows p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _sizeOfInfo = m_io.ReadU4();
                _flags1 = m_io.ReadU4();
                _processId = m_io.ReadU4();
                _processCreateTime = m_io.ReadU4();
                _processUserTime = m_io.ReadU4();
                _processKernelTime = m_io.ReadU4();
                _cpuMaxMhz = m_io.ReadU4();
                _cpuCurMhz = m_io.ReadU4();
                _cpuLimitMhz = m_io.ReadU4();
                _cpuMaxIdleState = m_io.ReadU4();
                _cpuCurIdleState = m_io.ReadU4();
            }
            private uint _sizeOfInfo;
            private uint _flags1;
            private uint _processId;
            private uint _processCreateTime;
            private uint _processUserTime;
            private uint _processKernelTime;
            private uint _cpuMaxMhz;
            private uint _cpuCurMhz;
            private uint _cpuLimitMhz;
            private uint _cpuMaxIdleState;
            private uint _cpuCurIdleState;
            private DumpWindows m_root;
            private DumpWindows.Dir m_parent;
            /// <summary>
            /// Размер структуры в байтах
            /// </summary>
            public uint SizeOfInfo { get { return _sizeOfInfo; } }
            /// <summary>
            /// Флаги, обозначающие допустимые элементы этой структуры
            /// </summary>
            public uint Flags1 { get { return _flags1; } }
            /// <summary>
            /// Идентификатор процесса. 
            /// Если Flags1 не указывает MINIDUMP_MISC1_PROCESS_ID, этот элемент не используется
            /// </summary>
            public uint ProcessId { get { return _processId; } }
            /// <summary>
            /// Идентификатор процесса. 
            /// Если Flags1 не указывает MINIDUMP_MISC1_PROCESS_ID, этот элемент не используется
            /// </summary>
            public uint ProcessCreateTime { get { return _processCreateTime; } }
            /// <summary>
            /// Время выполнения процесса в пользовательском режиме, в секундах. Определяется время, которое каждый из потоков процесса выполнил в пользовательском режиме, затем все эти времена суммируются для получения этого значения. 
            /// Если Flags1 не указывает MINIDUMP_MISC1_PROCESS_TIMES, этот элемент не используется
            /// </summary>
            public uint ProcessUserTime { get { return _processUserTime; } }
            /// <summary>
            /// Время выполнения процесса в режиме ядра в секундах. Определяется время, которое каждый из потоков процесса выполнил в режиме ядра, затем все эти времена суммируются для получения этого значения. 
            /// Если Flags1 не указывает MINIDUMP_MISC1_PROCESS_TIMES, этот элемент не используется
            /// </summary>
            public uint ProcessKernelTime { get { return _processKernelTime; } }
            public uint CpuMaxMhz { get { return _cpuMaxMhz; } }
            public uint CpuCurMhz { get { return _cpuCurMhz; } }
            public uint CpuLimitMhz { get { return _cpuLimitMhz; } }
            public uint CpuMaxIdleState { get { return _cpuMaxIdleState; } }
            public uint CpuCurIdleState { get { return _cpuCurIdleState; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.Dir M_Parent { get { return m_parent; } }
        } //4
    }
}