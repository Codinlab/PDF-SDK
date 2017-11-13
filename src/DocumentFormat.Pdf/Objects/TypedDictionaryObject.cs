using System;
using System.Collections.Generic;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a typed Pdf Dictionary Object.
    /// </summary>
    public abstract class TypedDictionaryObject : DictionaryObject
    {
        /// <summary>
        /// The Type key name
        /// </summary>
        protected const string TypeKey = "Type";

        /// <summary>
        /// When overrided, gets the Type entry value.
        /// </summary>
        protected abstract string TypeValue { get; }

        #region Constructors
        /// <summary>
        /// Instanciates a new DictionaryObject.
        /// </summary>
        public TypedDictionaryObject()
        {
            internalDictionary[TypeKey] = new NameObject(TypeValue);
        }

        /// <summary>
        /// Instanciates a new DictionaryObject.
        /// </summary>
        /// <param name="items">Dictionary items</param>
        public TypedDictionaryObject(IDictionary<string, PdfObject> items): base(items)
        {
            if (!internalDictionary.ContainsKey(TypeKey))
            {
                internalDictionary[TypeKey] = new NameObject(TypeValue);
            }
            else if((internalDictionary[TypeKey] as NameObject).Value != TypeValue)
            {
                throw new InvalidOperationException($"{nameof(items)} contains a {TypeKey} entry which is different from object's expected {TypeKey} value.");
            }
        }

        /// <summary>
        /// Instanciates a new DictionaryObject.
        /// </summary>
        /// <param name="items">Dictionary items</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        internal TypedDictionaryObject(IDictionary<string, PdfObject> items, bool isReadOnly) : base(items, isReadOnly)
        {
        }

        #endregion

        /// <summary>
        /// Gets the  type of PDF object that this dictionary describes.
        /// </summary>
        public string Type => (internalDictionary[TypeKey] as NameObject).Value;

    }
}
