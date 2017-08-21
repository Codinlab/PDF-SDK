using DocumentFormat.Pdf.Attributes;
using DocumentFormat.Pdf.Exceptions;
using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Array Object.
    /// </summary>
    [HasDelimiters]
    public class ArrayObject : PdfObject, IList<PdfObject>
    {
        /// <summary>
        /// ArrayObject's start token.
        /// </summary>
        public const char StartToken = '[';

        /// <summary>
        /// ArrayObject's end token.
        /// </summary>
        public const char EndToken = ']';

        /// <summary>
        /// Represents the internaly hold elements list.
        /// </summary>
        protected readonly List<PdfObject> internalList;

        /// <summary>
        /// Instanciates a new StringObject.
        /// </summary>
        public ArrayObject()
        {
            internalList = new List<PdfObject>();
        }

        /// <summary>
        /// Instanciates a new StringObject.
        /// </summary>
        /// <param name="items">Array items.</param>
        public ArrayObject(IEnumerable<PdfObject> items)
        {
            internalList = new List<PdfObject>(items);
        }

        /// <summary>
        /// Instanciates a new StringObject.
        /// </summary>
        /// <param name="items">Array items.</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        internal ArrayObject(IEnumerable<PdfObject> items, bool isReadOnly) : base(isReadOnly)
        {
            internalList = new List<PdfObject>(items);
        }

        /// <summary>
        /// Gets the number of objects in the array.
        /// </summary>
        public int Count => internalList.Count;

        /// <summary>
        /// Get or sets item at specified index.
        /// </summary>
        /// <param name="index">Item index.</param>
        /// <returns>Item at specified index.</returns>
        public PdfObject this[int index] {
            get {
                return internalList[index];
            }
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                internalList[index] = value;
            }
        }

        /// <summary>
        /// Gets items enumerator.
        /// </summary>
        /// <returns>items enumerator.</returns>
        public IEnumerator<PdfObject> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        /// <summary>
        /// gets items enumerator.
        /// </summary>
        /// <returns>items enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        public int IndexOf(PdfObject item)
        {
            return internalList.IndexOf(item);
        }

        public void Insert(int index, PdfObject item)
        {
            if (IsReadOnly)
                throw new ObjectReadOnlyException();

            internalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (IsReadOnly)
                throw new ObjectReadOnlyException();

            internalList.RemoveAt(index);
        }

        public void Add(PdfObject item)
        {
            if (IsReadOnly)
                throw new ObjectReadOnlyException();

            internalList.Add(item);
        }

        public void Clear()
        {
            if (IsReadOnly)
                throw new ObjectReadOnlyException();

            internalList.Clear();
        }

        public bool Contains(PdfObject item)
        {
            return internalList.Contains(item);
        }

        public void CopyTo(PdfObject[] array, int arrayIndex)
        {
            internalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(PdfObject item)
        {
            if (IsReadOnly)
                throw new ObjectReadOnlyException();

            return internalList.Remove(item);
        }

        /// <summary>
        /// Writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public override void Write(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.Write(StartToken);

            HasDelimitersAttribute hasDelimiter;
            bool endsWithDelimiter = true;

            foreach (var obj in internalList)
            {
                hasDelimiter = obj.GetHasDelimiterAttibute();

                if (!endsWithDelimiter && !(hasDelimiter?.AtStart ?? false))
                {
                    // Append separator
                    writer.Write(Chars.SP);
                }
                obj.Write(writer);

                endsWithDelimiter = hasDelimiter?.AtEnd ?? false;
            }

            writer.Write(EndToken);
        }

        /// <summary>
        /// Creates an ArrayObject from PdfReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>Created ArrayObject.</returns>
        public static ArrayObject FromReader(PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (reader.Read() != StartToken)
                throw new FormatException("An array start token was expected.");

            reader.MoveToNonWhiteSpace();

            var elementsList = new List<PdfObject>();

            while (reader.Peek() != EndToken)
            {
                elementsList.Add(reader.ReadObject());
                reader.MoveToNonWhiteSpace();
            }

            // Skip end token
            reader.Position++;

            return new ArrayObject(elementsList, true);
        }
    }
}
