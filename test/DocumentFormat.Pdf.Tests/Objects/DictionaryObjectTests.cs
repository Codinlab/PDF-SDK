using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Objects
{
    public class DictionaryObjectTests
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

        public static TheoryData<string, string[], long> DictionaryTestData {
            get => new TheoryData<string, string[], long> {
                {
                    "<< /Type /Example /Subtype /DictionaryExample /Version 0.01 /IntegerItem 12 /StringItem (a string) >> some content",
                    new string[] { "Type", "Subtype", "Version", "IntegerItem", "StringItem" },
                    101
                },
                {
                    "<</StringItem(a string)/Subdictionary<</Item1 0.4/Item2 true>>>>/NextToken",
                    new string[] { "StringItem", "Subdictionary" },
                    64
                },
                {
                    "<</Length 5/Other /Foo>>/NextToken",
                    new string[] { "Length", "Other" },
                    24
                }
            };
        }

        [Theory]
        [MemberData(nameof(DictionaryTestData))]
        public void ReadsDictionary(string streamContent, string[] expectedKeys, long expectedPosition)
        {
            // Arrange
            DictionaryObject dictionaryObj;
            long position;

            // Act
            using (var stream = BuildTestStream(streamContent))
            {
                var reader = new PdfReader(stream);
                reader.Position = 0;
                dictionaryObj = DictionaryObject.FromReader(reader);
                position = reader.Position;
            }

            // Assert
            Assert.NotNull(dictionaryObj);
            Assert.IsNotType<StreamObject>(dictionaryObj);
            Assert.Equal(expectedKeys.Length, dictionaryObj.Count);
            foreach(var key in expectedKeys)
            {
                Assert.Contains(key, dictionaryObj.Keys);
            }
            Assert.Equal(expectedPosition, position);
        }

        public static TheoryData<string, int> StreamTestData {
            get => new TheoryData<string, int> {
                { "<</Length 5>>\r\nstream\r\nabcde\r\nendstream\r\n/Other content", 5 },
                { "<</Length 2/Type /Test>>\nstream\nfg\nendstream\n/Other content", 2 }
            };
        }

        [Theory]
        [MemberData(nameof(StreamTestData))]
        public void ReadsStreamObject(string streamContent, int expectedLength)
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
        }

        [Fact]
        public void WritesDictionaryObject()
        {
            // Arrange
            var dictionaryObj = new DictionaryObject(new Dictionary<string, PdfObject> {
                { "Null", new NullObject() },
                { "Boolean", new BooleanObject(true) },
                { "Name", new NameObject("Name") },
                { "String", new LiteralStringObject("String") }
            });
            string result;

            // Act
            using (var pdfStream = new MemoryStream())
            {
                var writer = new PdfWriter(pdfStream);
                dictionaryObj.Write(writer);
                writer.Flush();
                result = ReadAsString(pdfStream);
            }

            // Assert
            Assert.Equal("<</Null null/Boolean true/Name/Name/String(String)>>", result);

        }
    }
}
