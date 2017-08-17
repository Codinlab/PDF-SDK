using DocumentFormat.Pdf.Attributes;
using DocumentFormat.Pdf.Objects;
using System;
using System.Reflection;

namespace DocumentFormat.Pdf.Extensions
{
    /// <summary>
    /// <see cref="PdfObject"/> extension methods.
    /// </summary>
    public static class PdfObjectExtensions
    {
        /// <summary>
        /// Gets <see cref="HasDelimitersAttribute"/> of object.
        /// </summary>
        /// <param name="target">The target <see cref="PdfObject"/>.</param>
        /// <returns>The objects <see cref="HasDelimitersAttribute"/> or null.</returns>
        internal static HasDelimitersAttribute GetHasDelimiterAttibute(this PdfObject target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return target.GetType().GetTypeInfo().GetCustomAttribute<HasDelimitersAttribute>();
        }

        /// <summary>
        /// Tests if an object has a start delimiter.
        /// </summary>
        /// <param name="target">The <see cref="PdfObject"/> to test.</param>
        /// <returns>True if object has a start delimiter, otherwise false.</returns>
        public static bool HasStartDelimiter(this PdfObject target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return target.GetType().GetTypeInfo().GetCustomAttribute<HasDelimitersAttribute>()?.AtStart ?? false;
        }

        /// <summary>
        /// Tests if an object has a end delimiter.
        /// </summary>
        /// <param name="target">The <see cref="PdfObject"/> to test.</param>
        /// <returns>True if object has a end delimiter, otherwise false.</returns>
        public static bool HasEndDelimiter(this PdfObject target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return target.GetType().GetTypeInfo().GetCustomAttribute<HasDelimitersAttribute>()?.AtEnd ?? false;
        }
    }
}
