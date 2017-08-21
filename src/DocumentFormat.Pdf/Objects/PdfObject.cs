using DocumentFormat.Pdf.IO;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Base class for Pdf objects
    /// </summary>
    public abstract class PdfObject
    {
        /// <summary>
        /// Indicates if object instance is read-only.
        /// </summary>
        public bool IsReadOnly { get; protected set; }

        /// <summary>
        /// Instanciates a new PdfPbject
        /// </summary>
        protected PdfObject() : this (false)
        {
        }

        /// <summary>
        /// Instanciates a new PdfPbject
        /// </summary>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        protected PdfObject(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
        }

        /// <summary>
        /// When overridden in a derived class, writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public abstract void Write(PdfWriter writer);
    }
}
