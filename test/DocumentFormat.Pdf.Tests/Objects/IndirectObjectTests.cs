using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using DocumentFormat.Pdf.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Objects
{
    public class IndirectObjectTests
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

        [Fact]
        public void ReadsIndirectObject()
        {
            // Arrange
            IndirectObject indirectObj;
            long position;

            // Act
            using (var stream = BuildTestStream($"10 0 obj\r\nnull\r\nendobj\r\n"))
            {
                using (var reader = new PdfReader(stream))
                {
                    reader.Position = 0;
                    indirectObj = IndirectObject.FromReader(reader);
                    position = reader.Position;
                }
            }

            // Assert
            Assert.NotNull(indirectObj);
            Assert.IsType<IndirectObject<NullObject>>(indirectObj);
            Assert.Equal(22, position);
        }

        [Fact]
        public void WritesIndirectReference()
        {
            // Arrange
            var indRef = new IndirectReference(new PdfObjectId(12, 3));
            string result;

            // Act
            using (var pdfStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(pdfStream))
                {
                    indRef.WriteReference(writer);
                    writer.Flush();
                    result = ReadAsString(pdfStream);
                }
            }

            // Assert
            Assert.Equal("12 3 R", result);
        }

        [Fact]
        public void WritesIndirectObject()
        {
            // Arrange
            var indirectObj = new IndirectObject<NullObject>(new PdfObjectId(10, 2), new NullObject());
            string result;

            // Act
            using (var pdfStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(pdfStream))
                {
                    indirectObj.Write(writer);
                    writer.Flush();
                    result = ReadAsString(pdfStream);
                }
            }

            // Assert
            Assert.Equal("10 2 obj\r\nnull\r\nendobj\r\n", result);

        }
    }
}
