using DumpViewer.Command;
using DumpViewer.ViewModels.Base;
using System.Windows.Input;

namespace DumpViewer.ViewModels
{
    public class DumpViewerViewModel : BaseViewModel
    {
        private bool _isOpenFile = false;
        private string _fileDump = "";
        private string _сrashTime = "";
        private string _bugCheckString = "";
        private string _bugCheckCode = "";
        private string _parameter1 = "";
        private string _parameter2 = "";
        private string _parameter3 = "";
        private string _parameter4 = "";
        private string _causedByDriver = "";
        private string _causedByAddress = "";
        private string _processor = "";
        private string _versionArchitecture = "";
        private string _crashAddress = "";
        private string _fullPath = "";
        private string _processorsCount = "";
        private string _majorVersion = "";
        private string _minorVersion = "";
        private string _osName = "";
        private string _dumpFileSize = "";
        private string _dumpFileTime = "";
        public bool IsOpenFile
        {
            get => _isOpenFile;
            set
            {
                Set(ref _isOpenFile, value);
                OnPropertyChanged(nameof(IsOpenFile));
            }
        }
        public string FileDump
        {
            get => _fileDump;
            set
            {
                Set(ref _fileDump, value);
                OnPropertyChanged(nameof(FileDump));
            }
        }
        public string СrashTime
        {
            get => _сrashTime;
            set
            {
                Set(ref _сrashTime, value);
                OnPropertyChanged(nameof(СrashTime));
            }
        }
        public string BugCheckString
        {
            get => _bugCheckString;
            set
            {
                Set(ref _bugCheckString, value);
                OnPropertyChanged(nameof(BugCheckString));
            }
        }
        public string BugCheckCode
        {
            get => _bugCheckCode;
            set
            {
                Set(ref _bugCheckCode, value);
                OnPropertyChanged(nameof(BugCheckCode));
            }
        }
        public string Parameter1
        {
            get => _parameter1;
            set
            {
                Set(ref _parameter1, value);
                OnPropertyChanged(nameof(Parameter1));
            }
        }
        public string Parameter2
        {
            get => _parameter2;
            set
            {
                Set(ref _parameter2, value);
                OnPropertyChanged(nameof(Parameter2));
            }
        }
        public string Parameter3
        {
            get => _parameter3;
            set
            {
                Set(ref _parameter3, value);
                OnPropertyChanged(nameof(Parameter3));
            }
        }
        public string Parameter4
        {
            get => _parameter4;
            set
            {
                Set(ref _parameter4, value);
                OnPropertyChanged(nameof(Parameter4));
            }
        }
        public string CausedByDriver
        {
            get => _causedByDriver;
            set
            {
                Set(ref _causedByDriver, value);
                OnPropertyChanged(nameof(CausedByDriver));
            }
        }
        public string CausedByAddress
        {
            get => _causedByAddress;
            set
            {
                Set(ref _causedByAddress, value);
                OnPropertyChanged(nameof(CausedByAddress));
            }
        }
        public string Processor
        {
            get => _processor;
            set
            {
                Set(ref _processor, value);
                OnPropertyChanged(nameof(Processor));
            }
        }
        public string VersionArchitecture
        {
            get => _versionArchitecture;
            set
            {
                Set(ref _versionArchitecture, value);
                OnPropertyChanged(nameof(VersionArchitecture));
            }
        }
        public string CrashAddress
        {
            get => _crashAddress;
            set
            {
                Set(ref _crashAddress, value);
                OnPropertyChanged(nameof(CrashAddress));
            }
        }
        public string FullPath
        {
            get => _fullPath;
            set
            {
                Set(ref _fullPath, value);
                OnPropertyChanged(nameof(FullPath));
            }
        }
        public string ProcessorsCount
        {
            get => _processorsCount;
            set
            {
                Set(ref _processorsCount, value);
                OnPropertyChanged(nameof(ProcessorsCount));
            }
        }
        public string MajorVersion
        {
            get => _majorVersion;
            set
            {
                Set(ref _majorVersion, value);
                OnPropertyChanged(nameof(MajorVersion));
            }
        }
        public string MinorVersion
        {
            get => _minorVersion;
            set
            {
                Set(ref _minorVersion, value);
                OnPropertyChanged(nameof(MinorVersion));
            }
        }
        public string OsName
        {
            get => _osName;
            set
            {
                Set(ref _osName, value);
                OnPropertyChanged(nameof(OsName));
            }
        }
        public string DumpFileSize
        {
            get => _dumpFileSize;
            set
            {
                Set(ref _dumpFileSize, value);
                OnPropertyChanged(nameof(DumpFileSize));
            }
        }
        public string DumpFileTime
        {
            get => _dumpFileTime;
            set
            {
                Set(ref _dumpFileTime, value);
                OnPropertyChanged(nameof(DumpFileTime));
            }
        }
        public ICommand OpenFileCommand { get; }
        public DumpViewerViewModel()
        {
            OpenFileCommand = new OpenFileCommand(this);
        }
    }
}