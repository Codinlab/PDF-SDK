using System;
using System.Collections;
using System.Collections.Generic;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Represents the Pdf Cross-Reference Table.
    /// </summary>
    public class XRefTable
    {
        /// <summary>
        /// The startxref keyword
        /// </summary>
        public const string StartXRefToken = "startxref";

        /// <summary>
        /// Internal list of Cross-Reference entries.
        /// </summary>
        private readonly Dictionary<int, PdfObjectReferenceBase> internalDictionary;

        /// <summary>
        /// Internal list of updated Cross-Reference entries.
        /// </summary>
        private Dictionary<int, PdfObjectReferenceBase> updatedEntries = new Dictionary<int, PdfObjectReferenceBase>();

        /// <summary>
        /// The total number of entries in the file’s cross-reference table,
        /// as defined by the combination of the original section and all update sections.
        /// Equivalently, this value is 1 greater than the highest object number used in the file.
        /// </summary>
        private int size;

        #region Constructors
        /// <summary>
        /// Instanciates a new PDF Cross-Reference Table.
        /// </summary>
        public XRefTable()
        {
            internalDictionary = new Dictionary<int, PdfObjectReferenceBase>();
            size = 1;
        }

        /// <summary>
        /// Instanciates a new PDF Cross-Reference Table with an initial <see cref="IXRefSection"/>.
        /// </summary>
        /// <param name="section">The initial Cross-Reference section.</param>
        /// <param name="size">The total number of entries in the file’s cross-reference table,
        /// as defined by the combination of the original section and all update sections.</param>
        public XRefTable(IXRefSection section, int size)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            var entries = section.Entries;

            if (entries == null)
                throw new NullReferenceException("Section's entries dictionary cannot be null");

            internalDictionary = new Dictionary<int, PdfObjectReferenceBase>();
            this.size = size;

            foreach (var entry in entries)
            {
                internalDictionary.Add(entry.Key, entry.Value);
            }
        }

        #endregion

        /// <summary>
        /// Registers a new <see cref="IXRefSection"/> into Cross-Reference Table.
        /// </summary>
        /// <param name="section">The Cross-Reference section to add.</param>
        public void AddSection(IXRefSection section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            var entries = section.Entries;

            if(entries == null)
                return;

            foreach (var entry in entries)
            {
                if (!internalDictionary.ContainsKey(entry.Key))
                {
                    internalDictionary.Add(entry.Key, entry.Value);
                }
            }
        }
    }
}
