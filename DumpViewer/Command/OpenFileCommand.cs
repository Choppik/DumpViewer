using DumpViewer.Command.Base;
using DumpViewer.ViewModels;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using static DumpViewer.Models.DumpWindows;

namespace DumpViewer.Command
{
    public class OpenFileCommand : BaseCommand
    {
        private readonly DumpViewerViewModel _dumpViewerViewModel;
        public OpenFileCommand(DumpViewerViewModel dumpViewerViewModel)
        {
            _dumpViewerViewModel = dumpViewerViewModel;
        }

        public override void Execute(object? parameter)
        {
            OpenFileDialog openFile = new()
            {
                Filter = "Дампы (*.dmp)|*.dmp;|Все файлы (*.*)|*.*"
            };
            if (openFile.ShowDialog() == true)
            {
                if (Path.GetExtension(openFile.FileName) == ".dmp")
                {
                    FileInfo fileInfo = new(openFile.FileName);
                    var data = FromFile(openFile.FileName);
                    _dumpViewerViewModel.IsOpenFile = true;
                    _dumpViewerViewModel.FileDump = Path.GetFileName(openFile.FileName);
                    _dumpViewerViewModel.СrashTime = "";
                    if (Enum.IsDefined(typeof(BugCheckCodeList), data.BugCheckCode))
                        _dumpViewerViewModel.BugCheckString = data.BugCheckCode.ToString();
                    else _dumpViewerViewModel.BugCheckString = "";
                    _dumpViewerViewModel.BugCheckCode = "0x" + ((uint)data.BugCheckCode).ToString("X8");
                    switch (data.VersionArchitecture)
                    {
                        case 64:
                        default:
                            _dumpViewerViewModel.Parameter1 = data.BugCheckParameters[1].ToString("X8") + '`' + data.BugCheckParameters[0].ToString("X8");
                            _dumpViewerViewModel.Parameter2 = data.BugCheckParameters[3].ToString("X8") + '`' + data.BugCheckParameters[2].ToString("X8");
                            _dumpViewerViewModel.Parameter3 = data.BugCheckParameters[5].ToString("X8") + '`' + data.BugCheckParameters[4].ToString("X8");
                            _dumpViewerViewModel.Parameter4 = data.BugCheckParameters[7].ToString("X8") + '`' + data.BugCheckParameters[6].ToString("X8");
                            break;
                        case 32:
                            _dumpViewerViewModel.Parameter1 = "0x" + data.BugCheckParameters[0].ToString("X8");
                            _dumpViewerViewModel.Parameter2 = "0x" + data.BugCheckParameters[1].ToString("X8");
                            _dumpViewerViewModel.Parameter3 = "0x" + data.BugCheckParameters[2].ToString("X8");
                            _dumpViewerViewModel.Parameter4 = "0x" + data.BugCheckParameters[3].ToString("X8");
                            break;
                    }
                    _dumpViewerViewModel.CausedByDriver = "";
                    _dumpViewerViewModel.CausedByAddress = "";
                    _dumpViewerViewModel.Processor = data.MachineImageType.ToString();
                    _dumpViewerViewModel.VersionArchitecture = 'x' + data.VersionArchitecture.ToString();
                    _dumpViewerViewModel.CrashAddress = "";
                    _dumpViewerViewModel.FullPath = openFile.FileName;
                    _dumpViewerViewModel.ProcessorsCount = data.NumberProcessors.ToString();
                    _dumpViewerViewModel.MajorVersion = data.MajorVersion.ToString();
                    _dumpViewerViewModel.MinorVersion = data.MinorVersion.ToString();
                    _dumpViewerViewModel.DumpFileSize = fileInfo.Length.ToString();
                    _dumpViewerViewModel.DumpFileTime = fileInfo.CreationTime.ToString();
                }
                else if (Path.GetExtension(openFile.FileName) != ".dmp")
                    MessageBox.Show("Открыть файл не удалось, так как он не соответствует расширению .dmp.", "Открытие файла", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
