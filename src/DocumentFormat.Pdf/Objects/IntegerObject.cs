using DocumentFormat.Pdf.Exceptions;
using DocumentFormat.Pdf.IO;
using System;
using System.Globalization;

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
        /// Instanciates a new IntegerObject
        /// </summary>
        /// <param name="value">The object's value</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        internal IntegerObject(int value, bool isReadOnly) : base(isReadOnly)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the object's value
        /// </summary>
        public override int IntergerValue {
            get => value;
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                this.value = value;
            }
        }

        /// <summary>
        /// Gets the object's value converted to float
        /// </summary>
        public override float RealValue {
            get => Convert.ToSingle(value);
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                this.value = Convert.ToInt32(value);
            }
        }

        /// <summary>
        /// Writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public override void Write(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.Write(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
