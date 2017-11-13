using DocumentFormat.Pdf.Objects;
using System.Collections.Generic;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Represents base class for PDF Page Tree nodes.
    /// </summary>
    public abstract class PageTreeItem : TypedDictionaryObject
    {
        /// <summary>
        /// The Pages key name
        /// </summary>
        protected const string ParentKey = "Parent";

        /// <summary>
        /// Instanciates a new PDF Page Tree node.
        /// </summary>
        public PageTreeItem()
        {
            internalDictionary[TypeKey] = new NameObject(TypeValue);
        }

        /// <summary>
        /// Instanciates a new PDF Page Tree node.
        /// </summary>
        /// <param name="items">Page tree node items.</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        protected PageTreeItem(IDictionary<string, PdfObject> items, bool isReadOnly) : base(items, isReadOnly)
        {
        }

        /// <summary>
        /// The page tree node that is the immediate parent of this one.
        /// </summary>
        public PageTreeNode Parent {
            get {
                // Must be an indirect reference.
                return internalDictionary.ContainsKey(ParentKey) ? (internalDictionary[ParentKey] as IndirectObject<PageTreeNode>).Object : null;
            }
        }

    }
}
