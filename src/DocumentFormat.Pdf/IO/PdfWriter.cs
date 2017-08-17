using System;
using System.IO;
using System.Text;

namespace DocumentFormat.Pdf.IO
{
    /// <summary>
    /// Represents a Pdf document reader
    /// </summary>
    public class PdfWriter : IDisposable
    {
        private static Encoding encoding = Encoding.GetEncoding("ASCII");

        private bool disposed = false;

        private Encoder encoder;

        private Stream pdfStream;

        private readonly byte[] EOL;

        /// <summary>
        /// Gets or sets the position of the writer in pdf stream
        /// </summary>
        public long Position {
            get {
                return pdfStream.Position;
            }
            set {
                pdfStream.Position = value;
            }
        }

        /// <summary>
        /// Gets the length of the PDF stream.
        /// </summary>
        public long Length {
            get => pdfStream.Length;
        }

        /// <summary>
        /// Initializes a new instance of PdfWriter for the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="pdfStream">Writed <see cref="Stream"/>.</param>
        public PdfWriter(Stream pdfStream)
        {
            this.pdfStream = pdfStream ?? throw new ArgumentNullException(nameof(pdfStream));
            encoder = encoding.GetEncoder();
            // TODO : Make EOL configurable.
            EOL = new byte[] { (byte)Chars.CR, (byte)Chars.LF };
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within the PDF stream
        /// by the number of bytes written.
        /// </summary>
        /// <param name="data">The array of bytes to write.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes.</param>
        /// <param name="length">The number of bytes to be written to the stream.</param>
        public void Write(byte[] data, int offset, int length)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "Cannot be negative");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "Cannot be negative");
            if (data.Length - offset < length)
                throw new ArgumentException("Invalid data length");

            pdfStream.Write(data, offset, length);
        }

        /// <summary>
        /// Writes an single characters to the current stream and advances the current position within the PDF stream by one.
        /// </summary>
        /// <param name="value">The character to write.</param>
        public void Write(char value)
        {
            Write(new char[] { value });
        }

        /// <summary>
        /// Writes an array of characters to the current stream and advances the current position within the PDF stream
        /// by the number of characters written.
        /// </summary>
        /// <param name="buffer">The array of characters to write.</param>
        public void Write(char[] buffer)
        {
            if (buffer == null)
                throw new NullReferenceException(nameof(buffer));

            var byteBuffer = new byte[buffer.Length];
            encoder.GetBytes(buffer, 0, buffer.Length, byteBuffer, 0, true);

            pdfStream.Write(byteBuffer, 0, byteBuffer.Length);
        }

        /// <summary>
        /// Writes a string to the current stream and advances the current position within the PDF stream
        /// by the number of characters written.
        /// </summary>
        /// <param name="value">The string to write. If value is null, nothing is written.</param>
        public void Write(string value)
        {
            if (value == null)
                return;

            Write(value.ToCharArray());
        }



        /// <summary>
        /// Writes a line terminator to the stream.
        /// </summary>
        public void WriteLine()
        {

            pdfStream.Write(EOL, 0, EOL.Length);
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the stream.
        /// </summary>
        /// <param name="value">The string to write. If the value is null, only a line terminator is written.</param>
        public void WriteLine(string value)
        {
            Write(value);
            WriteLine();
        }

        /// <summary>
        /// Clears all buffers and causes any buffered data to be written to the stream.
        /// </summary>
        public void Flush()
        {
            pdfStream.Flush();
        }

        #region IDisposable
        /// <summary>
        /// Disposes the PdfReader.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases PdfReader ressources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            try
            {
                if (pdfStream != null)
                {
                    pdfStream.Flush();
                }

            }
            finally
            {
                if (disposing)
                {
                    // Free managed ressources
                    pdfStream = null;
                    encoder = null;
                }
            }

            disposed = true;
        }

        #endregion
    }
}
