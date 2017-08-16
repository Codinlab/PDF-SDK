using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Structure;
using System;

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

        /// <summary>
        /// Writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public override void Write(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            // TODO : Implement Write method

            throw new NotImplementedException();
        }
    }
}
