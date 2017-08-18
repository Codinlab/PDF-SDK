using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using System.Collections.Generic;
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

        private static string ReadAsString(Stream stream)
        {
            var buffer = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);
            return Encoding.GetEncoding("ASCII").GetString(buffer);
        }

        public static TheoryData<string, int, byte[]> StreamTestData {
            get => new TheoryData<string, int, byte[]> {
                { "<</Length 5>>\r\nstream\r\nabcde\r\nendstream\r\n/Other content", 5, new byte[] { 97, 98, 99, 100, 101 } },
                { "<</Length 2>>stream\nfg\nendstream\n/Other content", 2, new byte[] { 102, 103 } }
            };
        }

        [Theory]
        [MemberData(nameof(StreamTestData))]
        public void ReadsStreamObject(string streamContent, int expectedLength, byte[] expectedData)
        {
            // Arrange
            DictionaryObject streamObj;
            byte[] readData = null;

            // Act
            using (var stream = BuildTestStream(streamContent))
            {
                using (var reader = new PdfReader(stream))
                {
                    reader.Position = 0;
                    streamObj = DictionaryObject.FromReader(reader);
                    if(streamObj is StreamObject)
                    {
                        using (var streamData = (streamObj as StreamObject).Data)
                        {
                            readData = new byte[streamData.Length];
                            streamData.Read(readData, 0, readData.Length);
                        }
                    }
                }
            }

            // Assert
            Assert.NotNull(streamObj);
            Assert.IsType<StreamObject>(streamObj);
            Assert.Equal(expectedLength, ((StreamObject)streamObj).Length);
            Assert.NotNull(readData);
            Assert.Equal(expectedData, readData);
        }

        [Fact]
        public void WritesStreamObject()
        {
            // Arrange
            var streamObj = new StreamObject(new Dictionary<string, PdfObject>(), new byte[] { 104, 105, 106, 107 });
            string result;

            // Act
            using (var pdfStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(pdfStream))
                {
                    streamObj.Write(writer);
                    writer.Flush();
                    result = ReadAsString(pdfStream);
                }
            }

            // Assert
            Assert.Equal("<</Length 4>>stream\r\nhijk\r\nendstream\r\n", result);
        }
    }
}
