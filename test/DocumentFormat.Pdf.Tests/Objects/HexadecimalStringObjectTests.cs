using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Objects
{
    public class HexadecimalStringObjectTests
    {
        private static string ReadAsString(Stream stream)
        {
            var buffer = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);
            return Encoding.GetEncoding("ASCII").GetString(buffer);
        }

        [Fact]
        public void WritesHexadecimalStringObject()
        {
            // Arrange
            var stringObj = new HexadecimalStringObject("hex encoded string");
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
            Assert.Equal("<68657820656E636F64656420737472696E67>", result);
        }
    }
}
