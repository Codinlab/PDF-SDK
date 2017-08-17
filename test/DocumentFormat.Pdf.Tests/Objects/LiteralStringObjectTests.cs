using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Objects
{
    public class LiteralStringObjectTests
    {
        private static string ReadAsString(Stream stream)
        {
            var buffer = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);
            return Encoding.GetEncoding("ASCII").GetString(buffer);
        }

        [Fact]
        public void WritesLiteralStringObject()
        {
            // Arrange
            var stringObj = new LiteralStringObject("I'm a\r\n(literal)\tstring");
            string result;

            // Act
            using (var pdfStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(pdfStream))
                {
                    stringObj.Write(writer);
                    writer.Flush();
                    result = ReadAsString(pdfStream);
                }
            }

            // Assert
            Assert.Equal("(I'm a\\r\\n\\(literal\\)\\tstring)", result);
        }
    }
}
