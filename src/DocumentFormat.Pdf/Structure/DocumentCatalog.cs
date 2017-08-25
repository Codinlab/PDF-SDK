using DocumentFormat.Pdf.Exceptions;
using DocumentFormat.Pdf.Objects;
using System;
using System.Collections.Generic;

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
        /// Instanciates a new Document Catalog with a direct Page Tree.
        /// </summary>
        /// <param name="pageTree">The <see cref="PageTreeNode"/> that is the root of the document’s page tree.</param>
        public DocumentCatalog(PageTreeNode pageTree)
        {
            internalDictionary[TypeKey] = new NameObject(TypeValue);
            internalDictionary[PagesKey] = pageTree ?? throw new ArgumentNullException(nameof(pageTree));
        }

        /// <summary>
        /// Instanciates a new Document Catalog with an indirect Page Tree.
        /// </summary>
        /// <param name="pageTree">The <see cref="IndirectObject{PageTree}"/> that is the root of the document’s page tree.</param>
        public DocumentCatalog(IndirectObject<PageTreeNode> pageTree)
        {
            internalDictionary[TypeKey] = new NameObject(TypeValue);
            internalDictionary[PagesKey] = pageTree ?? throw new ArgumentNullException(nameof(pageTree));
        }

        /// <summary>
        /// Instanciates a new Document Catalog.
        /// </summary>
        /// <param name="items">Catalog items.</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        internal DocumentCatalog(IDictionary<string, PdfObject> items, bool isReadOnly) : base(items, isReadOnly)
        {
        }

        /// <summary>
        /// Gets or sets the  type of PDF object that this dictionary describes;
        /// must be Catalog for the catalog dictionary.
        /// </summary>
        public string Type => (internalDictionary[TypeKey] as NameObject).Value;

        /// <summary>
        /// Gets or sets the version of the PDF specification to which the document conforms.
        /// </summary>
        public PdfVersion? Version {
            get {
                return internalDictionary.ContainsKey(VersionKey) ? new PdfVersion((internalDictionary[VersionKey] as NameObject).Value) : (PdfVersion?)null;
            }
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                if (value != null)
                {
                    internalDictionary[VersionKey] = new NameObject(value.ToString());
                }
                else if (internalDictionary.ContainsKey(VersionKey))
                {
                    internalDictionary.Remove(VersionKey);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="PageTreeNode"/> that is the root of the document’s page tree.
        /// </summary>
        public PageTreeNode Pages {
            get {
                return internalDictionary[PagesKey] as PageTreeNode;
            }
        }
    }
}
