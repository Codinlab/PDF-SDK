using DocumentFormat.Pdf.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentFormat.Pdf.Structure
{
    public class XRefStream : StreamObject, IPdfTrailer
    {
        public XRefStream(IDictionary<string, PdfObject> dictionaryItems) : base(dictionaryItems)
        {
        }

        public IReadOnlyDictionary<string, PdfObject> Encrypt => throw new NotImplementedException();

        public IEnumerable<PdfObject> ID => throw new NotImplementedException();

        public IReadOnlyDictionary<string, PdfObject> Info => throw new NotImplementedException();

        public int? Prev => throw new NotImplementedException();

        public IReadOnlyDictionary<string, PdfObject> Root => throw new NotImplementedException();

        public int Size => throw new NotImplementedException();
    }
}
