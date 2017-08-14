using DocumentFormat.Pdf.IO;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Boolean Object
    /// </summary>
    public class BooleanObject : PdfObject
    {
        /// <summary>
        /// The "true" token
        /// </summary>
        public const string TrueToken = "true";

        /// <summary>
        /// The "false" token
        /// </summary>
        public const string FalseToken = "false";

        private bool value;

        /// <summary>
        /// Instanciates a new BooleanObject
        /// </summary>
        /// <param name="value">The object's value</param>
        public BooleanObject(bool value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the object's value
        /// </summary>
        public bool Value {
            get => value;
        }
    }
}
