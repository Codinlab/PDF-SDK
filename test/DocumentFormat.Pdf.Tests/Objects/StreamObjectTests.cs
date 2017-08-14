using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Objects
{
    public class StreamObjectTests
    {
        private static Stream BuildTestStream(string content)
        {
            return new MemoryStream(Encoding.GetEncoding("ASCII").GetBytes(content));
        }

        public static TheoryData<string, int, double> StreamTestData {
            get => new TheoryData<string, int, double> {
                { "<</Length 5>>\r\nstream\r\nabcde\r\nendstream\r\n/Other content", 5, 23 },
                { "<</Length 2>>stream\nfg\nendstream\n/Other content", 2, 20 }
            };
        }

        [Theory]
        [MemberData(nameof(StreamTestData))]
        public void ReadsStreamObject(string streamContent, int expectedLength, long expectedStreamPosition)
        {
            // Arrange
            DictionaryObject streamObj;
            long position;

            // Act
            using (var stream = BuildTestStream(streamContent))
            {
                var reader = new PdfReader(stream);
                reader.Position = 0;
                streamObj = DictionaryObject.FromReader(reader);
                position = reader.Position;
            }

            // Assert
            Assert.NotNull(streamObj);
            Assert.IsType<StreamObject>(streamObj);
            Assert.Equal(expectedLength, ((StreamObject)streamObj).Length);
            Assert.Equal(expectedStreamPosition, ((StreamObject)streamObj).Position);
        }
    }
}
