using DocumentFormat.Pdf.IO;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Base class for numeric objects
    /// </summary>
    public abstract class NumericObject : PdfObject
    {
        /// <summary>
        /// Gets the object's value as an integer
        /// </summary>
        public abstract int IntergerValue { get; }

        /// <summary>
        /// Gets the object's value as a float
        /// </summary>
        public abstract float RealValue { get; }

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
                return new RealObject(float.Parse(sValue, CultureInfo.InvariantCulture));
            }
            else
            {
                return new IntegerObject(int.Parse(sValue));
            }
        }
    }
}
