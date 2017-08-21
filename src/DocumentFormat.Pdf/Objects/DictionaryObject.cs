using DocumentFormat.Pdf.Attributes;
using DocumentFormat.Pdf.Exceptions;
using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Dictionary Object.
    /// </summary>
    [HasDelimiters]
    public class DictionaryObject : PdfObject, IDictionary<string, PdfObject>
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
        /// Instanciates a new DictionaryObject.
        /// </summary>
        public DictionaryObject()
        {
            internalDictionary = new Dictionary<string, PdfObject>();
        }

        /// <summary>
        /// Instanciates a new DictionaryObject.
        /// </summary>
        /// <param name="items">Dictionary items</param>
        public DictionaryObject(IDictionary<string, PdfObject> items)
        {
            internalDictionary = new Dictionary<string, PdfObject>(items);
        }

        /// <summary>
        /// Instanciates a new DictionaryObject.
        /// </summary>
        /// <param name="items">Dictionary items</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        internal DictionaryObject(IDictionary<string, PdfObject> items, bool isReadOnly) : base(isReadOnly)
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

        ICollection<string> IDictionary<string, PdfObject>.Keys {
            get {
                if (IsReadOnly)
                {
                    return new ReadOnlyCollection<string>(internalDictionary.Keys);
                }
                else
                {
                    return internalDictionary.Keys;
                }
            }
        }

        ICollection<PdfObject> IDictionary<string, PdfObject>.Values {
            get {
                if (IsReadOnly)
                {
                    return new ReadOnlyCollection<PdfObject>(internalDictionary.Values);
                }
                else
                {
                    return internalDictionary.Values;
                }
            }
        }

        PdfObject IDictionary<string, PdfObject>.this[string key] {
            get {
                return internalDictionary[key];
            }
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                internalDictionary[key] = value;
            }
        }

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
            return internalDictionary.GetEnumerator();
        }

        /// <summary>
        /// Writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public override void Write(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.Write(StartToken);

            foreach(var entry in internalDictionary)
            {
                NameObject.WriteName(writer, entry.Key);

                if (!entry.Value.HasStartDelimiter())
                {
                    // Append separator
                    writer.Write(Chars.SP);
                }

                entry.Value.Write(writer);
            }

            writer.Write(EndToken);
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

            return new DictionaryObject(elementsDictionary, true);
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
                var key = NameObject.ReadName(reader);
                reader.MoveToNonWhiteSpace();
                var value = reader.ReadObject();

                elementsDictionary.Add(key, value);

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

        public void Add(string key, PdfObject value)
        {
            if (IsReadOnly)
                throw new ObjectReadOnlyException();

            internalDictionary.Add(key, value);
        }

        public bool Remove(string key)
        {
            if (IsReadOnly)
                throw new ObjectReadOnlyException();

            return internalDictionary.Remove(key);
        }

        public void Add(KeyValuePair<string, PdfObject> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            if (IsReadOnly)
                throw new ObjectReadOnlyException();

            internalDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, PdfObject> item)
        {
            if (internalDictionary.ContainsKey(item.Key) && internalDictionary[item.Key] == item.Value)
            {
                return true;
            }

            return false;
        }

        public void CopyTo(KeyValuePair<string, PdfObject>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (array.Length - arrayIndex < internalDictionary.Count)
                throw new ArgumentException(nameof(array), "Array is not large enough.");

            foreach(var entry in internalDictionary)
            {
                array[arrayIndex++] = new KeyValuePair<string, PdfObject>(entry.Key, entry.Value);
            }
        }

        public bool Remove(KeyValuePair<string, PdfObject> item)
        {
            if (internalDictionary.ContainsKey(item.Key) && internalDictionary[item.Key] == item.Value)
            {
                internalDictionary.Remove(item.Key);
                return true;
            }

            return false;
        }

        private class ReadOnlyCollection<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>
        {
            private readonly ICollection<T> collection;
            private Object syncRoot;

            internal ReadOnlyCollection(ICollection<T> collection)
            {
                this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
            }

            void ICollection<T>.Add(T item)
            {
                throw new NotSupportedException("Cannot add to a read-only collection.");
            }

            void ICollection<T>.Clear()
            {
                throw new NotSupportedException("Cannot clear a read-only collection.");
            }

            bool ICollection<T>.Contains(T item)
            {
                return collection.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                collection.CopyTo(array, arrayIndex);
            }

            public int Count {
                get { return collection.Count; }
            }

            bool ICollection<T>.IsReadOnly {
                get { return true; }
            }

            bool ICollection<T>.Remove(T item)
            {
                throw new NotSupportedException("Cannot remove from a read-only collection.");
            }

            public IEnumerator<T> GetEnumerator()
            {
                return collection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)collection).GetEnumerator();
            }

            void ICollection.CopyTo(Array array, int index)
            {
                throw new NotSupportedException("Connot copy to a non-generic ICollection.");
            }

            bool ICollection.IsSynchronized {
                get { return false; }
            }

            object ICollection.SyncRoot {
                get {
                    if (syncRoot == null)
                    {
                        ICollection c = collection as ICollection;
                        if (c != null)
                        {
                            syncRoot = c.SyncRoot;
                        }
                        else
                        {
                            System.Threading.Interlocked.CompareExchange<Object>(ref syncRoot, new Object(), null);
                        }
                    }
                    return syncRoot;
                }
            }
        }
    }

}
