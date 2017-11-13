using DocumentFormat.Pdf.Objects;
using System;
using System.Collections.Generic;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Represents the PDF Page Object
    /// </summary>
    public class PageObject : PageTreeItem
    {
        /// <summary>
        /// The Type entry value.
        /// </summary>
        protected override string TypeValue => "Page";

        /// <summary>
        /// The LastModified key name
        /// </summary>
        private const string LastModifiedKey = "LastModified";

        /// <summary>
        /// The Resources key name
        /// </summary>
        private const string ResourcesKey = "Resources";

        /// <summary>
        /// The LastModified key name
        /// </summary>
        private const string MediaBoxKey = "MediaBox";

        /// <summary>
        /// Instanciates a new PDF Page Object.
        /// </summary>
        public PageObject(IndirectObject<PageTreeNode> parentReference) : base()
        {
            internalDictionary[ParentKey] = parentReference ?? throw new ArgumentNullException(nameof(parentReference));
        }

        /// <summary>
        /// Instanciates a new PDF Page Object.
        /// </summary>
        /// <param name="items">Page items.</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        public PageObject(IDictionary<string, PdfObject> items, bool isReadOnly) : base(items, isReadOnly)
        {
        }

        /// <summary>
        /// The date and time when the page’s contents were most recently modified.
        /// </summary>
        public DateTimeOffset? LastModified => internalDictionary.ContainsKey(LastModifiedKey) ? (internalDictionary[LastModifiedKey] as DateObject).Value : (DateTimeOffset?)null;

        /// <summary>
        /// A <see cref="RectangleObject"/>, expressed in default user space units,
        /// defining the boundaries of the physical medium on which the page is intended to be displayed or printed.
        /// </summary>
        public RectangleObject MediaBox => internalDictionary.ContainsKey(MediaBoxKey) ? internalDictionary[MediaBoxKey] as RectangleObject : null;
    }
}
