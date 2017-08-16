using System.Collections.Generic;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Rectangle Object.
    /// </summary>
    public class RectangleObject : ArrayObject
    {
        /// <summary>
        /// Gets lower left x coordinate.
        /// </summary>
        public float X1 => (internalList[0] as NumericObject).RealValue;

        /// <summary>
        /// Gets lower left y coordinate.
        /// </summary>
        public float Y1 => (internalList[1] as NumericObject).RealValue;

        /// <summary>
        /// Gets upper right x coordinate.
        /// </summary>
        public float X2 => (internalList[2] as NumericObject).RealValue;

        /// <summary>
        /// Gets upper right y coordinate.
        /// </summary>
        public float Y2 => (internalList[3] as NumericObject).RealValue;

        /// <summary>
        /// Instanciates a new RectangleObject.
        /// </summary>
        /// <param name="items">Array items.</param>
        public RectangleObject(IEnumerable<PdfObject> items) : base(items)
        {

        }
    }
}
