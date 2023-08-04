using DumpViewer.Command.Base;
using DumpViewer.Services.DumpService;
using DumpViewer.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Windows;

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
                    DumpWindows dumpWindows = new(new DumpStreamService(openFile.FileName));
                    _dumpViewerViewModel.ResultStr = dumpWindows.NumStreams.ToString();
                }



            }
            else if (Path.GetExtension(openFile.FileName) != ".dmp")
                MessageBox.Show("Открыть файл не удалось, так как он не соответствует расширению .dmp", "Открытие файла", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
