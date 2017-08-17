using DocumentFormat.Pdf.Attributes;
using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Dictionary Object.
    /// </summary>
    [HasDelimiters]
    public class DictionaryObject : PdfObject, IReadOnlyDictionary<string, PdfObject>
    {
        /// <summary>
        /// DictionaryObject's start token
        /// </summary>
        public const string StartToken = "<<";

        /// <summary>
        /// DictionaryObject's end token
        /// </summary>
        public const string EndToken = ">>";

        /// <summary>
        /// The Type key name
        /// </summary>
        protected const string TypeKey = "Type";

        /// <summary>
        /// Represents the internaly hold elements dictionary
        /// </summary>
        protected readonly Dictionary<string, PdfObject> internalDictionary;

        /// <summary>
        /// Instanciates a new StringObject.
        /// </summary>
        /// <param name="items">Dictionary items</param>
        public DictionaryObject(IDictionary<string, PdfObject> items)
        {
            internalDictionary = new Dictionary<string, PdfObject>(items);
        }

        /// <summary>
        /// Gets <see cref="PdfObject"/> with specified key.
        /// </summary>
        /// <param name="key">Object key.</param>
        /// <returns>Object at specified key.</returns>
        public PdfObject this[string key] => throw new NotImplementedException();

        /// <summary>
        /// Gets the number of objects in the array.
        /// </summary>
        public int Count => internalDictionary.Count;

        /// <summary>
        /// Gets keys enumerator.
        /// </summary>
        public IEnumerable<string> Keys => internalDictionary.Keys;

        /// <summary>
        /// Gets values enumerator.
        /// </summary>
        public IEnumerable<PdfObject> Values => internalDictionary.Values;

        /// <summary>
        /// Determines whether the DictionaryObject contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>True if the DictionaryObject contains the specified key, otherwise false.</returns>
        public bool ContainsKey(string key)
        {
            return internalDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the DictionaryObject's elements.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<KeyValuePair<string, PdfObject>> GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }

        /// <summary>
        /// Gets the PdfObject associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the <see cref="PdfObject"/> associated with the specified key, if the key is found; otherwise null.</param>
        /// <returns>true if the DictionaryObject contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(string key, out PdfObject value)
        {
            return TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the DictionaryObject's elements.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public override void Write(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            // TODO : Implement Write method

            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an DictionaryObject from PdfReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>Created DictionaryObject.</returns>
        public static DictionaryObject FromReader(PdfReader reader)
        {
            var elementsDictionary = ParseDictionary(reader);

            if(elementsDictionary.ContainsKey(StreamObject.LengthKey) && elementsDictionary[StreamObject.LengthKey] is IntegerObject)
            {
                // Could be a StreamObject
                reader.MoveToNonWhiteSpace();

                if(reader.Peek() == StreamObject.StartKeyword[0])
                {
                    return StreamObject.FromReader(reader, elementsDictionary);
                }
            }

            return new DictionaryObject(elementsDictionary);
        }

        /// <summary>
        /// Creates an DictionaryObject from DocumentReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>Parsed dictionary.</returns>
        protected static Dictionary<string, PdfObject> ParseDictionary(PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            reader.ReadToken(StartToken);
            reader.MoveToNonWhiteSpace();

            var elementsDictionary = new Dictionary<string, PdfObject>();

            do
            {
                var key = NameObject.FromReader(reader);
                reader.MoveToNonWhiteSpace();
                var value = reader.ReadObject();

                elementsDictionary.Add(key.Value, value);

                reader.MoveToNonWhiteSpace();
                if (reader.Peek() == EndToken[0])
                {
                    reader.ReadToken(EndToken);
                    break;
                }
            }
            while (true);

            return elementsDictionary;
        }
    }
}
