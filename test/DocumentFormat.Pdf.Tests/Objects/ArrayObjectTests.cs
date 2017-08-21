using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Objects
{
    public class ArrayObjectTests
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

        public static TheoryData<string, int, long> ArrayTestData {
            get => new TheoryData<string, int, long> {
                { "[] some content", 0, 2 },
                { "[-5] some content", 1, 4 },
                { "[ 549 3.14 false ( Ralph ) /SomeName ]/some content", 5, 38 }
            };
        }

        [Theory]
        [MemberData(nameof(ArrayTestData))]
        public void ReadsArray(string streamContent, int expectedCount, long expectedPosition)
        {
            // Arrange
            ArrayObject arrayObj;
            long position;

            // Act
            using (var stream = BuildTestStream(streamContent))
            {
                using (var reader = new PdfReader(stream))
                {
                    reader.Position = 0;
                    arrayObj = ArrayObject.FromReader(reader);
                    position = reader.Position;
                }
            }

            // Assert
            Assert.NotNull(arrayObj);
            Assert.Equal(expectedCount, arrayObj.Count);
            Assert.Equal(expectedPosition, position);
        }

        [Fact]
        public void WritesArrayObject()
        {
            // Arrange
            var arrayObj = new ArrayObject() {
                new NullObject(),
                new BooleanObject(true),
                new NameObject("Name"),
                new LiteralStringObject("String")
            };
            string result;

            // Act
            using (var pdfStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(pdfStream))
                {
                    arrayObj.Write(writer);
                    writer.Flush();
                    result = ReadAsString(pdfStream);
                }
            }

            // Assert
            Assert.Equal("[null true/Name(String)]", result);

        }
    }
}
