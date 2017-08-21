using DocumentFormat.Pdf.Exceptions;
using DocumentFormat.Pdf.IO;
using System;
using System.Globalization;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Real Object
    /// </summary>
    public class RealObject : NumericObject
    {
        private float value;

        /// <summary>
        /// Instanciates a new RealObject
        /// </summary>
        /// <param name="value">The object's value</param>
        public RealObject(float value)
        {
            this.value = value;
        }

        /// <summary>
        /// Instanciates a new RealObject
        /// </summary>
        /// <param name="value">The object's value</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        internal RealObject(float value, bool isReadOnly) : base(isReadOnly)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the object's value converted to integer
        /// </summary>
        public override int IntergerValue {
            get => Convert.ToInt32(value);
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                this.value = Convert.ToSingle(value);
            }
        }

        /// <summary>
        /// Gets the object's value
        /// </summary>
        public override float RealValue {
            get => value;
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                this.value = value;
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
