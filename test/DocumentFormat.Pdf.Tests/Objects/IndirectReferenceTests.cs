using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using DocumentFormat.Pdf.Structure;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Objects
{
    public class IndirectReferenceTests
    {
        private static string ReadAsString(Stream stream)
        {
            var buffer = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);
            return Encoding.GetEncoding("ASCII").GetString(buffer);
        }

        [Fact]
        public void WritesIndirectReference()
        {
            // Arrange
            var indRef = new IndirectReference(new PdfObjectId(12, 3));
            string result;

            // Act
            using (var pdfStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(pdfStream))
                {
                    indRef.Write(writer);
                    writer.Flush();
                    result = ReadAsString(pdfStream);
                }
            }

            // Assert
            Assert.Equal("12 3 R", result);
        }
    }
}
