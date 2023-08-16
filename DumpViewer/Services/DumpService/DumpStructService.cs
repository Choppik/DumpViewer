﻿using System;
using System.Text;

namespace DumpViewer.Services.DumpService
{
    public abstract class DumpStructService
    {
        protected DumpStreamService _stream;
        public DumpStreamService Stream { get => _stream; }

        public DumpStructService(DumpStreamService stream)
        {
            _stream = stream;
        }
        /// <summary>
        /// Общий предок для всех ошибок, связанных с использованием Dump Struct.
        /// Сохраняет исходный путь DS, указывающий на элемент, предположительно виновный в
        /// ошибка
        /// </summary>
        public class DumpStructError : Exception
        {
            public DumpStructError(string msg, string srcPath)
                : base(srcPath + ": " + msg)
            {
                this.srcPath = srcPath;
            }

            protected string srcPath;
        }

        /// <summary>
        /// Общий предок для всех ошибок проверки. Сохраняет указатель на
        /// объект DumpStream IO, который был вовлечен в ошибку
        /// </summary>
        public class ValidationFailedError : DumpStructError
        {
            public ValidationFailedError(string msg, DumpStreamService io, string srcPath)
                : base("На позиции " + io.Pos + ": проверка не удалась: " + msg, srcPath)
            {
                this.io = io;
            }

            protected DumpStreamService io;

            protected static string ByteArrayToHex(byte[] arr)
            {
                StringBuilder sb = new StringBuilder("[");
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(string.Format("{0:X2}", arr[i]));
                }
                sb.Append(']');
                return sb.ToString();
            }
        }
        /// <summary>
        /// Ошибка проверки сигналов: мы требовали, чтобы «фактическое» значение было равно
        /// "ожидаемому", а оказалось, что это не так
        /// </summary>
        public class ValidationNotEqualError : ValidationFailedError
        {
            public ValidationNotEqualError(byte[] expected, byte[] actual, DumpStreamService io, string srcPath)
                : base("not equal, expected " + ByteArrayToHex(expected) + ", but got " + ByteArrayToHex(actual), io, srcPath)
            {
                this.expected = expected;
                this.actual = actual;
            }

            public ValidationNotEqualError(Object expected, Object actual, DumpStreamService io, string srcPath)
                : base("not equal, expected " + expected + ", but got " + actual, io, srcPath)
            {
                this.expected = expected;
                this.actual = actual;
            }

            protected object expected;
            protected object actual;
        }

        public class ValidationLessThanError : ValidationFailedError
        {
            public ValidationLessThanError(byte[] min, byte[] actual, DumpStreamService io, string srcPath)
                : base("not in range, min " + ByteArrayToHex(min) + ", but got " + ByteArrayToHex(actual), io, srcPath)
            {
                this.min = min;
                this.actual = actual;
            }

            public ValidationLessThanError(Object min, Object actual, DumpStreamService io, string srcPath)
                : base("not in range, min " + min + ", but got " + actual, io, srcPath)
            {
                this.min = min;
                this.actual = actual;
            }

            protected object min;
            protected object actual;
        }

        public class ValidationGreaterThanError : ValidationFailedError
        {
            public ValidationGreaterThanError(byte[] max, byte[] actual, DumpStreamService io, string srcPath)
                : base("not in range, max " + ByteArrayToHex(max) + ", but got " + ByteArrayToHex(actual), io, srcPath)
            {
                this.max = max;
                this.actual = actual;
            }

            public ValidationGreaterThanError(Object max, Object actual, DumpStreamService io, string srcPath)
                : base("not in range, max " + max + ", but got " + actual, io, srcPath)
            {
                this.max = max;
                this.actual = actual;
            }

            protected object max;
            protected object actual;
        }

        public class ValidationNotAnyOfError : ValidationFailedError
        {
            public ValidationNotAnyOfError(Object actual, DumpStreamService io, string srcPath)
                : base("not any of the list, got " + actual, io, srcPath)
            {
                this.actual = actual;
            }

            protected object actual;
        }

        public class ValidationExprError : ValidationFailedError
        {
            public ValidationExprError(Object actual, DumpStreamService io, string srcPath)
                : base("not matching the expression, got " + actual, io, srcPath)
            {
                this.actual = actual;
            }

            protected object actual;
        }
    }
}