namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Base class for Object reference stored in Cross-Reference table.
    /// </summary>
    public abstract class PdfObjectReferenceBase
    {
        protected PdfObjectId objectId;

        /// <summary>
        /// Instanciates a new PDF object reference.
        /// </summary>
        /// <param name="objectId">The referenced object's <see cref="PdfObjectId"/>.</param>
        protected PdfObjectReferenceBase(PdfObjectId objectId)
        {
            this.objectId = objectId;
        }

        /// <summary>
        /// Gets the referenced <see cref="PdfObjectId"/>.
        /// </summary>
        public PdfObjectId ObjectId => objectId;
    }
}
