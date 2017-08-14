using DocumentFormat.Pdf.Objects;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Reprensents a PdfObjectReference
    /// </summary>
    public class PdfObjectReference
    {
        bool inUse;

        long position;

        /// <summary>
        /// Instanciates a new PdfObjectReference
        /// Reference is marked in use by default
        /// </summary>
        /// <param name="position">The position of the object</param>
        public PdfObjectReference(long position) : this(position, true)
        {

        }

        /// <summary>
        /// Instanciates a new PdfObjectReference
        /// </summary>
        /// <param name="position">The position of the object</param>
        /// <param name="inUse">True if the reference is used, otherwise False</param>
        public PdfObjectReference(long position, bool inUse)
        {
            this.position = position;
            this.inUse = inUse;
        }

        /// <summary>
        /// Gets referenced object position
        /// </summary>
        public long Position => position;

        /// <summary>
        /// Gets a boolean indicating if the reference is marked in use
        /// </summary>
        public bool InUse => inUse;

        /// <summary>
        /// Gets or sets the referenced object
        /// </summary>
        public IndirectObject IndirectObject { get; set; }
    }
}
