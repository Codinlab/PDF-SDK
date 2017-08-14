using DocumentFormat.Pdf.IO;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Null Object
    /// </summary>
    public class NullObject : PdfObject
    {
        /// <summary>
        /// The "null" token
        /// </summary>
        public const string NullToken = "null";
    }
}
