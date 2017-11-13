using DocumentFormat.Pdf.IO;
using System;

namespace DocumentFormat.Pdf.Structure
{
    /// <summary>
    /// Represents a PdfObjectId.
    /// </summary>
    public struct PdfObjectId
    {
        /// <summary>
        /// Reprensents the zero object id.
        /// </summary>
        public static PdfObjectId Zero = new PdfObjectId(0, ushort.MaxValue);

        private int objectNumber;
        private ushort generationNumber;

        /// <summary>
        /// Gets object number.
        /// </summary>
        public int ObjectNumber => objectNumber;

        /// <summary>
        /// Gets generation number.
        /// </summary>
        public ushort GenerationNumber => generationNumber;

        /// <summary>
        /// Instanciates a PdfObjectId based on its object number.
        /// </summary>
        /// <param name="objectNumber">The object number.</param>
        public PdfObjectId(int objectNumber) : this(objectNumber, 0)
        {
        }

        /// <summary>
        /// Instanciates a PdfObjectId based on its object number and generation number.
        /// </summary>
        /// <param name="objectNumber">The object number.</param>
        /// <param name="generationNumber">The object generation number.</param>
        public PdfObjectId(int objectNumber, ushort generationNumber)
        {
            this.objectNumber = objectNumber;
            this.generationNumber = generationNumber;
        }

        /// <summary>
        /// Writes object reference to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        internal void WriteReference(PdfWriter writer)
        {
            writer.Write($"{objectNumber.ToString()} {generationNumber.ToString()} R");
        }

        /// <summary>
        /// Returns object's HashCode.
        /// </summary>
        /// <returns>Object's HashCode.</returns>
        public override int GetHashCode()
        {
            return objectNumber.GetHashCode() ^ generationNumber.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is PdfObjectId && this == (PdfObjectId)obj;
        }

        public static bool operator ==(PdfObjectId a, PdfObjectId b)
        {
            return a.objectNumber == b.objectNumber && a.generationNumber == b.generationNumber;
        }

        public static bool operator !=(PdfObjectId a, PdfObjectId b)
        {
            return a.objectNumber != b.objectNumber || a.generationNumber != b.generationNumber;
        }

        /// <summary>
        /// Parses a PdfObjectId from a string.
        /// </summary>
        /// <param name="parsedString">The string to parse.</param>
        /// <returns>Parsed PdfObjectId.</returns>
        public static PdfObjectId FromString(string parsedString)
        {
            if (parsedString == null)
                throw new ArgumentNullException(nameof(parsedString));

            int sepIdx = parsedString.IndexOf(Chars.SP);

            if(sepIdx < 1)
                throw new FormatException("Unable to parse ObjectId");

            return new PdfObjectId(int.Parse(parsedString.Substring(0, sepIdx)), ushort.Parse(parsedString.Substring(sepIdx + 1)));
        }
    }
}
