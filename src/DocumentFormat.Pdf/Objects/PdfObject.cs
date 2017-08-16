using DocumentFormat.Pdf.IO;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Base class for Pdf objects
    /// </summary>
    public abstract class PdfObject
    {
        /// <summary>
        /// When overridden in a derived class, writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public abstract void Write(PdfWriter writer);
    }
}
