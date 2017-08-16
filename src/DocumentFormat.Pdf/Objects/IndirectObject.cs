using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using DocumentFormat.Pdf.Structure;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
        protected readonly PdfObject referencedObject;

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
        /// Gets referenced PdfObject
        /// </summary>
        public PdfObject Object => referencedObject;

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
        /// Creates a IndirectObject from PdfReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use</param>
        /// <param name="objectId">Parsed <see cref="PdfObjectId"/></param>
        /// <returns>Created IndirectObject</returns>
        public static IndirectObject FromReader(PdfReader reader, PdfObjectId objectId)
        {
            var header = reader.ReadLine();

            if (!header.EndsWith(StartKeyword))
                throw new FormatException("An indirect object header was expected.");

            var readId = PdfObjectId.FromString(header.Substring(0, header.Length - (StartKeyword.Length + 1)));
            if (readId != objectId)
                throw new InvalidOperationException("Read ObjectId does not matches reference");

            var referencedObject = reader.ReadObject();

            reader.ReadToken(EndKeyword);

            var objectType = referencedObject.GetType();

            var genCtor = typeof(IndirectObject<>)
                .MakeGenericType(objectType)
                .GetTypeInfo()
                .DeclaredConstructors
                .First(ctor =>
                {
                    if(ctor.IsPublic)
                    {
                        var ctorParams = ctor.GetParameters();
                        return ctorParams.Length == 2 && ctorParams[0].ParameterType == typeof(PdfObjectId) && ctorParams[0].ParameterType == objectType;
                    }
                    return false;
                });

            return (IndirectObject)genCtor.Invoke(new object[] { objectId, referencedObject });
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
