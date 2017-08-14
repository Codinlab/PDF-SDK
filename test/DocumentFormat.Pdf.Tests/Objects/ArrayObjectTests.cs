using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Objects
{
    public class ArrayObjectTests
    {
        private static Stream BuildTestStream(string content)
        {
            return new MemoryStream(Encoding.GetEncoding("ASCII").GetBytes(content));
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
                var reader = new PdfReader(stream);
                reader.Position = 0;
                arrayObj = ArrayObject.FromReader(reader);
                position = reader.Position;
            }

            // Assert
            Assert.NotNull(arrayObj);
            Assert.Equal(expectedCount, arrayObj.Count);
            Assert.Equal(expectedPosition, position);
        }
    }
}
