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
                    //_dumpViewerViewModel.СrashTime = "";
                    if (Enum.IsDefined(typeof(BugCheckCodeList), data.BugCheckCode))
                        _dumpViewerViewModel.BugCheckString = data.BugCheckCode.ToString();
                    else _dumpViewerViewModel.BugCheckString = "";
                    _dumpViewerViewModel.BugCheckCode = "0x" + ((uint)data.BugCheckCode).ToString("X8");
                    switch (data.VersionArchitecture)
                    {
                        case 64:
                        default:
                            for (int i = 0, j = _dumpViewerViewModel.Parameters.Count - 1; i < data.BugCheckParameters.Length - 1; i += data.BugCheckParameters.Length / 4, j--)
                            {
                                _dumpViewerViewModel.Parameters.Add(data.BugCheckParameters[i + 1].ToString("X8") + '`' + data.BugCheckParameters[i].ToString("X8"));
                                _dumpViewerViewModel.Parameters.RemoveAt(j);
                            }
                            break;
                        case 32:
                            for (int i = 0, j = _dumpViewerViewModel.Parameters.Count - 1; i < data.BugCheckParameters.Length; i++, j--)
                            {
                                _dumpViewerViewModel.Parameters.Add("0x" + data.BugCheckParameters[i].ToString("X8"));
                                _dumpViewerViewModel.Parameters.RemoveAt(j);
                            }
                            break;
                    }
                    //_dumpViewerViewModel.CausedByDriver = "";
                    //_dumpViewerViewModel.CausedByAddress = "";
                    _dumpViewerViewModel.Processor = data.MachineImageType.ToString();
                    _dumpViewerViewModel.VersionArchitecture = 'x' + data.VersionArchitecture.ToString();
                    //_dumpViewerViewModel.CrashAddress = "";
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