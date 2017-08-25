using System;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Reprensents free PdfObjectReference
    /// </summary>
    public class PdfFreeObjectReference : PdfObjectReferenceBase
    {
        int nextFreeObjectNumber;

        /// <summary>
        /// Instanciates a new Free Object refernce.
        /// </summary>
        /// <param name="nextFreeObjectNumber">The object number of the next free object.</param>
        public PdfFreeObjectReference(int nextFreeObjectNumber)
        {
            if (nextFreeObjectNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(nextFreeObjectNumber), "Cannot be negative.");

            this.nextFreeObjectNumber = nextFreeObjectNumber;
        }

        /// <summary>
        /// Gets the object number of the next free object.
        /// </summary>
        public int NextFreeObjectNumber => nextFreeObjectNumber;
    }
}
