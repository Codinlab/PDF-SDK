using DocumentFormat.Pdf.IO;
using System;
using System.Text;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a literal Pdf String Object.
    /// </summary>
    public class LiteralStringObject : StringObject
    {
        /// <summary>
        /// Literal Pdf String Object start token.
        /// </summary>
        public const char StartToken = '(';

        /// <summary>
        /// Literal Pdf String Object end token.
        /// </summary>
        public const char EndToken = ')';

        /// <summary>
        /// Instanciates a new StringObject.
        /// </summary>
        /// <param name="value">The object's value.</param>
        public LiteralStringObject(string value) : base(value)
        {
        }

        /// <summary>
        /// Writes object to the current stream.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public override void Write(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var sb = new StringBuilder();
            sb.Append(StartToken);

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case Chars.LF:
                        sb.Append("\\n");
                        break;
                    case Chars.CR:
                        sb.Append("\\r");
                        break;
                    case Chars.HT:
                        sb.Append("\\t");
                        break;
                    case Chars.BS:
                        sb.Append("\\b");
                        break;
                    case Chars.FF:
                        sb.Append("\\f");
                        break;
                    case StartToken:
                        sb.Append("\\(");
                        break;
                    case EndToken:
                        sb.Append("\\)");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    default:
                        if (c < 32 || c > 126)
                        {
                            if (c > 0x1FF)  // 3 digits octal max value
                                break;

                            // Always write 3 digits
                            sb.Append('\\');
                            sb.Append((char)(c / 64 + '0'));
                            sb.Append((char)(c % 64 / 8 + '0'));
                            sb.Append((char)(c % 8 + '0'));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }

            sb.Append(EndToken);

            writer.Write(sb.ToString());
        }

        /// <summary>
        /// Creates a LiteralStringObject from PdfReader.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>Read StringObject.</returns>
        public static new LiteralStringObject FromReader(PdfReader reader)
        {
            if (reader.Read() != StartToken)
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

            return new LiteralStringObject(sb.ToString());
        }

        private static bool IsOctalDigit(char c)
        {
            return c >= '0' && c < '8';
        }
    }
}
