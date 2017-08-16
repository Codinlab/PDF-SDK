namespace DocumentFormat.Pdf.Filters
{
    /// <summary>
    /// Represents PDF Filter base class
    /// </summary>
    public abstract class PdfFilter
    {
        /// <summary>
        /// Filter's Name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// When implemented in a derived class encodes the specified data.
        /// </summary>
        /// <param name="data">The byte array to encode.</param>
        /// <returns>Encoded data.</returns>
        public abstract byte[] Encode(byte[] data);

        /// <summary>
        /// When implemented in a derived class decodes the specified data.
        /// </summary>
        /// <param name="data">The byte array to decode.</param>
        /// <returns>Decoded data.</returns>
        public abstract byte[] Decode(byte[] data);
    }
}
