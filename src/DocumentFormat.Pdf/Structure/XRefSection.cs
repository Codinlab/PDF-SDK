using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Represents the Pdf Cross-Reference Section
    /// </summary>
    public class XRefSection : IXRefSection
    {
        /// <summary>
        /// Indicates if Cross-Reference Section instance is read-only.
        /// </summary>
        public bool IsReadOnly { get; protected set; }

        /// <summary>
        /// The xref keyword
        /// </summary>
        public const string StartKeyword = "xref";

        private readonly Dictionary<PdfObjectId, PdfObjectReferenceBase> internalDictionary;

        /// <summary>
        /// Instanciates a new PDF Cross-Reference Section.
        /// </summary>
        public XRefSection() : this(false)
        {
        }

        /// <summary>
        /// Instanciates a new PDF Cross-Reference Section.
        /// </summary>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        public XRefSection(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
            internalDictionary = new Dictionary<PdfObjectId, PdfObjectReferenceBase>();
        }

        /// <summary>
        /// Instanciates a new PDF Cross-Reference Section.
        /// </summary>
        /// <param name="entries">TCross-Reference Section entries.</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        private XRefSection(Dictionary<PdfObjectId, PdfObjectReferenceBase> entries, bool isReadOnly)
        {
            internalDictionary = entries;
        }

        /// <summary>
        /// Gets the list of Cross-Reference Stream's entries.
        /// </summary>
        public IReadOnlyDictionary<PdfObjectId, PdfObjectReferenceBase> Entries => new ReadOnlyDictionary<PdfObjectId, PdfObjectReferenceBase>(internalDictionary);

        /// <summary>
        /// Reads a section and append entries.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        public static XRefSection FromReader(PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var entries = new Dictionary<PdfObjectId, PdfObjectReferenceBase>();
            string subsectionHeader;
            var entryBuffer = new char[20];

            while ((subsectionHeader = reader.ReadLine()) != PdfTrailer.StartKeyword)
            {
                int separatorIdx, firstId, count;

                try
                {
                    separatorIdx = subsectionHeader.IndexOf(' ');
                    firstId = Int32.Parse(subsectionHeader.Substring(0, separatorIdx));
                    count = Int32.Parse(subsectionHeader.Substring(separatorIdx + 1));
                }
                catch
                {
                    throw new FormatException("Invalid Cross-Reference Table subsection header.");
                }

                for (int i = firstId; i < firstId + count; i++)
                {
                    // Each entry is exactly 20 bytes long, including the end-of - line marker.
                    reader.Read(entryBuffer, 0, entryBuffer.Length);

                    PdfObjectReferenceBase objectReference;

                    switch (entryBuffer[17])
                    {
                        case 'n':
                            objectReference = new PdfObjectReference(long.Parse(new string(entryBuffer, 0, 10)));
                            break;
                        case 'f':
                            objectReference = new PdfFreeObjectReference(int.Parse(new string(entryBuffer, 0, 10)));
                            break;
                        default:
                            throw new FormatException("Invalid Cross-Reference entry");
                    }

                    entries.Add(
                        new PdfObjectId(i, ushort.Parse(new string(entryBuffer, 11, 5))),
                        objectReference
                    );
                }
            }

            return new XRefSection(entries, true);
        }
    }
}
