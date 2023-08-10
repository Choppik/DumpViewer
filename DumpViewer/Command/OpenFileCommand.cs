using DumpViewer.Command.Base;
using DumpViewer.Services.DumpService;
using DumpViewer.ViewModels;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using static DumpViewer.Services.DumpService.DumpWindows;

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
                    var data = FromFile(openFile.FileName);
                    //var bg = new SystemInfo(data.M_Io, null, data);
                    ////data.Streams.Add(bg);
                    //_dumpViewerViewModel.ResultStr += bg.ProcessCreateTime.ToString() + "\n";
                    //_dumpViewerViewModel.ResultStr += bg.ProcessCreateTime.ToString() + "\n";
                    //_dumpViewerViewModel.ResultStr += bg.ProcessCreateTime.ToString() + "\n";
                        _dumpViewerViewModel.ResultStr += data.BugCheckCode.ToString("X") + "\n";
                    /*foreach (var stream in data.Streams)
                    {
                        //_dumpViewerViewModel.ResultStr += stream.LenData.ToString() + "\n";
                    }*/
                    //SystemInfo h = new(new DumpStreamService(openFile.FileName));
                    //_dumpViewerViewModel.ResultStr = data.Version.ToString();

                    //_dumpViewerViewModel.ResultStr += h.ProcessorArchitecture.ToString() + "\n";
                   /* _dumpViewerViewModel.ResultStr += data.Flags.ToString() + "\n";
                    _dumpViewerViewModel.ResultStr += data.OfsStreams.ToString() + "\n";
                    _dumpViewerViewModel.ResultStr += bg.StreamType.ToString() + "\n";*/


                }
            }
            else if (Path.GetExtension(openFile.FileName) != ".dmp")
                MessageBox.Show("Открыть файл не удалось, так как он не соответствует расширению .dmp", "Открытие файла", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
