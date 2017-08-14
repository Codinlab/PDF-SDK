using DocumentFormat.Pdf.Structure;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents an IndirectReference
    /// </summary>
    public class IndirectReference : IndirectObject
    {
        /// <summary>
        /// Instanciates a new IndirectReference
        /// </summary>
        /// <param name="objectId">Referenced <see cref="PdfObjectId"/></param>
        public IndirectReference(PdfObjectId objectId) : base(objectId, null)
        {
        }
    }
}
