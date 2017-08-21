using DocumentFormat.Pdf.IO;
using System.Globalization;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Base class for numeric objects
    /// </summary>
    public abstract class NumericObject : PdfObject
    {
        /// <summary>
        /// Instanciates a new NumericObject
        /// </summary>
        protected NumericObject()
        {

        }

        /// <summary>
        /// Instanciates a new NumericObject
        /// </summary>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        protected NumericObject(bool isReadOnly) : base(isReadOnly)
        {

        }

        /// <summary>
        /// Gets or sets the object's value as an integer
        /// </summary>
        public abstract int IntergerValue { get; set; }

        /// <summary>
        /// Gets or sets the object's value as a float
        /// </summary>
        public abstract float RealValue { get; set; }

        /// <summary>
        /// Creates a NumericObject object from PdfReader
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use</param>
        /// <returns>Read NumericObject</returns>
        public static NumericObject FromReader(PdfReader reader)
        {
            var sValue = reader.ReadWhile(c => !Chars.IsDelimiterOrWhiteSpace(c));
            if (sValue.Contains("."))
            {
                return new RealObject(float.Parse(sValue, CultureInfo.InvariantCulture), true);
            }
            else
            {
                return new IntegerObject(int.Parse(sValue), true);
            }
        }
    }
}
