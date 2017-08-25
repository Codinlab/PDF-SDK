using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Structure;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Structure
{
    public class XRefSectionTests
    {
        private static string ReadAsString(Stream stream)
        {
            var buffer = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);
            return Encoding.GetEncoding("ASCII").GetString(buffer);
        }

        [Fact]
        public void ReadsSimpleSection()
        {
            // Arrange
            XRefSection section;

            using(var pdfStream = new MemoryStream())
            {
                using(var sw = new StreamWriter(pdfStream, Encoding.ASCII, 128, true))
                {
                    sw.WriteLine("xref");
                    sw.WriteLine("0 6");
                    sw.Write("0000000003 65535 f\r\n");
                    sw.Write("0000000017 00000 n\r\n");
                    sw.Write("0000000081 00000 n\r\n");
                    sw.Write("0000000000 00007 f\r\n");
                    sw.Write("0000000331 00000 n\r\n");
                    sw.Write("0000000409 00000 n\r\n");
                    sw.WriteLine("trailer");
                }

                pdfStream.Seek(0, SeekOrigin.Begin);

                // Act
                using (var reader = new PdfReader(pdfStream))
                {
                    section = XRefSection.FromReader(reader);
                }
            }

            // Assert
            Assert.NotNull(section);
            Assert.NotNull(section.Entries);
            Assert.Equal(6, section.Entries.Count);

            Assert.Equal(PdfObjectId.Zero, section.Entries[0].ObjectId);
            Assert.Equal(3, (section.Entries[0] as PdfFreeObjectReference).NextFreeObjectNumber);

            Assert.Equal(new PdfObjectId(1), section.Entries[1].ObjectId);
            Assert.Equal(17, (section.Entries[1] as PdfObjectReference).Position);

            Assert.Equal(new PdfObjectId(2), section.Entries[2].ObjectId);
            Assert.Equal(81, (section.Entries[2] as PdfObjectReference).Position);

            Assert.Equal(new PdfObjectId(3, 7), section.Entries[3].ObjectId);
            Assert.Equal(0, (section.Entries[3] as PdfFreeObjectReference).NextFreeObjectNumber);

            Assert.Equal(new PdfObjectId(4), section.Entries[4].ObjectId);
            Assert.Equal(331, (section.Entries[4] as PdfObjectReference).Position);

            Assert.Equal(new PdfObjectId(5), section.Entries[5].ObjectId);
            Assert.Equal(409, (section.Entries[5] as PdfObjectReference).Position);
        }

        [Fact]
        public void ReadsEmptySection()
        {
            // Arrange
            XRefSection section;

            using (var pdfStream = new MemoryStream())
            {
                using (var sw = new StreamWriter(pdfStream, Encoding.ASCII, 128, true))
                {
                    sw.WriteLine("xref");
                    sw.WriteLine("0 0");
                    sw.WriteLine("trailer");
                }

                pdfStream.Seek(0, SeekOrigin.Begin);

                // Act
                using (var reader = new PdfReader(pdfStream))
                {
                    section = XRefSection.FromReader(reader);
                }
            }

            // Assert
            Assert.NotNull(section);
            Assert.NotNull(section.Entries);
            Assert.Equal(0, section.Entries.Count);
        }

        [Fact]
        public void ReadsSubSections()
        {
            // Arrange
            XRefSection section;

            using (var pdfStream = new MemoryStream())
            {
                using (var sw = new StreamWriter(pdfStream, Encoding.ASCII, 128, true))
                {
                    sw.WriteLine("xref");
                    sw.WriteLine("0 1");
                    sw.Write("0000000000 65535 f\r\n");
                    sw.WriteLine("3 1");
                    sw.Write("0000025325 00000 n\r\n");
                    sw.WriteLine("23 2");
                    sw.Write("0000025518 00002 n\r\n");
                    sw.Write("0000025635 00000 n\r\n");
                    sw.WriteLine("30 1");
                    sw.Write("0000025777 00000 n\r\n");
                    sw.WriteLine("trailer");
                }

                pdfStream.Seek(0, SeekOrigin.Begin);

                // Act
                using (var reader = new PdfReader(pdfStream))
                {
                    section = XRefSection.FromReader(reader);
                }
            }

            // Assert
            Assert.NotNull(section);
            Assert.NotNull(section.Entries);
            Assert.Equal(5, section.Entries.Count);

            Assert.Equal(PdfObjectId.Zero, section.Entries[0].ObjectId);
            Assert.Equal(0, (section.Entries[0] as PdfFreeObjectReference).NextFreeObjectNumber);

            Assert.Equal(new PdfObjectId(3), section.Entries[3].ObjectId);
            Assert.Equal(25325, (section.Entries[3] as PdfObjectReference).Position);

            Assert.Equal(new PdfObjectId(23, 2), section.Entries[23].ObjectId);
            Assert.Equal(25518, (section.Entries[23] as PdfObjectReference).Position);

            Assert.Equal(new PdfObjectId(24), section.Entries[24].ObjectId);
            Assert.Equal(25635, (section.Entries[24] as PdfObjectReference).Position);

            Assert.Equal(new PdfObjectId(30), section.Entries[30].ObjectId);
            Assert.Equal(25777, (section.Entries[30] as PdfObjectReference).Position);
        }

        [Fact]
        public void WritesEmptySection()
        {
            // Arrange
            var entries = new Dictionary<int, PdfObjectReferenceBase>();
            XRefSection section = new XRefSection(entries);
            string result;

            using (var pdfStream = new MemoryStream())
            {
                // Act
                using (var writer = new PdfWriter(pdfStream))
                {
                    section.Write(writer);
                }

                result = ReadAsString(pdfStream);
            }

            // Assert
            Assert.Equal("xref\r\n0 0\r\n", result);
        }

        [Fact]
        public void WritesSimpleSection()
        {
            // Arrange
            var entries = new Dictionary<int, PdfObjectReferenceBase> {
                { 0, new PdfFreeObjectReference(PdfObjectId.Zero, 3) },
                { 1, new PdfObjectReference(new PdfObjectId(1), 17) },
                { 2, new PdfObjectReference(new PdfObjectId(2), 81) },
                { 3, new PdfFreeObjectReference(new PdfObjectId(3, 7), 0) },
                { 4, new PdfObjectReference(new PdfObjectId(4), 331) },
                { 5, new PdfObjectReference(new PdfObjectId(5), 409) }
            };
            XRefSection section = new XRefSection(entries);
            string result;
            var expected = new StringBuilder();
            expected.Append("xref\r\n");
            expected.Append("0 6\r\n");
            expected.Append("0000000003 65535 f\r\n");
            expected.Append("0000000017 00000 n\r\n");
            expected.Append("0000000081 00000 n\r\n");
            expected.Append("0000000000 00007 f\r\n");
            expected.Append("0000000331 00000 n\r\n");
            expected.Append("0000000409 00000 n\r\n");

            using (var pdfStream = new MemoryStream())
            {
                // Act
                using (var writer = new PdfWriter(pdfStream))
                {
                    section.Write(writer);
                }

                result = ReadAsString(pdfStream);
            }

            // Assert
            Assert.Equal(expected.ToString(), result);
        }

        [Fact]
        public void WritesSubSections()
        {
            // Arrange
            var entries = new Dictionary<int, PdfObjectReferenceBase> {
                { 30, new PdfObjectReference(new PdfObjectId(30), 25777) },
                { 3, new PdfObjectReference(new PdfObjectId(3), 25325) },
                { 23, new PdfObjectReference(new PdfObjectId(23, 2), 25518) },
                { 0, new PdfFreeObjectReference(PdfObjectId.Zero, 0) },
                { 24, new PdfObjectReference(new PdfObjectId(24), 25635) }
            };
            XRefSection section = new XRefSection(entries);
            string result;
            var expected = new StringBuilder();
            expected.Append("xref\r\n");
            expected.Append("0 1\r\n");
            expected.Append("0000000000 65535 f\r\n");
            expected.Append("3 1\r\n");
            expected.Append("0000025325 00000 n\r\n");
            expected.Append("23 2\r\n");
            expected.Append("0000025518 00002 n\r\n");
            expected.Append("0000025635 00000 n\r\n");
            expected.Append("30 1\r\n");
            expected.Append("0000025777 00000 n\r\n");

            using (var pdfStream = new MemoryStream())
            {
                // Act
                using (var writer = new PdfWriter(pdfStream))
                {
                    section.Write(writer);
                }

                result = ReadAsString(pdfStream);
            }

            // Assert
            Assert.Equal(expected.ToString(), result);
        }
    }
}
