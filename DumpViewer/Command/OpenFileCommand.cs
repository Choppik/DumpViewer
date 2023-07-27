using DumpViewer.Command.Base;
using DumpViewer.Services;
using DumpViewer.ViewModels;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using static DumpViewer.Services.ReadDumpService;

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
                    /*using FileStream fileStream = new(openFile.FileName, FileMode.Open, FileAccess.Read);
                    {
                        MINIDUMP_MODULE_LIST moduleList = (MINIDUMP_MODULE_LIST)Marshal.PtrToStructure(streamPointer, typeof(MINIDUMP_MODULE_LIST));

                        MINIDUMP_DIRECTORY directory = new MINIDUMP_DIRECTORY();
                        var streamPointer = IntPtr.Zero;
                        var streamSize = 0;

                        // baseOfView is the IntPtr we got when we created the memory mapped file
                        if (!ReadDumpService.MiniDumpReadDumpStream(baseOfView, MINIDUMP_STREAM_TYPE.ModuleListStream, ref directory, ref streamPointer, ref streamSize))
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                    }*/

                }
                else if (Path.GetExtension(openFile.FileName) != ".dmp")
                    MessageBox.Show("Открыть файл не удалось, так как он не соответствует расширению .dmp", "Открытие файла", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}