using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Xunit;

namespace DocumentFormat.Pdf.FunctionalTests
{
    public class PdfDocumentTests
    {
        private static string SampleFilesPath {
            get => Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "TestFiles");
        }

        [Theory]
        [InlineData("AdobeAcrobat.pdf", Skip = "Cross-Reference Streams are not supported.")]
        [InlineData("WordGenerated.pdf")]
        public void OpensSampleFile(string fileName)
        {
            // Arrange
            PdfDocument doc;

            // Act
            using (var fs = File.OpenRead(Path.Combine(SampleFilesPath, fileName)))
            {
                doc = PdfDocument.Open(fs);
            }

            // Assert
            Assert.NotNull(doc);
        }

    }
}
