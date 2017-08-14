using System.Collections.Generic;
using DocumentFormat.Pdf.Objects;

namespace DocumentFormat.Pdf.Structure
{
    public interface IPdfTrailer
    {
        IReadOnlyDictionary<string, PdfObject> Encrypt { get; }
        IEnumerable<PdfObject> ID { get; }
        IReadOnlyDictionary<string, PdfObject> Info { get; }
        int? Prev { get; }
        IReadOnlyDictionary<string, PdfObject> Root { get; }
        int Size { get; }
    }
}