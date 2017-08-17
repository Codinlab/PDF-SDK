using DocumentFormat.Pdf.IO;
using System;
using System.Globalization;
using System.Text;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents an hexadecimal Pdf String Object
    /// </summary>
    public class HexadecimalStringObject : StringObject
    {
        /// <summary>
        /// Hexadecimal Pdf String Object start token.
        /// </summary>
        public const char StartToken = '<';

        /// <summary>
        /// Hexadecimal Pdf String Object end token.
        /// </summary>
        public const char EndToken = '>';

        /// <summary>
        /// Instanciates a new HexadecimalStringObject.
        /// </summary>
        /// <param name="value">The object's value.</param>
        public HexadecimalStringObject(string value) : base(value)
        {
        }

        /// <summary>
        /// Writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public override void Write(PdfWriter writer)
        {
            var sb = new StringBuilder();
            sb.Append(StartToken);

            for (int i = 0; i < value.Length; i++)
            {
                sb.Append(((byte)value[i]).ToString("X2"));
            }

            sb.Append(EndToken);

            writer.Write(sb.ToString());
        }

        /// <summary>
        /// Creates a LiteralStringObject from PdfReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use</param>
        /// <returns>Read StringObject.</returns>
        public static new HexadecimalStringObject FromReader(PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (reader.Read() != StartToken)
                throw new FormatException("Unexpected hexadecimal string object start token.");

            var readChars = reader.ReadWhile((read) => read != EndToken);

            // Skip end token
            reader.Position++;

            if (readChars.Length == 0)
            {
                return new HexadecimalStringObject("");
            }

            var sb = new StringBuilder();

            var hex = new char[2];
            int i = 0, j = 0;
            while (i < readChars.Length)
            {
                do
                {
                    if (!Chars.IsWhiteSpace(readChars[i]))
                    {
                        hex[j] = readChars[i];

                        if (j == 0)
                        {
                            j++;
                        }
                        else
                        {
                            sb.Append((char)byte.Parse(new string(hex), NumberStyles.HexNumber));
                            j = 0;
                        }
                    }

                    i++;
                }
                while (i < readChars.Length);

                if (j == 1)
                {
                    hex[1] = '0';
                    sb.Append((char)byte.Parse(new string(hex), NumberStyles.HexNumber));
                }
            }

            return new HexadecimalStringObject(sb.ToString());
        }
    }
}
