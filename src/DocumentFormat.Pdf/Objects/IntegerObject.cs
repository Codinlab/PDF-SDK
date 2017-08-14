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
    }
}
