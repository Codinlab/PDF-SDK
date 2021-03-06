﻿using DocumentFormat.Pdf.Exceptions;
using DocumentFormat.Pdf.IO;
using System;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Boolean Object.
    /// </summary>
    public class BooleanObject : PdfObject
    {
        /// <summary>
        /// The "true" token
        /// </summary>
        public const string TrueToken = "true";

        /// <summary>
        /// The "false" token
        /// </summary>
        public const string FalseToken = "false";

        private bool value;

        /// <summary>
        /// Instanciates a new BooleanObject.
        /// </summary>
        /// <param name="value">The object's value.</param>
        public BooleanObject(bool value)
        {
            this.value = value;
        }

        /// <summary>
        /// Instanciates a new BooleanObject.
        /// </summary>
        /// <param name="value">The object's value.</param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        internal BooleanObject(bool value, bool isReadOnly) : base(isReadOnly)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets or sets the object's value.
        /// </summary>
        public bool Value {
            get => value;
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                this.value = value;
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

            writer.Write(value ? TrueToken : FalseToken);
        }
    }
}
