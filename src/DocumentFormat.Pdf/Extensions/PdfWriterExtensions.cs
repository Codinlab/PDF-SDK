using DocumentFormat.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentFormat.Pdf.Extensions
{
    /// <summary>
    /// <see cref="PdfWriter"/> extension methods.
    /// </summary>
    public static class PdfWriterExtensions
    {
        /// <summary>
        /// Writes a comment to the PDF stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        /// <param name="comment">The comment to write.</param>
        public static void WriteComment(this PdfWriter writer, string comment)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var lines = comment.Split(new char[] { Chars.CR, Chars.LF }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                writer.Write('%');
                writer.WriteLine(line);
            }
        }
    }
}
