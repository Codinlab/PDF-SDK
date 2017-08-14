using DocumentFormat.Pdf.IO;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests
{
    public class PdfVersionTests
    {
        private static Stream BuildTestStream(string content)
        {
            return new MemoryStream(Encoding.GetEncoding("ASCII").GetBytes(content));
        }

        [Theory]
        [InlineData(" %PDF-1.6\r\n")]
        [InlineData("%PDF1.6\r\n")]
        [InlineData("%PDF 1.6\r\n")]
        public void FromReader_ThrowsOnInvalidHeader(string header)
        {
            // Arrange

            // Act

            // Assert
            using (var pdfStream = BuildTestStream(header))
            {
                var reader = new PdfReader(pdfStream);
                Assert.Throws<FormatException>(() => PdfVersion.FromReader(reader));
            }
        }

        [Fact]
        public void FromReader_ReadsPdfVersion()
        {
            // Arrange
            PdfVersion version;

            // Act
            using (var pdfStream = BuildTestStream("%PDF-1.6\r\n"))
            {
                var reader = new PdfReader(pdfStream);
                version = PdfVersion.FromReader(reader);
            }

            // Assert
            Assert.NotNull(version);
            Assert.Equal(1, version.Major);
            Assert.Equal(6, version.Minor);
        }

        [Fact]
        public void FromReader_ReadsPdfVersionAtBeginning()
        {
            // Arrange
            PdfVersion version;

            // Act
            using (var pdfStream = BuildTestStream("%PDF-1.4\r\nSome Content\r\n%PDF-1.6\r\n"))
            {
                var reader = new PdfReader(pdfStream);
                reader.Position = 22;
                version = PdfVersion.FromReader(reader);
            }

            // Assert
            Assert.NotNull(version);
            Assert.Equal(1, version.Major);
            Assert.Equal(4, version.Minor);
        }
    }
}
