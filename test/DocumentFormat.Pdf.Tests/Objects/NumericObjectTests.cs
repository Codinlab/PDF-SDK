using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Objects
{
    public class NumericObjectTests
    {
        private static Stream BuildTestStream(string content)
        {
            return new MemoryStream(Encoding.GetEncoding("ASCII").GetBytes(content));
        }

        public static TheoryData<string, Type, float, long> NumericTestData {
            get => new TheoryData<string, Type, float, long> {
                { "4 some content", typeof(IntegerObject), 4, 1 },
                { "+123 some content", typeof(IntegerObject), 123, 4 },
                { "-56789/token", typeof(IntegerObject), -56789, 6 },
                { ".1/token", typeof(RealObject), .1f, 2 },
                { "+.2345\r\n(new line)", typeof(RealObject), .2345f, 6 }
            };
        }

        [Theory]
        [MemberData(nameof(NumericTestData))]
        public void ReadsNumeric(string streamContent, Type expectedType, float expectedValue, long expectedPosition)
        {
            // Arrange
            NumericObject numericObj;
            long position;

            // Act
            using (var stream = BuildTestStream(streamContent))
            {
                var reader = new PdfReader(stream);
                reader.Position = 0;
                numericObj = NumericObject.FromReader(reader);
                position = reader.Position;
            }

            // Assert
            Assert.NotNull(numericObj);
            Assert.IsType(expectedType, numericObj);
            Assert.Equal(expectedValue, numericObj.RealValue);
            Assert.Equal(expectedPosition, position);
        }
    }
}
