using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DocumentFormat.Pdf.IO
{
    /// <summary>
    /// Represents a Pdf document reader
    /// </summary>
    public class PdfReader
    {
        private static Encoding encoding = Encoding.GetEncoding("ASCII");

        internal static int DefaultBufferSize => 1024;

        private readonly Decoder decoder;
        
        private readonly Stream pdfStream;

        private byte[] byteBuffer;
        private char[] charBuffer;

        private readonly int bufferSize;

        private int readPos = 0;
        private int readLen = 0;

        private volatile Task _asyncReadTask;

        #region Public Properties
        /// <summary>
        /// Gets or sets the position of the reader in pdf stream
        /// </summary>
        public long Position {
            get {
                return pdfStream.Position + readPos - readLen;
            }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Position cannot be negative.");

                long streamPosition = pdfStream.Position;

                if (value < streamPosition && value >= streamPosition - readLen)
                {
                    // Seek within buffer
                    readPos = (int)(readLen + value - streamPosition);
                }
                else
                {
                    DiscardBufferData();
                    pdfStream.Seek(value, SeekOrigin.Begin);
                }
            }
        }

        /// <summary>
        /// Gets the length of the PDF stream.
        /// </summary>
        public long Length {
            get => pdfStream.Length;
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of PdfReader for the specified <see cref="Stream"/> with the default buffer size
        /// </summary>
        /// <param name="pdfStream">Read <see cref="Stream"/></param>
        public PdfReader(Stream pdfStream) : this(pdfStream, DefaultBufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of PdfReader for the specified <see cref="Stream"/>
        /// </summary>
        /// <param name="pdfStream">Read <see cref="Stream"/></param>
        /// <param name="bufferSize">Reader's buffer size</param>
        public PdfReader(Stream pdfStream, int bufferSize)
        {
            this.pdfStream = pdfStream ?? throw new ArgumentNullException(nameof(pdfStream));

            if(bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "Must be at positive number.");

            decoder = encoding.GetDecoder();
            byteBuffer = new byte[bufferSize];
            charBuffer = new char[bufferSize];
            this.bufferSize = bufferSize;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the next available character without changing the reader position.
        /// </summary>
        /// <returns>Next available character.</returns>
        public char Peek()
        {
            EnsureBufferFilled();

            return charBuffer[readPos];
        }

        /// <summary>
        /// Returns character at specified offset without changing the reader position.
        /// </summary>
        /// <param name="offset">Positive offset from current position of the character to read.</param>
        /// <returns>Character at specified offset.</returns>
        public char Peek(int offset)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "Cannot be negative.");
            if (offset > bufferSize)
                throw new ArgumentOutOfRangeException(nameof(offset), "Value must be less than buffer size.");

            if(readPos + offset > readLen)
            {
                FillBuffer();       // Triggers buffer trim and fill 
            }

            return charBuffer[readPos + offset];
        }

        /// <summary>
        /// Returns the next available character.
        /// </summary>
        /// <returns>Next available character.</returns>
        public char Read()
        {
            EnsureBufferFilled();

            var result = charBuffer[readPos];
            readPos++;
            return result;
        }

        /// <summary>
        /// Returns the string of all accepted characters from PDF stream
        /// </summary>
        /// <param name="acceptChar">Character evaluation function. Should return true to append character or false to stop capture</param>
        /// <returns>Accepted characters string</returns>
        public string ReadWhile(Func<char, bool> acceptChar)
        {
            if(acceptChar == null)
                throw new ArgumentNullException(nameof(acceptChar));

            EnsureBufferFilled();

            StringBuilder sb = null;
            do
            {
                int i = readPos;
                while(i < readLen)
                {
                    if (!acceptChar(charBuffer[i]))
                    {
                        string s;
                        if(sb != null)
                        {
                            sb.Append(charBuffer, readPos, i - readPos);
                            s = sb.ToString();
                        }
                        else
                        {
                            s = new string(charBuffer, readPos, i - readPos);
                        }
                        readPos = i;
                        return s;
                    }
                    i++;
                }
                while (i < readLen);

                if(sb == null)
                {
                    sb = new StringBuilder();
                }
                sb.Append(charBuffer, readPos, readLen - readPos);
                readPos = i;
            }
            while (FillBuffer() > 0);

            return sb.ToString();
        }

        /// <summary>
        /// Returns the string of the specified length from PDF stream.
        /// </summary>
        /// <param name="length">Number of characters to read.</param>
        /// <returns>Read string.</returns>
        public string ReadString(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Must be positive.");

            EnsureBufferFilled();

            if(length <= readLen - readPos)
            {
                // Direct read from buffer
                var result = new string(charBuffer, readPos, length);
                readPos += length;
                return result;
            }

            var sb = new StringBuilder();
            do
            {
                if(readLen - readPos >= length)
                {
                    sb.Append(charBuffer, readPos, length);
                    break;
                }
                else
                {
                    sb.Append(charBuffer, readPos, readLen - readPos);
                    length -= readLen - readPos;
                }
            }
            while (FillBuffer() > 0);

            return sb.ToString();
        }

        /// <summary>
        /// Move forward in the PDF stream to the first non-skipped character
        /// </summary>
        /// <param name="skipChar">Character evaluation function. Should return true to skip character or false to stop at character position</param>
        public void SkipWhile(Func<char, bool> skipChar)
        {
            if (skipChar == null)
                throw new ArgumentNullException(nameof(skipChar));

            EnsureBufferFilled();

            do
            {
                while(readPos < readLen)
                {
                    if (!skipChar(charBuffer[readPos]))
                    {
                        return;
                    }
                    readPos++;
                }
            }
            while (FillBuffer() > 0);
        }

        /// <summary>
        /// Reads a sequence of bytes from the current Pdf stream
        /// and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The array of bytes to write to.</param>
        /// <param name="index">The zero-based byte offset in buffer at which to begin storing the data read from the stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer.</returns>
        public int Read(byte[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Cannot be negative");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Cannot be negative");
            if (buffer.Length - index < count)
                throw new ArgumentException("Invalid buffer length");

            int read = 0;
            if (HasBuffer())
            {
                // Read from buffer
                read = readLen - readPos >= count ? count : readLen - readPos;
                Buffer.BlockCopy(byteBuffer, readPos, buffer, index, read);
                readPos += read;
            }
            if (read < count)
            {
                // Direct read
                read += pdfStream.Read(buffer, index + read, count - read);
            }
            return read;
        }

        /// <summary>
        /// Reads a sequence of characters from the current Pdf stream
        /// and advances the position within the stream by the number of characters read.
        /// </summary>
        /// <param name="buffer">The array of characters to write to.</param>
        /// <param name="index">The zero-based byte offset in buffer at which to begin storing the data read from the stream.</param>
        /// <param name="count">The maximum number of characters to be read from the current stream.</param>
        /// <returns>The total number of characters read into the buffer.</returns>
        public int Read(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Cannot be negative");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Cannot be negative");
            if (buffer.Length - index < count)
                throw new ArgumentException("Invalid buffer length");

            EnsureBufferFilled();

            int read = 0;
            do
            {
                // Read from buffer
                read = readLen - readPos >= count ? count : readLen - readPos;
                Buffer.BlockCopy(charBuffer, readPos * 2, buffer, index * 2, read * 2);
                index += read;
                readPos += read;
            }
            while (read < count && FillBuffer() > 0);
            
            return read;
        }

        #endregion

        #region Private Functions
        private void CheckAsyncTaskInProgress()
        {
            // Prevent simultaneous async operations.
            
            Task t = _asyncReadTask;
            
            if (t != null && !t.IsCompleted)
                throw new InvalidOperationException("The stream is currently in use by a previous operation on the stream.");
        }

        private void EnsureBufferFilled()
        {
            if (readPos == readLen && FillBuffer() == 0)
            {
                // Should not happen when reading a PDF
                throw new InvalidOperationException("End of file reached.");
            }
        }

        private bool HasBuffer()
        {
            return readPos < readLen;
        }

        private void DiscardBufferData()
        {
            readPos = 0;
            readLen = 0;
        }

        private void TrimBuffer()
        {
            int offset = readLen - readPos;
            Buffer.BlockCopy(byteBuffer, readPos, byteBuffer, 0, offset);
            Buffer.BlockCopy(charBuffer, readPos * 2, charBuffer, 0, offset * 2);
            readPos -= offset;
            readLen -= offset;
        }

        private int FillBuffer()
        {
            if(readPos < readLen)
            {
                // Buffer was partially consumed
                TrimBuffer();
            }
            else
            {
                // Buffer was totaly consumed
                readPos = 0;
                readLen = 0;
            }
            int read = pdfStream.Read(byteBuffer, readLen, bufferSize - readLen);
            decoder.GetChars(byteBuffer, readLen, read, charBuffer, readLen);
            readLen += read;
            return read;
        }

        private async Task<int> FillBufferAsync()
        {
            if (readPos < readLen)
            {
                // Buffer was partially consumed
                TrimBuffer();
            }
            else
            {
                // Buffer was totaly consumed
                readPos = 0;
                readLen = 0;
            }
            int read = await pdfStream.ReadAsync(byteBuffer, readLen, bufferSize - readLen);
            decoder.GetChars(byteBuffer, readLen, read, charBuffer, readLen);
            readLen += read;
            return read;
        }

        #endregion
    }
}
