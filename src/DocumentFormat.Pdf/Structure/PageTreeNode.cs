using System;
using System.Collections.Generic;
using System.Text;
using DocumentFormat.Pdf.Objects;
using System.Linq;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Represents the PDF Page Tree
    /// </summary>
    public class PageTreeNode : PageTreeItem
    {
        /// <summary>
        /// The Type entry value.
        /// </summary>
        protected override string TypeValue => "Pages";

        /// <summary>
        /// The Kids key name
        /// </summary>
        private const string KidsKey = "Kids";

        /// <summary>
        /// The Count key name
        /// </summary>
        private const string CountKey = "Count";

        /// <summary>
        /// Instanciates a new PDF Page Tree node.
        /// </summary>
        public PageTreeNode() : base()
        {
        }

        /// <summary>
        /// Instanciates a new PDF Page Tree node.
        /// </summary>
        /// <param name="items">Page tree items.</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        public PageTreeNode(IDictionary<string, PdfObject> items, bool isReadOnly) : base(items, isReadOnly)
        {
        }

        /// <summary>
        /// An array of indirect references to the immediate children of this node.
        /// The children may be page objects or other page tree nodes.
        /// </summary>
        public IEnumerable<PageTreeItem> Kids => internalDictionary.ContainsKey(KidsKey) ? (internalDictionary[KidsKey] as ArrayObject).Select(item => item as PageTreeItem) : null;

        /// <summary>
        /// The number of leaf nodes (page objects) that are descendants of this node within the page tree.
        /// </summary>
        public new int Count => (internalDictionary[CountKey] as IntegerObject).IntergerValue;
    }
}
