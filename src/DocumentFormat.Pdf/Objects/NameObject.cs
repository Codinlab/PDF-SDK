﻿using DocumentFormat.Pdf.Attributes;
using DocumentFormat.Pdf.Exceptions;
using DocumentFormat.Pdf.IO;
using System;
using System.Globalization;
using System.Text;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Name Object.
    /// </summary>
    [HasDelimiters(AtEnd = false)]
    public class NameObject : PdfObject
    {
        /// <summary>
        /// NameObject's start token
        /// </summary>
        public const char StartToken = '/';

        private string value;

        /// <summary>
        /// Instanciates a new NameObject.
        /// </summary>
        /// <param name="value">The object's value.</param>
        public NameObject(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Instanciates a new NameObject.
        /// </summary>
        /// <param name="value">The object's value.</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        protected NameObject(string value, bool isReadOnly) : base(isReadOnly)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the object's value.
        /// </summary>
        public string Value {
            get => value;
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                this.value = value;
            }
        }

        /// <summary>
        /// Writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public override void Write(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            WriteName(writer, value);
        }

        /// <summary>
        /// Creates a Name object from PdfReader.
        /// Read stream must start with '/' delimiter.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>Created NameObject.</returns>
        public static NameObject FromReader(PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            string name = ReadName(reader);

            return new NameObject(name);
        }

        /// <summary>
        /// Reads name from PdfReader.
        /// Read stream must start with '/' delimiter.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>Read name.</returns>
        internal static string ReadName(PdfReader reader)
        {
            if (reader.Read() != StartToken)
                throw new FormatException("A name object was expected.");

            var readChars = reader.ReadWhile(c => !Chars.IsDelimiterOrWhiteSpace(c));

            if (readChars.Length == 0)
            {
                return "";
            }

            var sb = new StringBuilder();
            for (var i = 0; i < readChars.Length; i++)
            {
                if (readChars[i] == '#')
                {
                    if (i < readChars.Length - 2)
                    {
                        // UTF-8 character
                        var hex = new char[] { readChars[i + 1], readChars[i + 2] };
                        sb.Append((char)byte.Parse(new string(hex), NumberStyles.HexNumber));
                        i += 2;
                    }
                }
                else
                {
                    sb.Append(readChars[i]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Writes name to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        /// <param name="value">The name value to write.</param>
        internal static void WriteName(PdfWriter writer, string value)
        {
            var sb = new StringBuilder();
            sb.Append(StartToken);

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] < 33 || value[i] > 126 || value[i] == '#' || Chars.IsDelimiter(value[i]))
                {
                    // UTF-8 code
                    sb.AppendFormat("#{0:x2}", (byte)value[i]);
                }
                else
                {
                    sb.Append(value[i]);
                }
            }

            writer.Write(sb.ToString());
        }
    }
}
