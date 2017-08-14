using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Array Object
    /// </summary>
    public class ArrayObject : PdfObject, IEnumerable<PdfObject>
    {
        /// <summary>
        /// ArrayObject's start token
        /// </summary>
        public const char StartToken = '[';

        /// <summary>
        /// ArrayObject's end token
        /// </summary>
        public const char EndToken = ']';

        private readonly List<PdfObject> internalList;

        /// <summary>
        /// Instanciates a new StringObject
        /// </summary>
        /// <param name="items">Array items</param>
        public ArrayObject(IEnumerable<PdfObject> items)
        {
            internalList = new List<PdfObject>(items);
        }

        /// <summary>
        /// Gets the number of objects in the array
        /// </summary>
        public int Count => internalList.Count;

        /// <summary>
        /// Gets items enumerator
        /// </summary>
        /// <returns>items enumerator</returns>
        public IEnumerator<PdfObject> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        /// <summary>
        /// gets items enumerator
        /// </summary>
        /// <returns>items enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        /// <summary>
        /// Creates an ArrayObject from PdfReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use</param>
        /// <returns>Created ArrayObject</returns>
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
