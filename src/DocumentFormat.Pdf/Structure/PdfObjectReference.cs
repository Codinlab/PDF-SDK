using DocumentFormat.Pdf.Objects;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Reprensents an in use PdfObjectReference
    /// </summary>
    public class PdfObjectReference : PdfObjectReferenceBase
    {
        long position;

        /// <summary>
        /// Instanciates a new PdfObjectReference
        /// Reference is marked in use by default
        /// </summary>
        /// <param name="position">The position of the object</param>
        public PdfObjectReference(long position)
        {
            this.position = position;
        }

        /// <summary>
        /// Gets referenced object position
        /// </summary>
        public long Position => position;

        /// <summary>
        /// Gets or sets the referenced object
        /// </summary>
        public IndirectObject IndirectObject { get; set; }
    }
}
