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
        private const string pdfHeader = "%PDF";

        private PdfVersion _version;

        /// <summary>
        /// Returns the PDF version of the document
        /// </summary>
        public PdfVersion Version {
            get {
                return _version;
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

            // Checks header
            using (StreamReader reader = new StreamReader(stream))
            {
                var header = await reader.ReadLineAsync();
                if (header == null || !header.StartsWith(pdfHeader))
                    throw new FormatException("Invalid file header");

                var version = new PdfVersion(header.Substring(pdfHeader.Length).TrimStart('-'));

                return new PdfDocument(version);
            }
        }

        protected PdfDocument(PdfVersion version)
        {
            _version = version;
        }
    }
}
