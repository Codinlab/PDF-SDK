using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Objects
{
    public class StringObjectTests
    {
        private static Stream BuildTestStream(string content)
        {
            return new MemoryStream(Encoding.GetEncoding("ASCII").GetBytes(content));
        }

        public static TheoryData<string, string, long> StringTestData {
            get => new TheoryData<string, string, long> {
                { "() some content", "", 2 },
                { "(Strings may contain newlines\r\nand such .) some content", "Strings may contain newlines\r\nand such .", 42 },
                { "(\t%(5\\0535)\\)) some content", "\t%(5+5))", 14 },
                { "<> some content", "", 2 },
                { "<68657820656E636f64656420737472696e67> some content", "hex encoded string", 38 },
                { "<656e642077697468203>(literal)", "end with 0", 21 }
            };
        }

        [Theory]
        [MemberData(nameof(StringTestData))]
        public void ReadsString(string streamContent, string expectedValue, long expectedPosition)
        {
            // Arrange
            StringObject stringObj;
            long position;

            // Act
            using (var stream = BuildTestStream(streamContent))
            {
                var reader = new PdfReader(stream);
                reader.Position = 0;
                stringObj = StringObject.FromReader(reader);
                position = reader.Position;
            }

            // Assert
            Assert.NotNull(stringObj);
            Assert.Equal(expectedValue, stringObj.Value);
            Assert.Equal(expectedPosition, position);
        }
    }
}
