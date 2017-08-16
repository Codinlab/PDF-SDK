using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DocumentFormat.Pdf.Filters
{
    /// <summary>
    /// Represents the FlateDecode filter.
    /// </summary>
    public class FlateDecode : PdfFilter
    {
        /// <summary>
        /// FlateDecode filter name.
        /// </summary>
        public override string Name => "FlateDecode";

        /// <summary>
        /// Decodes data using FlateDecode filter.
        /// </summary>
        /// <param name="data">The data to decode.</param>
        /// <returns>Decoded data.</returns>
        public override byte[] Decode(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            using (var inputStream = new MemoryStream(data))
            {
                var header = new byte[2];
                inputStream.Read(header, 0, header.Length);
                using (var outputStream = new MemoryStream())
                {
                    using (var deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress))
                    {
                        deflateStream.CopyTo(outputStream);
                    }

                    return outputStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Encodes data using FlateDecode filter.
        /// </summary>
        /// <param name="data">The data to encode.</param>
        /// <returns>Encoded data.</returns>
        public override byte[] Encode(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            using (var inputStream = new MemoryStream(data))
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var deflateStream = new DeflateStream(outputStream, CompressionMode.Compress))
                    {
                        inputStream.CopyTo(deflateStream);
                    }

                    return outputStream.ToArray();
                }
            }
        }
    }
}
