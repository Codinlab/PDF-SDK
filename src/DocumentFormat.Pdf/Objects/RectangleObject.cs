using DocumentFormat.Pdf.Exceptions;
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
        public float X1 {
            get => GetValueAt(0);
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                SetValueAt(0, value);
            }
        }

        /// <summary>
        /// Gets lower left y coordinate.
        /// </summary>
        public float Y1 {
            get => GetValueAt(1);
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                SetValueAt(1, value);
            }
        }

        /// <summary>
        /// Gets upper right x coordinate.
        /// </summary>
        public float X2 {
            get => GetValueAt(2);
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                SetValueAt(2, value);
            }
        }

        /// <summary>
        /// Gets upper right y coordinate.
        /// </summary>
        public float Y2 {
            get => GetValueAt(3);
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                SetValueAt(3, value);
            }
        }

        /// <summary>
        /// Instanciates a new RectangleObject.
        /// </summary>
        public RectangleObject() : this(0 ,0 ,0 ,0)
        {

        }

        /// <summary>
        /// Instanciates a new RectangleObject from coordinates.
        /// </summary>
        /// <param name="x1">Lower left x coordinate.</param>
        /// <param name="y1">Lower left y coordinate.</param>
        /// <param name="x2">Upper right x coordinate.</param>
        /// <param name="y2">Upper right y coordinate.</param>
        public RectangleObject(float x1, float y1, float x2, float y2)
        {
            internalList.Add(new RealObject(x1));
            internalList.Add(new RealObject(y1));
            internalList.Add(new RealObject(x2));
            internalList.Add(new RealObject(y2));
        }

        /// <summary>
        /// Instanciates a new RectangleObject.
        /// </summary>
        /// <param name="items">Array items.</param>
        internal RectangleObject(IEnumerable<PdfObject> items) : base(items)
        {
            // TODO : Check items.
        }

        private float GetValueAt(int index)
        {
            return (internalList[index] as NumericObject).RealValue;
        }

        private void SetValueAt(int index, float value)
        {
            if (internalList[index].IsReadOnly)
            {
                internalList[index] = new RealObject(value);
            }
            else
            {
                (internalList[index] as NumericObject).RealValue = value;
            }
        }
    }
}
