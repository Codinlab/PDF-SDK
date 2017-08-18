using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Structure;
using System;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents an unresolved IndirectReference
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

        /// <summary>
        /// Throws NotSupportedException.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public override void Write(PdfWriter writer)
        {
            throw new NotSupportedException("Connot write an unresolved object.");
        }
    }
}
