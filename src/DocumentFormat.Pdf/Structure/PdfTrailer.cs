using System.Collections.Generic;
using DocumentFormat.Pdf.Objects;
using System.Threading.Tasks;
using DocumentFormat.Pdf.IO;
using System;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Represents a Pdf Document trailer
    /// </summary>
    public class PdfTrailer : DictionaryObject, IPdfTrailer
    {
        /// <summary>
        /// The trailer keyword
        /// </summary>
        public const string StartKeyword = "trailer";

        private const string SizeKey = "Size";
        private const string PrevKey = "Prev";
        private const string RootKey = "Root";
        private const string EncryptKey = "Encrypt";
        private const string InfoKey = "Info";
        private const string IdKey = "ID";

        /// <summary>
        /// Instanciates a new StringObject
        /// </summary>
        /// <param name="items">Trailer items</param>
        public PdfTrailer(IDictionary<string, PdfObject> items) : base(items)
        {
        }

        /// <summary>
        /// Gets the total number of entries in the file’s cross-reference table,
        /// as defined by the combination of the original section and all update sections.
        /// </summary>
        public int Size => (internalDictionary[SizeKey] as IntegerObject).IntergerValue;

        /// <summary>
        /// The byte offset from the beginning of the file to the beginning of the previous cross-reference section.
        /// </summary>
        public int? Prev => internalDictionary.ContainsKey(PrevKey) ? (internalDictionary[PrevKey] as IntegerObject).IntergerValue : (int?)null;

        /// <summary>
        /// The catalog dictionary for the PDF document contained in the file
        /// </summary>
        public IReadOnlyDictionary<string, PdfObject> Root => (internalDictionary[RootKey] as DictionaryObject);

        /// <summary>
        /// The document’s encryption dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, PdfObject> Encrypt => (internalDictionary[EncryptKey] as DictionaryObject);

        /// <summary>
        /// The document’s information dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, PdfObject> Info => internalDictionary.ContainsKey(InfoKey) ? (internalDictionary[InfoKey] as DictionaryObject) : null;

        /// <summary>
        /// An array of two byte-strings constituting a file identifier.
        /// </summary>
        public IEnumerable<PdfObject> ID => internalDictionary.ContainsKey(IdKey) ? (internalDictionary[IdKey] as ArrayObject) : null;

        /// <summary>
        /// Creates an PdfTrailer from PdfReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use</param>
        /// <returns>Created PdfTrailer</returns>
        public static new PdfTrailer FromReader(PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return new PdfTrailer(ParseDictionary(reader));
        }
    }
}
