using System;
using System.Collections.Generic;
using System.Globalization;
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
        protected byte[] ReadBytesNormalisedLittleEndian(int count)
        {
            byte[] bytes = ReadBytes(count);
            if (!_isLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// Чтение байт из патока в порядке от старшего к младшему байту и их преобразование в порядок, соответствующий текущей платформе
        /// </summary>
        /// <param name="count">Количество байт для чтения</param>
        /// <returns>Байты преобразованы в порядок, соответствующий текущей платформе</returns>
        protected byte[] ReadBytesNormalisedBigEndian(int count)
        {
            byte[] bytes = ReadBytes(count);
            if (_isLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// Чтение завершенной строки в потоке
        /// </summary>
        /// <param name="terminator">Значение конца строки</param>
        /// <param name="includeTerminator">Обозначает, что необходимо включить конец строки в итоговую строку</param>
        /// <param name="consumeTerminator">Обозначает, что нужно сместить позицию в потоке на один байт</param>
        /// <param name="eosError">Выдать ошибку, если поток достиг конца, а завершающая строка не была найдена</param>
        /// <returns>Возвращает массив байт</returns>
        public byte[] ReadBytesTerm(byte terminator, bool includeTerminator, bool consumeTerminator, bool eosError)
        {
            List<byte> bytes = new();
            while (true)
            {
                if (IsEof)
                {
                    if (eosError) throw new EndOfStreamException(string.Format("Достигнут конец потока, но завершающая строка `{0}` не найдена", terminator));
                    break;
                }

                byte b = ReadByte();
                if (b == terminator)
                {
                    if (includeTerminator) bytes.Add(b);
                    if (!consumeTerminator) Seek(Pos - 1);
                    break;
                }
                bytes.Add(b);
            }
            return bytes.ToArray();
        }
        public static byte[] BytesStripRight(byte[] src, byte padByte)
        {
            int newLen = src.Length;
            while (newLen > 0 && src[newLen - 1] == padByte)
                newLen--;

            byte[] dst = new byte[newLen];
            Array.Copy(src, dst, newLen);
            return dst;
        }
        public static byte[] BytesTerminate(byte[] src, byte terminator, bool includeTerminator)
        {
            int newLen = 0;
            int maxLen = src.Length;

            while (newLen < maxLen && src[newLen] != terminator)
                newLen++;

            if (includeTerminator && newLen < maxLen)
                newLen++;

            byte[] dst = new byte[newLen];
            Array.Copy(src, dst, newLen);
            return dst;
        }
        #endregion

        #region Обработка байтового массива
        /// <summary>
        /// Выполняет операцию XOR с заданными данными, происходит объединение каждого байта с определенным значением.
        /// </summary>
        /// <param name="data">Данные для обработки</param>
        /// <param name="key">Определенное значение для XOR</param>
        /// <returns>Возвращает обработанный массив байт</returns>
        public byte[] ProcessXor(byte[] data, int key)
        {
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ key);
            }
            return result;
        }
        /// <summary>
        /// Выполняет операцию XOR с заданными данными, происходит объединение каждого байта с определенным значением из массива
        /// array, repeating from the beginning of the key array if necessary
        /// </summary>
        /// <param name="data">Данные для обработки</param>
        /// <param name="key">Определенные значения в массиви для XOR</param>
        /// <returns>Возвращает обработанный массив байт</returns>
        public byte[] ProcessXor(byte[] data, byte[] key)
        {
            int keyLen = key.Length;
            byte[] result = new byte[data.Length];
            for (int i = 0, j = 0; i < data.Length; i++, j = (j + 1) % keyLen)
            {
                result[i] = (byte)(data[i] ^ key[j]);
            }
            return result;
        }
        /// <summary>
        /// Выполняется циклический сдвиг в заданном буфере на определенное количество байт
        /// Для сдвига влево необходимо передать неотрицательное число, а вправо наоборот
        /// </summary>
        /// <param name="data">Данные для сдвига</param>
        /// <param name="amount">Количество байт для сдвига</param>
        /// <param name="groupSize">Количество групп</param>
        /// <returns>Возвращает массив байт</returns>
        public byte[] ProcessRotate(byte[] data, int amount, int groupSize)
        {
            if (amount > 7 || amount < -7) throw new ArgumentException("Сдвиг более чем на 7 байт не может быть выполнено.", nameof(amount));
            if (amount < 0) amount += 8; // Сдвиг -2 такой же как сдвиг +6

            byte[] result = new byte[data.Length];
            switch (groupSize)
            {
                case 1:
                    for (int i = 0; i < data.Length; i++)
                    {
                        byte bits = data[i];
                        result[i] = (byte)((bits << amount) | (bits >> (8 - amount)));
                    }
                    break;
                default:
                    throw new NotImplementedException(string.Format("Не удалось выполнить сдвиг из {0} групп", groupSize));
            }
            return result;
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
        public short ReadS2() => _isLittleEndian ? BitConverter.ToInt16(ReadBytesNormalisedBigEndian(2), 0) : BitConverter.ToInt16(ReadBytesNormalisedLittleEndian(2), 0);
        /// <summary>
        /// Чтение четырех байт в потоке взависимости от порядка типа int
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public int ReadS4() => _isLittleEndian ? BitConverter.ToInt32(ReadBytesNormalisedBigEndian(4), 0) : BitConverter.ToInt32(ReadBytesNormalisedLittleEndian(4), 0);
        /// <summary>
        /// Чтение восьми байт в потоке взависимости от порядка типа long
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public long ReadS8() => _isLittleEndian ? BitConverter.ToInt64(ReadBytesNormalisedBigEndian(8), 0) : BitConverter.ToInt64(ReadBytesNormalisedLittleEndian(8), 0);
        /// <summary>
        /// Чтение одного байта в потоке типа byte
        /// </summary>
        /// <returns>Возвращает байт определенного типа</returns>
        public byte ReadU1() => ReadByte();
        /// <summary>
        /// Чтение двух байт в потоке взависимости от порядка типа ushort
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public ushort ReadU2() => _isLittleEndian ? BitConverter.ToUInt16(ReadBytesNormalisedBigEndian(2), 0) : BitConverter.ToUInt16(ReadBytesNormalisedLittleEndian(2), 0);
        /// <summary>
        /// Чтение четырех байт в потоке взависимости от порядка типа uint
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public uint ReadU4() => _isLittleEndian ? BitConverter.ToUInt32(ReadBytesNormalisedBigEndian(4), 0) : BitConverter.ToUInt32(ReadBytesNormalisedLittleEndian(4), 0);
        /// <summary>
        /// Чтение восьми байт в потоке взависимости от порядка типа long
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public ulong ReadU8() => _isLittleEndian ? BitConverter.ToUInt64(ReadBytesNormalisedBigEndian(8), 0) : BitConverter.ToUInt64(ReadBytesNormalisedLittleEndian(8), 0);
        /// <summary>
        /// Чтение четырех байт в потоке взависимости от порядка типа float
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public float ReadF4() => _isLittleEndian ? BitConverter.ToSingle(ReadBytesNormalisedLittleEndian(4), 0) : BitConverter.ToSingle(ReadBytesNormalisedLittleEndian(4), 0);
        /// <summary>
        /// Чтение восьми байт в потоке взависимости от порядка типа double
        /// </summary>
        /// <returns>Возвращает байты в нужном порядке и определенного типа</returns>
        public double ReadF8() => _isLittleEndian ? BitConverter.ToDouble(ReadBytesNormalisedBigEndian(8), 0) : BitConverter.ToDouble(ReadBytesNormalisedLittleEndian(8), 0);
        #endregion

        #region Невыровненные биты значений
        /// <summary>
        /// Выравнивание обнуляется
        /// </summary>
        public void AlignToByte()
        {
            _bitsLeft = 0;
            _bits = 0;
        }
        /// <summary>
        /// Чтение n-битного целого числа из потока
        /// </summary>
        /// <returns>Возвращает измененное число</returns>
        public ulong ReadBitsInt(int n)
        {
            ulong newBits;
            ulong mask;
            ulong res = 0;

            int bitsNeeded = n - _bitsLeft;
            if (_isLittleEndian) _bitsLeft = -bitsNeeded & 7; // `-bitsNeeded mod 8`

            if (bitsNeeded > 0)
            {
                // 1 bit  => 1 byte
                // 8 bits => 1 byte
                // 9 bits => 2 bytes
                int bytesNeeded = ((bitsNeeded - 1) / 8) + 1; // `ceil(bitsNeeded / 8)`
                byte[] buf = ReadBytes(bytesNeeded);
                for (int i = 0; i < bytesNeeded; i++)
                {
                    if (_isLittleEndian) res = res << 8 | buf[i];
                    else res |= ((ulong)buf[i]) << (i * 8);
                }
                switch (_isLittleEndian)
                {
                    case true:
                    default:
                        newBits = res;
                        res = res >> _bitsLeft | _bits << bitsNeeded;
                        break;
                    case false:
                        newBits = bitsNeeded < 64 ? res >> bitsNeeded : 0;
                        res = res << _bitsLeft | _bits;
                        break;
                }
                _bits = newBits; // will be masked at the end of the function
            }
            else
            {
                switch (_isLittleEndian)
                {
                    case true:
                    default:
                        res = _bits >> -bitsNeeded; // shift unneeded bits out
                        break;
                    case false:
                        res = _bits;
                        _bits >>= n;
                        break;
                }
            }
            switch (_isLittleEndian)
            {
                case true:
                default:
                    mask = (1UL << _bitsLeft) - 1; // `BitsLeft` is in range 0..7, so `(1UL << 64)` does not have to be considered
                    _bits &= mask;
                    break;
                case false:
                    _bitsLeft = -bitsNeeded & 7; // `-bitsNeeded mod 8`
                    if (n < 64)
                    {
                        mask = (1UL << n) - 1;
                        res &= mask;
                    }
                    break;
            }
            return res;
        }
        #endregion

        #region Дополнительные служебные методы
        /// <summary>
        /// Performs modulo operation between two integers.
        /// </summary>
        /// <remarks>
        /// This method is required because C# lacks a "true" modulo
        /// operator, the % operator rather being the "remainder"
        /// operator. We want mod operations to always be positive.
        /// </remarks>
        /// <param name="a">The value to be divided</param>
        /// <param name="b">The value to divide by. Must be greater than zero.</param>
        /// <returns>The result of the modulo opertion. Will always be positive.</returns>
        public static int Mod(int a, int b)
        {
            if (b <= 0) throw new ArgumentException("Divisor of mod operation must be greater than zero.", "b");
            int r = a % b;
            if (r < 0) r += b;
            return r;
        }

        /// <summary>
        /// Performs modulo operation between two integers.
        /// </summary>
        /// <remarks>
        /// This method is required because C# lacks a "true" modulo
        /// operator, the % operator rather being the "remainder"
        /// operator. We want mod operations to always be positive.
        /// </remarks>
        /// <param name="a">The value to be divided</param>
        /// <param name="b">The value to divide by. Must be greater than zero.</param>
        /// <returns>The result of the modulo opertion. Will always be positive.</returns>
        public static long Mod(long a, long b)
        {
            if (b <= 0) throw new ArgumentException("Divisor of mod operation must be greater than zero.", "b");
            long r = a % b;
            if (r < 0) r += b;
            return r;
        }

        /// <summary>
        /// Compares two byte arrays in lexicographical order.
        /// </summary>
        /// <returns>negative number if a is less than b, <c>0</c> if a is equal to b, positive number if a is greater than b.</returns>
        /// <param name="a">First byte array to compare</param>
        /// <param name="b">Second byte array to compare.</param>
        public static int ByteArrayCompare(byte[] a, byte[] b)
        {
            if (a == b)
                return 0;
            int al = a.Length;
            int bl = b.Length;
            int minLen = al < bl ? al : bl;
            for (int i = 0; i < minLen; i++)
            {
                int cmp = a[i] - b[i];
                if (cmp != 0)
                    return cmp;
            }

            // Reached the end of at least one of the arrays
            if (al == bl)
            {
                return 0;
            }
            else
            {
                return al - bl;
            }
        }

        /// <summary>
        /// Reverses the string, Unicode-aware.
        /// </summary>
        /// <a href="https://stackoverflow.com/a/15029493">taken from here</a>
        public static string StringReverse(string s)
        {
            TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(s);

            List<string> elements = new();
            while (enumerator.MoveNext())
                elements.Add(enumerator.GetTextElement());

            elements.Reverse();
            return string.Concat(elements);
        }
        #endregion
    }
}
