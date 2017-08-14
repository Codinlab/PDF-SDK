using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Represents the Pdf Cross-Reference Table
    /// </summary>
    public class XRefTable : IDictionary<PdfObjectId, PdfObjectReference>
    {
        /// <summary>
        /// The startxref keyword
        /// </summary>
        public const string StartXRefToken = "startxref";

        /// <summary>
        /// The xref keyword
        /// </summary>
        public const string StartKeyword = "xref";

        private readonly Dictionary<PdfObjectId, PdfObjectReference> internalDictionary;

        /// <summary>
        /// Instanciates a new PDF Cross-Reference Table
        /// </summary>
        public XRefTable()
        {
            internalDictionary = new Dictionary<PdfObjectId, PdfObjectReference>();
        }

        #region IDictionary implementation
        public PdfObjectReference this[PdfObjectId key] {
            get => internalDictionary[key];
            set => internalDictionary[key] = value;
        }

        public ICollection<PdfObjectId> Keys => internalDictionary.Keys;

        public ICollection<PdfObjectReference> Values => internalDictionary.Values;

        public int Count => internalDictionary.Count;

        public bool IsReadOnly => false;

        public void Add(PdfObjectId key, PdfObjectReference value)
        {
            internalDictionary.Add(key, value);
        }

        public void Add(KeyValuePair<PdfObjectId, PdfObjectReference> item)
        {
            internalDictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            internalDictionary.Clear();
        }

        public bool Contains(KeyValuePair<PdfObjectId, PdfObjectReference> item)
        {
            return internalDictionary.ContainsKey(item.Key) && internalDictionary[item.Key] == item.Value;
        }

        public bool ContainsKey(PdfObjectId key)
        {
            return internalDictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<PdfObjectId, PdfObjectReference>[] array, int arrayIndex)
        {
            foreach(var item in internalDictionary)
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        public IEnumerator<KeyValuePair<PdfObjectId, PdfObjectReference>> GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }

        public bool Remove(PdfObjectId key)
        {
            return internalDictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<PdfObjectId, PdfObjectReference> item)
        {
            if (internalDictionary.ContainsKey(item.Key))
            {
                return internalDictionary.Remove(item.Key);
            }
            else
            {
                return false;
            }
        }

        public bool TryGetValue(PdfObjectId key, out PdfObjectReference value)
        {
            return internalDictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Reads a section and append entries.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        public void ReadSection(PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

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

                    internalDictionary.Add(
                        new PdfObjectId(i, ushort.Parse(new string(entryBuffer, 11, 5))),
                        new PdfObjectReference(long.Parse(new string(entryBuffer, 0, 10)), entryBuffer[17] == 'n')
                    );
                }
            }
        }
    }
}
