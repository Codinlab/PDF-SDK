using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf String Object
    /// </summary>
    public class StringObject : PdfObject
    {
        private string value;

        /// <summary>
        /// Instanciates a new StringObject
        /// </summary>
        /// <param name="value">The object's value</param>
        public StringObject(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the object's value
        /// </summary>
        public string Value => value;

        /// <summary>
        /// Creates a StringObject from PdfReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use</param>
        /// <returns>Read StringObject</returns>
        public static StringObject FromReader(PdfReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            char firstRead = reader.Peek();

            if(firstRead == '(')
            {
                return LiteralFromReader(reader);
            }
            else if(firstRead == '<')
            {
                return HexadecimalFromReader(reader);
            }
            else
            {
                throw new FormatException("Unexpected string object start");
            }
        }

        /// <summary>
        /// Creates a StringObject from PdfReader
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use</param>
        /// <returns>Read StringObject</returns>

        internal static StringObject LiteralFromReader(PdfReader reader)
        {
            if (reader.Read() != '(')
                throw new FormatException("Unexpected litteral string object start token.");

            var parenthesisDepth = 0;
            char prev = (char)0;
            var readChars = reader.ReadWhile((read) => {
                if (read == '(' && prev != '\\')
                {
                    parenthesisDepth++;
                }
                else if (read == ')' && prev != '\\')
                {
                    if (parenthesisDepth > 0)
                    {
                        parenthesisDepth--;
                    }
                    else
                    {
                        return false;
                    }
                }
                prev = read;
                return true;
            });

            // Skip end token
            reader.Position++;

            var sb = new StringBuilder();
            for (var i = 0; i < readChars.Length; i++)
            {
                if (readChars[i] == '\\')
                {
                    if (i >= readChars.Length - 1)
                    {
                        break;
                    }
                    var nextChar = readChars[i + 1];
                    switch (nextChar)
                    {
                        case 'n':
                            sb.Append(Chars.LF);
                            i++;
                            break;
                        case 'r':
                            sb.Append(Chars.CR);
                            i++;
                            break;
                        case 't':
                            sb.Append(Chars.HT);
                            i++;
                            break;
                        case 'b':
                            sb.Append(Chars.BS);
                            i++;
                            break;
                        case 'f':
                            sb.Append(Chars.FF);
                            i++;
                            break;
                        case '(':
                        case ')':
                        case '\\':
                            sb.Append(nextChar);
                            i++;
                            break;
                        default:
                            if (Chars.IsEndOfLine(nextChar))
                            {
                                // Line continuation
                                if (i < readChars.Length - 2 && Chars.IsEndOfLine(readChars[i + 2]))
                                {
                                    // Two chars EOL
                                    i += 2;
                                }
                                else
                                {
                                    // One char EOL
                                    i++;
                                }
                            }
                            else if (IsOctalDigit(nextChar))
                            {
                                // Octal character code
                                var charCode = nextChar - '0';
                                i++;
                                if (i < readChars.Length - 1 && IsOctalDigit(readChars[i + 1]))
                                {
                                    // Two digits octal
                                    charCode = (charCode << 3) + readChars[i + 1] - '0';
                                    i++;
                                }
                                if (i < readChars.Length - 1 && IsOctalDigit(readChars[i + 1]))
                                {
                                    // Three digits octal
                                    charCode = (charCode << 3) + readChars[i + 1] - '0';
                                    i++;
                                }

                                sb.Append((char)charCode);
                            }
                            break;
                    }
                }
                else
                {
                    sb.Append(readChars[i]);
                }
            }
            
            return new StringObject(sb.ToString());
        }

        /// <summary>
        /// Creates a StringObject from PdfReader
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use</param>
        /// <returns>Read StringObject</returns>
        internal static StringObject HexadecimalFromReader(PdfReader reader)
        {
            if (reader.Read() != '<')
                throw new FormatException("Unexpected hexadecimal string object start token.");

            var readChars = reader.ReadWhile((read) => read != '>');

            // Skip end token
            reader.Position++;

            if(readChars.Length == 0)
            {
                return new StringObject("");
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
                        
                        if(j == 0)
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

                if(j == 1)
                {
                    hex[1] = '0';
                    sb.Append((char)byte.Parse(new string(hex), NumberStyles.HexNumber));
                }
            }

            return new StringObject(sb.ToString());
        }

        private static bool IsOctalDigit(char c)
        {
            return c >= '0' && c <= '8';
        }
    }
}
