using DocumentFormat.Pdf.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Represents the PDF Document Catalog.
    /// </summary>
    public class DocumentCatalog : DictionaryObject
    {
        /// <summary>
        /// The Type entry value.
        /// </summary>
        private const string TypeValue = "Catalog";

        /// <summary>
        /// The Version key name.
        /// </summary>
        private const string VersionKey = "Version";

        /// <summary>
        /// The Pages key name
        /// </summary>
        private const string PagesKey = "Pages";

        /// <summary>
        /// Instanciates a new Document Catalog.
        /// </summary>
        /// <param name="items">Catalog items.</param>
        public DocumentCatalog(IDictionary<string, PdfObject> items) : base(items)
        {
        }

        /// <summary>
        /// The type of PDF object that this dictionary describes;
        /// must be Catalog for the catalog dictionary.
        /// </summary>
        public string Type => (internalDictionary[TypeKey] as NameObject).Value;

        /// <summary>
        /// The version of the PDF specification to which the document conforms.
        /// </summary>
        public PdfVersion? Version => internalDictionary.ContainsKey(VersionKey) ? new PdfVersion((internalDictionary[VersionKey] as NameObject).Value) : (PdfVersion?)null;

        /// <summary>
        /// The page tree node that is the root of the document’s page tree
        /// </summary>
        public PageTreeNode Pages => internalDictionary[PagesKey] as PageTreeNode;
    }
}
