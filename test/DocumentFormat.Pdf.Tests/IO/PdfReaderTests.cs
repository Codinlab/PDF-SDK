using DocumentFormat.Pdf.IO;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.IO
{
    public class PdfReaderTests
    {
        private static Stream BuildTestStream(string content)
        {
            return new MemoryStream(Encoding.GetEncoding("ASCII").GetBytes(content));
        }

        [Fact]
        public void ThrowsOnNullStream()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => new PdfReader(null));
        }

        [Fact]
        public void Peek_ReadsAtCurrentPosition()
        {
            // Arrange
            char peeked;

            // Act
            using (var pdfStream = BuildTestStream("abcdef"))
            {
                var reader = new PdfReader(pdfStream);
                reader.Position = 2;
                peeked = reader.Peek();
            }

            // Assert
            Assert.Equal('c', peeked);
        }

        [Fact]
        public void Peek_PreservesPosition()
        {
            // Arrange
            char peeked;
            long returnPosition;

            // Act
            using (var pdfStream = BuildTestStream("abcdef"))
            {
                var reader = new PdfReader(pdfStream);
                reader.Position = 3;
                peeked = reader.Peek();
                returnPosition = reader.Position;
            }

            // Assert
            Assert.Equal('d', peeked);
            Assert.Equal(3, returnPosition);
        }

        [Fact]
        public void Read_ReadsAtCurrentPosition()
        {
            // Arrange
            char peeked;

            // Act
            using (var pdfStream = BuildTestStream("abcdef"))
            {
                var reader = new PdfReader(pdfStream);
                reader.Position = 2;
                peeked = reader.Read();
            }

            // Assert
            Assert.Equal('c', peeked);
        }

        [Fact]
        public void Read_IncrementsPosition()
        {
            // Arrange
            char peeked;
            long returnPosition;

            // Act
            using (var pdfStream = BuildTestStream("abcdef"))
            {
                var reader = new PdfReader(pdfStream);
                reader.Position = 3;
                peeked = reader.Read();
                returnPosition = reader.Position;
            }

            // Assert
            Assert.Equal('d', peeked);
            Assert.Equal(4, returnPosition);
        }

        [Fact]
        public void ReadWhile_StartsAtCurrentPosition()
        {
            // Arrange
            string read;

            // Act
            using (var pdfStream = BuildTestStream("abcdef"))
            {
                var reader = new PdfReader(pdfStream);
                reader.Position = 2;
                read = reader.ReadWhile(c => c != 'f');
            }

            // Assert
            Assert.Equal("cde", read);
        }

        [Fact]
        public void ReadWhile_KeepsPositionOfFirstRejected()
        {
            // Arrange
            string read;
            long returnPosition;

            // Act
            using (var pdfStream = BuildTestStream("abcdef"))
            {
                var reader = new PdfReader(pdfStream);
                reader.Position = 3;
                read = reader.ReadWhile(c => c != 'f');
                returnPosition = reader.Position;
            }

            // Assert
            Assert.Equal("de", read);
            Assert.Equal(5, returnPosition);
        }

        [Fact]
        public void ReadWhile_ReadLongStrings()
        {
            // Arrange
            string read;
            long returnPosition;

            // Act
            using (var pdfStream = BuildTestStream("abcdefghijklmnopqrstuvwxyz"))
            {
                var reader = new PdfReader(pdfStream, 4);   // Small buffer to force buffer rotation
                reader.Position = 3;
                read = reader.ReadWhile(c => c != 'x');
                returnPosition = reader.Position;
            }

            // Assert
            Assert.Equal("defghijklmnopqrstuvw", read);
            Assert.Equal(23, returnPosition);
        }

        [Fact]
        public void SkipWhile_KeepsPositionOfFirstNotSkipped()
        {
            // Arrange
            long returnPosition;

            // Act
            using (var pdfStream = BuildTestStream("abcdef"))
            {
                var reader = new PdfReader(pdfStream);
                reader.Position = 0;
                reader.SkipWhile(c => c != 'f');
                returnPosition = reader.Position;
            }

            // Assert
            Assert.Equal(5, returnPosition);
        }

        [Fact]
        public void SkipWhile_StartsAtCurrentPosition()
        {
            // Arrange
            long returnPosition;

            // Act
            using (var pdfStream = BuildTestStream("abcdefabcdef"))
            {
                var reader = new PdfReader(pdfStream);
                reader.Position = 2;
                reader.SkipWhile(c => c != 'a');
                returnPosition = reader.Position;
            }

            // Assert
            Assert.Equal(6, returnPosition);
        }

        [Fact]
        public void SkipWhile_WorksOnLongStrings()
        {
            // Arrange
            long returnPosition;

            // Act
            using (var pdfStream = BuildTestStream("abcdefghijklmnopqrstuvwxyz"))
            {
                var reader = new PdfReader(pdfStream, 4);   // Small buffer to force buffer rotation
                reader.Position = 0;
                reader.SkipWhile(c => c != 'x');
                returnPosition = reader.Position;
            }

            // Assert
            Assert.Equal(23, returnPosition);
        }

        [Fact]
        public void Read_ReadBytes()
        {
            // Arrange
            long returnPosition;
            int read;

            var byteData = new byte[256];
            for(int i = 0; i < byteData.Length; i++)
            {
                byteData[i] = (byte)i;
            }

            using (var pdfStream = new MemoryStream())
            {
                pdfStream.Write(byteData, 0, byteData.Length);
                pdfStream.Flush();

                var reader = new PdfReader(pdfStream);
                reader.Position = 0;

                // Act
                read = reader.Read(byteData, 10, 20);
                returnPosition = reader.Position;
            }

            // Assert
            Assert.Equal(20, read);
            Assert.Equal(20, returnPosition);
            Assert.Equal(9, byteData[9]);
            Assert.Equal(0, byteData[10]);
            Assert.Equal(19, byteData[29]);
            Assert.Equal(30, byteData[30]);
        }

        [Fact]
        public void Read_ReadChars()
        {
            // Arrange
            long returnPosition;
            int read;

            var charData = new char[40];

            using (var pdfStream = BuildTestStream("abcdefghijklmnopqrstuvwxyz"))
            {
                var reader = new PdfReader(pdfStream);
                reader.Position = 5;

                // Act
                read = reader.Read(charData, 10, 20);
                returnPosition = reader.Position;
            }

            var readString = new string(charData, 10, 20);

            // Assert
            Assert.Equal(20, read);
            Assert.Equal(25, returnPosition);
            Assert.Equal("fghijklmnopqrstuvwxy", readString);
        }
    }
}
