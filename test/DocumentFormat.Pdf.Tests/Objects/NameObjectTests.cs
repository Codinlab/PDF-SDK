using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Objects
{
    public class NameObjectTests
    {
        private static Stream BuildTestStream(string content)
        {
            return new MemoryStream(Encoding.GetEncoding("ASCII").GetBytes(content));
        }

        private static string ReadAsString(Stream stream)
        {
            var buffer = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);
            return Encoding.GetEncoding("ASCII").GetString(buffer);
        }

        public static TheoryData<string, string, long> NameTestData {
            get => new TheoryData<string, string, long> {
                { "/ empty name", "", 1 },
                { "/A;Name_With-Various***Characters? some content", "A;Name_With-Various***Characters?", 34 },
                { "/Name#20with#20spaces/Another", "Name with spaces", 21 }
            };
        }

        [Theory]
        [MemberData(nameof(NameTestData))]
        public void ReadsName(string streamContent, string expectedValue, long expectedPosition)
        {
            // Arrange
            NameObject nameObj;
            long position;

            // Act
            using (var stream = BuildTestStream(streamContent))
            {
                using (var reader = new PdfReader(stream))
                {
                    reader.Position = 0;
                    nameObj = NameObject.FromReader(reader);
                    position = reader.Position;
                }
            }

            // Assert
            Assert.NotNull(nameObj);
            Assert.Equal(expectedValue, nameObj.Value);
            Assert.Equal(expectedPosition, position);
        }

        [Fact]
        public void WritesNameObject()
        {
            // Arrange
            var nameObj = new NameObject("C# Rocks !");
            string result;

            // Act
            using (var pdfStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(pdfStream))
                {
                    nameObj.Write(writer);
                    writer.Flush();
                    result = ReadAsString(pdfStream);
                }
            }

            // Assert
            Assert.Equal("/C#23#20Rocks#20!", result);
        }
    }
}
