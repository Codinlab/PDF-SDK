using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        private readonly Dictionary<int, PdfObjectReferenceBase> internalDictionary;

        #region Constructors
        /// <summary>
        /// Instanciates a new PDF Cross-Reference Section.
        /// </summary>
        /// <param name="entries">TCross-Reference Section entries.</param>
        public XRefSection(IDictionary<int, PdfObjectReferenceBase> entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            internalDictionary = new Dictionary<int, PdfObjectReferenceBase>(entries);
        }

        /// <summary>
        /// Instanciates a new PDF Cross-Reference Section.
        /// </summary>
        /// <param name="entries">TCross-Reference Section entries.</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        private XRefSection(Dictionary<int, PdfObjectReferenceBase> entries, bool isReadOnly)
        {
            internalDictionary = entries;
        }

        #endregion

        /// <summary>
        /// Gets the list of Cross-Reference Stream's entries.
        /// </summary>
        public IReadOnlyDictionary<int, PdfObjectReferenceBase> Entries => new ReadOnlyDictionary<int, PdfObjectReferenceBase>(internalDictionary);

        /// <summary>
        /// Writes PDF Cross-Reference Section instance to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public void Write(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteLine(StartKeyword);

            if (internalDictionary.Count == 0)
            {
                // Write empty section
                writer.WriteLine("0 0");
                return;
            }

            // Key : first object number
            // Value : number of entries
            var subSections = new Dictionary<int, int>();

            int i = 0, j;
            var ids = internalDictionary.Keys.ToArray();
            Array.Sort(ids);

            do
            {
                j = i + 1;
                while(j < ids.Length && ids[j] == ids[j - 1] + 1)
                {
                    j++;
                }
                subSections.Add(ids[i], j - i);
                i = j;
            }
            while (i < ids.Length);

            foreach(var sub in subSections)
            {
                // Write header
                writer.WriteLine($"{sub.Key} {sub.Value}");

                for (int k = sub.Key; k < sub.Key + sub.Value; k++)
                {
                    // Write entry
                    var entry = internalDictionary[k];
                    long n;
                    char l;

                    ushort g = entry.ObjectId.GenerationNumber;

                    if (entry is PdfObjectReference)
                    {
                        n = ((PdfObjectReference)entry).Position;
                        l = 'n';
                    }
                    else if (entry is PdfFreeObjectReference)
                    {
                        n = ((PdfFreeObjectReference)entry).NextFreeObjectNumber;
                        l = 'f';
                    }
                    else
                    {
                        throw new NotSupportedException("Unsupported object reference.");
                    }

                    writer.Write($"{n.ToString("D10")} {g.ToString("D5")} {l}\r\n");
                }
            }
        }

        /// <summary>
        /// Reads a section and append entries.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        public static XRefSection FromReader(PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            reader.ReadToken(StartKeyword);
            reader.MoveToNonWhiteSpace();

            var entries = new Dictionary<int, PdfObjectReferenceBase>();
            string subsectionHeader;
            var entryBuffer = new char[20];

            while (char.IsDigit(reader.Peek()))
            {
                int separatorIdx, firstId, count;

                try
                {
                    subsectionHeader = reader.ReadLine();
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

                    var objectId = new PdfObjectId(i, ushort.Parse(new string(entryBuffer, 11, 5)));
                    switch (entryBuffer[17])
                    {
                        case 'n':
                            objectReference = new PdfObjectReference(objectId, long.Parse(new string(entryBuffer, 0, 10)));
                            break;
                        case 'f':
                            objectReference = new PdfFreeObjectReference(objectId, int.Parse(new string(entryBuffer, 0, 10)));
                            break;
                        default:
                            throw new FormatException("Invalid Cross-Reference entry");
                    }

                    entries.Add(
                        i,
                        objectReference
                    );
                }
            }

            return new XRefSection(entries, true);
        }
    }
}
