using System.Collections.Generic;
using DocumentFormat.Pdf.Objects;

namespace DocumentFormat.Pdf.Structure
{
    public interface IPdfTrailer
    {
        IDictionary<string, PdfObject> Encrypt { get; }
        IEnumerable<PdfObject> ID { get; }
        IDictionary<string, PdfObject> Info { get; }
        int? Prev { get; }
        IDictionary<string, PdfObject> Root { get; }
        int Size { get; }
    }
}