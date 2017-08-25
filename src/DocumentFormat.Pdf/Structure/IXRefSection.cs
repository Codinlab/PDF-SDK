using System.Collections.Generic;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Interface for Cross-Reference Section objets.
    /// </summary>
    public interface IXRefSection
    {
        /// <summary>
        /// Gets section's entries.
        /// </summary>
        IReadOnlyDictionary<int, PdfObjectReferenceBase> Entries { get; }
    }
}
