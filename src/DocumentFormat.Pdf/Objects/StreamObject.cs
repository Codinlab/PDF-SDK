using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using System;
using System.Collections.Generic;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Stream Object
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
        /// The length key name
        /// </summary>
        public const string LengthKey = "Length";

        private long? position;

        /// <summary>
        /// Instanciates a new StringObject
        /// </summary>
        /// <param name="dictionaryItems">Dictionary items</param>
        public StreamObject(IDictionary<string, PdfObject> dictionaryItems) : base(dictionaryItems)
        {
        }

        /// <summary>
        /// Instanciates a new StringObject
        /// </summary>
        /// <param name="dictionaryItems">Dictionary items</param>
        /// <param name="streamPosition">Position of the underlying stream</param>
        private StreamObject(IDictionary<string, PdfObject> dictionaryItems, long streamPosition) : base(dictionaryItems)
        {
            position = streamPosition;
        }

        /// <summary>
        /// Gets the stream length
        /// </summary>
        public int Length => (internalDictionary[LengthKey] as IntegerObject).IntergerValue;

        /// <summary>
        /// Gets the stream position in document stream
        /// </summary>
        public long? Position => position;

        /// <summary>
        /// Creates a StreamObject from PdfReader
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use</param>
        /// <param name="streamDictionary">The dictionary describing the stream</param>
        /// <returns>Created StreamObject</returns>
        internal static StreamObject FromReader(PdfReader reader, Dictionary<string, PdfObject> streamDictionary)
        {
            double startPosition;

            reader.ReadToken(StartKeyword);

            char nextChar = reader.Read();
            if(nextChar == Chars.CR)
            {
                if(reader.Read() == Chars.LF)
                {
                    startPosition = reader.Position;
                }
                else
                {
                    throw new FormatException("Bad end of line after stream keyword");
                }
            }
            else if(nextChar == Chars.LF)
            {
                startPosition = reader.Position;
            }
            else
            {
                throw new FormatException("Bad end of line after stream keyword");
            }

            return new StreamObject(streamDictionary, reader.Position);
        }
    }
}
