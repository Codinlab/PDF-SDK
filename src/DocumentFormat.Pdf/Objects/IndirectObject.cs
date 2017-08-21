﻿using DocumentFormat.Pdf.Exceptions;
using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Structure;
using System;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Base class for Indirect Objects
    /// </summary>
    public abstract class IndirectObject : PdfObject
    {
        /// <summary>
        /// The obj keyword
        /// </summary>
        public const string StartKeyword = "obj";

        /// <summary>
        /// The endobj keyword
        /// </summary>
        public const string EndKeyword = "endobj";

        /// <summary>
        /// Referenced Object's <see cref="PdfObjectId"/>
        /// </summary>
        protected readonly PdfObjectId objectId;

        /// <summary>
        /// Referenced PdfObject
        /// </summary>
        protected PdfObject referencedObject;

        /// <summary>
        /// Instanciates a new IndirectObject
        /// </summary>
        /// <param name="objectId">The <see cref="PdfObjectId"/> of the referenced object</param>
        /// <param name="referencedObject">Referenced <see cref="PdfObject"/></param>
        protected IndirectObject(PdfObjectId objectId, PdfObject referencedObject)
        {
            this.objectId = objectId;
            this.referencedObject = referencedObject;
        }

        /// <summary>
        /// Instanciates a new IndirectObject
        /// </summary>
        /// <param name="objectId">The <see cref="PdfObjectId"/> of the referenced object</param>
        /// <param name="referencedObject">Referenced <see cref="PdfObject"/></param>
        /// <param name="isReadOnly">True if object is read-only, otherwise false.</param>
        protected IndirectObject(PdfObjectId objectId, PdfObject referencedObject, bool isReadOnly) : base(isReadOnly)
        {
            this.objectId = objectId;
            this.referencedObject = referencedObject;
        }

        /// <summary>
        /// Gets or sets referenced PdfObject
        /// </summary>
        public PdfObject Object {
            get => referencedObject;
            set {
                if (IsReadOnly)
                    throw new ObjectReadOnlyException();

                referencedObject = value;
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

            writer.WriteLine($"{objectId.ObjectNumber.ToString()} {objectId.GenerationNumber.ToString()} {StartKeyword}");

            referencedObject.Write(writer);

            writer.WriteLine();
            writer.WriteLine(EndKeyword);
        }

        /// <summary>
        /// Writes indirect object reference to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public void WriteReference(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            objectId.WriteReference(writer);
        }

        /// <summary>
        /// Creates a IndirectObject from PdfReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use</param>
        /// <returns>Created IndirectObject</returns>
        public static IndirectObject FromReader(PdfReader reader)
        {
            var header = reader.ReadLine();

            if (!header.EndsWith(StartKeyword))
                throw new FormatException("An indirect object header was expected.");

            var objectId = PdfObjectId.FromString(header.Substring(0, header.Length - (StartKeyword.Length + 1)));

            var referencedObject = reader.ReadObject();

            reader.MoveToNonWhiteSpace();
            reader.ReadToken(EndKeyword);

            var objectType = referencedObject.GetType();

            return (IndirectObject)Activator.CreateInstance(
                typeof(IndirectObject<>).MakeGenericType(objectType),
                objectId,
                referencedObject
            );
        }
    }

    /// <summary>
    /// Represents a Pdf Indirect Object
    /// </summary>
    /// <typeparam name="T">Referenced object type</typeparam>
    public class IndirectObject<T> : IndirectObject
        where T : PdfObject
    {
        /// <summary>
        /// Instanciates a new IndirectObject
        /// </summary>
        /// <param name="objectId">The <see cref="PdfObjectId"/> of the referenced object</param>
        /// <param name="referencedObject">Referenced <see cref="PdfObject"/></param>
        public IndirectObject(PdfObjectId objectId, T referencedObject) : base(objectId, referencedObject)
        {
        }

        /// <summary>
        /// Gets referenced PdfObject
        /// </summary>
        public new T Object => (T)referencedObject;
    }
}
