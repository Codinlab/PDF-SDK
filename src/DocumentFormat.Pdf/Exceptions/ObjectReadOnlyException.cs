using DocumentFormat.Pdf.Objects;
using System;

namespace DocumentFormat.Pdf.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to update a read-only <see cref="PdfObject"/>.
    /// </summary>
    public class ObjectReadOnlyException : Exception
    {
        private const string DefaultMessage = "Object is currently in a read-only state. You should copy the object before changing its value.";

        /// <summary>
        /// Instanciates a new ObjectReadOnlyException with default message.
        /// </summary>
        public ObjectReadOnlyException() : base(DefaultMessage)
        {

        }

        /// <summary>
        /// Instanciates a new ObjectReadOnlyException with custom message.
        /// </summary>
        public ObjectReadOnlyException(string message) : base(message)
        {

        }
    }
}
