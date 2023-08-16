using System;
using System.IO;
using System.Text;

namespace DumpViewer.Services.DumpService
{
    public class DumpStreamService : BinaryReader
    {
        #region Конструкторы
        public DumpStreamService(Stream stream) : base(stream) { }
        public DumpStreamService(Stream stream, Encoding encoding) : base(stream, encoding) { }
        public DumpStreamService(Stream stream, Encoding encoding, bool leaveOpen) : base(stream, encoding, leaveOpen) { }
        public DumpStreamService(byte[] bytes) : base(new MemoryStream(bytes)) { }
        public DumpStreamService(string file) : base(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read)) { }

        private int _bitsLeft = 0;
        private ulong _bits = 0;
        static readonly bool _isLittleEndian = BitConverter.IsLittleEndian;
        #endregion

        #region Позиционирование в потоке
        /// <summary>
        /// Проверка на конец потока
        /// </summary>
        public bool IsEof { get => BaseStream.Position >= BaseStream.Length && _bitsLeft == 0; }
        /// <summary>
        /// Получить текущую позицию в потоке
        /// </summary>
        public long Pos { get => BaseStream.Position; }
        /// <summary>
        /// Получить длинну потока (размер файла)
        /// </summary>
        public long Size { get => BaseStream.Length; }
        /// <summary>
        /// Установка текущей позиции в потоке от его начала
        /// </summary>
        /// <param name="position">Текущая позиция</param>
        public void Seek(long position) => BaseStream.Seek(position, SeekOrigin.Begin);
        #endregion

        #region Различные массивы байт
        /// <summary>
        /// Чтение фиксированного числа байт в потоке
        /// </summary>
        /// <param name="count">Количество байт для чтения</param>
        /// <returns>Возвращает массив байт</returns>
        public byte[] ReadBytes(long count)
        {
            if (count < 0 || count > Int32.MaxValue)
                throw new ArgumentOutOfRangeException("Запрошено " + count + " байт, в то время как возможно только неотрицательное количество байт");
            byte[] bytes = base.ReadBytes((int)count);
            if (bytes.Length < count)
                throw new EndOfStreamException("Запрошено " + count + " байт, но возможно только " + bytes.Length + " байт");
            return bytes;
        }
        /// <summary>
        /// Чтение байт до самого конца потока
        /// </summary>
        /// <returns>Возвращает массив байт</returns>
        public byte[] ReadBytesFull() => ReadBytes(BaseStream.Length - BaseStream.Position);
        /// <summary>
        /// Чтение байт из патока в порядке от младшего к старшему байту и их преобразование в порядок, соответствующий текущей платформе
        /// </summary>
        /// <param name="count">Количество байт для чтения</param>
        /// <returns>Байты преобразованы в порядок, соответствующий текущей платформе</returns>
        protected byte[] ReadBytesNormalisedEndian(int count)
        {
            byte[] bytes = ReadBytes(count);
            Array.Reverse(bytes);
            return bytes;
        }
        #endregion

        #region Чтение байт в разных типах
        /// <summary>
        /// Чтение одного байта в потоке типа sbyte
        /// </summary>
        /// <returns>Возвращает байт определенного типа</returns>
        public sbyte ReadS1() => ReadSByte();
        /// <summary>
        /// Чтение двух байт в потоке взависимости от порядка типа short
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public short ReadS2() => !_isLittleEndian ? BitConverter.ToInt16(ReadBytesNormalisedEndian(2), 0) : BitConverter.ToInt16(ReadBytes(2), 0);
        /// <summary>
        /// Чтение четырех байт в потоке взависимости от порядка типа int
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public int ReadS4() => !_isLittleEndian ? BitConverter.ToInt32(ReadBytesNormalisedEndian(4), 0) : BitConverter.ToInt32(ReadBytes(4), 0);
        /// <summary>
        /// Чтение восьми байт в потоке взависимости от порядка типа long
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public long ReadS8() => !_isLittleEndian ? BitConverter.ToInt64(ReadBytesNormalisedEndian(8), 0) : BitConverter.ToInt64(ReadBytes(8), 0);
        /// <summary>
        /// Чтение одного байта в потоке типа byte
        /// </summary>
        /// <returns>Возвращает байт определенного типа</returns>
        public byte ReadU1() => ReadByte();
        /// <summary>
        /// Чтение двух байт в потоке взависимости от порядка типа ushort
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public ushort ReadU2() => !_isLittleEndian ? BitConverter.ToUInt16(ReadBytesNormalisedEndian(2), 0) : BitConverter.ToUInt16(ReadBytes(2), 0);
        /// <summary>
        /// Чтение четырех байт в потоке взависимости от порядка типа uint
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public uint ReadU4() => !_isLittleEndian ? BitConverter.ToUInt32(ReadBytesNormalisedEndian(4), 0) : BitConverter.ToUInt32(ReadBytes(4), 0);
        /// <summary>
        /// Чтение восьми байт в потоке взависимости от порядка типа long
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public ulong ReadU8() => !_isLittleEndian ? BitConverter.ToUInt64(ReadBytesNormalisedEndian(8), 0) : BitConverter.ToUInt64(ReadBytes(8), 0);
        /// <summary>
        /// Чтение четырех байт в потоке взависимости от порядка типа float
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public float ReadF4() => !_isLittleEndian ? BitConverter.ToSingle(ReadBytesNormalisedEndian(4), 0) : BitConverter.ToSingle(ReadBytes(4), 0);
        /// <summary>
        /// Чтение восьми байт в потоке взависимости от порядка типа double
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public double ReadF8() => !_isLittleEndian ? BitConverter.ToDouble(ReadBytesNormalisedEndian(8), 0) : BitConverter.ToDouble(ReadBytes(8), 0);
        #endregion

        #region Дополнительные служебные методы
        /// <summary>
        /// Сравнивание двух массивов байт
        /// </summary>
        /// <returns>Возвращает 0, если массивы равны</returns>
        /// <param name="a">Первый байтовый массив для сравнения</param>
        /// <param name="b">Второй байтовый массив для сравнения</param>
        public static int ByteArrayCompare(byte[] a, byte[] b)
        {
            if (a == b) return 0;
            int al = a.Length;
            int bl = b.Length;
            int minLen = al < bl ? al : bl;

            for (int i = 0; i < minLen; i++)
            {
                int cmp = a[i] - b[i];
                if (cmp != 0) return cmp;
            }

            if (al == bl) return 0;
            else return al - bl;
        }
        #endregion
    }
}