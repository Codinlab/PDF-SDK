using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using DocumentFormat.Pdf.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentFormat.Pdf.Extensions
{
    /// <summary>
    /// <see cref="PdfReader"/> extension methods
    /// </summary>
    public static class PdfReaderExtensions
    {
        /// <summary>
        /// Reads a line of characters from the current PDF stream and returns the data as a string.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>The next line from the PDF stream.</returns>
        public static string ReadLine(this PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var result = reader.ReadWhile(c => !Chars.IsEndOfLine(c));
            reader.SkipWhile(Chars.IsEndOfLine);
            return result;
        }

        /// <summary>
        /// Move reader's position to first non-whitespace character.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        public static void MoveToNonWhiteSpace(this PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            reader.SkipWhile(Chars.IsWhiteSpace);
        }

        /// <summary>
        /// Move reader's position to first non-whitespace character,
        /// and returns first non-whitespace character read.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>First read non-whitespace character.</returns>
        public static char ReadNonWhiteSpace(this PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            char result;
            do
            {
                result = reader.Read();
            }
            while (Chars.IsWhiteSpace(result));
            return result;
        }

        /// <summary>
        /// Reads specified token from PDF stream.
        /// The method will throw if it reads a different string
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <param name="token">The token to read.</param>
        public static void ReadToken(this PdfReader reader, string token)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (reader.ReadString(token.Length) != token)
                throw new FormatException($"The token \"{token}\" was expected.");
        }

        /// <summary>
        /// Find Cross-Reference Table position.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>The Cross-Reference Table position.</returns>
        public static long GetXRefPosition(this PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            // Acrobat viewers require only that the %%EOF marker appear somewhere within the last 1024 bytes of the file.
            long startXrefSearchStartIndex = reader.Length > 1024 + PdfDocument.PdfHeader.Length ? reader.Length - 1024 : PdfDocument.PdfHeader.Length;
            int searchLength = (int)(reader.Length - startXrefSearchStartIndex);

            reader.Position = startXrefSearchStartIndex;
            var buffer = new char[searchLength];

            if (searchLength != reader.Read(buffer, 0, searchLength))
                throw new FormatException("Unexpected end of file");

            string fileEnd = new string(buffer);

            int toto = fileEnd.Length;

            var eofIndex = fileEnd.LastIndexOf(PdfDocument.EofMarker);
            if (eofIndex < 0)
                throw new FormatException("Missing file trailer");

            var startXRefIndex = fileEnd.LastIndexOf(XRefTable.StartXRefToken, eofIndex);

            if (startXRefIndex < 0)
                throw new FormatException("Missing startxref token");

            reader.Position = startXrefSearchStartIndex + startXRefIndex + XRefTable.StartXRefToken.Length;
            reader.SkipWhile(Chars.IsEndOfLine);

            return long.Parse(reader.ReadLine());
        }

        /// <summary>
        /// Reads next <see cref="PdfObject"/> from PDF stream.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>Read PdfObject.</returns>
        public static PdfObject ReadObject(this PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            reader.MoveToNonWhiteSpace();

            char firstChar = reader.Peek();
            switch (firstChar)
            {
                case '%':
                    // Skip comment
                    reader.SkipWhile(c => !Chars.IsEndOfLine(c));
                    reader.SkipWhile(Chars.IsEndOfLine);
                    // Then try read next object
                    return ReadObject(reader);
                case 't':
                    reader.ReadToken(BooleanObject.TrueToken);
                    return new BooleanObject(true);
                case 'f':
                    reader.ReadToken(BooleanObject.FalseToken);
                    return new BooleanObject(false);
                case 'n':
                    reader.ReadToken(NullObject.NullToken);
                    return new NullObject();
                case '(':
                    return LiteralStringObject.FromReader(reader);
                case '<':
                    if (reader.Peek(1) == '<')
                    {
                        return DictionaryObject.FromReader(reader);
                    }
                    else
                    {
                        return HexadecimalStringObject.FromReader(reader);
                    }
                case '/':
                    return NameObject.FromReader(reader);
                case '[':
                    return ArrayObject.FromReader(reader);
                case '+':
                case '-':
                    return NumericObject.FromReader(reader);
            }
            if (char.IsDigit(firstChar))
            {
                int indRefLength = IsIndirectReference(reader);
                if(indRefLength >= 5)
                {
                    var indRefStr = reader.ReadString(indRefLength);
                    var objectId = PdfObjectId.FromString(indRefStr.Substring(0, indRefLength - 2));

                    return new IndirectReference(objectId);
                }

                return NumericObject.FromReader(reader);
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tests if the reader is reading an IndirectObject reference.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>Indirect reference length if it's an IndirectObject reference, otherwise -1.</returns>
        private static int IsIndirectReference(PdfReader reader)
        {
            int i = 0;
            char c = reader.Peek(i++);

            if (!char.IsDigit(c))
                return -1;

            do
            {
                c = reader.Peek(i++);
            }
            while (char.IsDigit(c));

            if (c != Chars.SP)
                return -1;

            c = reader.Peek(i++);
            if (!char.IsDigit(c))
                return -1;

            do
            {
                c = reader.Peek(i++);
            }
            while (char.IsDigit(c));

            if (c != Chars.SP)
                return -1;

            c = reader.Peek(i++);
            if (c == 'R')
                return i;
            else
                return -1;
        }
    }
}
