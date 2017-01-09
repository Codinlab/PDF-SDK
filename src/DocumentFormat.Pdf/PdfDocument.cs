using DocumentFormat.Pdf.IO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DocumentFormat.Pdf
{
    /// <summary>
    /// Defines a PDF document
    /// </summary>
    public class PdfDocument
    {
        public const string PdfHeader = "%PDF";
        public const string PdfTrailer = "%%EOF";

        private PdfVersion _pdfVersion;

        /// <summary>
        /// Returns the PDF version of the document
        /// </summary>
        public PdfVersion PdfVersion {
            get {
                return _pdfVersion;
            }
        }

        /// <summary>
        /// Creates a new instance of the PdfDocument class from the IO stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> on which to open the PdfDocument.</param>
        /// <returns></returns>
        public static async Task<PdfDocument> OpenAsync(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new ArgumentException("Cannot read stream", nameof(stream));

            using (DocumentReader documentReader = new DocumentReader(stream))
            {
                // Check header
                var version = await documentReader.ReadPdfVersionAsync();

                return new PdfDocument(version);
            }
        }

        protected PdfDocument(PdfVersion version)
        {
            _pdfVersion = version;
        }
    }
}
