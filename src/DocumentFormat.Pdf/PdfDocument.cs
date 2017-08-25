using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Structure;
using System;
using System.IO;

namespace DocumentFormat.Pdf
{
    /// <summary>
    /// Defines a PDF document
    /// </summary>
    public class PdfDocument
    {
        public const string PdfHeader = "%PDF-";
        public const string EofMarker = "%%EOF";

        internal PdfVersion pdfVersion;
        internal XRefTable xrefTable;
        internal IPdfTrailer trailer;

        internal Stream documentStream;
        
        /// <summary>
        /// Returns the PDF version of the document
        /// </summary>
        public PdfVersion PdfVersion {
            get {
                return pdfVersion;
            }
        }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of PdfDocument.
        /// </summary>
        /// <param name="documentStream">The document stream</param>
        private PdfDocument(Stream documentStream)
        {
            this.documentStream = documentStream;
        }

        #endregion

        /// <summary>
        /// Creates a new instance of the PdfDocument class from the IO stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> on which to open the PdfDocument.</param>
        /// <returns></returns>
        public static PdfDocument Open(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new ArgumentException("Cannot read stream", nameof(stream));

            var document = new PdfDocument(stream);

            using (var reader = new PdfReader(stream))
            {
                // Check header
                document.pdfVersion = PdfVersion.FromReader(reader);

                // Check trailer
                reader.Position = reader.GetXRefPosition();

                // Read Cross-Reference Table
                IPdfTrailer trailer;
                IXRefSection xrefSection;
                char fisrtChar;

                do
                {
                    fisrtChar = reader.Peek();
                    if (fisrtChar == XRefSection.StartKeyword[0])
                    {
                        // Cross-Reference Section
                        xrefSection = XRefSection.FromReader(reader);
                        trailer = PdfTrailer.FromReader(reader);
                    }
                    else if (char.IsDigit(fisrtChar))
                    {
                        // Cross-Reference Stream
                        var xrefStream = XRefStream.FromReader(reader);
                        trailer = xrefStream;
                        xrefSection = xrefStream;
                    }
                    else
                    {
                        throw new FormatException("Invalid Cross-Reference section.");
                    }

                    if(document.xrefTable == null)
                    {
                        // Initialize document's Cross-Reference Table
                        document.xrefTable = new XRefTable(xrefSection, trailer.Size);
                    }
                    else
                    {
                        // Register section
                        document.xrefTable.AddSection(xrefSection);
                    }

                    // Keep instance of last trailer
                    if (document.trailer == null)
                    {
                        document.trailer = trailer;
                    }

                    // Seek to previous trailer
                    if (trailer.Prev.HasValue)
                    {
                        reader.Position = trailer.Prev.Value;
                    }
                }
                while (trailer.Prev != null);
            }

            return document;
        }
    }
}
