using DocumentFormat.Pdf.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Represents base class for PDF Page Tree nodes.
    /// </summary>
    public abstract class PageTreeItem : DictionaryObject
    {
        /// <summary>
        /// When overrided, gets the Type entry value.
        /// </summary>
        protected abstract string TypeValue { get; }

        /// <summary>
        /// The Pages key name
        /// </summary>
        private const string ParentKey = "Parent";

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
        /// The type of PDF object that this dictionary describes;
        /// must be Pages for a page tree node.
        /// </summary>
        public string Type => (internalDictionary[TypeKey] as NameObject).Value;

        /// <summary>
        /// The page tree node that is the immediate parent of this one.
        /// </summary>
        public PageTreeNode Parent => internalDictionary[ParentKey] as PageTreeNode;

    }
}
