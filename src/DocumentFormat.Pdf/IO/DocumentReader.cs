using DocumentFormat.Pdf.Internal;
using DocumentFormat.Pdf.Internal.Objects;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DocumentFormat.Pdf.IO
{
    public class DocumentReader : StreamReader
    {
        public DocumentReader(Stream stream) : base(stream, Encoding.GetEncoding("ASCII")) {
        }

        /// <summary>
        /// sets the position within the base stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type <seealso cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the base stream.</returns>
        public long Seek(long offset, SeekOrigin origin)
        {
            DiscardBufferedData();
            return BaseStream.Seek(offset, origin);
        }

        /// <summary>
        /// Reads Pdf file's header and return Pdf Version
        /// </summary>
        /// <returns>Pdf Version of the file</returns>
        public async Task<PdfVersion> ReadPdfVersionAsync() {
            Seek(0, SeekOrigin.Begin);

            var header = await ReadLineAsync();
            if (header == null || !header.StartsWith(PdfDocument.PdfHeader))
                throw new FormatException("Invalid file header");

            return new PdfVersion(header.Substring(PdfDocument.PdfHeader.Length).TrimStart('-'));
        }

        /// <summary>
        /// Find last startxref entry and returns Cross Reference Table Potion in the file
        /// </summary>
        /// <returns></returns>
        public async Task<long> GetXRefPositionAsync()
        {
            const string StartXRefToken = "startxref";

            long startXrefSearchStartIndex = BaseStream.Length > 1024 + PdfDocument.PdfHeader.Length ? BaseStream.Length - 1024 : PdfDocument.PdfHeader.Length;

            Seek(startXrefSearchStartIndex, SeekOrigin.Begin);
            string fileEnd = await ReadToEndAsync();

            var eofIndex = fileEnd.LastIndexOf(PdfDocument.PdfTrailer);
            if(eofIndex < 0)
            {
                throw new FormatException("Missing file trailer");
            }

            var startXRefIndex = fileEnd.LastIndexOf(StartXRefToken, eofIndex);
            if (startXRefIndex < 0)
            {
                throw new FormatException("Missing startxref token");
            }

            var startXRefPosition = startXrefSearchStartIndex + startXRefIndex;
            DiscardBufferedData();
            Seek(startXRefPosition + StartXRefToken.Length, SeekOrigin.Begin);

            return await ReadInt64Async();
        }

        public async Task<PdfObject> ReadObjectAsync()
        {
            var firstChar = await MoveToNonWhiteSpaceAsync();

            switch (firstChar)
            {
                case '%':
                    // TODO : Read comment
                    throw new NotImplementedException();
                    // Then try read next object
                    return await ReadObjectAsync();
                case '/':
                    // TODO : Read name object
                    throw new NotImplementedException();
                case '+':
                case '-':
                    return await NumericObject.CreateFromReaderAsync(this, firstChar);
            }
            if (char.IsDigit(firstChar))
            {
                return await NumericObject.CreateFromReaderAsync(this, firstChar);
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads stream to next non-whitespace character
        /// </summary>
        /// <returns>Last read non-whitespace character</returns>
        public async Task<char> MoveToNonWhiteSpaceAsync()
        {
            int read;
            char lastRead;
            var buffer = new byte[1];
            do
            {
                read = await BaseStream.ReadAsync(buffer, 0, 1);
                lastRead = (char)buffer[0];
            }
            while (read > 0 && Chars.IsWhiteSpace(lastRead));
            return lastRead;
        }

        /// <summary>
        /// Reads stream up to first whitespace character
        /// </summary>
        /// <returns></returns>
        public async Task<char[]> ReadCharsToWhiteSpaceAsync()
        {
            const int buffSize = 1024;

            int read;
            int bufferWritePos = 0;
            var buffer = new byte[buffSize];
            do
            {
                if(bufferWritePos == buffer.Length)
                {
                    // Needs larger buffer
                    Array.Resize(ref buffer, buffer.Length + buffSize);
                }
                // Read next char
                read = await BaseStream.ReadAsync(buffer, bufferWritePos, 1);
                bufferWritePos += read;
            }
            while (read > 0 && !Chars.IsWhiteSpace((char)buffer[bufferWritePos - 1]));
            return CurrentEncoding.GetChars(buffer, 0, bufferWritePos - 1);
        }

        /// <summary>
        /// Reads a 32 bit integer numeric object
        /// </summary>
        /// <returns>The integer value</returns>
        public async Task<int> ReadInt32Async()
        {
            PdfObject readObject = await ReadObjectAsync();
            if (readObject is NumericObject)
            {
                var numObj = (NumericObject)readObject;
                if (numObj.IsInteger)
                {
                    return numObj.Int32Value;
                }
                else
                {
                    throw new FormatException("An integer value was expected");
                }
            }
            else
            {
                throw new FormatException("Numeric object was expected");
            }
        }


        /// <summary>
        /// Reads a 64 bit integer numeric object
        /// </summary>
        /// <returns>The integer value</returns>
        public async Task<long> ReadInt64Async()
        {
            PdfObject readObject = await ReadObjectAsync();
            if (readObject is NumericObject)
            {
                var numObj = (NumericObject)readObject;
                if (numObj.IsInteger)
                {
                    return numObj.Int64Value;
                }
                else
                {
                    throw new FormatException("An integer value was expected");
                }
            }
            else
            {
                throw new FormatException("Numeric object was expected");
            }
        }
    }
}
