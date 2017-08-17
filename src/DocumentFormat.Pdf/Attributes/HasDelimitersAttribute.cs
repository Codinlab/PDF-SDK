using System;

namespace DocumentFormat.Pdf.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class HasDelimitersAttribute : Attribute
    {
        /// <summary>
        /// Indicates if object has a start delimiter.
        /// Default value is true;
        /// </summary>
        public bool AtStart { get; set; } = true;

        /// <summary>
        /// Indicates if object has a end delimiter.
        /// Default value is true;
        /// </summary>
        public bool AtEnd { get; set; } = true;
    }
}
