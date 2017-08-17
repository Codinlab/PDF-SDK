using DocumentFormat.Pdf.IO;
using System;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Base class for Pdf String Object
    /// </summary>
    public abstract class StringObject : PdfObject
    {
        /// <summary>
        /// Internaly hold value.
        /// </summary>
        protected string value;

        /// <summary>
        /// Instanciates a new StringObject
        /// </summary>
        /// <param name="value">The object's value</param>
        public StringObject(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the object's value
        /// </summary>
        public string Value => value;

        /// <summary>
        /// Creates a StringObject from PdfReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use</param>
        /// <returns>Read StringObject</returns>
        public static StringObject FromReader(PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            char firstChar = reader.Peek();

            if(firstChar == LiteralStringObject.StartToken)
            {
                return LiteralStringObject.FromReader(reader);
            }
            else if(firstChar == HexadecimalStringObject.StartToken)
            {
                return HexadecimalStringObject.FromReader(reader);
            }
            else
            {
                throw new FormatException("Unexpected string object start");
            }
        }
    }
}
