using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DocumentFormat.Pdf.IO
{
    public class DocumentReader : StreamReader
    {
        public DocumentReader(Stream stream) : base(stream, Encoding.GetEncoding("ASCII")) { }

        public async Task<PdfVersion> ReadPdfVersionAsync() {
            BaseStream.Seek(0, SeekOrigin.Begin);

            var header = await ReadLineAsync();
            if (header == null || !header.StartsWith(PdfDocument.PdfHeader))
                throw new FormatException("Invalid file header");

            return new PdfVersion(header.Substring(PdfDocument.PdfHeader.Length).TrimStart('-'));
        }

    }
}
