using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DumpViewer.Services
{
    public class ReadDumpService
    {
        public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const uint SECTION_QUERY = 0x0001;
        public const uint SECTION_MAP_WRITE = 0x0002;
        public const uint SECTION_MAP_READ = 0x0004;
        public const uint SECTION_MAP_EXECUTE = 0x0008;
        public const uint SECTION_EXTEND_SIZE = 0x0010;
        public const uint SECTION_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
            SECTION_QUERY |
            SECTION_MAP_WRITE |
            SECTION_MAP_READ |
            SECTION_MAP_EXECUTE |
            SECTION_EXTEND_SIZE);

        public const uint FILE_MAP_COPY = SECTION_QUERY;
        public const uint FILE_MAP_WRITE = SECTION_MAP_WRITE;
        public const uint FILE_MAP_READ = SECTION_MAP_READ;
        public const uint FILE_MAP_EXECUTE = SECTION_MAP_EXECUTE;
        public const uint FILE_MAP_ALL_ACCESS = SECTION_ALL_ACCESS;

        //Определяет тип информации, которая будет записана в файл минидампа функцией MiniDumpWriteDump
        public enum MINIDUMP_TYPE
        {
            MiniDumpNormal = 0x00000000,
            MiniDumpWithDataSegs = 0x00000001,
            MiniDumpWithFullMemory = 0x00000002,
            MiniDumpWithHandleData = 0x00000004,
            MiniDumpFilterMemory = 0x00000008,
            MiniDumpScanMemory = 0x00000010,
            MiniDumpWithUnloadedModules = 0x00000020,
            MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
            MiniDumpFilterModulePaths = 0x00000080,
            MiniDumpWithProcessThreadData = 0x00000100,
            MiniDumpWithPrivateReadWriteMemory = 0x00000200,
            MiniDumpWithoutOptionalData = 0x00000400,
            MiniDumpWithFullMemoryInfo = 0x00000800,
            MiniDumpWithThreadInfo = 0x00001000,
            MiniDumpWithCodeSegs = 0x00002000,
            MiniDumpWithoutAuxiliaryState = 0x00004000,
            MiniDumpWithFullAuxiliaryState = 0x00008000,
            MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
            MiniDumpIgnoreInaccessibleMemory = 0x00020000,
            MiniDumpWithTokenInformation = 0x00040000,
            MiniDumpWithModuleHeaders = 0x00080000,
            MiniDumpFilterTriage = 0x00100000,
            MiniDumpWithAvxXStateContext = 0x00200000,
            MiniDumpWithIptTrace = 0x00400000,
            MiniDumpScanInaccessiblePartialPages = 0x00800000,
            MiniDumpFilterWriteCombinedMemory,
            MiniDumpValidTypeFlags = 0x01ffffff
        }

        //Можно получить доступ к различным потокам, передав разные значения с StreamNumber с помощью данного перечисления
        public enum MINIDUMP_STREAM_TYPE : uint
        {
            UnusedStream = 0,
            ReservedStream0 = 1,
            ReservedStream1 = 2,
            ThreadListStream = 3,
            ModuleListStream = 4,
            MemoryListStream = 5,
            ExceptionStream = 6,
            SystemInfoStream = 7,
            ThreadExListStream = 8,
            Memory64ListStream = 9,
            CommentStreamA = 10,
            CommentStreamW = 11,
            HandleDataStream = 12,
            FunctionTableStream = 13,
            UnloadedModuleListStream = 14,
            MiscInfoStream = 15,
            MemoryInfoListStream = 16,
            ThreadInfoListStream = 17,
            HandleOperationListStream = 18,
            LastReservedStream = 0xffff
        }

        /// <summary>
        /// Метод для чтения данных из минидампа
        /// </summary>
        /// <param name="BaseOfDump">Указатель на базу сопоставленного файла минидампа. Файл должен быть отображен в память с помощью функции MapViewOfFile</param>
        /// <param name="StreamNumber">Тип данных, которые нужно прочитать из файла минидампа. Этот член может быть одним из значений перечисления MINIDUMP_STREAM_TYPE</param>
        /// <param name="Dir">Указатель на структуру MINIDUMP_DIRECTORY</param>
        /// <param name="StreamPointer">Указатель на начало потока минидампа. Формат этого потока зависит от значения StreamNumber</param>
        /// <param name="StreamSize">Размер потока, на который указывает StreamPointer, в байтах</param>
        /// <returns></returns>
        [DllImport("dbghelp.dll", SetLastError = true)] //В данной библиотеке находятся все функции для взаимодействия с минидампами
        public static extern bool MiniDumpReadDumpStream(
            IntPtr BaseOfDump,
            uint StreamNumber,
            ref MINIDUMP_DIRECTORY Dir,
            ref IntPtr StreamPointer,
            ref uint StreamSize);

        /// <summary>
        /// Метод необходим для выяснения BaseOfDump
        /// </summary>
        /// <param name="hFileMappingObject">Дескриптор объекта сопоставления файлов. Функции CreateFileMapping и OpenFileMapping возвращают этот дескриптор.</param>
        /// <param name="dwDesiredAccess">Тип доступа к объекту сопоставления файлов, который определяет защиту страниц страниц. Этот параметр может быть одним из текущих констант</param>
        /// <param name="dwFileOffsetHigh">DWORD с высоким порядком смещения файла, с которого начинается представление</param>
        /// <param name="dwFileOffsetLow">DWORD с низким порядком смещения файла, в котором начинается представление. Сочетание высоких и низких смещения должно указывать смещение в сопоставлении файлов. Они также должны соответствовать степени детализации выделения памяти системы. То есть смещение должно быть кратным степенью детализации выделения</param>
        /// <param name="dwNumberOfBytesToMap">Количество байтов сопоставления файлов для сопоставления с представлением. Все байты должны находиться в пределах максимального размера, указанного в CreateFileMapping. Если этот параметр равен 0 (ноль), сопоставление расширяется от указанного смещения до конца сопоставления файлов</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(
            SafeHandle hFileMappingObject,
            uint dwDesiredAccess,
            uint dwFileOffsetHigh,
            uint dwFileOffsetLow,
            IntPtr dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        //Содержит сведения, описывающие расположение потока данных в файле минидампа
        public struct MINIDUMP_LOCATION_DESCRIPTOR
        {
            public UInt32 DataSize; //Размер потока данных в байтах
            public uint Rva; //Относительный виртуальный адрес (RVA) данных. Это смещение потока данных байтов от начала файла минидампа
        }

        //Содержит сведения, необходимые для доступа к определенному потоку данных в файле минидампа
        public struct MINIDUMP_DIRECTORY
        {
            public UInt32 StreamType; //Этот элемент может быть одним из значений перечисления MINIDUMP_STREAM_TYPE
            public MINIDUMP_LOCATION_DESCRIPTOR Location;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct MINIDUMP_MODULE_LIST
        {
            public uint NumberOfModules;
            public IntPtr Modules;
        }

        /*protected unsafe T ReadStream(IntPtr baseOfView, MINIDUMP_STREAM_TYPE streamToRead)
        {
            MINIDUMP_DIRECTORY directory = new MINIDUMP_DIRECTORY();
            IntPtr streamPointer = IntPtr.Zero;
            uint streamSize = 0;

            if (!MiniDumpReadDumpStream(baseOfView, streamToRead, ref directory, ref streamPointer, ref streamSize))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return (T)Marshal.PtrToStructure(streamPointer, typeof(T));
        }*/
    }
}