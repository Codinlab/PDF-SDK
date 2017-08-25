using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Represents a Cross-Reference Stream object.
    /// </summary>
    public class XRefStream : StreamObject, IXRefSection, IPdfTrailer
    {
        /// <summary>
        /// The Type key name
        /// </summary>
        private const string TypeKey = "Type";

        /// <summary>
        /// The Type entry value.
        /// </summary>
        private const string TypeValue = "XRef";

        /// <summary>
        /// The Index key name.
        /// </summary>
        private const string IndexKey = "Index";

        /// <summary>
        /// The W key name.
        /// </summary>
        private const string WKey = "W";

        /// <summary>
        /// Instanciates a new Cross-Reference stream object.
        /// </summary>
        /// <param name="dictionaryItems">Dictionary items.</param>
        /// <param name="data">Stream encoded data.</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        public XRefStream(IDictionary<string, PdfObject> dictionaryItems, byte[] data, bool isReadOnly) : base(dictionaryItems, data, isReadOnly)
        {
        }

        /// <summary>
        /// The type of PDF object that this dictionary describes;
        /// must be XRef for a cross-reference stream.
        /// </summary>
        public string Type => (internalDictionary[TypeKey] as NameObject).Value;

        /// <summary>
        /// Gets the total number of entries in the file’s cross-reference table,
        /// as defined by the combination of the original section and all update sections.
        /// </summary>
        public int Size => (internalDictionary[PdfTrailer.SizeKey] as IntegerObject).IntergerValue;

        /// <summary>
        /// An array containing a pair of integers for each subsection in this section.
        /// The first integer is the first object number in the subsection;
        /// the second integer is the number of entries in the subsection.
        /// </summary>
        public int[] Index => internalDictionary.ContainsKey(IndexKey) ? (internalDictionary[IndexKey] as ArrayObject).Select(item => (item as IntegerObject).IntergerValue).ToArray() : new int[] { 0, Size };

        /// <summary>
        /// The byte offset from the beginning of the file to the beginning of the previous cross-reference section.
        /// </summary>
        public int? Prev => internalDictionary.ContainsKey(PdfTrailer.PrevKey) ? (internalDictionary[PdfTrailer.PrevKey] as IntegerObject).IntergerValue : (int?)null;

        /// <summary>
        /// An array containing a pair of integers for each subsection in this section.
        /// The first integer is the first object number in the subsection;
        /// the second integer is the number of entries in the subsection.
        /// </summary>
        public int[] W => (internalDictionary[WKey] as ArrayObject).Select(item => (item as IntegerObject).IntergerValue).ToArray();

        /// <summary>
        /// The catalog dictionary for the PDF document contained in the file
        /// </summary>
        public IDictionary<string, PdfObject> Root => (internalDictionary[PdfTrailer.RootKey] as DictionaryObject);

        /// <summary>
        /// The document’s encryption dictionary.
        /// </summary>
        public IDictionary<string, PdfObject> Encrypt => internalDictionary.ContainsKey(PdfTrailer.EncryptKey) ? (internalDictionary[PdfTrailer.EncryptKey] as DictionaryObject) : null;

        /// <summary>
        /// The document’s information dictionary.
        /// </summary>
        public IDictionary<string, PdfObject> Info => internalDictionary.ContainsKey(PdfTrailer.InfoKey) ? (internalDictionary[PdfTrailer.InfoKey] as DictionaryObject) : null;

        /// <summary>
        /// An array of two byte-strings constituting a file identifier.
        /// </summary>
        public IEnumerable<PdfObject> ID => internalDictionary.ContainsKey(PdfTrailer.IdKey) ? (internalDictionary[PdfTrailer.IdKey] as ArrayObject) : null;

        /// <summary>
        /// Gets the list of Cross-Reference Stream's entries.
        /// </summary>
        public IReadOnlyDictionary<PdfObjectId, PdfObjectReferenceBase> Entries => throw new NotImplementedException();

        /// <summary>
        /// Creates a StreamObject from PdfReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>Created StreamObject.</returns>
        public static new XRefStream FromReader(PdfReader reader)
        {
            var streamDictionary = ParseDictionary(reader);

            reader.ReadToken(StartKeyword);

            char nextChar = reader.Read();
            if (nextChar == Chars.CR)
            {
                if (reader.Read() != Chars.LF)
                {
                    throw new FormatException("Bad end of line after stream keyword.");
                }
            }
            else if (nextChar != Chars.LF)
            {
                throw new FormatException("Bad end of line after stream keyword.");
            }

            var length = (streamDictionary[LengthKey] as IntegerObject).IntergerValue;
            var data = new byte[length];

            // Read data
            var read = reader.Read(data, 0, length);

            if (read != length)
                throw new FormatException("Bad stream length.");

            return new XRefStream(streamDictionary, data, true);
        }
    }
}
