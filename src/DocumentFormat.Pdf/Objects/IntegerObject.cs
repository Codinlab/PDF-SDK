using DocumentFormat.Pdf.IO;
using System;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Integer Object
    /// </summary>
    public class IntegerObject : NumericObject
    {
        private int value;

        /// <summary>
        /// Instanciates a new IntegerObject
        /// </summary>
        /// <param name="value">The object's value</param>
        public IntegerObject(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the object's value
        /// </summary>
        public override int IntergerValue => value;

        /// <summary>
        /// Gets the object's value converted to float
        /// </summary>
        public override float RealValue => Convert.ToSingle(value);

        /// <summary>
        /// Writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public override void Write(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            // TODO : Implement Write method

            throw new NotImplementedException();
        }
    }
}
