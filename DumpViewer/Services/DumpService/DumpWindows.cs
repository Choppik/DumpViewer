using System.Collections.Generic;

namespace DumpViewer.Services.DumpService
{
    public class DumpWindows : DumpStructService
    {
        public static DumpWindows FromFile(string fileName)
        {
            return new DumpWindows(new DumpStreamService(fileName));
        }

        public enum StreamTypes
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
            _magic1 = m_io.ReadBytes(4);
            if (!(DumpStreamService.ByteArrayCompare(Magic1, "MDMP"u8.ToArray()) == 0))
            {
                throw new ValidationNotEqualError("MDMP"u8.ToArray(), Magic1, M_Io, "/seq/0");
            }
            _magic2 = m_io.ReadBytes(2);
            if (!(DumpStreamService.ByteArrayCompare(Magic2, new byte[] { 147, 167 }) == 0))
            {
                throw new ValidationNotEqualError(new byte[] { 147, 167 }, Magic2, M_Io, "/seq/1");
            }
            _version = m_io.ReadU2();
            _numStreams = m_io.ReadU4();
            _ofsStreams = m_io.ReadU4();
            _checksum = m_io.ReadU4();
            _timestamp = m_io.ReadU4();
            _flags = m_io.ReadU8();
        }

        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_thread_list">Source</a>
        /// </remarks>
        public partial class ThreadList : DumpStructService
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
                _numThreads = m_io.ReadU4();
                _threads = new List<Thread>();
                for (var i = 0; i < NumThreads; i++)
                {
                    _threads.Add(new Thread(m_io, this, m_root));
                }
            }
            private uint _numThreads;
            private List<Thread> _threads;
            private DumpWindows m_root;
            private DumpWindows.Dir m_parent;
            public uint NumThreads { get { return _numThreads; } }
            public List<Thread> Threads { get { return _threads; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.Dir M_Parent { get { return m_parent; } }
        }

        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_location_descriptor">Source</a>
        /// </remarks>
        public partial class LocationDescriptor : DumpStructService
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
            public uint LenData { get { return _lenData; } }
            public uint OfsData { get { return _ofsData; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpStructService M_Parent { get { return m_parent; } }
        }

        /// <summary>
        /// Specific string serialization scheme used in MiniDump format is
        /// actually a simple 32-bit length-prefixed UTF-16 string.
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_string">Source</a>
        /// </remarks>
        public partial class MinidumpString : DumpStructService
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
            public uint LenStr { get { return _lenStr; } }
            public string Str { get { return _str; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.SystemInfo M_Parent { get { return m_parent; } }
        }

        /// <summary>
        /// &quot;System info&quot; stream provides basic information about the
        /// hardware and operating system which produces this dump.
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_system_info">Source</a>
        /// </remarks>
        public partial class SystemInfo : DumpStructService
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
                _cpuArch = ((CpuArchs)m_io.ReadU2());
                _cpuLevel = m_io.ReadU2();
                _cpuRevision = m_io.ReadU2();
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
            private CpuArchs _cpuArch;
            private ushort _cpuLevel;
            private ushort _cpuRevision;
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
            public CpuArchs CpuArch { get { return _cpuArch; } }
            public ushort CpuLevel { get { return _cpuLevel; } }
            public ushort CpuRevision { get { return _cpuRevision; } }
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
        }

        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_exception">Source</a>
        /// </remarks>
        public partial class ExceptionRecord : DumpStructService
        {
            public static ExceptionRecord FromFile( string fileName)
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
            public uint Code { get { return _code; } }
            public uint Flags { get { return _flags; } }
            public ulong InnerException { get { return _innerException; } }

            /// <summary>
            /// Memory address where exception has occurred
            /// </summary>
            public ulong Addr { get { return _addr; } }
            public uint NumParams { get { return _numParams; } }
            public uint Reserved { get { return _reserved; } }

            /// <summary>
            /// Additional parameters passed along with exception raise
            /// function (for WinAPI, that is `RaiseException`). Meaning is
            /// exception-specific. Given that this type is originally
            /// defined by a C structure, it is described there as array of
            /// fixed number of elements (`EXCEPTION_MAXIMUM_PARAMETERS` =
            /// 15), but in reality only first `num_params` would be used.
            /// </summary>
            public List<ulong> Params { get { return _params; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.ExceptionStream M_Parent { get { return m_parent; } }
        }

        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_misc_info">Source</a>
        /// </remarks>
        public partial class MiscInfo : DumpStructService
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
                _lenInfo = m_io.ReadU4();
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
            private uint _lenInfo;
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
            public uint LenInfo { get { return _lenInfo; } }
            public uint Flags1 { get { return _flags1; } }
            public uint ProcessId { get { return _processId; } }
            public uint ProcessCreateTime { get { return _processCreateTime; } }
            public uint ProcessUserTime { get { return _processUserTime; } }
            public uint ProcessKernelTime { get { return _processKernelTime; } }
            public uint CpuMaxMhz { get { return _cpuMaxMhz; } }
            public uint CpuCurMhz { get { return _cpuCurMhz; } }
            public uint CpuLimitMhz { get { return _cpuLimitMhz; } }
            public uint CpuMaxIdleState { get { return _cpuMaxIdleState; } }
            public uint CpuCurIdleState { get { return _cpuCurIdleState; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.Dir M_Parent { get { return m_parent; } }
        }

        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_directory">Source</a>
        /// </remarks>
        public partial class Dir : DumpStructService
        {
            public static Dir FromFile(string fileName)
            {
                return new Dir(new DumpStreamService(fileName));
            }

            public Dir(DumpStreamService p__io, DumpWindows p__parent = null, DumpWindows p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_data = false;
                _read();
            }
            private void _read()
            {
                _streamType = ((DumpWindows.StreamTypes)m_io.ReadU4());
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
            public StreamTypes StreamType { get { return _streamType; } }

            /// <remarks>
            /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_location_descriptor">Source</a>
            /// </remarks>
            public uint LenData { get { return _lenData; } }
            public uint OfsData { get { return _ofsData; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows M_Parent { get { return m_parent; } }
            public byte[] M_RawData { get { return __raw_data; } }
        }

        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_thread">Source</a>
        /// </remarks>
        public partial class Thread : DumpStructService
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
            public uint ThreadId { get { return _threadId; } }
            public uint SuspendCount { get { return _suspendCount; } }
            public uint PriorityClass { get { return _priorityClass; } }
            public uint Priority { get { return _priority; } }

            /// <summary>
            /// Thread Environment Block
            /// </summary>
            public ulong Teb { get { return _teb; } }
            public MemoryDescriptor Stack { get { return _stack; } }
            public LocationDescriptor ThreadContext { get { return _threadContext; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.ThreadList M_Parent { get { return m_parent; } }
        }

        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_memory64_list">Source</a>
        /// </remarks>
        public partial class MemoryList : DumpStructService
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
                _numMemRanges = m_io.ReadU4();
                _memRanges = new List<MemoryDescriptor>();
                for (var i = 0; i < NumMemRanges; i++)
                {
                    _memRanges.Add(new MemoryDescriptor(m_io, this, m_root));
                }
            }
            private uint _numMemRanges;
            private List<MemoryDescriptor> _memRanges;
            private DumpWindows m_root;
            private DumpWindows.Dir m_parent;
            public uint NumMemRanges { get { return _numMemRanges; } }
            public List<MemoryDescriptor> MemRanges { get { return _memRanges; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.Dir M_Parent { get { return m_parent; } }
        }

        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_memory_descriptor">Source</a>
        /// </remarks>
        public partial class MemoryDescriptor : DumpStructService
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
                _addrMemoryRange = m_io.ReadU8();
                _memory = new LocationDescriptor(m_io, this, m_root);
            }
            private ulong _addrMemoryRange;
            private LocationDescriptor _memory;
            private DumpWindows m_root;
            private DumpStructService m_parent;
            public ulong AddrMemoryRange { get { return _addrMemoryRange; } }
            public LocationDescriptor Memory { get { return _memory; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpStructService M_Parent { get { return m_parent; } }
        }

        /// <remarks>
        /// Reference: <a href="https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_exception_stream">Source</a>
        /// </remarks>
        public partial class ExceptionStream : DumpStructService
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
            public uint ThreadId { get { return _threadId; } }
            public uint Reserved { get { return _reserved; } }
            public ExceptionRecord ExceptionRec { get { return _exceptionRec; } }
            public LocationDescriptor ThreadContext { get { return _threadContext; } }
            public DumpWindows M_Root { get { return m_root; } }
            public DumpWindows.Dir M_Parent { get { return m_parent; } }
        }
        private bool f_streams;
        private List<Dir> _streams;
        public List<Dir> Streams
        {
            get
            {
                if (f_streams)
                    return _streams;
                long _pos = m_io.Pos;
                m_io.Seek(OfsStreams);
                _streams = new List<Dir>();
                for (var i = 0; i < NumStreams; i++)
                {
                    _streams.Add(new Dir(m_io, this, m_root));
                }
                m_io.Seek(_pos);
                f_streams = true;
                return _streams;
            }
        }
        private byte[] _magic1;
        private byte[] _magic2;
        private ushort _version;
        private uint _numStreams;
        private uint _ofsStreams;
        private uint _checksum;
        private uint _timestamp;
        private ulong _flags;
        private DumpWindows m_root;
        private DumpStructService m_parent;
        public byte[] Magic1 { get { return _magic1; } }
        public byte[] Magic2 { get { return _magic2; } }
        public ushort Version { get { return _version; } }
        public uint NumStreams { get { return _numStreams; } }
        public uint OfsStreams { get { return _ofsStreams; } }
        public uint Checksum { get { return _checksum; } }
        public uint Timestamp { get { return _timestamp; } }
        public ulong Flags { get { return _flags; } }
        public DumpWindows M_Root { get { return m_root; } }
        public DumpStructService M_Parent { get { return m_parent; } }
    }
}
