using DocumentFormat.Pdf.Exceptions;
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
        /// Instanciates a new StreamObject.
        /// </summary>
        /// <param name="dictionaryItems">Dictionary items.</param>
        /// <param name="data">Stream data.</param>
        public StreamObject(IDictionary<string, PdfObject> dictionaryItems, byte[] data) : base(dictionaryItems)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Instanciates a new StreamObject.
        /// </summary>
        /// <param name="dictionaryItems">Dictionary items.</param>
        /// <param name="data">Stream encoded data.</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        protected StreamObject(IDictionary<string, PdfObject> dictionaryItems, byte[] data, bool isReadOnly) : base(dictionaryItems, isReadOnly)
        {
            encodedData = data;
        }

        /// <summary>
        /// Gets or sets the stream length.
        /// </summary>
        public int Length {
            get => internalDictionary.ContainsKey(LengthKey) ? (internalDictionary[LengthKey] as IntegerObject).IntergerValue : 0;
            protected set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                if (internalDictionary.ContainsKey(LengthKey) && !internalDictionary[LengthKey].IsReadOnly)
                {
                    (internalDictionary[LengthKey] as IntegerObject).IntergerValue = value;
                }
                else
                {
                    internalDictionary[LengthKey] = new IntegerObject(value);
                }
            }
        }

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
        public PdfObject Filter {
            get => internalDictionary.ContainsKey(FilterKey) ? internalDictionary[FilterKey] : null;
            protected set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                internalDictionary[LengthKey] = value;
            }
        }
        

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
            Length = encodedData.Length;
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

            return new StreamObject(streamDictionary, data, true);
        }
    }
}
