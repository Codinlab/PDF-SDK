using DocumentFormat.Pdf.IO;
using System;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Null Object.
    /// </summary>
    public class NullObject : PdfObject
    {
        /// <summary>
        /// The "null" token.
        /// </summary>
        public const string NullToken = "null";

        /// <summary>
        /// Writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public override void Write(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.Write(NullToken);
        }
    }
}
