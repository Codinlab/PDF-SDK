using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Stream Object.
    /// </summary>
    public class StreamObject : DictionaryObject
    {
        /// <summary>
        /// The stream keyword
        /// </summary>
        public const string StartKeyword = "stream";

        /// <summary>
        /// The endstream keyword
        /// </summary>
        public const string EndKeyword = "endstream";

        /// <summary>
        /// The Length key name
        /// </summary>
        public const string LengthKey = "Length";

        /// <summary>
        /// The Filter key name
        /// </summary>
        public const string FilterKey = "Filter";

        /// <summary>
        /// Holds unfiltered stream data
        /// </summary>
        private byte[] data;

        /// <summary>
        /// Holds filtered stream data
        /// </summary>
        private byte[] encodedData;

        /// <summary>
        /// Instanciates a new StringObject.
        /// </summary>
        /// <param name="dictionaryItems">Dictionary items.</param>
        /// <param name="data">Stream data.</param>
        public StreamObject(IDictionary<string, PdfObject> dictionaryItems, byte[] data) : base(dictionaryItems)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Instanciates a new StringObject.
        /// </summary>
        /// <param name="dictionaryItems">Dictionary items.</param>
        protected StreamObject(IDictionary<string, PdfObject> dictionaryItems) : base(dictionaryItems)
        {

        }

        /// <summary>
        /// Gets the stream length.
        /// </summary>
        public int Length => (internalDictionary[LengthKey] as IntegerObject).IntergerValue;

        /// <summary>
        /// Gets unfiltered read-only data stream
        /// </summary>
        public Stream Data {
            get {
                if (data == null)
                {
                    if (encodedData != null)
                    {
                        DecodeData();
                    }
                    else
                    {
                        return null;
                    }
                }
                return new MemoryStream(data, false);
            }
        }

        /// <summary>
        /// Gets the name of the filter applied to data.
        /// </summary>
        public PdfObject Filter => internalDictionary.ContainsKey(FilterKey) ? internalDictionary[FilterKey] : null;

        /// <summary>
        /// 
        /// </summary>
        private void EncodeData()
        {
            if (Filter == null)
            {
                encodedData = data;
            }
            else
            {
                // TODO : handle filters
                throw new NotImplementedException();
            }

            // Set length
            internalDictionary[LengthKey] = new IntegerObject(encodedData.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        private void DecodeData()
        {
            if (Filter == null)
            {
                data = encodedData;
            }
            else
            {
                // TODO : handle filters
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public override void Write(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (encodedData == null)
            {
                if (data != null)
                {
                    EncodeData();
                }
                else
                {
                    throw new NullReferenceException("Cannot write null stream data");
                }
            }

            base.Write(writer); // Write dictionary
            writer.WriteLine(StartKeyword);

            writer.Write(encodedData, 0, encodedData.Length);

            writer.WriteLine();
            writer.WriteLine(EndKeyword);
        }

        /// <summary>
        /// Creates a StreamObject from PdfReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <param name="streamDictionary">The dictionary describing the stream.</param>
        /// <returns>Created StreamObject.</returns>
        internal static StreamObject FromReader(PdfReader reader, Dictionary<string, PdfObject> streamDictionary)
        {
            reader.ReadToken(StartKeyword);

            char nextChar = reader.Read();
            if(nextChar == Chars.CR)
            {
                if(reader.Read() != Chars.LF)
                {
                    throw new FormatException("Bad end of line after stream keyword.");
                }
            }
            else if(nextChar != Chars.LF)
            {
                throw new FormatException("Bad end of line after stream keyword.");
            }

            var length = (streamDictionary[LengthKey] as IntegerObject).IntergerValue;
            var data = new byte[length];

            // Read data
            var read = reader.Read(data, 0, length);

            if(read != length)
                throw new FormatException("Bad stream length.");

            var streamObj = new StreamObject(streamDictionary)
            {
                encodedData = data
            };

            return streamObj;
        }
    }
}
