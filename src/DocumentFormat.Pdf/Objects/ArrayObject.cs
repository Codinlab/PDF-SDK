using DocumentFormat.Pdf.Attributes;
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
    public class ArrayObject : PdfObject, IEnumerable<PdfObject>
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
        /// <param name="items">Array items.</param>
        public ArrayObject(IEnumerable<PdfObject> items)
        {
            internalList = new List<PdfObject>(items);
        }

        /// <summary>
        /// Gets the number of objects in the array.
        /// </summary>
        public int Count => internalList.Count;

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

            foreach(var obj in internalList)
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

            while(reader.Peek() != EndToken)
            {
                elementsList.Add(reader.ReadObject());
                reader.MoveToNonWhiteSpace();
            }

            // Skip end token
            reader.Position++;

            return new ArrayObject(elementsList);
        }
    }
}
